using AutenticationService.Application.Common.Interfaces;
using AutenticationService.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AutenticationService.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
	private readonly PasswordHasher<object> _passwordHasher = new();

	public string Hash(string password) =>
		_passwordHasher.HashPassword(null!, password);

	public bool Verify(
		string password,
		string passwordHash)
	{
		var result = _passwordHasher.VerifyHashedPassword(
			null!,
			passwordHash,
			password);

		return result is PasswordVerificationResult.Success
			or PasswordVerificationResult.SuccessRehashNeeded;
	}
}
