using AutenticationService.Domain.Common;

namespace AutenticationService.Domain.Entities;

public class UserSession : BaseEntity
{
	public Guid UserId { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset ExpiresAt { get; set; }
	public DateTimeOffset? RevokedAt { get; set; }
	public DateTimeOffset? LastUsedAt { get; set; }
	public string? IpAddress { get; set; }
	public string? UserAgent { get; set; }
	public User User { get; set; } = null!;
	public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
