using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.ViewModels
{
    public class ProfileVM
    {
        [Display(Name = "Tên đăng nhập")]
        public required string MaKh { get; set; }

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public required string HoTen { get; set; }

        //public bool GioiTinh { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [Display(Name = "Địa chỉ")]
        [MaxLength(60, ErrorMessage = "Tối đa 60 kí tự")]
        public required string DiaChi { get; set; }

        [Display(Name = "Điện thoại")]
        [MaxLength(24, ErrorMessage = "Tối đa 24 kí tự")]
        [RegularExpression(@"^(03[2-9]|05[6|8|9]|070|076|077|078|079|08[1-5|8]|09[0-9])\d{7}$", ErrorMessage = "Chưa đúng định dạng di động Việt Nam")]
        public required string DienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public required string Email { get; set; }

        public string? Hinh { get; set; }
    }
}
