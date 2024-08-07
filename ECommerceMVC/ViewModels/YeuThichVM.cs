using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.ViewModels
{
	public class YeuThichVM
	{
		public int MaHh { get; set; }

		[Required]
		public required string MoTa { get; set; }

		[Required]
		[Range(1, 5, ErrorMessage = "Mức độ yêu thích phải từ 1 đến 5")]
		public int MucDoYeuThich { get; set; }

		[Required]  // Nếu cần thiết, bạn có thể để thuộc tính này bắt buộc
		public string HoTenKhachHang { get; set; }
	}
}
