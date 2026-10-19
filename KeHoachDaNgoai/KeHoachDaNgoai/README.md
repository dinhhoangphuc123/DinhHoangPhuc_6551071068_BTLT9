# Kế hoạch dã ngoại (Bài tập chương 11 – Câu 3)

Ứng dụng WinForms .NET 8, EF Core Database First, SQL Server, gọi API thời tiết Open-Meteo, hỗ trợ tiếng Việt và tiếng Anh.

## Chạy trong Visual Studio

Mở SSMS, chạy file `Database/QL_DaNgoai.sql` để tạo CSDL `QL_DaNgoai` với 8 chuyến đi mẫu (ngày đi tính theo ngày hôm nay, từ 3 đến 28 ngày tới). Sau đó mở `KeHoachDaNgoai.csproj` bằng Visual Studio, đợi NuGet khôi phục gói rồi bấm F5. Connection string trong `appsettings.json` và `appsettings.Development.json` đã trỏ sẵn tới `LAPTOP-O8K1T192\SQLEXPRESS`.

## Xem thời tiết

Chọn một chuyến trong danh sách rồi bấm "Xem thời tiết". Ứng dụng gọi hai bước: tìm tọa độ địa điểm qua geocoding-api.open-meteo.com, rồi lấy dự báo 16 ngày từ api.open-meteo.com và đọc đúng ngày đi. Nếu xác suất mưa từ 60% trở lên thì hiện khung cam kèm lời khuyên. Ngày đi cách hôm nay hơn 16 ngày sẽ báo "Chưa có dữ liệu dự báo". Đề bài ghi tên miền là `openmeteo.com` (không có dấu gạch ngang) nhưng tên miền thật của Open-Meteo là `open-meteo.com`, nên code dùng tên miền thật.

## Đa ngôn ngữ

Chọn ngôn ngữ ở góc trên bên phải, giao diện đổi ngay không cần mở lại ứng dụng. Lựa chọn được lưu vào `%LocalAppData%\KeHoachDaNgoai\usersettings.json` và tự áp dụng ở lần mở sau.
