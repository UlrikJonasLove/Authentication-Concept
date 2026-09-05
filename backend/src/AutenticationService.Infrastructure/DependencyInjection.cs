using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AutenticationService.Application.Interfaces;
using AutenticationService.Infrastructure.Data;
using AutenticationService.Infrastructure.Authentication;
using AutenticationService.Application.Common.Interfaces;
using AutenticationService.Application.Authentication.Models;

namespace AutenticationService.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<AuthenticationOptions>(
			configuration.GetSection(AuthenticationOptions.SectionName));

		services.Configure<JwtOptions>(
			configuration.GetSection(JwtOptions.SectionName));

		services.AddDbContext<ApplicationDbContext>((sp, options) =>
		{
			options
				.UseSqlServer(configuration.GetConnectionString("AuthDb"))
				.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
				.EnableDetailedErrors();
		});

		services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
		services.AddScoped<IPasswordHasher, PasswordHasher>();
		services.AddScoped<IRefreshTokenService, RefreshTokenService>();
		services.AddScoped<IAccessTokenService, AccessTokenService>();

		return services;
	}
}
