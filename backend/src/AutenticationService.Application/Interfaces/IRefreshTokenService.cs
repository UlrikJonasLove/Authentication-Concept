namespace AutenticationService.Application.Common.Interfaces;

public interface IRefreshTokenService
{
	string Generate();

	string Hash(string token);
}
