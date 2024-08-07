using System;
using System.Collections.Generic;

namespace ECommerceMVC.ViewModels
{
    public class OrderStatusVM
    {
        public int MaHd { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime? NgayGiao { get; set; }
        public string HoTen { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public string DienThoai { get; set; } = null!;
        public string CachThanhToan { get; set; } = null!;
        public string CachVanChuyen { get; set; } = null!;
        public double PhiVanChuyen { get; set; }
        public string TenTrangThai { get; set; } = null!;
        public string MoTa { get; set; } = null!;

        // Thêm thông tin chi tiết hàng hóa
        public List<OrderItemVM> ChiTietHds { get; set; } = new();
    }

    public class OrderItemVM
    {
        public string TenHangHoa { get; set; } = null!;
        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public double GiamGia { get; set; }
        public double ThanhTien => (DonGia - GiamGia) * SoLuong; // Tính tổng tiền
    }
}
