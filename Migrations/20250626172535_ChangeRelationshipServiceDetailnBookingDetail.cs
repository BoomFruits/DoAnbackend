using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationshipServiceDetailnBookingDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceDetail_BookingDetails_BookingId_RoomId",
                table: "ServiceDetail");

            migrationBuilder.UpdateData(
                table: "ServiceDetail",
                keyColumn: "Id",
                keyValue: 1,
                column: "RoomId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "ServiceDetail",
                keyColumn: "Id",
                keyValue: 2,
                column: "RoomId",
                value: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceDetail_BookingDetails_BookingId_RoomId",
                table: "ServiceDetail",
                columns: new[] { "BookingId", "RoomId" },
                principalTable: "BookingDetails",
                principalColumns: new[] { "BookingId", "RoomId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceDetail_BookingDetails_BookingId_RoomId",
                table: "ServiceDetail");

            migrationBuilder.UpdateData(
                table: "ServiceDetail",
                keyColumn: "Id",
                keyValue: 1,
                column: "RoomId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ServiceDetail",
                keyColumn: "Id",
                keyValue: 2,
                column: "RoomId",
                value: 3);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceDetail_BookingDetails_BookingId_RoomId",
                table: "ServiceDetail",
                columns: new[] { "BookingId", "RoomId" },
                principalTable: "BookingDetails",
                principalColumns: new[] { "BookingId", "RoomId" },
                onDelete: ReferentialAction.SetNull);
        }
    }
}
