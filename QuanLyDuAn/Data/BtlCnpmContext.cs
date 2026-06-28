using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Models;

namespace QuanLyDuAn.Data;

public partial class BtlCnpmContext : DbContext
{
    public BtlCnpmContext()
    {
    }

    public BtlCnpmContext(DbContextOptions<BtlCnpmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Binhluan> Binhluans { get; set; }

    public virtual DbSet<Congviec> Congviecs { get; set; }

    public virtual DbSet<Duan> Duans { get; set; }

    public virtual DbSet<Goidichvu> Goidichvus { get; set; }

    public virtual DbSet<Invitelink> Invitelinks { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    public virtual DbSet<Tepdinhkem> Tepdinhkems { get; set; }

    public virtual DbSet<Thanhvien> Thanhviens { get; set; }

    public virtual DbSet<Thongbao> Thongbaos { get; set; }

    public virtual DbSet<Workspace> Workspaces { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Binhluan>(entity =>
        {
            entity.Property(e => e.ThoiGian).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaCongViecNavigation).WithMany(p => p.Binhluans).HasConstraintName("FK_BINHLUAN_CONGVIEC");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.Binhluans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BINHLUAN_TAIKHOAN");
        });

        modelBuilder.Entity<Congviec>(entity =>
        {
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaDuAnNavigation).WithMany(p => p.Congviecs).HasConstraintName("FK_CONGVIEC_DUAN");

            entity.HasOne(d => d.MaThanhVienNavigation).WithMany(p => p.Congviecs).HasConstraintName("FK_CONGVIEC_THANHVIEN");
        });

        modelBuilder.Entity<Duan>(entity =>
        {
            entity.HasOne(d => d.MaWorkspaceNavigation).WithMany(p => p.Duans).HasConstraintName("FK_DUAN_WORKSPACE");
        });

        modelBuilder.Entity<Goidichvu>(entity =>
        {
            entity.Property(e => e.MaGoi).HasComment("Mã định danh duy nhất của gói dịch vụ");
            entity.Property(e => e.DungLuongMax).HasComment("Dung lượng lưu trữ tối đa cho phép (tính bằng MB)");
            entity.Property(e => e.Gia).HasComment("Giá tiền của gói dịch vụ/tháng");
            entity.Property(e => e.MoTa).HasComment("Mô tả chi tiết quyền lợi của gói dịch vụ");
            entity.Property(e => e.SoDuAnToiDa).HasComment("Giới hạn số lượng dự án được phép tạo lập");
            entity.Property(e => e.SoTvToiDa).HasComment("Giới hạn số lượng thành viên tham gia Workspace");
            entity.Property(e => e.TenGoi).HasComment("Tên của gói dịch vụ (Free, Pro, Enterprise...)");
        });

        modelBuilder.Entity<Invitelink>(entity =>
        {
            entity.HasOne(d => d.MaWorkspaceNavigation).WithMany(p => p.Invitelinks).HasConstraintName("FK_INVITELINK_WORKSPACE");
        });

        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaGoiNavigation).WithMany(p => p.Taikhoans)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_TAIKHOAN_GOIDICHVU");
        });

        modelBuilder.Entity<Tepdinhkem>(entity =>
        {
            entity.HasOne(d => d.MaCongViecNavigation).WithMany(p => p.Tepdinhkems).HasConstraintName("FK_TEPDINHKEM_CONGVIEC");
        });

        modelBuilder.Entity<Thanhvien>(entity =>
        {
            entity.Property(e => e.NgayThamGia).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaDuAnNavigation).WithMany(p => p.Thanhviens).HasConstraintName("FK_THANHVIEN_DUAN");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.Thanhviens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_THANHVIEN_TAIKHOAN");
        });

        modelBuilder.Entity<Thongbao>(entity =>
        {
            entity.Property(e => e.ThoiGian).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.Thongbaos).HasConstraintName("FK_THONGBAO_TAIKHOAN");
        });

        modelBuilder.Entity<Workspace>(entity =>
        {
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.Workspaces).HasConstraintName("FK_WORKSPACE_TAIKHOAN");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
