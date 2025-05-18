using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLySuaChua_BaoHanh.Models;

[Table("SanPham")]
public partial class SanPham
{
    [Key]
    [Column("SanPhamID")]
    public int SanPhamId { get; set; }

    [Column("KhachHangID")]
    public string KhachHangId { get; set; } = null!;


    [Column("DanhMucID")]
    public int DanhMucId { get; set; }

    [Column("MaBH")]
    [StringLength(100)]
    [Unicode(false)]
    public string? MaBh { get; set; }

    [StringLength(255)]
    public string TenSanPham { get; set; } = null!;

    public DateOnly NgayMua { get; set; }

    public int ThoiGianBaoHanh { get; set; }

    [Column("NgayHetHanBH")]
    public DateOnly NgayHetHanBh { get; set; }

    [StringLength(1000)]
    public string? MoTa { get; set; }

    [InverseProperty("SanPham")]
    public virtual ICollection<ChiTietSuaChua> ChiTietSuaChuas { get; set; } = new List<ChiTietSuaChua>();

    [ForeignKey("DanhMucId")]
    [InverseProperty("SanPhams")]
    public virtual DanhMuc DanhMuc { get; set; } = null!;

    [ForeignKey("KhachHangId")]
    [InverseProperty("SanPhams")]
    public virtual NguoiDung KhachHang { get; set; } = null!;
}
