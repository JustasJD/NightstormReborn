using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nightstorm.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentSystemToItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConstitutionBonus",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DexterityBonus",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IntelligenceBonus",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "LuckBonus",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SpiritBonus",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "StrengthBonus",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "WisdomBonus",
                table: "Items",
                newName: "Grade");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Items",
                newName: "MinDamage");

            migrationBuilder.AddColumn<int>(
                name: "ArmorMaterial",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArmorValue",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "AttackSpeed",
                table: "Items",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 1.0m);

            migrationBuilder.AddColumn<int>(
                name: "BaseValue",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BlockChance",
                table: "Items",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BonusConstitution",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BonusDexterity",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BonusIntelligence",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BonusLuck",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BonusMaxHealth",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BonusMaxMana",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BonusSpirit",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BonusStrength",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BonusWisdom",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CriticalChance",
                table: "Items",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentSlot",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HealthRestore",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IconId",
                table: "Items",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsQuestItem",
                table: "Items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSoulbound",
                table: "Items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MagicResistance",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManaRestore",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxDamage",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequiredClass",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeaponType",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_Grade",
                table: "Items",
                column: "Grade");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Type_Grade_Rarity",
                table: "Items",
                columns: new[] { "Type", "Grade", "Rarity" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_Grade",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_Type_Grade_Rarity",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ArmorMaterial",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ArmorValue",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "AttackSpeed",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BaseValue",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BlockChance",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BonusConstitution",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BonusDexterity",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BonusIntelligence",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BonusLuck",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BonusMaxHealth",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BonusMaxMana",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BonusSpirit",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BonusStrength",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BonusWisdom",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CriticalChance",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "EquipmentSlot",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "HealthRestore",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IconId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsQuestItem",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsSoulbound",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MagicResistance",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ManaRestore",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MaxDamage",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "RequiredClass",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "WeaponType",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "MinDamage",
                table: "Items",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "Grade",
                table: "Items",
                newName: "WisdomBonus");

            migrationBuilder.AddColumn<int>(
                name: "ConstitutionBonus",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DexterityBonus",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IntelligenceBonus",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LuckBonus",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpiritBonus",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StrengthBonus",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
