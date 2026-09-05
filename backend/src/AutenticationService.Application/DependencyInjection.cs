using System.Reflection;
using AutenticationService.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AutenticationService.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
			cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
		});

		return services;
	}
}
