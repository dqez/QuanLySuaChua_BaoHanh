using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLySuaChua_BaoHanh.Models;

[Table("DanhMuc")]
public partial class DanhMuc
{
    [Key]
    [Column("DanhMucID")]
    [StringLength(20)]
    [Unicode(false)]
    public string DanhMucId { get; set; }

    [StringLength(255)]
    public string TenDanhMuc { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string PhanLoai { get; set; } = null!;

    [InverseProperty("DanhMuc")]
    public virtual ICollection<LinhKien> LinhKiens { get; set; } = new List<LinhKien>();

    [InverseProperty("DanhMuc")]
    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
