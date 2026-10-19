# Quản lý nhà trọ (Bài tập chương 11 – Câu 2)

Ứng dụng WinForms .NET 8, EF Core Database First, SQL Server, Serilog, gọi API hành chính provinces.open-api.vn.

## Chạy trong Visual Studio

Mở SSMS, chạy file `Database/QL_NhaTro.sql` để tạo CSDL `QL_NhaTro` với 6 phòng và 5 khách mẫu. Sau đó mở `QuanLyNhaTro.csproj` trong Visual Studio, đợi NuGet khôi phục gói rồi bấm F5. Connection string trong `appsettings.json` và `appsettings.Development.json` đã trỏ sẵn tới `LAPTOP-O8K1T192\SQLEXPRESS`; khi chạy Debug app đọc file Development, còn bản publish chỉ đọc `appsettings.json`.

## Kiểm tra các yêu cầu

Khi mở form, ô Tỉnh/Thành hiện "Đang tải…" rồi tự đầy; chọn một tỉnh thì ô Phường/Xã nạp danh sách tương ứng. Để thử lỗi toàn cục, bấm Ctrl+Shift+E để hiện nút "Gây lỗi thử" (nếu chạy trong Visual Studio nên bấm Ctrl+F5 để không bị debugger dừng lại), bấm nút sẽ thấy hộp thoại thân thiện, app vẫn chạy tiếp và file `Logs\log-yyyyMMdd.txt` có đủ stack trace. Để thử mất mạng, tắt Wi-Fi rồi mở form: app báo "Không tải được danh sách địa chỉ", hai ô địa chỉ chuyển sang gõ tay được.

## Quy tắc phòng

Khi thêm mới, ô Phòng chỉ liệt kê các phòng đang "Trống". Khi sửa một khách, danh sách gồm các phòng trống cộng phòng hiện tại của khách đó. Sau mỗi lần thêm, sửa, xóa, trạng thái phòng được tính lại: còn khách thì "Đang thuê", hết khách thì "Trống".
