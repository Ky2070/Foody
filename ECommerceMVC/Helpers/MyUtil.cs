using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ECommerceMVC.Helpers
{
    public class MyUtil
    {
        public static string UploadHinh(IFormFile hinhUpload, string folder)
        {
            if (hinhUpload == null || hinhUpload.Length == 0)
            {
                return "default.jpg"; // Trả về giá trị mặc định nếu không có file
            }

            try
            {
                var fileName = Path.GetFileName(hinhUpload.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    hinhUpload.CopyTo(stream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                return "default.jpg"; // Trả về giá trị mặc định nếu có lỗi
            }
        }




        public static string GenerateRamdomKey(int length = 5)
        {
            var pattern = @"qazwsxedcrfvtgbyhnujmiklopQAZWSXEDCRFVTGBYHNUJMIKLOP!";
            var sb = new StringBuilder();
            var rd = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }

            return sb.ToString();
        }
    }
}
