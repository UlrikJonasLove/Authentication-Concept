using AutenticationService.Domain.Common;

namespace AutenticationService.Domain.Entities;

public class User : BaseEntity
{
	public string Username { get; set; } = null!;
	public string NormalizedUsername { get; set; } = null!;
	public string PasswordHash { get; set; } = null!;
	public int FailedLoginAttempts { get; set; }
	public DateTimeOffset? LockoutEnd { get; set; }
	public bool IsActive { get; set; } = true;
	public DateTimeOffset CreatedAt { get; set; }
	public ICollection<UserSession> Sessions { get; set; } = [];
}
