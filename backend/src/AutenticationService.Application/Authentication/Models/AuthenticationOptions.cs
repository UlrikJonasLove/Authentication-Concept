namespace AutenticationService.Application.Authentication.Models;

public class AuthenticationOptions
{
	public const string SectionName = "Authentication";
	public int RefreshTokenLifetimeDays { get; set; } = 7;
	public int SessionLifetimeDays { get; set; } = 30;
	public int MaxFailedLoginAttempts { get; set; } = 5;
	public int LockoutDurationMinutes { get; set; } = 5;
}
