using ECommerceMVC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq;

namespace ECommerceMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly Hshop2023Context db;

        public AdminController(Hshop2023Context context)
        {
            db = context;
        }

        public IActionResult Dashboard()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa và có vai trò Admin
            if (!User.Identity.IsAuthenticated && !User.IsInRole("Admin"))
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "NhanVien");
            }

            // Trả về view Dashboard nằm trong thư mục Views/Admin
            return View();
        }

        // Thực hiện đăng xuất
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Đăng xuất người dùng
            await HttpContext.SignOutAsync();

            return RedirectToAction("EmployeePage", "Home");
        }
        //CRUD cho HangHoas
        public IActionResult HangHoas()
        {
            var hangHoas = db.HangHoas.ToList();
            return View(hangHoas);
        }

        [HttpGet]
        public IActionResult EditHangHoa(int id)
        {
            var hangHoa = db.HangHoas.SingleOrDefault(hh => hh.MaHh == id);
            if (hangHoa == null)
            {
                return NotFound();
            }
            return View(hangHoa);
        }

        [HttpPost]
        public IActionResult EditHangHoa(HangHoa hangHoa)
        {
            if (ModelState.IsValid)
            {
                var existingHangHoa = db.HangHoas.SingleOrDefault(hh => hh.MaHh == hangHoa.MaHh);
                if (existingHangHoa != null)
                {
                    db.Entry(existingHangHoa).CurrentValues.SetValues(hangHoa);
                    db.SaveChanges();
                    ViewData["UpdateSuccess"] = true;
                    return RedirectToAction("HangHoas");
                }
                ViewData["UpdateSuccess"] = false;
            }
            else
            {
                ViewData["UpdateSuccess"] = false;
            }
            return View(hangHoa);
        }


        [HttpGet]
        public IActionResult CreateHangHoa()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateHangHoa(HangHoa hangHoa)
        {
            if (ModelState.IsValid)
            {
                db.HangHoas.Add(hangHoa);
                db.SaveChanges();
                ViewData["CreateSuccess"] = true; // Thiết lập thành công
                return RedirectToAction("HangHoas");
            }

            ViewData["CreateSuccess"] = false; // Cập nhật không thành công
            return View(hangHoa);
        }


        [HttpPost]
        public IActionResult DeleteHangHoa(int id)
        {
            // Xóa các bản ghi chi tiết hóa đơn liên quan
            var chiTietHds = db.ChiTietHds.Where(c => c.MaHh == id).ToList();
            db.ChiTietHds.RemoveRange(chiTietHds);

            // Xóa bản ghi hàng hóa chính
            var hangHoa = db.HangHoas.SingleOrDefault(hh => hh.MaHh == id);
            if (hangHoa != null)
            {
                db.HangHoas.Remove(hangHoa);
                db.SaveChanges();
            }

            return RedirectToAction("HangHoas");
        }
        //CRUD cho Danh Mục
        public IActionResult DanhMucs()
        {
            var danhmucs = db.Loais.ToList();
            return View(danhmucs); // Xác nhận 'danhmucs' là tên view đúng
        }

        [HttpGet]
        public IActionResult EditLoai(int id)
        {
            var loai = db.Loais.SingleOrDefault(l => l.MaLoai == id);
            if (loai == null)
            {
                return NotFound();
            }
            return View(loai);
        }

        [HttpPost]
        public IActionResult EditLoai(Loai loai)
        {
            if (ModelState.IsValid)
            {
                db.Loais.Update(loai);
                db.SaveChanges();
                return RedirectToAction("DanhMucs");
            }
            return View(loai);
        }

        [HttpGet]
        public IActionResult CreateLoai()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateLoai(Loai loai)
        {
            if (ModelState.IsValid)
            {
                db.Loais.Add(loai);
                db.SaveChanges();
                return RedirectToAction("DanhMucs");
            }
            return View(loai);
        }

        [HttpPost]
        public IActionResult DeleteLoai(int id)
        {
            var loai = db.Loais.SingleOrDefault(l => l.MaLoai == id);
            if (loai != null)
            {
                db.Loais.Remove(loai);
                db.SaveChanges();
            }
            return RedirectToAction("DanhMucs");
        }
        //CRUD cho Nhân Viên
        public IActionResult NhanViens()
        {
            // Retrieve only employees with HieuLuc = false
            var nhanViens = db.NhanViens.Where(nv => nv.HieuLuc == false).ToList();
            return View(nhanViens);
        }
        [HttpGet]
        public IActionResult EditNhanVien(string id)
        {
            var nhanVien = db.NhanViens.SingleOrDefault(nv => nv.MaNv == id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            return View(nhanVien);
        }
        [HttpPost]
        public IActionResult EditNhanVien(NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                var existingNhanVien = db.NhanViens.SingleOrDefault(nv => nv.MaNv == nhanVien.MaNv);
                if (existingNhanVien != null)
                {
                    // Cập nhật các thuộc tính của nhân viên
                    existingNhanVien.HoTen = nhanVien.HoTen;
                    existingNhanVien.Email = nhanVien.Email;
                    existingNhanVien.HieuLuc = nhanVien.HieuLuc;
                    existingNhanVien.VaiTro = nhanVien.VaiTro;

                    db.SaveChanges();
                }
                return RedirectToAction("NhanViens");
            }
            return View(nhanVien);
        }

        //[HttpGet]
        //public IActionResult CreateNhanVien()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult CreateNhanVien(NhanVien nhanVien)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        nhanVien.HieuLuc = false; // New employees should have HieuLuc = false by default
        //        db.NhanViens.Add(nhanVien);
        //        db.SaveChanges();
        //        return RedirectToAction("NhanViens");
        //    }
        //    return View(nhanVien);
        //}
        [HttpPost]
        public IActionResult DeleteNhanVien(string id)
        {
            // Tìm bản ghi nhân viên theo id
            var nhanVien = db.NhanViens.SingleOrDefault(nv => nv.MaNv == id);

            if (nhanVien != null)
            {
                // Xóa tất cả các bản ghi liên quan trong bảng PhanCong
                var phanCongs = db.PhanCongs.Where(pc => pc.MaNv == id).ToList();
                db.PhanCongs.RemoveRange(phanCongs);

                // Xóa bản ghi nhân viên
                db.NhanViens.Remove(nhanVien);
                db.SaveChanges();
            }

            return RedirectToAction("NhanViens");
        }


        //public IActionResult ManageEmployees()
        //{
        //    var employees = db.NhanViens.ToList();
        //    return View(employees);
        //}

        //[HttpPost]
        //public IActionResult UpdateEmployeeStatus(string maNv, bool hieuLuc, int vaiTro)
        //{
        //    var nhanVien = db.NhanViens.SingleOrDefault(nv => nv.MaNv == maNv);
        //    if (nhanVien != null)
        //    {
        //        nhanVien.HieuLuc = hieuLuc;
        //        nhanVien.VaiTro = vaiTro;
        //        db.SaveChanges();
        //    }
        //    return RedirectToAction("ManageEmployees");
        //}
    }
}
