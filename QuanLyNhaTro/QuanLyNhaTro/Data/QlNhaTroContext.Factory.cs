using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaTro.Data;

public partial class QlNhaTroContext
{
    /// <summary>Tạo DbContext với connection string đọc từ appsettings.json.</summary>
    public static QlNhaTroContext Create()
    {
        var options = new DbContextOptionsBuilder<QlNhaTroContext>()
            .UseSqlServer(AppSettings.Current.ConnectionStrings.QL_NhaTro)
            .Options;
        return new QlNhaTroContext(options);
    }
}
