using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.ViewModels
{
	public class HangHoaVM
	{
        public int MaHh { get; set; }
        [Required(ErrorMessage = "Tên hàng hóa là bắt buộc.")]
        public string TenHH { get; set; }
        public required string Hinh { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là giá trị dương.")]
        public double DonGia { get; set; }
        [Required(ErrorMessage = "Mô tả ngắn là bắt buộc.")]
        public string MoTaNgan { get; set; }
        [Required(ErrorMessage = "Tên loại hàng hóa là bắt buộc.")]
        public string TenLoai { get; set; }
        public int MaLoai { get; set; }
        public List<YeuThichVM> YeuThichs { get; set; } = new List<YeuThichVM>();
    }

	public class ChiTietHangHoaVM
	{
		public int MaHh { get; set; }
		public required string TenHH { get; set; }
		public required string Hinh { get; set; }
		[Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
		public double DonGia { get; set; }
		public required string MoTaNgan { get; set; }
		public required string TenLoai { get; set; }
		public required string ChiTiet { get; set; }
		[Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
		public int DiemDanhGia { get; set; }
		[Range(0, int.MaxValue, ErrorMessage = "Quantity in stock must be a non-negative value.")]
		public int SoLuongTon { get; set; }
		public List<YeuThichVM> YeuThichs { get; set; } = new List<YeuThichVM>(); // Thêm thuộc tính này
	}

}
