using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDuAn.Models;

[Table("THANHVIEN")]
public partial class Thanhvien
{
    [Key]
    [Column("ma_thanh_vien")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhVien { get; set; } = null!;

    [Column("ma_du_an")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaDuAn { get; set; } = null!;

    [Column("ma_tai_khoan")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaTaiKhoan { get; set; } = null!;

    [Column("vai_tro_du_an")]
    [StringLength(50)]
    public string VaiTroDuAn { get; set; } = null!;

    [Column("ngay_tham_gia", TypeName = "datetime")]
    public DateTime NgayThamGia { get; set; }

    [InverseProperty("MaThanhVienNavigation")]
    public virtual ICollection<Congviec> Congviecs { get; set; } = new List<Congviec>();

    [ForeignKey("MaDuAn")]
    [InverseProperty("Thanhviens")]
    public virtual Duan MaDuAnNavigation { get; set; } = null!;

    [ForeignKey("MaTaiKhoan")]
    [InverseProperty("Thanhviens")]
    public virtual Taikhoan MaTaiKhoanNavigation { get; set; } = null!;
}
