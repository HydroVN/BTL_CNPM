using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDuAn.Models;

[Table("DUAN")]
public partial class Duan
{
    [Key]
    [Column("ma_du_an")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaDuAn { get; set; } = null!;

    [Column("ma_workspace")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaWorkspace { get; set; } = null!;

    [Column("ten_du_an")]
    [StringLength(150)]
    public string TenDuAn { get; set; } = null!;

    [Column("mo_ta")]
    public string? MoTa { get; set; }

    [Column("ngay_bat_dau", TypeName = "datetime")]
    public DateTime? NgayBatDau { get; set; }

    [Column("ngay_ket_thuc", TypeName = "datetime")]
    public DateTime? NgayKetThuc { get; set; }

    [Column("trang_thai")]
    [StringLength(50)]
    public string TrangThai { get; set; } = null!;

    [InverseProperty("MaDuAnNavigation")]
    public virtual ICollection<Congviec> Congviecs { get; set; } = new List<Congviec>();

    [ForeignKey("MaWorkspace")]
    [InverseProperty("Duans")]
    public virtual Workspace MaWorkspaceNavigation { get; set; } = null!;

    [InverseProperty("MaDuAnNavigation")]
    public virtual ICollection<Thanhvien> Thanhviens { get; set; } = new List<Thanhvien>();
}
