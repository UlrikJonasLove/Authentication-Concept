using AutenticationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutenticationService.Infrastructure.Data.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
	public void Configure(EntityTypeBuilder<UserSession> builder)
	{
		builder.ToTable("UserSession", "dbo");

		builder.HasKey(userSession => userSession.Id);

		builder.Property(userSession => userSession.IpAddress)
			.HasMaxLength(45);

		builder.Property(userSession => userSession.UserAgent)
			.HasMaxLength(500);

		builder.HasIndex(userSession => userSession.UserId);

		builder.HasOne(userSession => userSession.User)
			.WithMany(user => user.Sessions)
			.HasForeignKey(userSession => userSession.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(userSession => userSession.RefreshTokens)
			.WithOne(refreshToken => refreshToken.Session)
			.HasForeignKey(refreshToken => refreshToken.SessionId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
