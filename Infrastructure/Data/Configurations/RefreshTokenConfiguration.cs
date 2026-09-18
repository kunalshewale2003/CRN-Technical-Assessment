using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(r => r.Token)
                .IsUnique();

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.ExpiresOn)
                .IsRequired();

            builder.Property(r => r.IsRevoked)
                .IsRequired();

            builder.HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
