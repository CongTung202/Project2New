using System;
using System.Collections.Generic;

namespace Project2_NewDB.Models;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public DateTime NgayLap { get; set; }

    public decimal TongTien { get; set; }

    public int? MaKh { get; set; }

    public int? MaQtv { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual QuanTriVien? MaQtvNavigation { get; set; }
}
