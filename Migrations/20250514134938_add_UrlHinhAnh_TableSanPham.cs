using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLySuaChua_BaoHanh.Migrations
{
    /// <inheritdoc />
    public partial class add_UrlHinhAnh_TableSanPham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlHinhAnh",
                table: "SanPham",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlHinhAnh",
                table: "SanPham");
        }
    }
}
