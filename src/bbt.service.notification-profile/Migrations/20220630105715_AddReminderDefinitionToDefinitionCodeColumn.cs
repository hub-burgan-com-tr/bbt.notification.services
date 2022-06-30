using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Profile.Migrations
{
    public partial class AddReminderDefinitionToDefinitionCodeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReminderDefinition",
                table: "ReminderDefinition");

            migrationBuilder.RenameTable(
                name: "ReminderDefinition",
                newName: "ReminderDefinitions");

            migrationBuilder.AddColumn<string>(
                name: "DefinitionCode",
                table: "ReminderDefinitions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReminderDefinitions",
                table: "ReminderDefinitions",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReminderDefinitions",
                table: "ReminderDefinitions");

            migrationBuilder.DropColumn(
                name: "DefinitionCode",
                table: "ReminderDefinitions");

            migrationBuilder.RenameTable(
                name: "ReminderDefinitions",
                newName: "ReminderDefinition");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReminderDefinition",
                table: "ReminderDefinition",
                column: "Id");
        }
    }
}
