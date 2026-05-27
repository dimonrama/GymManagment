using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagment.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMemberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "JoinDate",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Members");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Trainers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "Trainers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Members",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinDate",
                table: "Members",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Members",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
