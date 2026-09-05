using AutenticationService.Application.Authentication.Models;
using AutenticationService.Domain.Entities;

namespace AutenticationService.Application.Common.Interfaces;

public interface IAccessTokenService
{
	AccessTokenResult Generate(
		User user,
		UserSession userSession);
}
