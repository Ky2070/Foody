using AutoMapper;
using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;

namespace ECommerceMVC.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Ánh xạ cho khách hàng
            CreateMap<RegisterVM, KhachHang>()
                .ForMember(dest => dest.HoTen, opt => opt.MapFrom(src => src.HoTen))
                .ReverseMap();

            // Ánh xạ cho nhân viên
            CreateMap<EmployeeRegisterVM, NhanVien>()
                .ForMember(dest => dest.MatKhau, opt => opt.Ignore()) // Không ánh xạ mật khẩu từ ViewModel
                .ForMember(dest => dest.HoTen, opt => opt.MapFrom(src => src.HoTen))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ReverseMap();

            CreateMap<EmployeeLoginVM, NhanVien>()
                .ForMember(dest => dest.MaNv, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.MatKhau, opt => opt.Ignore()) // Không ánh xạ mật khẩu từ ViewModel
                .ReverseMap();
        }
    }
}
