using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceMVC.Controllers
{
	public class CartController : Controller
	{
		private readonly PaypalClient _paypalClient;
		private readonly Hshop2023Context db;

		public CartController(Hshop2023Context context, PaypalClient paypalClient)
		{
			_paypalClient = paypalClient;
			db = context;
		}

		public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

		public IActionResult Index()
		{
			return View(Cart);
		}

		public IActionResult AddToCart(int id, int quantity = 1)
		{
			var gioHang = Cart;
			var item = gioHang.SingleOrDefault(p => p.MaHh == id);
			if (item == null)
			{
				var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
				if (hangHoa == null)
				{
					TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
					return Redirect("/404");
				}
				item = new CartItem
				{
					MaHh = hangHoa.MaHh,
					TenHH = hangHoa.TenHh,
					DonGia = hangHoa.DonGia ?? 0,
					Hinh = hangHoa.Hinh ?? string.Empty,
					SoLuong = quantity
				};
				gioHang.Add(item);
			}
			else
			{
				item.SoLuong += quantity;
			}

			HttpContext.Session.Set(MySetting.CART_KEY, gioHang);

			return RedirectToAction("Index");
		}

		public IActionResult RemoveCart(int id)
		{
			var gioHang = Cart;
			var item = gioHang.SingleOrDefault(p => p.MaHh == id);
			if (item != null)
			{
				gioHang.Remove(item);
				HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
			}
			return RedirectToAction("Index");
		}
        [HttpGet]
        public IActionResult OrderFailed()
        {
            return View("Failed");
        }
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }
			ViewBag.PaypalClientdId = _paypalClient.ClientId;
			return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                }
                //Tạo thời gian giao dự kiến cho hóa đơn khi KH đặt hàng
                var ngayDat = DateTime.Now;
                var random = new Random();
                var deliveryMinutes = random.Next(10, 21); // Tạo một số ngẫu nhiên từ 10 đến 20 phút
                var ngayGiao = ngayDat.AddMinutes(deliveryMinutes);

                var hoadon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    DienThoai = model.DienThoai ?? khachHang.DienThoai,
                    NgayDat = ngayDat,
                    NgayGiao = ngayGiao,
                    CachThanhToan = "COD",
                    CachVanChuyen = "GRAB",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu
                };

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Add(hoadon);
                        db.SaveChanges();

                        var cthds = new List<ChiTietHd>();
                        foreach (var item in Cart)
                        {
                            cthds.Add(new ChiTietHd
                            {
                                MaHd = hoadon.MaHd,
                                SoLuong = item.SoLuong,
                                DonGia = item.DonGia,
                                MaHh = item.MaHh,
                                GiamGia = 0
                            });
                        }
                        db.AddRange(cthds);
                        db.SaveChanges();

                        // Commit the transaction
                        transaction.Commit();

                        // Clear the cart from session
                        HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                        return View("Success");
                    }
                    catch
                    {
                        // Rollback the transaction if there is an exception
                        
                        transaction.Rollback();
                        return RedirectToAction("OrderFailed"); // Chuyển hướng đến trang thất bại
                        //throw; // Optionally rethrow the exception to handle it elsewhere
                    }
                }
            }

            return View(Cart);
        }

		[Authorize]
		public IActionResult PaymentSuccess()
		{
			return View("Success");
		}

		#region Paypal payment
		[Authorize]
		[HttpPost("/Cart/create-paypal-order")]
		public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
		{
			// Thông tin đơn hàng gửi qua Paypal
			var tongTien = Cart.Sum(p => p.ThanhTien).ToString();
			var donViTienTe = "USD";
			var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

			try
			{
				var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);

				return Ok(response);
			}
			catch (Exception ex)
			{
				var error = new { ex.GetBaseException().Message };
				return BadRequest(error);
			}
		}
        [Authorize]
        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);

                if (response.status == "COMPLETED")
                {
                    var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
                    var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);

                    // Tạo hóa đơn và lưu vào cơ sở dữ liệu
                    var hoadon = new HoaDon
                    {
                        MaKh = customerId,
                        //HoTen = response.payer.name.given_name + " " + response.payer.name.surname,
                        HoTen = khachHang?.HoTen ?? response.payer.name.given_name + response.payer.name.surname,
                        DiaChi = khachHang?.DiaChi, // Lấy từ cơ sở dữ liệu hoặc giá trị mặc định
                        DienThoai = khachHang?.DienThoai ?? "Số điện thoại mặc định", // Lấy từ cơ sở dữ liệu hoặc giá trị mặc định
                        NgayDat = DateTime.Now,
                        NgayGiao = DateTime.Now.AddMinutes(new Random().Next(10, 21)),
                        CachThanhToan = "PayPal",
                        CachVanChuyen = "GRAB",
                        MaTrangThai = 1,
                        GhiChu = "Ghi chú mặc định" // Có thể lấy từ model hoặc gán giá trị mặc định
                    };

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.Add(hoadon);
                            db.SaveChanges();

                            var cthds = Cart.Select(item => new ChiTietHd
                            {
                                MaHd = hoadon.MaHd,
                                SoLuong = item.SoLuong,
                                DonGia = item.DonGia,
                                MaHh = item.MaHh,
                                GiamGia = 0
                            }).ToList();

                            db.AddRange(cthds);
                            db.SaveChanges();

                            transaction.Commit();

                            HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                            return View("Success");
                        }
                        catch
                        {
                            transaction.Rollback();
                            return RedirectToAction("OrderFailed");
                        }
                    }
                }
                else
                {
                    return BadRequest("Payment not completed");
                }
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        #endregion
    }
}
