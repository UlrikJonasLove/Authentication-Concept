using System.Text;
using System.Threading.RateLimiting;
using AutenticationService.Infrastructure.Authentication;
using AutenticationService.Presentation.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace AutenticationService.Presentation;

public static class DependencyInjection
{
	public static IServiceCollection AddPresentationServices(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddHealthChecks();

		var jwtOptions = configuration
			.GetSection(JwtOptions.SectionName)
			.Get<JwtOptions>()
			?? throw new InvalidOperationException(
				"JWT configuration is missing.");

		services
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = jwtOptions.Issuer,

					ValidateAudience = true,
					ValidAudience = jwtOptions.Audience,

					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),

					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero
				};
			});
		services.AddRateLimiter(options =>
		{
			options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

			options.AddPolicy(
				"register",
				httpContext =>
				{
					var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString()
						?? "unknown";

					return RateLimitPartition.GetFixedWindowLimiter(
						ipAddress,
						_ => new FixedWindowRateLimiterOptions
						{
							PermitLimit = 5,
							Window = TimeSpan.FromMinutes(1),
							QueueLimit = 0,
							AutoReplenishment = true
						});
				});

			options.AddPolicy(
				"login",
				httpContext =>
				{
					var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString()
						?? "unknown";

					return RateLimitPartition.GetFixedWindowLimiter(
						ipAddress,
						_ => new FixedWindowRateLimiterOptions
						{
							PermitLimit = 10,
							Window = TimeSpan.FromMinutes(1),
							QueueLimit = 0,
							AutoReplenishment = true
						});
				});

			options.AddPolicy(
				"renew",
				httpContext =>
				{
					var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString()
						?? "unknown";

					return RateLimitPartition.GetFixedWindowLimiter(
						ipAddress,
						_ => new FixedWindowRateLimiterOptions
						{
							PermitLimit = 20,
							Window = TimeSpan.FromMinutes(1),
							QueueLimit = 0,
							AutoReplenishment = true
						});
				});
		});
		services.AddCors(options =>
		{
			options.AddPolicy(
				"Angular",
				corsPolicyBuilder =>
				{
					corsPolicyBuilder
						.WithOrigins(
							"https://localhost:4200",
							"https://auth-api-axdkhafcfqc5c7c8.swedencentral-01.azurewebsites.net/api")
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowCredentials();
				});
		});
		services
			.AddEndpointsApiExplorer()
			.AddSwaggerGen()
			.AddAuthorization()
			.AddExceptionHandler<GlobalExceptionHandler>()
			.AddProblemDetails();

		return services;
	}
}
