using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Contract.DTOs
{
    // Routine tạo/sửa
    // Routine session type
    public enum RoutineSession
    {
        Morning,
        Noon,
        Night
    }

    // Tạo routine mới từ ảnh
    public class CreateRoutineRequestDto
    {
        public IFormFile Image { get; set; }
    }

    // Product dùng trong routine
    public class RoutineProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string ImageLink { get; set; }
        public List<string> Categories { get; set; }
    }

    // Kết quả routine trả về
    public class RoutineResultDto
    {
        public Guid RoutineId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<RoutineProductDto> Morning { get; set; }
        public List<RoutineProductDto> Noon { get; set; }
        public List<RoutineProductDto> Night { get; set; }
        public string Advice { get; set; }
    }

    // Đánh dấu/check sản phẩm từng buổi
    public class CheckRoutineProductDto
    {
        public Guid RoutineId { get; set; }
        public Guid ProductId { get; set; }
        public RoutineSession Session { get; set; }
        public DateTime UsageDate { get; set; } // chỉ truyền yyyy-MM-dd là đủ
        public bool IsChecked { get; set; }
    }

    // Lịch sử checkin/hoàn thành từng ngày
    public class RoutineCheckHistoryDto
    {
        public DateTime UsageDate { get; set; }
        public int Total { get; set; }    // tổng số cần dùng
        public int Checked { get; set; }  // số đã check
        public int Percent { get; set; }
    }
    public class RoutineDailyProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Session { get; set; } // "morning", "noon", "night"
        public bool IsChecked { get; set; }
        public string ImageLink { get; set; }
        public List<string> Categories { get; set; }
    }
    public class RoutineDailyResultDto
    {
        public DateTime UsageDate { get; set; }
        public List<RoutineDailyProductDto> Morning { get; set; }
        public List<RoutineDailyProductDto> Noon { get; set; }
        public List<RoutineDailyProductDto> Night { get; set; }
        public int Total { get; set; }
        public int Checked { get; set; }
        public int Percent { get; set; }
    }



}
