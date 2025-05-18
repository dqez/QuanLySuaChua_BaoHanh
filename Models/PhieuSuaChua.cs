using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLySuaChua_BaoHanh.Models;

[Table("PhieuSuaChua")]
public partial class PhieuSuaChua
{
    [Key]
    [Column("PhieuSuaChuaID")]
    public int PhieuSuaChuaId { get; set; }

    [Column("KhachHangID")]
    public string KhachHangId { get; set; } = null!;

    [Column("KyThuatID")]
    public string? KyThuatId { get; set; } 

    [Column("PhuongID")]
    public int PhuongId { get; set; }

    [StringLength(500)]
    public string MoTaKhachHang { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? TrangThai { get; set; }

    [Precision(0)]
    public DateTime NgayGui { get; set; }

    [Precision(0)]
    public DateTime? NgayHen { get; set; }

    [Precision(0)]
    public DateTime? NgayTra { get; set; }

    [StringLength(500)]
    public string DiaChiNhanTraSanPham { get; set; } = null!;

    public double? KhoangCach { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal? PhiVanChuyen { get; set; }

    [Precision(0)]
    public DateTime? NgayThanhToan { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? PhuongThucThanhToan { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal? TongTien { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }

    [InverseProperty("PhieuSuaChua")]
    public virtual ICollection<ChiTietPx> ChiTietPxes { get; set; } = new List<ChiTietPx>();

    [InverseProperty("PhieuSuaChua")]
    public virtual ICollection<ChiTietSuaChua> ChiTietSuaChuas { get; set; } = new List<ChiTietSuaChua>();

    [ForeignKey("KhachHangId")]
    [InverseProperty("PhieuSuaChuaKhachHangs")]
    public virtual NguoiDung KhachHang { get; set; } = null!;

    [ForeignKey("KyThuatId")]
    [InverseProperty("PhieuSuaChuaKyThuats")]
    public virtual NguoiDung? KyThuat { get; set; }

    [ForeignKey("PhuongId")]
    [InverseProperty("PhieuSuaChuas")]
    public virtual Phuong Phuong { get; set; } = null!;
}
