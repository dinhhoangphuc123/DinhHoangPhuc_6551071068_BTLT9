using Microsoft.EntityFrameworkCore;

namespace SoChiTieu.Data;

public partial class QlChiTieuContext
{
    /// <summary>Tạo DbContext với connection string đọc từ appsettings.json.</summary>
    public static QlChiTieuContext Create()
    {
        var options = new DbContextOptionsBuilder<QlChiTieuContext>()
            .UseSqlServer(AppSettings.Current.ConnectionStrings.QL_ChiTieu)
            .Options;
        return new QlChiTieuContext(options);
    }
}
