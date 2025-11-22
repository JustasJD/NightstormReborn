using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nightstorm.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGameEngineEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CombatInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NightstormEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CurrentTurn = table.Column<int>(type: "integer", nullable: false),
                    CurrentActorId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlayerVictory = table.Column<bool>(type: "boolean", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatInstances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TravelLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginZoneId = table.Column<Guid>(type: "uuid", nullable: false),
                    DestinationZoneId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntryFeePaid = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelLogs_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TravelLogs_Zone_DestinationZoneId",
                        column: x => x.DestinationZoneId,
                        principalTable: "Zone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TravelLogs_Zone_OriginZoneId",
                        column: x => x.OriginZoneId,
                        principalTable: "Zone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZoneTreasuries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZoneId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentGold = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    TotalCollected = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    TotalWithdrawn = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    LastWithdrawalAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastWithdrawnByGuildId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneTreasuries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZoneTreasuries_Guilds_LastWithdrawnByGuildId",
                        column: x => x.LastWithdrawnByGuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ZoneTreasuries_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CombatLogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CombatInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Turn = table.Column<int>(type: "integer", nullable: false),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ActionType = table.Column<int>(type: "integer", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Damage = table.Column<int>(type: "integer", nullable: true),
                    IsCritical = table.Column<bool>(type: "boolean", nullable: true),
                    IsMiss = table.Column<bool>(type: "boolean", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatLogEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatLogEntries_CombatInstances_CombatInstanceId",
                        column: x => x.CombatInstanceId,
                        principalTable: "CombatInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CombatParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CombatInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CurrentHealth = table.Column<int>(type: "integer", nullable: false),
                    MaxHealth = table.Column<int>(type: "integer", nullable: false),
                    IsAlive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    InitiativeRoll = table.Column<int>(type: "integer", nullable: false),
                    TurnOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatParticipants_Characters_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CombatParticipants_CombatInstances_CombatInstanceId",
                        column: x => x.CombatInstanceId,
                        principalTable: "CombatInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CombatParticipants_Monsters_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Monsters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NightstormEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZoneId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RegistrationOpenedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RegistrationClosesAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: false, defaultValue: 10),
                    CombatInstanceId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlayerVictory = table.Column<bool>(type: "boolean", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RaidPenaltyAmount = table.Column<long>(type: "bigint", nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NightstormEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NightstormEvents_CombatInstances_CombatInstanceId",
                        column: x => x.CombatInstanceId,
                        principalTable: "CombatInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NightstormEvents_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TreasuryTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZoneTreasuryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: true),
                    GuildId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreasuryTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreasuryTransactions_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreasuryTransactions_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreasuryTransactions_ZoneTreasuries_ZoneTreasuryId",
                        column: x => x.ZoneTreasuryId,
                        principalTable: "ZoneTreasuries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Location = table.Column<int>(type: "integer", nullable: false),
                    CurrentZoneId = table.Column<Guid>(type: "uuid", nullable: false),
                    DestinationZoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    TravelStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TravelEndsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CurrentCombatId = table.Column<Guid>(type: "uuid", nullable: true),
                    RegisteredEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerStates_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerStates_CombatInstances_CurrentCombatId",
                        column: x => x.CurrentCombatId,
                        principalTable: "CombatInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PlayerStates_NightstormEvents_RegisteredEventId",
                        column: x => x.RegisteredEventId,
                        principalTable: "NightstormEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PlayerStates_Zone_CurrentZoneId",
                        column: x => x.CurrentZoneId,
                        principalTable: "Zone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerStates_Zone_DestinationZoneId",
                        column: x => x.DestinationZoneId,
                        principalTable: "Zone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CombatInstances_NightstormEventId",
                table: "CombatInstances",
                column: "NightstormEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CombatInstances_Status",
                table: "CombatInstances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CombatInstances_Status_CurrentTurn",
                table: "CombatInstances",
                columns: new[] { "Status", "CurrentTurn" });

            migrationBuilder.CreateIndex(
                name: "IX_CombatLogEntries_ActorId",
                table: "CombatLogEntries",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatLogEntries_CombatInstanceId",
                table: "CombatLogEntries",
                column: "CombatInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatLogEntries_CombatInstanceId_Turn",
                table: "CombatLogEntries",
                columns: new[] { "CombatInstanceId", "Turn" });

            migrationBuilder.CreateIndex(
                name: "IX_CombatParticipants_CombatInstanceId",
                table: "CombatParticipants",
                column: "CombatInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatParticipants_CombatInstanceId_TurnOrder",
                table: "CombatParticipants",
                columns: new[] { "CombatInstanceId", "TurnOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CombatParticipants_CombatInstanceId_Type",
                table: "CombatParticipants",
                columns: new[] { "CombatInstanceId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_CombatParticipants_EntityId",
                table: "CombatParticipants",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_NightstormEvents_CombatInstanceId",
                table: "NightstormEvents",
                column: "CombatInstanceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NightstormEvents_ScheduledAt",
                table: "NightstormEvents",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_NightstormEvents_Status",
                table: "NightstormEvents",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_NightstormEvents_Status_ScheduledAt",
                table: "NightstormEvents",
                columns: new[] { "Status", "ScheduledAt" });

            migrationBuilder.CreateIndex(
                name: "IX_NightstormEvents_ZoneId",
                table: "NightstormEvents",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_NightstormEvents_ZoneId_Status",
                table: "NightstormEvents",
                columns: new[] { "ZoneId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_CharacterId",
                table: "PlayerStates",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_CurrentCombatId",
                table: "PlayerStates",
                column: "CurrentCombatId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_CurrentZoneId_Location",
                table: "PlayerStates",
                columns: new[] { "CurrentZoneId", "Location" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_DestinationZoneId",
                table: "PlayerStates",
                column: "DestinationZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_Location",
                table: "PlayerStates",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_RegisteredEventId",
                table: "PlayerStates",
                column: "RegisteredEventId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelLogs_CharacterId",
                table: "TravelLogs",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelLogs_CharacterId_Status",
                table: "TravelLogs",
                columns: new[] { "CharacterId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TravelLogs_DestinationZoneId",
                table: "TravelLogs",
                column: "DestinationZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelLogs_OriginZoneId",
                table: "TravelLogs",
                column: "OriginZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelLogs_Status_StartedAt",
                table: "TravelLogs",
                columns: new[] { "Status", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TreasuryTransactions_CharacterId",
                table: "TreasuryTransactions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_TreasuryTransactions_CreatedAt",
                table: "TreasuryTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TreasuryTransactions_GuildId",
                table: "TreasuryTransactions",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_TreasuryTransactions_ZoneTreasuryId",
                table: "TreasuryTransactions",
                column: "ZoneTreasuryId");

            migrationBuilder.CreateIndex(
                name: "IX_TreasuryTransactions_ZoneTreasuryId_Type",
                table: "TreasuryTransactions",
                columns: new[] { "ZoneTreasuryId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_ZoneTreasuries_LastWithdrawalAt",
                table: "ZoneTreasuries",
                column: "LastWithdrawalAt");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneTreasuries_LastWithdrawnByGuildId",
                table: "ZoneTreasuries",
                column: "LastWithdrawnByGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneTreasuries_ZoneId",
                table: "ZoneTreasuries",
                column: "ZoneId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CombatLogEntries");

            migrationBuilder.DropTable(
                name: "CombatParticipants");

            migrationBuilder.DropTable(
                name: "PlayerStates");

            migrationBuilder.DropTable(
                name: "TravelLogs");

            migrationBuilder.DropTable(
                name: "TreasuryTransactions");

            migrationBuilder.DropTable(
                name: "NightstormEvents");

            migrationBuilder.DropTable(
                name: "ZoneTreasuries");

            migrationBuilder.DropTable(
                name: "CombatInstances");
        }
    }
}
