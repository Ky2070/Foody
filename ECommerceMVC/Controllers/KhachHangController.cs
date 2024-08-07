using AutoMapper;
using ECommerceMVC.Data;
using ECommerceMVC.Helpers;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceMVC.Controllers
{
	public class KhachHangController : Controller
	{
		private readonly Hshop2023Context db;
		private readonly IMapper _mapper;

		public KhachHangController(Hshop2023Context context, IMapper mapper)
		{
			db = context;
			_mapper = mapper;
		}
        #region Register
        [HttpGet]
		public IActionResult DangKy()
		{
			return View();
		}

        [HttpPost]
        public IActionResult DangKy(RegisterVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.KhachHangs.Any(kh => kh.MaKh == model.MaKh))
                    {
                        ModelState.AddModelError("MaKh", "Tên đăng nhập đã tồn tại");
                        return View(model);
                    }

                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = MyUtil.GenerateRamdomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true; //sẽ xử lý khi dùng Mail để active
                    khachHang.VaiTro = 0;

                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }

                    db.Add(khachHang);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Đăng ký thành công!";
                    return RedirectToAction("DangKy", "KhachHang");
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
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
		[HttpPost]
		public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
		{
			ViewBag.ReturnUrl = ReturnUrl;

			if (ModelState.IsValid)
			{
				var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
				if (khachHang == null)
				{
					ModelState.AddModelError("loi", "Không có khách hàng này");
				}
				else
				{
					if (!khachHang.HieuLuc)
					{
						ModelState.AddModelError("loi", "Tài khoản đã bị khóa. Vui lòng liên hệ Admin.");
					}
					else
					{
						if (khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
						{
							ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
						}
						else
						{
							var claims = new List<Claim> {
						new Claim(ClaimTypes.Email, khachHang.Email),
						new Claim(ClaimTypes.Name, khachHang.HoTen),
						new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.MaKh),
						new Claim(ClaimTypes.Role, "Customer")
					};

							var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
							var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

							await HttpContext.SignInAsync(claimsPrincipal);

							// Kiểm tra và chuyển hướng đến ReturnUrl nếu hợp lệ, nếu không sẽ chuyển đến trang chính
							if (Url.IsLocalUrl(ReturnUrl))
							{
								return Redirect(ReturnUrl);
							}
							else
							{
                                return RedirectToAction("Index", "HangHoa");
							}
						}
					}
				}
			}
			return View(model);
		}
		#endregion
		#region Profile
		[Authorize]
        public IActionResult Profile()
        {
            // Lấy giá trị MaKh từ claims
            var maKh = User.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
            Console.WriteLine(maKh);
            if (string.IsNullOrEmpty(maKh))
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            // Tìm khách hàng theo MaKh
            var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKh == maKh);
            if (khachHang == null)
            {
                return NotFound();
            }

            // Tạo đối tượng ProfileVM từ đối tượng KhachHang
            var model = new ProfileVM
            {
                MaKh = khachHang.MaKh,
                HoTen = khachHang.HoTen,
                //GioiTinh = khachHang.GioiTinh,
                NgaySinh = khachHang.NgaySinh,
                DiaChi = khachHang.DiaChi ?? string.Empty,
                DienThoai = khachHang.DienThoai ?? string.Empty,
                Email = khachHang.Email,
                Hinh = khachHang.Hinh
            };

            return View(model);
            
        }

        
        [Authorize]
        [HttpGet]
        public IActionResult EditProfile()
        {
            // Lấy giá trị MaKh từ claims
            var maKh = User.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
            if (string.IsNullOrEmpty(maKh))
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            // Tìm khách hàng theo MaKh
            var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKh == maKh);
            if (khachHang == null)
            {
                return NotFound();
            }

            // Tạo đối tượng ProfileVM từ đối tượng KhachHang
            var model = new ProfileVM
            {
                MaKh = khachHang.MaKh,
                HoTen = khachHang.HoTen,
                //GioiTinh = khachHang.GioiTinh,
                //NgaySinh = khachHang.NgaySinh,
                DiaChi = khachHang.DiaChi ?? string.Empty,
                DienThoai = khachHang.DienThoai ?? string.Empty,
                Email = khachHang.Email,
                Hinh = khachHang.Hinh
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(ProfileVM model, IFormFile? Hinh)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy giá trị MaKh từ claims
                    var maKh = User.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
                    if (string.IsNullOrEmpty(maKh))
                    {
                        return RedirectToAction("DangNhap", "KhachHang");
                    }

                    // Tìm khách hàng theo MaKh
                    var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKh == maKh);
                    if (khachHang == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin người dùng từ ProfileVM
                    khachHang.HoTen = model.HoTen;
                    khachHang.DiaChi = model.DiaChi;
                    khachHang.DienThoai = model.DienThoai;
                    khachHang.Email = model.Email;

                    // Xử lý hình ảnh nếu có
                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.Update(khachHang);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Profile", "KhachHang");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Cập nhật thất bại: " + ex.Message;
                }
            }

            // Nếu có lỗi, quay lại trang EditProfile với thông tin hiện tại và hiển thị thông báo lỗi
            TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng kiểm tra và thử lại.";
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        #endregion
    }
}
