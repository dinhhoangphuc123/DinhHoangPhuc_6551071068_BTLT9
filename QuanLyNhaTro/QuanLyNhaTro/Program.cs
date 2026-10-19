using System.Text;
using Serilog;

namespace QuanLyNhaTro;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // Lỗi trong luồng giao diện sẽ đi vào Application.ThreadException thay vì làm chết app
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        ApplicationConfiguration.Initialize();

        try
        {
            AppSettings.Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Không đọc được file appsettings.json cạnh file .exe.\n\n" + ex.Message,
                "Quản lý nhà trọ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                Path.Combine(AppContext.BaseDirectory, "Logs", "log-.txt"),
                rollingInterval: RollingInterval.Day,
                encoding: Encoding.UTF8)
            .CreateLogger();

        Application.ThreadException += (s, e) => XuLyLoi(e.Exception, "ThreadException");
        AppDomain.CurrentDomain.UnhandledException += (s, e) => XuLyLoi(e.ExceptionObject as Exception, "UnhandledException");

        try
        {
            Log.Information("Ứng dụng khởi động");
            Application.Run(new FormKhachThue());
        }
        finally
        {
            Log.Information("Ứng dụng đóng");
            Log.CloseAndFlush();
        }
    }

    private static void XuLyLoi(Exception? ex, string nguon)
    {
        Log.Error(ex, "Lỗi chưa được bắt ({Nguon})", nguon);
        MessageBox.Show(
            "Đã xảy ra lỗi, thông tin đã được ghi lại.",
            "Quản lý nhà trọ", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
