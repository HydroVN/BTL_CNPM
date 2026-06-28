using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDuAn.Models;

[Table("TAIKHOAN")]
[Index("Email", Name = "UQ_TAIKHOAN_EMAIL", IsUnique = true)]
public partial class Taikhoan
{
    [Key]
    [Column("ma_tai_khoan")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaTaiKhoan { get; set; } = null!;

    [Column("ma_goi")]
    [StringLength(50)]
    [Unicode(false)]
    public string? MaGoi { get; set; }

    [Column("ho_ten")]
    [StringLength(100)]
    public string HoTen { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Column("mat_khau")]
    [StringLength(255)]
    [Unicode(false)]
    public string MatKhau { get; set; } = null!;

    [Column("so_dien_thoai")]
    [StringLength(15)]
    [Unicode(false)]
    public string? SoDienThoai { get; set; }

    [Column("vai_tro")]
    [StringLength(50)]
    [Unicode(false)]
    public string VaiTro { get; set; } = null!;

    [Column("trang_thai")]
    [StringLength(50)]
    [Unicode(false)]
    public string TrangThai { get; set; } = null!;

    [Column("ngay_tao", TypeName = "datetime")]
    public DateTime NgayTao { get; set; }

    [InverseProperty("MaTaiKhoanNavigation")]
    public virtual ICollection<Binhluan> Binhluans { get; set; } = new List<Binhluan>();

    [ForeignKey("MaGoi")]
    [InverseProperty("Taikhoans")]
    public virtual Goidichvu? MaGoiNavigation { get; set; }

    [InverseProperty("MaTaiKhoanNavigation")]
    public virtual ICollection<Thanhvien> Thanhviens { get; set; } = new List<Thanhvien>();

    [InverseProperty("MaTaiKhoanNavigation")]
    public virtual ICollection<Thongbao> Thongbaos { get; set; } = new List<Thongbao>();

    [InverseProperty("MaTaiKhoanNavigation")]
    public virtual ICollection<Workspace> Workspaces { get; set; } = new List<Workspace>();
}
