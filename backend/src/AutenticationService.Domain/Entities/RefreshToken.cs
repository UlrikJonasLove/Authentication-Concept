using AutenticationService.Domain.Common;

namespace AutenticationService.Domain.Entities;

public class RefreshToken : BaseEntity
{
	public Guid SessionId { get; set; }
	public string TokenHash { get; set; } = null!;
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset ExpiresAt { get; set; }
	public DateTimeOffset? UsedAt { get; set; }
	public DateTimeOffset? RevokedAt { get; set; }
	public Guid? ReplacedByTokenId { get; set; }
	public byte[] RowVersion { get; set; } = [];
	public UserSession Session { get; set; } = null!;
	public RefreshToken? ReplacedByToken { get; set; }
}
