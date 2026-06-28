using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDuAn.Models;

[Table("WORKSPACE")]
public partial class Workspace
{
    [Key]
    [Column("ma_workspace")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaWorkspace { get; set; } = null!;

    [Column("ma_tai_khoan")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaTaiKhoan { get; set; } = null!;

    [Column("ten_workspace")]
    [StringLength(100)]
    public string TenWorkspace { get; set; } = null!;

    [Column("mo_ta")]
    [StringLength(500)]
    public string? MoTa { get; set; }

    [Column("ngay_tao", TypeName = "datetime")]
    public DateTime NgayTao { get; set; }

    [InverseProperty("MaWorkspaceNavigation")]
    public virtual ICollection<Duan> Duans { get; set; } = new List<Duan>();

    [InverseProperty("MaWorkspaceNavigation")]
    public virtual ICollection<Invitelink> Invitelinks { get; set; } = new List<Invitelink>();

    [ForeignKey("MaTaiKhoan")]
    [InverseProperty("Workspaces")]
    public virtual Taikhoan MaTaiKhoanNavigation { get; set; } = null!;
}
