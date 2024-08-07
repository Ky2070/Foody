using AutoMapper;
using ECommerceMVC.Data;
using ECommerceMVC.Helpers;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ECommerceMVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly Hshop2023Context _context;
        private readonly IMapper _mapper;

        public OrderController(Hshop2023Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Status()
        {
            var maKh = User.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
            if (string.IsNullOrEmpty(maKh))
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            var orders = _context.HoaDons
                .Where(hd => hd.MaKh == maKh)
                .Select(hd => new OrderStatusVM
                {
                    MaHd = hd.MaHd,
                    NgayDat = hd.NgayDat,
                    NgayGiao = hd.NgayGiao,
                    HoTen = hd.HoTen,
                    DiaChi = hd.DiaChi,
                    DienThoai = hd.DienThoai,
                    CachThanhToan = hd.CachThanhToan,
                    CachVanChuyen = hd.CachVanChuyen,
                    PhiVanChuyen = hd.PhiVanChuyen,
                    TenTrangThai = hd.MaTrangThaiNavigation.TenTrangThai,
                    MoTa = hd.MaTrangThaiNavigation.MoTa,
                    ChiTietHds = _context.ChiTietHds
                        .Where(ct => ct.MaHd == hd.MaHd)
                        .Select(ct => new OrderItemVM
                        {
                            TenHangHoa = ct.MaHhNavigation.TenHh,
                            DonGia = ct.DonGia,
                            SoLuong = ct.SoLuong,
                            GiamGia = ct.GiamGia
                        }).ToList()
                }).ToList();

            return View(orders);
        }
    }
}
