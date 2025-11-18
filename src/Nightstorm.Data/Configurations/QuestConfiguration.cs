using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Quest entity.
/// </summary>
public class QuestConfiguration : IEntityTypeConfiguration<Quest>
{
    public void Configure(EntityTypeBuilder<Quest> builder)
    {
        builder.ToTable("Quests");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(q => q.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(q => q.RequiredLevel)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(q => q.ExperienceReward)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(q => q.GoldReward)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(q => q.ObjectiveDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(q => q.ObjectiveCount)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(q => q.IsRepeatable)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(q => q.Name);

        builder.HasIndex(q => q.RequiredLevel);

        builder.HasIndex(q => q.IsRepeatable);

        // Relationships
        builder.HasOne(q => q.ItemReward)
            .WithMany()
            .HasForeignKey(q => q.ItemRewardId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
