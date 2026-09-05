namespace AutenticationService.Presentation.Infrastructure;

public class SecurityHeadersMiddleware(
	RequestDelegate next)
{
	private readonly RequestDelegate _next = next;

	public async Task InvokeAsync(HttpContext httpContext)
	{
		httpContext.Response.OnStarting(() =>
		{
			var headers = httpContext.Response.Headers;

			headers["Strict-Transport-Security"] =
				"max-age=31536000; includeSubDomains";

			headers["X-Content-Type-Options"] =
				"nosniff";

			headers["X-Frame-Options"] =
				"DENY";

			headers["Referrer-Policy"] =
				"strict-origin-when-cross-origin";

			headers["Permissions-Policy"] =
				"camera=(), microphone=(), geolocation=()";

			headers["Content-Security-Policy"] =
				"default-src 'self'; " +
				"script-src 'self'; " +
				"style-src 'self'; " +
				"img-src 'self' data:; " +
				"font-src 'self'; " +
				"connect-src 'self'; " +
				"object-src 'none'; " +
				"base-uri 'self'; " +
				"frame-ancestors 'none'; " +
				"form-action 'self';";

			return Task.CompletedTask;
		});

		await _next(httpContext);
	}
}
