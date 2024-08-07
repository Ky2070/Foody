using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ECommerceMVC.Controllers
{
	public class HangHoaController : Controller
	{
		private readonly Hshop2023Context db;

		public HangHoaController(Hshop2023Context context)
		{
			db = context;
		}

		public IActionResult Index(int? loai, int page = 1)
		{
			int pageSize = 9;
			var hangHoas = db.HangHoas.AsQueryable();

			if (loai.HasValue)
			{
				hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
			}

			var totalItems = hangHoas.Count();
			var result = hangHoas
				.Select(p => new HangHoaVM
				{
					MaHh = p.MaHh,
					TenHH = p.TenHh,
					DonGia = p.DonGia ?? 0,
					Hinh = p.Hinh ?? "",
					MoTaNgan = p.MoTaDonVi ?? "",
					TenLoai = p.MaLoaiNavigation.TenLoai
				})
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
			ViewBag.CurrentPage = page;
			ViewBag.Loai = loai;

			return View(result);
		}

		public IActionResult Search(string? query, int page = 1)
		{
			int pageSize = 9;
			var hangHoas = db.HangHoas.AsQueryable();

			if (!string.IsNullOrEmpty(query))
			{
				hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
			}

			var totalItems = hangHoas.Count();
			var result = hangHoas
				.Select(p => new HangHoaVM
				{
					MaHh = p.MaHh,
					TenHH = p.TenHh,
					DonGia = p.DonGia ?? 0,
					Hinh = p.Hinh ?? "",
					MoTaNgan = p.MoTaDonVi ?? "",
					TenLoai = p.MaLoaiNavigation.TenLoai
				})
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
			ViewBag.CurrentPage = page;
			ViewBag.Query = query;

			return View(result);
		}

		public IActionResult Detail(int id)
		{
			var data = db.HangHoas
				.Include(p => p.MaLoaiNavigation)
				.Include(p => p.YeuThiches)
				.ThenInclude(y => y.MaKhNavigation) // Include khách hàng liên quan nếu cần thiết
				.SingleOrDefault(p => p.MaHh == id);

			if (data == null)
			{
				TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
				return Redirect("/404");
			}

			var result = new ChiTietHangHoaVM
			{
				MaHh = data.MaHh,
				TenHH = data.TenHh,
				DonGia = data.DonGia ?? 0,
				ChiTiet = data.MoTa ?? string.Empty,
				Hinh = data.Hinh ?? string.Empty,
				MoTaNgan = data.MoTaDonVi ?? string.Empty,
				TenLoai = data.MaLoaiNavigation.TenLoai,
				SoLuongTon = 10, // Tính sau
				DiemDanhGia = 5, // Check sau
				YeuThichs = data.YeuThiches.Select(y => new YeuThichVM
				{
					MaHh = y.MaHh.GetValueOrDefault(), // Sử dụng GetValueOrDefault() để chuyển nullable int sang int
					MoTa = y.MoTa,
					MucDoYeuThich = y.MucDoYeuThich.GetValueOrDefault(),
					HoTenKhachHang = y.MaKhNavigation.HoTen // Gán tên khách hàng từ đối tượng khách hàng
				}).ToList()
			};

			return View(result);
		}
	}
}

