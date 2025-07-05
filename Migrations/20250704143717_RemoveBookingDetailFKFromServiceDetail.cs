using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBookingDetailFKFromServiceDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceDetail_BookingDetails_BookingId_RoomId",
                table: "ServiceDetail");

            migrationBuilder.DropIndex(
                name: "IX_ServiceDetail_BookingId_RoomId",
                table: "ServiceDetail");

            migrationBuilder.DropColumn(
                name: "BuyDate",
                table: "ServiceDetail");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDetail_BookingId",
                table: "ServiceDetail",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceDetail_Bookings_BookingId",
                table: "ServiceDetail",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                   name: "Thumbnail",
                   table: "Products", // bảng chứa cột Thumbnail
                   type: "nchar(10)", // hoặc kiểu phù hợp
                   nullable: true);
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceDetail_Bookings_BookingId",
                table: "ServiceDetail");

            migrationBuilder.DropIndex(
                name: "IX_ServiceDetail_BookingId",
                table: "ServiceDetail");

            migrationBuilder.AddColumn<DateTime>(
                name: "BuyDate",
                table: "ServiceDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));


            migrationBuilder.UpdateData(
                table: "ServiceDetail",
                keyColumn: "Id",
                keyValue: 1,
                column: "BuyDate",
                value: new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ServiceDetail",
                keyColumn: "Id",
                keyValue: 2,
                column: "BuyDate",
                value: new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDetail_BookingId_RoomId",
                table: "ServiceDetail",
                columns: new[] { "BookingId", "RoomId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceDetail_BookingDetails_BookingId_RoomId",
                table: "ServiceDetail",
                columns: new[] { "BookingId", "RoomId" },
                principalTable: "BookingDetails",
                principalColumns: new[] { "BookingId", "RoomId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
