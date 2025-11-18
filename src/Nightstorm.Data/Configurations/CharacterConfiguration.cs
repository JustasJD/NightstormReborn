using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Character entity.
/// </summary>
public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.ToTable("Characters");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.DiscordUserId)
            .IsRequired();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Class)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.Level)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(c => c.Experience)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.Gold)
            .IsRequired()
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(c => c.DiscordUserId)
            .IsUnique();

        builder.HasIndex(c => c.Name);

        builder.HasIndex(c => c.Level);

        builder.HasIndex(c => c.GuildId);

        // Relationships
        builder.HasOne(c => c.Guild)
            .WithMany(g => g.Members)
            .HasForeignKey(c => c.GuildId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Inventory)
            .WithOne(ci => ci.Character)
            .HasForeignKey(ci => ci.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Quests)
            .WithOne(cq => cq.Character)
            .HasForeignKey(cq => cq.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
