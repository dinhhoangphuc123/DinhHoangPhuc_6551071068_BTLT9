using System.Globalization;

namespace KeHoachDaNgoai;

/// <summary>Đa ngôn ngữ Việt / Anh: mỗi khóa có 2 chuỗi [vi, en]. Đổi ngôn ngữ có hiệu lực ngay.</summary>
public static class Lang
{
    public static string Code { get; private set; } = "vi";
    public static CultureInfo Culture { get; private set; } = new("vi-VN");

    public static void Set(string code)
    {
        Code = code == "en" ? "en" : "vi";
        Culture = new CultureInfo(Code == "en" ? "en-US" : "vi-VN");
        CultureInfo.CurrentCulture = Culture;
        CultureInfo.CurrentUICulture = Culture;
        CultureInfo.DefaultThreadCurrentCulture = Culture;
        CultureInfo.DefaultThreadCurrentUICulture = Culture;
    }

    public static string T(string key)
    {
        return Map.TryGetValue(key, out var v) ? v[Code == "en" ? 1 : 0] : key;
    }

    public static string T(string key, params object[] args)
    {
        return string.Format(Culture, T(key), args);
    }

    private static readonly Dictionary<string, string[]> Map = new()
    {
        // Giao diện chính
        ["form"] = new[] { "Kế hoạch dã ngoại", "Picnic Planner" },
        ["title"] = new[] { "Kế hoạch dã ngoại", "Picnic Planner" },
        ["lang"] = new[] { "Ngôn ngữ", "Language" },
        ["grpInfo"] = new[] { "Thông tin chuyến đi", "Trip details" },
        ["lblTen"] = new[] { "Tên chuyến", "Trip name" },
        ["lblDiaDiem"] = new[] { "Địa điểm", "Destination" },
        ["lblNgayDi"] = new[] { "Ngày đi", "Date" },
        ["lblSoNguoi"] = new[] { "Số người", "People" },
        ["lblNganSach"] = new[] { "Ngân sách", "Budget" },
        ["lblPhuongTien"] = new[] { "Phương tiện", "Transport" },
        ["chkHoanThanh"] = new[] { "Đã hoàn thành", "Completed" },
        ["btnThem"] = new[] { "Thêm", "Add" },
        ["btnSua"] = new[] { "Sửa", "Edit" },
        ["btnXoa"] = new[] { "Xóa", "Delete" },
        ["btnLamMoi"] = new[] { "Làm mới", "Refresh" },
        ["btnThoiTiet"] = new[] { "Xem thời tiết", "Check weather" },
        ["btnTim"] = new[] { "Tìm", "Search" },
        ["timHint"] = new[] { "Tìm theo tên chuyến hoặc địa điểm...", "Search by trip name or destination..." },
        ["chkChuaHT"] = new[] { "Chỉ hiện chuyến chưa hoàn thành", "Only show unfinished trips" },
        ["chkLocNgay"] = new[] { "Lọc theo ngày đi", "Filter by date" },
        ["lblTu"] = new[] { "Từ", "From" },
        ["lblDen"] = new[] { "Đến", "To" },

        // Cột lưới
        ["colMa"] = new[] { "Mã", "ID" },
        ["colTen"] = new[] { "Tên chuyến", "Trip" },
        ["colDiaDiem"] = new[] { "Địa điểm", "Destination" },
        ["colNgay"] = new[] { "Ngày đi", "Date" },
        ["colSoNguoi"] = new[] { "Số người", "People" },
        ["colNganSach"] = new[] { "Ngân sách", "Budget" },
        ["colPT"] = new[] { "Phương tiện", "Transport" },
        ["colHT"] = new[] { "Hoàn thành", "Done" },

        // Phương tiện (khóa là giá trị lưu trong CSDL)
        ["Xe máy"] = new[] { "Xe máy", "Motorbike" },
        ["Ô tô"] = new[] { "Ô tô", "Car" },
        ["Xe khách"] = new[] { "Xe khách", "Coach" },
        ["Tàu hỏa"] = new[] { "Tàu hỏa", "Train" },

        // Thời tiết
        ["wDefaultTitle"] = new[] { "Dự báo thời tiết", "Weather forecast" },
        ["wTitle"] = new[] { "Dự báo thời tiết ngày {0} - {1}", "Weather forecast for {0} - {1}" },
        ["wTemp"] = new[] { "Nhiệt độ: {0}°C - {1}°C", "Temperature: {0}°C - {1}°C" },
        ["wRain"] = new[] { "Khả năng mưa: {0}%", "Chance of rain: {0}%" },
        ["wAdvice"] = new[] { "Nên mang áo mưa hoặc cân nhắc dời lịch", "Bring a raincoat or consider rescheduling" },
        ["wNoData"] = new[] { "Chưa có dữ liệu dự báo", "No forecast data yet" },
        ["wNotFound"] = new[]
        {
            "Không tìm thấy địa danh \"{0}\". Hãy thử tên không dấu, ví dụ \"Da Lat\".",
            "Location \"{0}\" was not found. Try the name without accents, e.g. \"Da Lat\"."
        },
        ["wNetwork"] = new[]
        {
            "Không lấy được dự báo thời tiết. Vui lòng kiểm tra kết nối mạng và thử lại.",
            "Could not get the weather forecast. Please check your network connection and try again."
        },
        ["c0"] = new[] { "Trời quang", "Clear sky" },
        ["c1"] = new[] { "Có mây", "Cloudy" },
        ["c45"] = new[] { "Sương mù", "Fog" },
        ["c51"] = new[] { "Mưa", "Rain" },
        ["c95"] = new[] { "Dông", "Thunderstorm" },
        ["cOther"] = new[] { "Khác", "Other" },

        // Kiểm tra hợp lệ và thông báo
        ["vTitle"] = new[] { "Dữ liệu chưa hợp lệ", "Invalid data" },
        ["vTen"] = new[] { "Tên chuyến không được để trống.", "Trip name must not be empty." },
        ["vDiaDiem"] = new[] { "Địa điểm không được để trống.", "Destination must not be empty." },
        ["vPhuongTien"] = new[] { "Vui lòng chọn phương tiện.", "Please choose a transport." },
        ["vNgay"] = new[] { "Ngày đi không được nhỏ hơn hôm nay.", "The date must not be earlier than today." },
        ["vSoNguoi"] = new[] { "Số người phải từ 1 trở lên.", "Number of people must be at least 1." },
        ["vNganSach"] = new[] { "Ngân sách phải từ 0 trở lên.", "Budget must be 0 or more." },
        ["selectRow"] = new[] { "Hãy bấm chọn một dòng trong danh sách.", "Please select a row in the list." },
        ["selectTitle"] = new[] { "Chưa chọn dòng", "No row selected" },
        ["confirmDelete"] = new[] { "Bạn có chắc muốn xóa chuyến đi mã {0}?", "Delete trip #{0}?" },
        ["confirmTitle"] = new[] { "Xác nhận xóa", "Confirm delete" },
        ["dbError"] = new[]
        {
            "Không thể truy cập cơ sở dữ liệu. Vui lòng kiểm tra lại kết nối trong file appsettings.json rồi thử lại.",
            "Cannot access the database. Please check the connection string in appsettings.json and try again."
        },
        ["dbErrTitle"] = new[] { "Lỗi cơ sở dữ liệu", "Database error" },
        ["rowGone"] = new[] { "Chuyến đi này không còn tồn tại.", "This trip no longer exists." },
        ["filterErr"] = new[] { "\"Từ\" không được lớn hơn \"Đến\".", "\"From\" must not be later than \"To\"." },
        ["filterErrTitle"] = new[] { "Bộ lọc chưa hợp lệ", "Invalid filter" },
    };
}
