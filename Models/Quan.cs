using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace test_identityBHSC.Models;

[Table("Quan")]
public partial class Quan
{
    [Key]
    [Column("QuanID")]
    public int QuanId { get; set; }

    [StringLength(100)]
    public string TenQuan { get; set; } = null!;

    [Column("ThanhPhoID")]
    public int ThanhPhoId { get; set; }

    [InverseProperty("Quan")]
    public virtual ICollection<Phuong> Phuongs { get; set; } = new List<Phuong>();

    [ForeignKey("ThanhPhoId")]
    [InverseProperty("Quans")]
    public virtual ThanhPho ThanhPho { get; set; } = null!;
}
