using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDuAn.Models;

[Table("THONGBAO")]
public partial class Thongbao
{
    [Key]
    [Column("ma_thong_bao")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThongBao { get; set; } = null!;

    [Column("ma_tai_khoan")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaTaiKhoan { get; set; } = null!;

    [Column("noi_dung")]
    [StringLength(500)]
    public string NoiDung { get; set; } = null!;

    [Column("trang_thai")]
    [StringLength(50)]
    [Unicode(false)]
    public string TrangThai { get; set; } = null!;

    [Column("thoi_gian", TypeName = "datetime")]
    public DateTime ThoiGian { get; set; }

    [ForeignKey("MaTaiKhoan")]
    [InverseProperty("Thongbaos")]
    public virtual Taikhoan MaTaiKhoanNavigation { get; set; } = null!;
}
