using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDuAn.Models;

[Table("INVITELINK")]
[Index("Token", Name = "UQ_INVITELINK_TOKEN", IsUnique = true)]
public partial class Invitelink
{
    [Key]
    [Column("ma_link")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaLink { get; set; } = null!;

    [Column("ma_workspace")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaWorkspace { get; set; } = null!;

    [Column("token")]
    [StringLength(255)]
    [Unicode(false)]
    public string Token { get; set; } = null!;

    [Column("vai_tro_mac_dinh")]
    [StringLength(50)]
    [Unicode(false)]
    public string VaiTroMacDinh { get; set; } = null!;

    [Column("ngay_het_han", TypeName = "datetime")]
    public DateTime NgayHetHan { get; set; }

    [ForeignKey("MaWorkspace")]
    [InverseProperty("Invitelinks")]
    public virtual Workspace MaWorkspaceNavigation { get; set; } = null!;
}
