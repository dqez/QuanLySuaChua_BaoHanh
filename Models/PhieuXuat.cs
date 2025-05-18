using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLySuaChua_BaoHanh.Models;

[Table("PhieuXuat")]
public partial class PhieuXuat
{
    [Key]
    [Column("PhieuXuatID")]
    public int PhieuXuatId { get; set; }

    [Column("KhoID")]
    public string KhoId { get; set; }

    [Precision(0)]
    public DateTime NgayXuat { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal? TongTien { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? TrangThai { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }

    [InverseProperty("PhieuXuat")]
    public virtual ICollection<ChiTietPx> ChiTietPxes { get; set; } = new List<ChiTietPx>();

    [ForeignKey("KhoId")]
    [InverseProperty("PhieuXuats")]
    public virtual NguoiDung Kho { get; set; } = null!;
}
