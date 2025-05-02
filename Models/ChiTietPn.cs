using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace test_identityBHSC.Models;

[PrimaryKey("PhieuNhapId", "LinhKienId")]
[Table("ChiTietPN")]
public partial class ChiTietPn
{
    [Key]
    [Column("PhieuNhapID")]
    public int PhieuNhapId { get; set; }

    [Key]
    [Column("LinhKienID")]
    public int LinhKienId { get; set; }

    public int SoLuong { get; set; }

    [ForeignKey("LinhKienId")]
    [InverseProperty("ChiTietPns")]
    public virtual LinhKien LinhKien { get; set; } = null!;

    [ForeignKey("PhieuNhapId")]
    [InverseProperty("ChiTietPns")]
    public virtual PhieuNhap PhieuNhap { get; set; } = null!;
}
