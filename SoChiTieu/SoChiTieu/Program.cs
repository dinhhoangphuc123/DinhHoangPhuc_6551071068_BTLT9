using System.Text;
using Serilog;

namespace SoChiTieu;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            AppSettings.Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Không đọc được file appsettings.json cạnh file .exe.\n\n" + ex.Message,
                "Sổ chi tiêu cá nhân", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Log theo ngày vào thư mục Logs cạnh file .exe -> Logs\log-yyyyMMdd.txt
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                Path.Combine(AppContext.BaseDirectory, "Logs", "log-.txt"),
                rollingInterval: RollingInterval.Day,
                encoding: Encoding.UTF8)
            .CreateLogger();

        try
        {
            Log.Information("Ứng dụng khởi động");
            Application.Run(new FormChiTieu());
        }
        finally
        {
            Log.Information("Ứng dụng đóng");
            Log.CloseAndFlush();
        }
    }
}
