using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLySuaChua_BaoHanh.Models;

[Table("Phuong")]
public partial class Phuong
{
    [Key]
    [Column("PhuongID")]
    [StringLength(20)]
    [Unicode(false)]
    public string PhuongId { get; set; }

    [StringLength(100)]
    public string TenPhuong { get; set; } = null!;

    [Column("QuanID")]
    [StringLength(20)]
    [Unicode(false)]
    public string QuanId { get; set; }

    [InverseProperty("Phuong")]
    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();

    [InverseProperty("Phuong")]
    public virtual ICollection<PhieuSuaChua> PhieuSuaChuas { get; set; } = new List<PhieuSuaChua>();

    [ForeignKey("QuanId")]
    [InverseProperty("Phuongs")]
    public virtual Quan Quan { get; set; } = null!;
}
