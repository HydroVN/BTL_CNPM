using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDuAn.Models;

[Table("TEPDINHKEM")]
public partial class Tepdinhkem
{
    [Key]
    [Column("ma_tep")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaTep { get; set; } = null!;

    [Column("ma_cong_viec")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCongViec { get; set; } = null!;

    [Column("ten_tep")]
    [StringLength(255)]
    public string TenTep { get; set; } = null!;

    [Column("duong_dan")]
    [StringLength(500)]
    [Unicode(false)]
    public string DuongDan { get; set; } = null!;

    [Column("dung_luong")]
    public int DungLuong { get; set; }

    [ForeignKey("MaCongViec")]
    [InverseProperty("Tepdinhkems")]
    public virtual Congviec MaCongViecNavigation { get; set; } = null!;
}
