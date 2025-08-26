using System;
using System.Collections.Generic;

namespace Project2_NewDB.Models;

public partial class SanPham
{
    public int MaSp { get; set; }

    public string TenSp { get; set; } = null!;

    public string? MoTa { get; set; }

    public decimal DonGia { get; set; }

    public int SoLuong { get; set; }

    public int? MaLoai { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual LoaiSanPham? MaLoaiNavigation { get; set; }
}
