# Sổ chi tiêu cá nhân (Bài tập chương 11 – Câu 1)

Ứng dụng WinForms .NET 8, EF Core Database First, SQL Server, Serilog.

## Chạy trong Visual Studio

Cài Visual Studio 2022 (bản 17.8 trở lên) với workload ".NET desktop development". Mở file `SoChiTieu.csproj`, VS sẽ tự khôi phục các gói NuGet. Trước khi chạy, mở SSMS và chạy file `Database/QL_ChiTieu.sql` để tạo CSDL `QL_ChiTieu` cùng 12 dòng dữ liệu mẫu. Sau đó sửa connection string trong `appsettings.json` (và `appsettings.Development.json` khi chạy Debug) cho đúng tên SQL Server của bạn, rồi bấm F5.

## Các gói NuGet

Microsoft.EntityFrameworkCore.SqlServer 8.0.11, Microsoft.EntityFrameworkCore.Design 8.0.11, Microsoft.Extensions.Configuration.Json 8.0.1, Microsoft.Extensions.Configuration.Binder 8.0.2, Serilog 4.2.0, Serilog.Sinks.File 6.0.0. Nếu cần cài thủ công, dùng `dotnet restore`.

## Scaffold lại Model (nếu thầy cô yêu cầu chạy lệnh)

Thư mục `Data` đã có sẵn Model và DbContext giống hệt kết quả scaffold. Muốn sinh lại, chạy trong thư mục project:

    dotnet ef dbcontext scaffold "Server=.;Database=QL_ChiTieu;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Data --context QlChiTieuContext --force

Lệnh này sẽ ghi đè `ChiTieu.cs` và `QlChiTieuContext.cs`; hãy xóa phần `OnConfiguring` chứa connection string cứng mà scaffold sinh ra, vì ứng dụng dùng `QlChiTieuContext.Create()` trong `QlChiTieuContext.Factory.cs`.

## Đóng gói (Framework-Dependent, win-x64)

Chạy `publish.bat` (hoặc trong VS: chuột phải project → Publish → Folder, Target runtime win-x64, Deployment mode Framework-dependent). Chép cả thư mục `publish` sang máy khác đã cài .NET 8 Desktop Runtime, mở `appsettings.json` trong thư mục đó, sửa connection string cho đúng máy, rồi chạy `SoChiTieu.exe`. Không cần build lại. Log nằm trong thư mục `Logs` cạnh file .exe, tên dạng `log-yyyyMMdd.txt`.

## Cấu hình và Git

`appsettings.json` và `appsettings.Development.json` có thể chứa mật khẩu nên đã nằm trong `.gitignore`. Hãy dùng `appsettings.example.json` làm file mẫu khi đưa code lên GitHub.
