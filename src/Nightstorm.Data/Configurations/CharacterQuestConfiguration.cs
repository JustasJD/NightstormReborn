using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nightstorm.Core.Entities;

namespace Nightstorm.Data.Configurations;

/// <summary>
/// Entity Framework configuration for CharacterQuest entity.
/// </summary>
public class CharacterQuestConfiguration : IEntityTypeConfiguration<CharacterQuest>
{
    public void Configure(EntityTypeBuilder<CharacterQuest> builder)
    {
        builder.ToTable("CharacterQuests");

        builder.HasKey(cq => cq.Id);

        builder.Property(cq => cq.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(cq => cq.CurrentProgress)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cq => cq.AcceptedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(cq => cq.CharacterId);

        builder.HasIndex(cq => cq.QuestId);

        builder.HasIndex(cq => new { cq.CharacterId, cq.QuestId });

        builder.HasIndex(cq => cq.Status);

        // Relationships
        builder.HasOne(cq => cq.Character)
            .WithMany(c => c.Quests)
            .HasForeignKey(cq => cq.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cq => cq.Quest)
            .WithMany(q => q.CharacterQuests)
            .HasForeignKey(cq => cq.QuestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
