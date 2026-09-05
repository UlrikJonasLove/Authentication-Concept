using AutenticationService.Application.Authentication.Models;
using AutenticationService.Application.Common.Interfaces;
using AutenticationService.Application.Interfaces;
using AutenticationService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AutenticationService.Application.Authentication.Commands.Login;

public record LoginCommand(
	string Username,
	string Password)
	: IRequest<AuthenticationResult>;

public class LoginCommandHandler(
	IApplicationDbContext context,
	IPasswordHasher passwordHasher,
	IRefreshTokenService refreshTokenService,
	IAccessTokenService accessTokenService,
	IOptions<AuthenticationOptions> authenticationOptions)
	: IRequestHandler<LoginCommand, AuthenticationResult>
{
	private readonly AuthenticationOptions _authenticationOptions =
		authenticationOptions.Value;

	public async Task<AuthenticationResult> Handle(
		LoginCommand command,
		CancellationToken cancellationToken)
	{
		var normalizedUsername = command.Username
			.Trim()
			.ToUpperInvariant();

		var user = await context.Users
			.AsTracking()
			.FirstOrDefaultAsync(
				user => user.NormalizedUsername == normalizedUsername,
				cancellationToken) ?? throw new InvalidOperationException(
				"Invalid username or password.");

		var now = DateTimeOffset.UtcNow;

		if (!user.IsActive)
			throw new InvalidOperationException(
				"Invalid username or password.");

		if (user.LockoutEnd.HasValue &&
			user.LockoutEnd.Value > now)
			throw new InvalidOperationException(
				"Invalid username or password.");

		var passwordIsValid = passwordHasher.Verify(
			command.Password,
			user.PasswordHash);

		if (!passwordIsValid)
		{
			user.FailedLoginAttempts++;

			if (user.FailedLoginAttempts >=
				_authenticationOptions.MaxFailedLoginAttempts)
			{
				user.LockoutEnd = now.AddMinutes(
					_authenticationOptions.LockoutDurationMinutes);

				user.FailedLoginAttempts = 0;
			}

			await context.SaveChangesAsync(cancellationToken);

			throw new InvalidOperationException(
				"Invalid username or password.");
		}

		user.FailedLoginAttempts = 0;
		user.LockoutEnd = null;

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

		context.UserSessions.Add(userSession);
		context.RefreshTokens.Add(refreshToken);

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
