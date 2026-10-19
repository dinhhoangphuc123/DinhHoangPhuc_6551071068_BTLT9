using Microsoft.Extensions.Configuration;

namespace SoChiTieu;

public class ConnectionStringsOptions
{
    public string QL_ChiTieu { get; set; } = "";
}

/// <summary>Lớp cấu hình có kiểu (strongly-typed), nạp từ appsettings.json.</summary>
public class AppSettings
{
    public ConnectionStringsOptions ConnectionStrings { get; set; } = new();

    public static AppSettings Current { get; private set; } = new();

    public static void Load()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
#if DEBUG
        // Chỉ ghi đè khi đang lập trình (Debug)
        builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false);
#endif
        Current = builder.Build().Get<AppSettings>() ?? new AppSettings();

        if (string.IsNullOrWhiteSpace(Current.ConnectionStrings.QL_ChiTieu))
            throw new InvalidOperationException("Thiếu ConnectionStrings:QL_ChiTieu trong appsettings.json.");
    }
}
