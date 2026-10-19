using Microsoft.EntityFrameworkCore;

namespace KeHoachDaNgoai.Data;

public partial class QlDaNgoaiContext
{
    /// <summary>Tạo DbContext với connection string đọc từ appsettings.json.</summary>
    public static QlDaNgoaiContext Create()
    {
        var options = new DbContextOptionsBuilder<QlDaNgoaiContext>()
            .UseSqlServer(AppSettings.Current.ConnectionStrings.QL_DaNgoai)
            .Options;
        return new QlDaNgoaiContext(options);
    }
}
