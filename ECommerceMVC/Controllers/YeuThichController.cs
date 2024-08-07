using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using System.Security.Claims;
using ECommerceMVC.Helpers;
using System.Linq;

namespace ECommerceMVC.Controllers
{
	[Authorize]
	public class YeuThichController : Controller
	{
		private readonly Hshop2023Context _context;

		public YeuThichController(Hshop2023Context context)
		{
			_context = context;
		}

		[HttpPost]
		public IActionResult Create(YeuThichVM viewModel)
		{
			if (!User.Identity.IsAuthenticated)
			{
				// Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập với ReturnUrl
				var returnUrl = Url.Action("Detail", "HangHoa", new { id = viewModel.MaHh });
				return RedirectToAction("DangNhap", "KhachHang", new { ReturnUrl = returnUrl });
			}

			if (ModelState.IsValid)
			{
				var userId = User.FindFirstValue(MySetting.CLAIM_CUSTOMERID);

				if (userId != null)
				{
					try
					{
						var yeuThich = new YeuThich
						{
							MaHh = viewModel.MaHh,
							MaKh = userId,
							MoTa = viewModel.MoTa,
							MucDoYeuThich = viewModel.MucDoYeuThich,
							NgayChon = DateTime.Now
						};

						_context.YeuThiches.Add(yeuThich);
						_context.SaveChanges();

						// Lưu thông báo thành công vào TempData
						TempData["SuccessMessage"] = "Bình luận của bạn đã được gửi thành công.";

						return RedirectToAction("Detail", "HangHoa", new { id = viewModel.MaHh });
					}
					catch (Exception ex)
					{
						// Ghi lỗi vào log (nếu cần)
						// Log.Error(ex, "Failed to create YeuThich");

						TempData["ErrorMessage"] = "Có lỗi xảy ra khi thực hiện yêu thích. Vui lòng thử lại.";
						return RedirectToAction("Detail", "HangHoa", new { id = viewModel.MaHh });
					}
				}
			}

			TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra và thử lại.";
			return RedirectToAction("Detail", "HangHoa", new { id = viewModel.MaHh });
		}
	}
}
