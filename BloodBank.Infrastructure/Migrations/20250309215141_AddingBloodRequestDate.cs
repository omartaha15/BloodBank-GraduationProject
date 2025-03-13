﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodBank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingBloodRequestDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDate",
                table: "BloodRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "BloodRequests");
        }
    }
}
