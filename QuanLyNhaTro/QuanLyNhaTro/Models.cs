using System.Text.Json.Serialization;
using QuanLyNhaTro.Data;

namespace QuanLyNhaTro;

/// <summary>Một tỉnh/thành hoặc phường/xã trả về từ API.</summary>
public class DiaDanh
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("code")]
    public int Code { get; set; }
}

/// <summary>Kết quả GET /p/{code}?depth=2 (chỉ lấy danh sách phường/xã).</summary>
public class TinhChiTiet
{
    [JsonPropertyName("wards")]
    public List<DiaDanh> Wards { get; set; } = new();
}

/// <summary>Mục hiển thị trong ComboBox phòng.</summary>
public class PhongItem
{
    public int MaPhong { get; set; }
    public string Text { get; set; } = "";
}

/// <summary>Một dòng của DataGridView (kết quả JOIN khách + phòng).</summary>
public class KhachRow
{
    public int MaKhach { get; set; }
    public string HoTen { get; set; } = "";
    public string SoDienThoai { get; set; } = "";
    public string TenPhong { get; set; } = "";
    public decimal GiaThue { get; set; }
    public string QueQuan { get; set; } = "";
    public bool TamTru { get; set; }
    public KhachThue Khach { get; set; } = null!;
}
