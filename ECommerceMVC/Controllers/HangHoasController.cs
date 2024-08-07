using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerceMVC.Data;
using ECommerceMVC.Helpers;
using ECommerceMVC.ViewModels;

namespace ECommerceMVC.Controllers
{
    public class HangHoasController : Controller
    {
        private readonly Hshop2023Context _context;

        public HangHoasController(Hshop2023Context context)
        {
            _context = context;
        }

        // GET: HangHoas
        public async Task<IActionResult> Index()
        {
            var hangHoas = await _context.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .Select(h => new HangHoaVM
                {
                    MaHh = h.MaHh,
                    TenHH = h.TenHh,
                    Hinh = h.Hinh,
                    DonGia = h.DonGia ?? 0,
                    MoTaNgan = h.MoTa ?? string.Empty,
                    TenLoai = h.MaLoaiNavigation.TenLoai
                }).ToListAsync();

            return View(hangHoas);
        }

        // GET: HangHoas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangHoa = await _context.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .Select(h => new ChiTietHangHoaVM
                {
                    MaHh = h.MaHh,
                    TenHH = h.TenHh,
                    Hinh = h.Hinh,
                    DonGia = h.DonGia ?? 0,
                    MoTaNgan = h.MoTa ?? string.Empty,
                    TenLoai = h.MaLoaiNavigation.TenLoai,
                    ChiTiet = h.MoTa ?? string.Empty, // Adjust as needed for details
                    DiemDanhGia = 0, // Adjust as needed
                    SoLuongTon = 0 // Adjust as needed
                }).FirstOrDefaultAsync(m => m.MaHh == id);

            if (hangHoa == null)
            {
                return NotFound();
            }

            return View(hangHoa);
        }

        // GET: HangHoas/Create
        [HttpGet]
        public IActionResult Create()
        {
            // Lấy danh sách các loại sản phẩm
            var loaiList = _context.Loais.ToList();
            ViewBag.MaLoai = new SelectList(loaiList, "MaLoai", "TenLoai");
            return View();
        }

        // POST: HangHoas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HangHoaVM hangHoaVM)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    // Lấy tên loại từ cơ sở dữ liệu dựa trên MaLoai
                    var loai = await _context.Loais.FindAsync(hangHoaVM.MaLoai);
                    if (loai == null)
                    {
                        ModelState.AddModelError("", "Loại hàng hóa không tồn tại.");
                        ViewBag.MaLoai = new SelectList(_context.Loais, "MaLoai", "TenLoai", hangHoaVM.MaLoai);
                        return View(hangHoaVM);
                    }
                    hangHoaVM.TenLoai = loai.TenLoai;

                    var hangHoa = new HangHoa
                    {
                        TenHh = hangHoaVM.TenHH,
                        Hinh = hangHoaVM.Hinh ?? "default.jpg",
                        DonGia = hangHoaVM.DonGia,
                        MoTa = hangHoaVM.MoTaNgan,
                        MaLoai = hangHoaVM.MaLoai,
                    };

                    _context.Add(hangHoa);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }

                foreach (var state in ModelState.Values)
                {
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }

                ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "TenLoai", hangHoaVM.MaLoai);
                return View(hangHoaVM);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra trong quá trình tạo sản phẩm. Vui lòng thử lại.");
                ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "TenLoai");
                return View(hangHoaVM);
            }
        }


        // POST: HangHoas/Edit/5
        //      [HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, HangHoaVM hangHoaVM)
        //{
        //	if (id != hangHoaVM.MaHh)
        //	{
        //		return NotFound();
        //	}

        //	if (ModelState.IsValid)
        //	{
        //		try
        //		{
        //			var hangHoa = await _context.HangHoas.FindAsync(id);
        //			if (hangHoa == null)
        //			{
        //				return NotFound();
        //			}

        //			hangHoa.TenHh = hangHoaVM.TenHH;
        //			hangHoa.DonGia = hangHoaVM.DonGia;
        //			hangHoa.MoTa = hangHoaVM.MoTaNgan;
        //			hangHoa.MaLoai = hangHoaVM.MaLoai;

        //			// Xử lý upload hình ảnh
        //			if (hangHoaVM.HinhUpload != null && hangHoaVM.HinhUpload.Length > 0)
        //			{
        //				var fileName = Path.GetFileName(hangHoaVM.HinhUpload.FileName); // Sử dụng HinhUpload
        //				var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "hanghoa");

        //				// Ensure the directory exists
        //				if (!Directory.Exists(folderPath))
        //				{
        //					Directory.CreateDirectory(folderPath);
        //				}

        //				var fullPath = Path.Combine(folderPath, fileName);

        //				// Save the file
        //				using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        //				{
        //					await hangHoaVM.HinhUpload.CopyToAsync(stream); // Sử dụng HinhUpload
        //				}

        //				hangHoa.Hinh = fileName;
        //			}

        //			_context.Update(hangHoa);
        //			await _context.SaveChangesAsync();

        //			TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công.";
        //			return RedirectToAction(nameof(Index));
        //		}
        //		catch (DbUpdateConcurrencyException)
        //		{
        //			TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật sản phẩm.";
        //			return View(hangHoaVM);
        //		}
        //	}

        //	// Lấy lại danh sách loại hàng hóa trong trường hợp có lỗi
        //	ViewData["MaLoai"] = new SelectList(await _context.Loais.ToListAsync(), "MaLoai", "TenLoai", hangHoaVM.MaLoai);
        //	return View(hangHoaVM);
        //}



        private bool HangHoaExists(int id)
        {
            return _context.HangHoas.Any(e => e.MaHh == id);
        }

        // GET: HangHoas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangHoa = await _context.HangHoas
                .Include(h => h.ChiTietHds) // Bao gồm thông tin chi tiết hóa đơn
                .Include(h => h.MaLoaiNavigation)
                .Where(h => h.MaHh == id)
                .Select(h => new HangHoaVM
                {
                    MaHh = h.MaHh,
                    TenHH = h.TenHh,
                    Hinh = h.Hinh,
                    DonGia = h.DonGia ?? 0,
                    MoTaNgan = h.MoTaDonVi ?? "Chưa Có",
                    TenLoai = h.MaLoaiNavigation.TenLoai,
                    MaLoai = h.MaLoai
                })
                .FirstOrDefaultAsync();

            if (hangHoa == null)
            {
                return NotFound();
            }

            return View(hangHoa);
        }

        // POST: HangHoas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hangHoa = await _context.HangHoas
                .Include(h => h.ChiTietHds) // Bao gồm thông tin chi tiết hóa đơn
                .FirstOrDefaultAsync(h => h.MaHh == id);

            if (hangHoa != null)
            {
                // Xóa các bản ghi chi tiết hóa đơn liên quan
                foreach (var chiTiet in hangHoa.ChiTietHds.ToList())
                {
                    _context.ChiTietHds.Remove(chiTiet);
                }

                // Xóa sản phẩm
                _context.HangHoas.Remove(hangHoa);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }



    }
}
