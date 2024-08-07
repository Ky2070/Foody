using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVC.Controllers
{
    [Authorize(Roles = "Employee")]
    public class DeliveryController : Controller
    {
        private readonly Hshop2023Context _context;

        public DeliveryController(Hshop2023Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách các trạng thái
            var statuses = await _context.TrangThais.ToListAsync();
            ViewBag.Statuses = statuses;

            // Lấy các đơn hàng từ năm 2024 trở đi
            var orders = await _context.HoaDons
               .Include(h => h.MaTrangThaiNavigation)
               .Where(o => o.NgayDat >= new DateTime(2024, 7, 1))
               .ToListAsync();

            var model = orders.Select(o => new DeliveryStatusVM
            {
                MaHd = o.MaHd,
                MaKh = o.MaKh,
                NgayDat = o.NgayDat,
                NgayCan = o.NgayCan,
                NgayGiao = o.NgayGiao,
                HoTen = o.HoTen,
                DiaChi = o.DiaChi,
                DienThoai = o.DienThoai,
                CachThanhToan = o.CachThanhToan,
                CachVanChuyen = o.CachVanChuyen,
                PhiVanChuyen = o.PhiVanChuyen,
                TenTrangThai = o.MaTrangThaiNavigation.TenTrangThai,
                GhiChu = o.GhiChu
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int maHd, int trangThaiId)
        {
            var order = await _context.HoaDons.FindAsync(maHd);
            if (order == null)
            {
                return NotFound();
            }

            // Kiểm tra phương thức thanh toán và trạng thái cập nhật
            // Trạng thái "Mới đặt hàng" là 0 và "Hủy đơn hàng" là -1
            if (order.CachThanhToan == "PayPal" && (trangThaiId == 0 || trangThaiId == -1))
            {
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái 'Mới đặt hàng' hoặc 'Hủy đơn hàng' cho đơn hàng đã thanh toán qua PayPal.";
                return RedirectToAction(nameof(Index));
            }

            order.MaTrangThai = trangThaiId;
            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order status updated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
