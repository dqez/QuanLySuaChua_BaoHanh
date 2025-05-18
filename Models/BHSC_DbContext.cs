using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace QuanLySuaChua_BaoHanh.Models;

public partial class BHSC_DbContext : IdentityDbContext<NguoiDung, IdentityRole<string>, string>
{
    public BHSC_DbContext()
    {
    }

    public BHSC_DbContext(DbContextOptions<BHSC_DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietPn> ChiTietPns { get; set; }

    public virtual DbSet<ChiTietPx> ChiTietPxes { get; set; }

    public virtual DbSet<ChiTietSuaChua> ChiTietSuaChuas { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<LinhKien> LinhKiens { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<PhieuNhap> PhieuNhaps { get; set; }

    public virtual DbSet<PhieuSuaChua> PhieuSuaChuas { get; set; }

    public virtual DbSet<PhieuXuat> PhieuXuats { get; set; }

    public virtual DbSet<Phuong> Phuongs { get; set; }

    public virtual DbSet<Quan> Quans { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<ThanhPho> ThanhPhos { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    public virtual DbSet<TinNhan> TinNhans { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);



        //modelBuilder.Entity<IdentityUser>(b =>
        //{
        //    b.ToTable("NguoiDung");
        //    b.HasKey(u => u.Id);
        //    b.Property(u => u.Id).HasColumnName("NguoiDungID");
        //    b.Property(u => u.UserName).HasColumnName("TaiKhoan");
        //    b.Property(u => u.PasswordHash).HasColumnName("MatKhau");
        //    b.Property(u => u.Email).HasColumnName("Email");
        //    b.Property(u => u.PhoneNumber).HasColumnName("Sdt");
        //});


        modelBuilder.Entity<NguoiDung>(b =>
        {
            b.ToTable("NguoiDung");
            b.HasKey(u => u.Id);
            b.Property(u => u.Id).HasColumnName("NguoiDungID").ValueGeneratedOnAdd();
            b.Property(u => u.UserName).HasColumnName("TaiKhoan");
            b.Property(u => u.PasswordHash).HasColumnName("MatKhau");
            b.Property(u => u.Email).HasColumnName("Email");
            b.Property(u => u.PhoneNumber).HasColumnName("Sdt");
            b.Property(u => u.HoTen).HasColumnName("HoTen");
            b.Property(u => u.PhuongId).HasColumnName("PhuongID");
            b.Property(u => u.DiaChi).HasColumnName("DiaChi");
            b.HasOne(u => u.Phuong)
                                .WithMany(p => p.NguoiDungs)
                                .HasForeignKey(u => u.PhuongId);
        });

        modelBuilder.Entity<IdentityRole<string>>(b =>
        {
            b.ToTable("AspNetRoles");
            b.Property(r => r.Id)
                .ValueGeneratedNever(); // ID được cung cấp bởi code, không tự sinh
        });



        modelBuilder.Entity<ChiTietPn>(entity =>
        {
            entity.HasKey(e => new { e.PhieuNhapId, e.LinhKienId }).HasName("PK__ChiTietP__CE785146FEE0EA2E");

            entity.HasOne(d => d.LinhKien).WithMany(p => p.ChiTietPns)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietPN__LinhK__6B24EA82");

            entity.HasOne(d => d.PhieuNhap).WithMany(p => p.ChiTietPns)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietPN__Phieu__6A30C649");
        });

        modelBuilder.Entity<ChiTietPx>(entity =>
        {
            entity.HasKey(e => new { e.PhieuXuatId, e.PhieuSuaChuaId }).HasName("PK__ChiTietP__5169BB2B3005A347");

            entity.HasOne(d => d.PhieuSuaChua).WithMany(p => p.ChiTietPxes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietPX__Phieu__6754599E");

            entity.HasOne(d => d.PhieuXuat).WithMany(p => p.ChiTietPxes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietPX__Phieu__66603565");
        });

        modelBuilder.Entity<ChiTietSuaChua>(entity =>
        {
            entity.HasKey(e => e.ChiTietId).HasName("PK__ChiTietS__B117E9EAF0334BA5");

            entity.Property(e => e.ChiTietId).ValueGeneratedOnAdd();
            entity.HasOne(d => d.LinhKien).WithMany(p => p.ChiTietSuaChuas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietSu__LinhK__70DDC3D8");

            entity.HasOne(d => d.PhieuSuaChua).WithMany(p => p.ChiTietSuaChuas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietSu__Phieu__6FE99F9F");

            entity.HasOne(d => d.SanPham).WithMany(p => p.ChiTietSuaChuas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietSu__SanPh__6EF57B66");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.DanhMucId).HasName("PK__DanhMuc__1C53BA7B3EE29315");
            entity.Property(e => e.DanhMucId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<LinhKien>(entity =>
        {
            entity.HasKey(e => e.LinhKienId).HasName("PK__LinhKien__04269C40A0329C9C"); }); }}