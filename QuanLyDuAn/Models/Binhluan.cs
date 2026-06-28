using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDuAn.Models;

[Table("BINHLUAN")]
public partial class Binhluan
{
    [Key]
    [Column("ma_binh_luan")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaBinhLuan { get; set; } = null!;

    [Column("ma_cong_viec")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCongViec { get; set; } = null!;

    [Column("ma_tai_khoan")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaTaiKhoan { get; set; } = null!;

    [Column("noi_dung")]
    public string NoiDung { get; set; } = null!;

    [Column("thoi_gian", TypeName = "datetime")]
    public DateTime ThoiGian { get; set; }

    [ForeignKey("MaCongViec")]
    [InverseProperty("Binhluans")]
    public virtual Congviec MaCongViecNavigation { get; set; } = null!;

    [ForeignKey("MaTaiKhoan")]
    [InverseProperty("Binhluans")]
    public virtual Taikhoan MaTaiKhoanNavigation { get; set; } = null!;
}
