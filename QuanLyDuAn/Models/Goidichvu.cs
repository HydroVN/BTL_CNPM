using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDuAn.Models;

[Table("GOIDICHVU")]
public partial class Goidichvu
{
    /// <summary>
    /// Mã định danh duy nhất của gói dịch vụ
    /// </summary>
    [Key]
    [Column("ma_goi")]
    [StringLength(50)]
    [Unicode(false)]
    public string MaGoi { get; set; } = null!;

    /// <summary>
    /// Tên của gói dịch vụ (Free, Pro, Enterprise...)
    /// </summary>
    [Column("ten_goi")]
    [StringLength(100)]
    public string TenGoi { get; set; } = null!;

    /// <summary>
    /// Giá tiền của gói dịch vụ/tháng
    /// </summary>
    [Column("gia", TypeName = "decimal(10, 2)")]
    public decimal Gia { get; set; }

    /// <summary>
    /// Giới hạn số lượng dự án được phép tạo lập
    /// </summary>
    [Column("so_du_an_toi_da")]
    public int SoDuAnToiDa { get; set; }

    /// <summary>
    /// Giới hạn số lượng thành viên tham gia Workspace
    /// </summary>
    [Column("so_tv_toi_da")]
    public int SoTvToiDa { get; set; }

    /// <summary>
    /// Dung lượng lưu trữ tối đa cho phép (tính bằng MB)
    /// </summary>
    [Column("dung_luong_max")]
    public long DungLuongMax { get; set; }

    /// <summary>
    /// Mô tả chi tiết quyền lợi của gói dịch vụ
    /// </summary>
    [Column("mo_ta")]
    [StringLength(500)]
    public string? MoTa { get; set; }

    [InverseProperty("MaGoiNavigation")]
    public virtual ICollection<Taikhoan> Taikhoans { get; set; } = new List<Taikhoan>();
}
