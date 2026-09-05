using AutenticationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutenticationService.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
	public void Configure(EntityTypeBuilder<RefreshToken> builder)
	{
		builder.ToTable("RefreshToken", "dbo");

		builder.HasKey(refreshToken => refreshToken.Id);

		builder.Property(refreshToken => refreshToken.RowVersion)
			.IsRowVersion();
		builder.Property(refreshToken => refreshToken.TokenHash)
			.IsRequired()
			.HasMaxLength(128);

		builder.HasIndex(refreshToken => refreshToken.TokenHash)
			.IsUnique();

		builder.HasOne(refreshToken => refreshToken.Session)
			.WithMany(userSession => userSession.RefreshTokens)
			.HasForeignKey(refreshToken => refreshToken.SessionId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(refreshToken => refreshToken.ReplacedByToken)
			.WithMany()
			.HasForeignKey(refreshToken => refreshToken.ReplacedByTokenId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
