namespace AutenticationService.Presentation.ResponseModels;

public sealed record AuthenticationResponse(
	string AccessToken,
	DateTimeOffset AccessTokenExpiresAt,
	Guid UserId,
	string Username);
