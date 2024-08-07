namespace ECommerceMVC.ViewModels
{
    public class EmployeeProfileVM
    {
        public string MaNv { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; } // Nếu bạn muốn cho phép đổi mật khẩu
        public string VaiTro { get; set; } // Thay đổi để lưu vai trò của nhân viên
    }
}
