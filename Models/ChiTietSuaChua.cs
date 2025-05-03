using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLySuaChua_BaoHanh.Models;

[Table("ChiTietSuaChua")]
public partial class ChiTietSuaChua
{
    [Key]
    [Column("ChiTietID")]
    public int ChiTietId { get; set; }

    [Column("SanPhamID")]
    public int SanPhamId { get; set; }

    [Column("PhieuSuaChuaID")]
    public int PhieuSuaChuaId { get; set; }

    [Column("LinhKienID")]
    public int LinhKienId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? LoaiDon { get; set; }

    public int SoLuongLinhKien { get; set; }

    [StringLength(1000)]
    public string? MoTaKhachHang { get; set; }

    [StringLength(500)]
    public string? DanhGiaKyThuat { get; set; }

    [ForeignKey("LinhKienId")]
    [InverseProperty("ChiTietSuaChuas")]
    public virtual LinhKien LinhKien { get; set; } = null!;

    [ForeignKey("PhieuSuaChuaId")]
    [InverseProperty("ChiTietSuaChuas")]
    public virtual PhieuSuaChua PhieuSuaChua { get; set; } = null!;

    [ForeignKey("SanPhamId")]
    [InverseProperty("ChiTietSuaChuas")]
    public virtual SanPham SanPham { get; set; } = null!;
}
