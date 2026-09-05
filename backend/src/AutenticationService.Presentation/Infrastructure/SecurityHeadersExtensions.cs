namespace AutenticationService.Presentation.Infrastructure;

public static class SecurityHeadersExtensions
{
	public static IApplicationBuilder UseSecurityHeaders(
		this IApplicationBuilder application) =>
		application.UseMiddleware<SecurityHeadersMiddleware>();
}
