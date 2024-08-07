using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.ViewModels
{
    public class EmployeeRegisterVM : IValidatableObject
    {
        [Key]
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        public required string MaNv { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự")]
        public required string MatKhau { get; set; }

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public required string HoTen { get; set; }

        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public required string Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MatKhau != null && MaNv != null && MatKhau.Contains(MaNv))
            {
                yield return new ValidationResult("Mật khẩu không được chứa tên đăng nhập", new[] { "MatKhau" });
            }
        }
    }
}

