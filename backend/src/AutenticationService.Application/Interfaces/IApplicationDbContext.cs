using Microsoft.EntityFrameworkCore;
using AutenticationService.Domain.Entities;

namespace AutenticationService.Application.Interfaces;

public interface IApplicationDbContext
{
	public DbSet<Employee> Employees { get; }
	public DbSet<RefreshToken> RefreshTokens { get; }
	public DbSet<User> Users { get; }
	public DbSet<UserSession> UserSessions { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
