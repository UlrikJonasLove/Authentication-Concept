using AutenticationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutenticationService.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("User", "dbo");

		builder.HasKey(user => user.Id);

		builder.Property(user => user.Username)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(user => user.NormalizedUsername)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(user => user.PasswordHash)
			.IsRequired()
			.HasMaxLength(500);

		builder.HasIndex(user => user.NormalizedUsername)
			.IsUnique();

		builder.HasMany(user => user.Sessions)
			.WithOne(userSession => userSession.User)
			.HasForeignKey(userSession => userSession.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
