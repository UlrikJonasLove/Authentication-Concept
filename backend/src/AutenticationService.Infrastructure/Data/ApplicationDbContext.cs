using Microsoft.EntityFrameworkCore;
using System.Reflection;
using AutenticationService.Application.Interfaces;
using AutenticationService.Domain.Entities;

namespace AutenticationService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
	public DbSet<Employee> Employees => Set<Employee>();
	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
	public DbSet<User> Users => Set<User>();
	public DbSet<UserSession> UserSessions => Set<UserSession>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
}
