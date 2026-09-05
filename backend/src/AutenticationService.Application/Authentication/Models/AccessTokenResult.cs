namespace AutenticationService.Application.Authentication.Models;

public sealed record AccessTokenResult(
	string Token,
	DateTimeOffset ExpiresAt);
