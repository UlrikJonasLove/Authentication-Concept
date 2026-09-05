using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutenticationService.Application.Authentication.Models;
using AutenticationService.Application.Common.Interfaces;
using AutenticationService.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AutenticationService.Infrastructure.Authentication;

public class AccessTokenService(
	IOptions<JwtOptions> jwtOptions)
	: IAccessTokenService
{
	private readonly JwtOptions _jwtOptions = jwtOptions.Value;

	public AccessTokenResult Generate(
		User user,
		UserSession userSession)
	{
		var issuedAt = DateTimeOffset.UtcNow;

		var expiresAt = issuedAt.AddMinutes(
			_jwtOptions.AccessTokenLifetimeMinutes);

		var claims = new List<Claim>
		{
			new(
				JwtRegisteredClaimNames.Sub,
				user.Id.ToString()),

			new(
				JwtRegisteredClaimNames.UniqueName,
				user.Username),

			new(
				JwtRegisteredClaimNames.Jti,
				Guid.NewGuid().ToString()),

			new(
				"sid",
				userSession.Id.ToString())
		};

		var signingKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(
				_jwtOptions.SigningKey));

		var signingCredentials = new SigningCredentials(
			signingKey,
			SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: _jwtOptions.Issuer,
			audience: _jwtOptions.Audience,
			claims: claims,
			notBefore: issuedAt.UtcDateTime,
			expires: expiresAt.UtcDateTime,
			signingCredentials: signingCredentials);

		var tokenValue = new JwtSecurityTokenHandler()
			.WriteToken(token);

		return new AccessTokenResult(
			tokenValue,
			expiresAt);
	}
}
