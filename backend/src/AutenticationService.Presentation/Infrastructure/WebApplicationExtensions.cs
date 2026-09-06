using System.Reflection;

namespace AutenticationService.Presentation.Infrastructure;

public static class WebApplicationExtensions
{
	public static RouteGroupBuilder MapGroup(
		this WebApplication app,
		EndpointGroupBase group)
	{
		var groupName = group.GetType().Name.ToLower();

		return app
			.MapGroup($"/api/{groupName}")
			.WithTags(groupName);
	}

	public static WebApplication MapEndpoints(
		this WebApplication app)
	{
		var endpointGroupType = typeof(EndpointGroupBase);

		var assembly = Assembly.GetExecutingAssembly();

		var endpointGroupTypes = assembly
			.GetExportedTypes()
			.Where(type => type.IsSubclassOf(endpointGroupType))
			.ToList();

		foreach (var type in endpointGroupTypes)
			Console.WriteLine($"Mapping endpoint group: {type.FullName}");

		foreach (var type in endpointGroupTypes)
		{
			if (Activator.CreateInstance(type) is EndpointGroupBase instance)
				instance.Map(app);
		}

		app.MapHealthChecks("/");

		return app;
	}
}
