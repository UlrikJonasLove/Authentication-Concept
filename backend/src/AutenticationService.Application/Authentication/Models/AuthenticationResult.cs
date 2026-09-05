using System;
using System.Collections.Generic;
using System.Text;

namespace AutenticationService.Application.Authentication.Models;

public sealed record AuthenticationResult(
	string AccessToken,
	DateTimeOffset AccessTokenExpiresAt,
	string RefreshToken,
	DateTimeOffset RefreshTokenExpiresAt,
	Guid UserId,
	string Username);
