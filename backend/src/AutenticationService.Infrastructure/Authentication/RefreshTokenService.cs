using System.Security.Cryptography;
using System.Text;
using AutenticationService.Application.Common.Interfaces;
using Microsoft.AspNetCore.WebUtilities;

namespace AutenticationService.Infrastructure.Authentication;

public class RefreshTokenService : IRefreshTokenService
{
	public string Generate()
	{
		var tokenBytes = RandomNumberGenerator.GetBytes(64);

		return WebEncoders.Base64UrlEncode(tokenBytes);
	}

	public string Hash(string token)
	{
		var tokenBytes = Encoding.UTF8.GetBytes(token);
		var hashBytes = SHA256.HashData(tokenBytes);

		return Convert.ToHexString(hashBytes);
	}
}
