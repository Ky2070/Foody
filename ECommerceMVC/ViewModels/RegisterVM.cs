using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.ViewModels
{
    public class RegisterVM : IValidatableObject
    {
        [Key]
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        public required string MaKh { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự")]
        public required string MatKhau { get; set; }

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public required string HoTen { get; set; }

        public bool GioiTinh { get; set; } = true;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "*")]
        [AgeValidation(16, ErrorMessage = "Người dùng phải từ 16 tuổi trở lên")]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Địa chỉ")]
        [MaxLength(60, ErrorMessage = "Tối đa 60 kí tự")]
        public string DiaChi { get; set; }

        [Display(Name = "Điện thoại")]
        [MaxLength(24, ErrorMessage = "Tối đa 24 kí tự")]
        [RegularExpression(@"^(03[2-9]|05[6|8|9]|070|076|077|078|079|08[1-5|8]|09[0-9])\d{7}$", ErrorMessage = "Chưa đúng định dạng di động Việt Nam")]
        public required string DienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public required string Email { get; set; }

        public string? Hinh { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MatKhau != null && MaKh != null && MatKhau.Contains(MaKh))
            {
                yield return new ValidationResult("Mật khẩu không được chứa tên đăng nhập", new[] { "MatKhau" });
            }
        }
    }

    public class AgeValidationAttribute : ValidationAttribute
    {
        private readonly int _minAge;
        public AgeValidationAttribute(int minAge)
        {
            _minAge = minAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTime birthDate = (DateTime)value;
                if (birthDate.AddYears(_minAge) > DateTime.Now)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
