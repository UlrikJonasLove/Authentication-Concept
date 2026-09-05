using AutenticationService.Application;
using AutenticationService.Infrastructure;
using AutenticationService.Presentation;
using AutenticationService.Presentation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddApplicationServices()
	.AddInfrastructureServices(builder.Configuration)
	.AddPresentationServices(builder.Configuration);

var application = builder.Build();

if (application.Environment.IsDevelopment())
	application
		.UseSwagger()
		.UseSwaggerUI();

application
	.UseHttpsRedirection()
	.UseSecurityHeaders()
	.UseCors("Angular")
	.UseRateLimiter()
	.UseAuthentication()
	.UseAuthorization();

application
	.MapEndpoints()
	.Run();
