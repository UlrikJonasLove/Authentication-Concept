using AutenticationService.Application.Common.Interfaces;
using AutenticationService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutenticationService.Application.Authentication.Commands.Logout;

public record LogoutCommand(
	string RefreshToken)
	: IRequest;

public class LogoutCommandHandler(
	IApplicationDbContext context,
	IRefreshTokenService refreshTokenService)
	: IRequestHandler<LogoutCommand>
{
	public async Task Handle(
		LogoutCommand command,
		CancellationToken cancellationToken)
	{
		var tokenHash = refreshTokenService.Hash(
			command.RefreshToken);

		var refreshToken = await context.RefreshTokens
			.AsTracking()
			.Include(refreshToken => refreshToken.Session)
			.FirstOrDefaultAsync(
				refreshToken => refreshToken.TokenHash == tokenHash,
				cancellationToken);

		if (refreshToken is null)
			return;

		var now = DateTimeOffset.UtcNow;

		var userSession = refreshToken.Session;

		userSession.RevokedAt ??= now;

		var refreshTokens = await context.RefreshTokens
			.AsTracking()
			.Where(refreshToken =>
				refreshToken.SessionId == userSession.Id &&
				!refreshToken.RevokedAt.HasValue)
			.ToListAsync(cancellationToken);

		foreach (var sessionRefreshToken in refreshTokens)
			sessionRefreshToken.RevokedAt = now;

		await context.SaveChangesAsync(cancellationToken);
	}
}
