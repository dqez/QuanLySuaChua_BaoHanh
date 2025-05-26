using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLySuaChua_BaoHanh.Migrations
{
    /// <inheritdoc />
    public partial class intIDtostringID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhMuc",
                columns: table => new
                {
                    DanhMucID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhanLoai = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DanhMuc__1C53BA7B3EE29315", x => x.DanhMucID);
                });

            migrationBuilder.CreateTable(
                name: "ThanhPho",
                columns: table => new
                {
                    ThanhPhoID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    TenThanhPho = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ThanhPho__6E7123A00A09EC48", x => x.ThanhPhoID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinhKien",
                columns: table => new
                {
                    LinhKienID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    DanhMucID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    TenLinhKien = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PhamViSuDung = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LinhKien__04269C40A0329C9C", x => x.LinhKienID);
                    table.ForeignKey(
                        name: "FK__LinhKien__DanhMu__47DBAE45",
                        column: x => x.DanhMucID,
                        principalTable: "DanhMuc",
                        principalColumn: "DanhMucID");
                });

            migrationBuilder.CreateTable(
                name: "Quan",
                columns: table => new
                {
                    QuanID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    TenQuan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ThanhPhoID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Quan__B0ADAE910FF0D4CB", x => x.QuanID);
                    table.ForeignKey(
                        name: "FK__Quan__ThanhPhoID__3D5E1FD2",
                        column: x => x.ThanhPhoID,
                        principalTable: "ThanhPho",
                        principalColumn: "ThanhPhoID");
                });

            migrationBuilder.CreateTable(
                name: "Phuong",
                columns: table => new
                {
                    PhuongID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    TenPhuong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QuanID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Phuong__7FC46F50B400BD51", x => x.PhuongID);
                    table.ForeignKey(
                        name: "FK__Phuong__QuanID__403A8C7D",
                        column: x => x.QuanID,
                        principalTable: "Quan",
                        principalColumn: "QuanID");
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    NguoiDungID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    TaiKhoan = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                    MatKhau = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Email = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    Sdt = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    PhuongID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    VaiTro = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.NguoiDungID);
                    table.ForeignKey(
                        name: "FK_NguoiDung_Phuong_PhuongID",
                        column: x => x.PhuongID,
                        principalTable: "Phuong",
                        principalColumn: "PhuongID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "varchar(20)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_NguoiDung_UserId",
                        column: x => x.UserId,
                        principalTable: "NguoiDung",
                        principalColumn: "NguoiDungID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_NguoiDung_UserId",
                        column: x => x.UserId,
                        principalTable: "NguoiDung",
                        principalColumn: "NguoiDungID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(20)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_NguoiDung_UserId",
                        column: x => x.UserId,
                        principalTable: "NguoiDung",
                        principalColumn: "NguoiDungID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(20)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_NguoiDung_UserId",
                        column: x => x.UserId,
                        principalTable: "NguoiDung",
                        principalColumn: "NguoiDungID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuNhap",
                columns: table => new
                {
                    PhieuNhapID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    KhoID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    TrangThai = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PhieuNha__DE3A3882C1EFE80A", x => x.PhieuNhapID);
                    table.ForeignKey(
                        name: "FK__PhieuNhap__KhoID__5AEE82B9",
                        column: x => x.KhoID,
                        principalTable: "NguoiDung",
                        principalColumn: "NguoiDungID");
                });

            migrationBuilder.CreateTable(
                name: "PhieuSuaChua",
                columns: table => new
                {
                    PhieuSuaChuaID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    KhachHangID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    KyThuatID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    PhuongID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    MoTaKhachHang = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TrangThai = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NgayGui = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false, defaultValueSql: "(getdate())"),
                    NgayHen = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    NgayTra = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    DiaChiNhanTraSanPham = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    KhoangCach = table.Column<double>(type: "float", nullable: true),
                    PhiVanChuyen = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    PhuongThucThanhToan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TongTien = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PhieuSua__B46641E6385CDB55", x => x.PhieuSuaChuaID);
                    table.ForeignKey(
                        name: "FK__PhieuSuaC__Khach__5070F446",
                        column: x => x.KhachHangID,
                        principalTable: "NguoiDung",
                        principalColumn: "NguoiDungID");
                    table.ForeignKey(
                        name: "FK__PhieuSuaC__KyThu__5165187F",
                        column: x => x.KyThuatID,
                        principalTable: "NguoiDung",
                        principalColumn: "NguoiDungID");
                    table.ForeignKey(
                        name: "FK__PhieuSuaC__Phuon__52593CB8",
                        column: x => x.PhuongID,
                        principalTable: "Phuong",
                        principalColumn: "PhuongID");
                });

            migrationBuilder.CreateTable(
                name: "PhieuXuat",
                columns: table => new
                {
                    PhieuXuatID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    KhoID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    NgayXuat = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    TrangThai = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PhieuXua__2A2FDF35F3666748", x => x.PhieuXuatID);
                    table.ForeignKey(
                        name: "FK__PhieuXuat__KhoID__6383C8BA",
                        column: x => x.KhoID,
                        principalTable: "NguoiDung",
                        principalColumn: "NguoiDungID");
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    SanPhamID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    KhachHangID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    DanhMucID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    MaBH = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    TenSanPham = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayMua = table.Column<DateOnly>(type: "date", nullable: false),
                    ThoiGianBaoHanh = table.Column<int>(type: "int", nullable: false),
                    NgayHetHanBH = table.Column<DateOnly>(type: "date", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SanPham__05180FF4A990F499", x => x.SanPhamID);
                    table.ForeignKey(
                        name: "FK__SanPham__DanhMuc__4BAC3F29",
                        column: x => x.DanhMucID,
                        principalTable: "DanhMuc",
                        principalColumn: "DanhMucID");
                    table.ForeignKey(
                        name: "FK__SanPham__KhachHa__4AB81AF0",
                        column: x => x.KhachHangID,
                        principalTable: "NguoiDung",
                        principalColumn: "NguoiDungID");
                });

            migrationBuilder.CreateTable(
                name: "ThongBao",
                columns: table => new
                {
                    ThongBaoID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    NguoiDungID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TrangThai = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "ChuaDoc"),
                    NgayTao = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ThongBao__6E51A53BDBDCBFB7", x => x.ThongBaoID);
                    table.ForeignKey(
                        name: "FK__ThongBao__NguoiD__571DF1D5",
                        column: x => x.NguoiDungID,
                        principalTable: "NguoiDung",
                        principalColumn: "NguoiDungID");
                });

            migrationBuilder.CreateTable(
                name: "TinNhan",
                columns: table => new
                {
                    TinNhanID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    NguoiGuiID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    NguoiNhanID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ThoiGianGui = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    TrangThai = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TinNhan__40CE177CC404C147", x => x.TinNhanID);
                    table.ForeignKey(
                        name: "FK__TinNhan__NguoiGu__5EBF139D",
                        column: x => x.NguoiGuiID,
                        principalTable: "NguoiDung",
                        principalColumn: "NguoiDungID");
                    table.ForeignKey(
                        name: "FK__TinNhan__NguoiNh__5FB337D6",
                        column: x => x.NguoiNhanID,
                        principalTable: "NguoiDung",
                        principalColumn: "NguoiDungID");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPN",
                columns: table => new
                {
                    PhieuNhapID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    LinhKienID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietP__CE785146FEE0EA2E", x => new { x.PhieuNhapID, x.LinhKienID });
                    table.ForeignKey(
                        name: "FK__ChiTietPN__LinhK__6B24EA82",
                        column: x => x.LinhKienID,
                        principalTable: "LinhKien",
                        principalColumn: "LinhKienID");
                    table.ForeignKey(
                        name: "FK__ChiTietPN__Phieu__6A30C649",
                        column: x => x.PhieuNhapID,
                        principalTable: "PhieuNhap",
                        principalColumn: "PhieuNhapID");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPX",
                columns: table => new
                {
                    PhieuXuatID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    PhieuSuaChuaID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietP__5169BB2B3005A347", x => new { x.PhieuXuatID, x.PhieuSuaChuaID });
                    table.ForeignKey(
                        name: "FK__ChiTietPX__Phieu__66603565",
                        column: x => x.PhieuXuatID,
                        principalTable: "PhieuXuat",
                        principalColumn: "PhieuXuatID");
                    table.ForeignKey(
                        name: "FK__ChiTietPX__Phieu__6754599E",
                        column: x => x.PhieuSuaChuaID,
                        principalTable: "PhieuSuaChua",
                        principalColumn: "PhieuSuaChuaID");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietSuaChua",
                columns: table => new
                {
                    ChiTietID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    SanPhamID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    PhieuSuaChuaID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    LinhKienID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    LoaiDon = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    SoLuongLinhKien = table.Column<int>(type: "int", nullable: false),
                    MoTaKhachHang = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DanhGiaKyThuat = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietS__B117E9EAF0334BA5", x => x.ChiTietID);
                    table.ForeignKey(
                        name: "FK__ChiTietSu__LinhK__70DDC3D8",
                        column: x => x.LinhKienID,
                        principalTable: "LinhKien",
                        principalColumn: "LinhKienID");
                    table.ForeignKey(
                        name: "FK__ChiTietSu__Phieu__6FE99F9F",
                        column: x => x.PhieuSuaChuaID,
                        principalTable: "PhieuSuaChua",
                        principalColumn: "PhieuSuaChuaID");
                    table.ForeignKey(
                        name: "FK__ChiTietSu__SanPh__6EF57B66",
                        column: x => x.SanPhamID,
                        principalTable: "SanPham",
                        principalColumn: "SanPhamID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPN_LinhKienID",
                table: "ChiTietPN",
                column: "LinhKienID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPX_PhieuSuaChuaID",
                table: "ChiTietPX",
                column: "PhieuSuaChuaID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietSuaChua_LinhKienID",
                table: "ChiTietSuaChua",
                column: "LinhKienID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietSuaChua_PhieuSuaChuaID",
                table: "ChiTietSuaChua",
                column: "PhieuSuaChuaID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietSuaChua_SanPhamID",
                table: "ChiTietSuaChua",
                column: "SanPhamID");

            migrationBuilder.CreateIndex(
                name: "IX_LinhKien_DanhMucID",
                table: "LinhKien",
                column: "DanhMucID");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "NguoiDung",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_PhuongID",
                table: "NguoiDung",
                column: "PhuongID");

            migrationBuilder.CreateIndex(
                name: "UQ__NguoiDun__D5B8C7F043064735",
                table: "NguoiDung",
                column: "TaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "NguoiDung",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_KhoID",
                table: "PhieuNhap",
                column: "KhoID");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSuaChua_KhachHangID",
                table: "PhieuSuaChua",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSuaChua_KyThuatID",
                table: "PhieuSuaChua",
                column: "KyThuatID");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuSuaChua_PhuongID",
                table: "PhieuSuaChua",
                column: "PhuongID");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuXuat_KhoID",
                table: "PhieuXuat",
                column: "KhoID");

            migrationBuilder.CreateIndex(
                name: "IX_Phuong_QuanID",
                table: "Phuong",
                column: "QuanID");

            migrationBuilder.CreateIndex(
                name: "IX_Quan_ThanhPhoID",
                table: "Quan",
                column: "ThanhPhoID");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_DanhMucID",
                table: "SanPham",
                column: "DanhMucID");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_KhachHangID",
                table: "SanPham",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_NguoiDungID",
                table: "ThongBao",
                column: "NguoiDungID");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhan_NguoiGuiID",
                table: "TinNhan",
                column: "NguoiGuiID");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhan_NguoiNhanID",
                table: "TinNhan",
                column: "NguoiNhanID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ChiTietPN");

            migrationBuilder.DropTable(
                name: "ChiTietPX");

            migrationBuilder.DropTable(
                name: "ChiTietSuaChua");

            migrationBuilder.DropTable(
                name: "ThongBao");

            migrationBuilder.DropTable(
                name: "TinNhan");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "PhieuNhap");

            migrationBuilder.DropTable(
                name: "PhieuXuat");

            migrationBuilder.DropTable(
                name: "LinhKien");

            migrationBuilder.DropTable(
                name: "PhieuSuaChua");

            migrationBuilder.DropTable(
                name: "SanPham");

            migrationBuilder.DropTable(
                name: "DanhMuc");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "Phuong");

            migrationBuilder.DropTable(
                name: "Quan");

            migrationBuilder.DropTable(
                name: "ThanhPho");
        }
    }
}
