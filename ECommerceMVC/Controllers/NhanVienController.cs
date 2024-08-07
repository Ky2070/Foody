using AutoMapper;
using ECommerceMVC.Data;
using ECommerceMVC.Helpers;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerceMVC.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;

        public NhanVienController(Hshop2023Context context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(EmployeeRegisterVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.NhanViens.Any(nv => nv.MaNv == model.MaNv))
                    {
                        ModelState.AddModelError("MaNv", "Mã nhân viên đã tồn tại");
                        return View(model);
                    }

                    var nhanVien = _mapper.Map<NhanVien>(model);
                    nhanVien.MatKhau = model.MatKhau.ToSHA256Hash("your_salt_key");
                    nhanVien.VaiTro = 0; // Default to Employee
                    nhanVien.HieuLuc = false; // Default to not active

                    db.Add(nhanVien);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Đăng ký thành công! Chờ admin phê duyệt.";
                    return RedirectToAction("Login", "NhanVien");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Đăng ký thất bại: " + ex.Message;
                }
            }
            return View(model);
        }

        #endregion

        #region Login
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(EmployeeLoginVM model, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var nhanVien = db.NhanViens.SingleOrDefault(nv => nv.MaNv == model.UserName);
                if (nhanVien == null)
                {
                    ModelState.AddModelError("loi", "Không có nhân viên này");
                }
                else
                {
                    var hashedPassword = model.Password.ToSHA256Hash("your_salt_key");

                    if (nhanVien.MatKhau != hashedPassword)
                    {
                        ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                    }
                    else if (!nhanVien.HieuLuc)
                    {
                        ModelState.AddModelError("loi", "Tài khoản chưa được kích hoạt");
                    }
                    else
                    {
                        var claims = new List<Claim> {
                    new Claim(ClaimTypes.Email, nhanVien.Email),
                    new Claim(ClaimTypes.Name, nhanVien.HoTen),
                    new Claim(MySetting.CLAIM_EMPLOYEEID, nhanVien.MaNv),
                    new Claim(ClaimTypes.Role, nhanVien.VaiTro == 1 ? "Admin" : "Employee") // Phân biệt vai trò
                };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(claimsPrincipal);

                        // Redirect đến EmployeePage sau khi đăng nhập thành công
                        return RedirectToAction("EmployeePage", "Home");
                    }
                }
            }
            return View();
        }

        #endregion

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Lấy giá trị MaNv từ claims
            var maNv = User.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_EMPLOYEEID)?.Value;
            if (string.IsNullOrEmpty(maNv))
            {
                return RedirectToAction("Login", "NhanVien");
            }

            // Tìm nhân viên theo MaNv
            var nhanVien = await db.NhanViens.SingleOrDefaultAsync(nv => nv.MaNv == maNv);
            if (nhanVien == null)
            {
                return NotFound();
            }

            // Tạo đối tượng EmployeeProfileVM từ đối tượng NhanVien
            var model = new EmployeeProfileVM
            {
                MaNv = nhanVien.MaNv,
                HoTen = nhanVien.HoTen,
                Email = nhanVien.Email,
                MatKhau = nhanVien.MatKhau, // Nếu cần hiển thị và chỉnh sửa mật khẩu
                VaiTro = nhanVien.VaiTro == 1 ? "Admin" : "Employee" // Ánh xạ vai trò
            };

            return View(model);
        }


        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("EmployeePage", "Home");
        }
    }
}
