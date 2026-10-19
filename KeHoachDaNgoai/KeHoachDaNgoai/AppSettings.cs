using Microsoft.Extensions.Configuration;

namespace KeHoachDaNgoai;

public class ConnectionStringsOptions
{
    public string QL_DaNgoai { get; set; } = "";
}

/// <summary>Lớp cấu hình có kiểu, nạp từ appsettings.json.</summary>
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
        builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false);
#endif
        Current = builder.Build().Get<AppSettings>() ?? new AppSettings();

        if (string.IsNullOrWhiteSpace(Current.ConnectionStrings.QL_DaNgoai))
            throw new InvalidOperationException("Thiếu ConnectionStrings:QL_DaNgoai trong appsettings.json.");
    }
}
