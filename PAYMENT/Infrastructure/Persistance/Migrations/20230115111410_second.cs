using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentPersistance.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrderId",
                schema: "paymentdb",
                table: "PaymentOutboxes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                schema: "paymentdb",
                table: "PaymentOutboxes");
        }
    }
}
