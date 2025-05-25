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
    [Display(Name = "Mã phường")]
    public string PhuongId { get; set; }

    [Required(ErrorMessage = "Tên phường là bắt buộc")]
    [StringLength(100)]
    [Display(Name = "Tên phường")]
    public string TenPhuong { get; set; } = null!;

    [Required(ErrorMessage = "Quận là bắt buộc")]
    [Column("QuanID")]
    [StringLength(20)]
    [Unicode(false)]
    [Display(Name = "Mã quận")]
    public string QuanId { get; set; }

    [InverseProperty("Phuong")]
    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();

    [InverseProperty("Phuong")]
    public virtual ICollection<PhieuSuaChua> PhieuSuaChuas { get; set; } = new List<PhieuSuaChua>();

    [ForeignKey("QuanId")]
    [InverseProperty("Phuongs")]
    [Display(Name = "Quận")]
    public virtual Quan? Quan { get; set; }
}
