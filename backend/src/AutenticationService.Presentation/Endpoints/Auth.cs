using AutenticationService.Application.Authentication.Commands.Login;
using AutenticationService.Application.Authentication.Commands.Logout;
using AutenticationService.Application.Authentication.Commands.Register;
using AutenticationService.Application.Authentication.Commands.Renew;
using AutenticationService.Presentation.Infrastructure;
using AutenticationService.Presentation.ResponseModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutenticationService.Presentation.Endpoints;

public class Auth : EndpointGroupBase
{
	public override void Map(WebApplication app)
	{
		var authGroup = app.MapGroup(this);

		authGroup
			.MapPost(RegisterAsync, "register")
			.RequireRateLimiting("register");

		authGroup
			.MapPost(LoginAsync, "login")
			.RequireRateLimiting("login");

		authGroup
			.MapPost(LogoutAsync, "logout");

		authGroup
			.MapPost(RenewAsync, "renew")
			.RequireRateLimiting("renew");
	}

	public async Task<IResult> RegisterAsync(
		ISender sender,
		HttpContext httpContext,
		[FromBody] RegisterCommand command)
	{
		var result = await sender.Send(command);

		SetRefreshTokenCookie(
			httpContext,
			result.RefreshToken,
			result.RefreshTokenExpiresAt);

		return Results.Ok(
			new AuthenticationResponse(
				result.AccessToken,
				result.AccessTokenExpiresAt,
				result.UserId,
				result.Username));
	}

	public async Task<IResult> LoginAsync(
		ISender sender,
		HttpContext httpContext,
		[FromBody] LoginCommand command)
	{
		var result = await sender.Send(command);

		SetRefreshTokenCookie(
			httpContext,
			result.RefreshToken,
			result.RefreshTokenExpiresAt);

		return Results.Ok(
			new AuthenticationResponse(
				result.AccessToken,
				result.AccessTokenExpiresAt,
				result.UserId,
				result.Username));
	}

	public async Task<IResult> LogoutAsync(
	ISender sender,
	HttpContext httpContext)
	{
		if (httpContext.Request.Cookies.TryGetValue(
			"refresh_token",
			out var refreshToken))
			await sender.Send(
				new LogoutCommand(refreshToken));

		httpContext.Response.Cookies.Delete(
			"refresh_token",
			new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				Path = "/"
			});

		return Results.NoContent();
	}

	public async Task<IResult> RenewAsync(
		ISender sender,
		HttpContext httpContext)
	{
		if (!httpContext.Request.Cookies.TryGetValue(
			"refresh_token",
			out var refreshToken))
			return Results.Unauthorized();

		var result = await sender.Send(
			new RenewCommand(refreshToken));

		SetRefreshTokenCookie(
			httpContext,
			result.RefreshToken,
			result.RefreshTokenExpiresAt);

		return Results.Ok(
			new AuthenticationResponse(
				result.AccessToken,
				result.AccessTokenExpiresAt,
				result.UserId,
				result.Username));
	}

	private static void SetRefreshTokenCookie(
		HttpContext httpContext,
		string refreshToken,
		DateTimeOffset expiresAt) =>
		httpContext.Response.Cookies.Append(
			"refresh_token",
			refreshToken,
			new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				Expires = expiresAt,
				Path = "/"
			});
}
