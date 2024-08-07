// ViewModels/DeliveryStatusVM.cs
using System;

namespace ECommerceMVC.ViewModels
{
    public class DeliveryStatusVM
    {
        public int MaHd { get; set; }
        public string MaKh { get; set; } = null!;
        public DateTime NgayDat { get; set; }
        public DateTime? NgayCan { get; set; }
        public DateTime? NgayGiao { get; set; }
        public string HoTen { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public string? DienThoai { get; set; }
        public string CachThanhToan { get; set; } = null!;
        public string CachVanChuyen { get; set; } = null!;
        public double PhiVanChuyen { get; set; }
        public string TenTrangThai { get; set; } = null!;
        public string? GhiChu { get; set; }
    }
}
