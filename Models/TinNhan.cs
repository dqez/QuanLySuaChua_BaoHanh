using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace test_identityBHSC.Models;

[Table("TinNhan")]
public partial class TinNhan
{
    [Key]
    [Column("TinNhanID")]
    public int TinNhanId { get; set; }

    [Column("NguoiGuiID")]
    public int NguoiGuiId { get; set; }

    [Column("NguoiNhanID")]
    public int NguoiNhanId { get; set; }

    [StringLength(2000)]
    public string? NoiDung { get; set; }

    [Precision(0)]
    public DateTime ThoiGianGui { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? TrangThai { get; set; }

    [ForeignKey("NguoiGuiId")]
    [InverseProperty("TinNhanNguoiGuis")]
    public virtual NguoiDung NguoiGui { get; set; } = null!;

    [ForeignKey("NguoiNhanId")]
    [InverseProperty("TinNhanNguoiNhans")]
    public virtual NguoiDung NguoiNhan { get; set; } = null!;
}
