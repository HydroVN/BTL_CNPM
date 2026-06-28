using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDuAn.Models;

[Table("CONGVIEC")]
public partial class Congviec
{
    [Key]
    [Column("ma_cong_viec")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCongViec { get; set; } = null!;

    [Column("ma_du_an")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaDuAn { get; set; } = null!;

    [Column("ma_thanh_vien")]
    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhVien { get; set; }

    [Column("ten_cong_viec")]
    [StringLength(255)]
    public string TenCongViec { get; set; } = null!;

    [Column("mo_ta")]
    public string? MoTa { get; set; }

    [Column("deadline", TypeName = "datetime")]
    public DateTime? Deadline { get; set; }

    [Column("muc_do_uu_tien")]
    [StringLength(50)]
    public string MucDoUuTien { get; set; } = null!;

    [Column("trang_thai")]
    [StringLength(50)]
    public string TrangThai { get; set; } = null!;

    [Column("ngay_tao", TypeName = "datetime")]
    public DateTime NgayTao { get; set; }

    [InverseProperty("MaCongViecNavigation")]
    public virtual ICollection<Binhluan> Binhluans { get; set; } = new List<Binhluan>();

    [ForeignKey("MaDuAn")]
    [InverseProperty("Congviecs")]
    public virtual Duan MaDuAnNavigation { get; set; } = null!;

    [ForeignKey("MaThanhVien")]
    [InverseProperty("Congviecs")]
    public virtual Thanhvien? MaThanhVienNavigation { get; set; }

    [InverseProperty("MaCongViecNavigation")]
    public virtual ICollection<Tepdinhkem> Tepdinhkems { get; set; } = new List<Tepdinhkem>();
}
