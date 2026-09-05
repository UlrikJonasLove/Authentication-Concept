using AutenticationService.Application.Authentication.Models;
using AutenticationService.Application.Common.Interfaces;
using AutenticationService.Application.Interfaces;
using AutenticationService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AutenticationService.Application.Authentication.Commands.Renew;

public record RenewCommand(
	string RefreshToken)
	: IRequest<AuthenticationResult>;

public class RenewCommandHandler(
	IApplicationDbContext context,
	IRefreshTokenService refreshTokenService,
	IAccessTokenService accessTokenService,
	IOptions<AuthenticationOptions> authenticationOptions)
	: IRequestHandler<RenewCommand, AuthenticationResult>
{
	private readonly AuthenticationOptions _authenticationOptions =
		authenticationOptions.Value;

	public async Task<AuthenticationResult> Handle(
		RenewCommand command,
		CancellationToken cancellationToken)
	{
		var now = DateTimeOffset.UtcNow;

		var tokenHash = refreshTokenService.Hash(
			command.RefreshToken);

		var refreshToken = await context.RefreshTokens
			.AsTracking()
			.Include(refreshToken => refreshToken.Session)
			.ThenInclude(userSession => userSession.User)
			.FirstOrDefaultAsync(
				refreshToken => refreshToken.TokenHash == tokenHash,
				cancellationToken);

		if (refreshToken is null)
			throw new InvalidOperationException(
				"Invalid refresh token.");

		var userSession = refreshToken.Session;
		var user = userSession.User;

		if (!user.IsActive)
			throw new InvalidOperationException(
				"Invalid refresh token.");

		if (userSession.RevokedAt.HasValue ||
			userSession.ExpiresAt <= now)
			throw new InvalidOperationException(
				"Invalid refresh token.");

		if (refreshToken.UsedAt.HasValue ||
			refreshToken.RevokedAt.HasValue)
		{
			await RevokeSessionAsync(
				userSession,
				now,
				cancellationToken);

			throw new InvalidOperationException(
				"Refresh token reuse detected.");
		}

		if (refreshToken.ExpiresAt <= now)
			throw new InvalidOperationException(
				"Refresh token has expired.");

		var refreshTokenValue =
			refreshTokenService.Generate();

		var refreshTokenExpiresAt = now.AddDays(
			_authenticationOptions.RefreshTokenLifetimeDays);

		if (refreshTokenExpiresAt > userSession.ExpiresAt)
			refreshTokenExpiresAt = userSession.ExpiresAt;

		var newRefreshToken = new RefreshToken
		{
			Id = Guid.NewGuid(),
			SessionId = userSession.Id,
			TokenHash = refreshTokenService.Hash(
				refreshTokenValue),
			CreatedAt = now,
			ExpiresAt = refreshTokenExpiresAt
		};

		refreshToken.UsedAt = now;
		refreshToken.ReplacedByTokenId =
			newRefreshToken.Id;

		userSession.LastUsedAt = now;

		userSession.RefreshTokens.Add(
			newRefreshToken);

		context.RefreshTokens.Add(
			newRefreshToken);

		try
		{
			await context.SaveChangesAsync(
				cancellationToken);
		}
		catch (DbUpdateConcurrencyException)
		{
			throw new InvalidOperationException(
				"Refresh token has already been used.");
		}

		var accessToken = accessTokenService.Generate(
			user,
			userSession);

		return new AuthenticationResult(
			accessToken.Token,
			accessToken.ExpiresAt,
			refreshTokenValue,
			newRefreshToken.ExpiresAt,
			user.Id,
			user.Username);
	}

	private async Task RevokeSessionAsync(
		UserSession userSession,
		DateTimeOffset revokedAt,
		CancellationToken cancellationToken)
	{
		userSession.RevokedAt = revokedAt;

		var refreshTokens = await context.RefreshTokens
			.AsTracking()
			.Where(refreshToken =>
				refreshToken.SessionId == userSession.Id &&
				!refreshToken.RevokedAt.HasValue)
			.ToListAsync(cancellationToken);

		foreach (var refreshToken in refreshTokens)
			refreshToken.RevokedAt = revokedAt;

		await context.SaveChangesAsync(
			cancellationToken);
	}
}
