using AutenticationService.Application.Authentication.Models;
using AutenticationService.Application.Common.Interfaces;
using AutenticationService.Application.Interfaces;
using AutenticationService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AutenticationService.Application.Authentication.Commands.Register;

public record RegisterCommand(
	string Username,
	string Password)
	: IRequest<AuthenticationResult>;

public class RegisterCommandHandler(
	IApplicationDbContext context,
	IPasswordHasher passwordHasher,
	IRefreshTokenService refreshTokenService,
	IAccessTokenService accessTokenService,
	IOptions<AuthenticationOptions> authenticationOptions)
	: IRequestHandler<RegisterCommand, AuthenticationResult>
{
	private readonly AuthenticationOptions _authenticationOptions =
		authenticationOptions.Value;

	public async Task<AuthenticationResult> Handle(
		RegisterCommand command,
		CancellationToken cancellationToken)
	{
		var normalizedUsername = command.Username
			.Trim()
			.ToUpperInvariant();

		var usernameExists = await context.Users
			.AnyAsync(
				user => user.NormalizedUsername == normalizedUsername,
				cancellationToken);

		if (usernameExists)
			throw new InvalidOperationException(
				"Username is already in use.");

		var now = DateTimeOffset.UtcNow;

		var user = new User
		{
			Id = Guid.NewGuid(),
			Username = command.Username.Trim(),
			NormalizedUsername = normalizedUsername,
			PasswordHash = passwordHasher.Hash(command.Password),
			CreatedAt = now,
			IsActive = true
		};

		var userSession = new UserSession
		{
			Id = Guid.NewGuid(),
			UserId = user.Id,
			CreatedAt = now,
			ExpiresAt = now.AddDays(
				_authenticationOptions.SessionLifetimeDays)
		};

		var refreshTokenValue = refreshTokenService.Generate();

		var refreshToken = new RefreshToken
		{
			Id = Guid.NewGuid(),
			SessionId = userSession.Id,
			TokenHash = refreshTokenService.Hash(
				refreshTokenValue),
			CreatedAt = now,
			ExpiresAt = now.AddDays(
				_authenticationOptions.RefreshTokenLifetimeDays)
		};

		user.Sessions.Add(userSession);
		userSession.RefreshTokens.Add(refreshToken);

		context.Users.Add(user);

		await context.SaveChangesAsync(cancellationToken);

		var accessToken = accessTokenService.Generate(
			user,
			userSession);

		return new AuthenticationResult(
			accessToken.Token,
			accessToken.ExpiresAt,
			refreshTokenValue,
			refreshToken.ExpiresAt,
			user.Id,
			user.Username);
	}
}
