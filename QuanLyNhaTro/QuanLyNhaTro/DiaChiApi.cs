using System.Net.Http.Json;

namespace QuanLyNhaTro;

/// <summary>Gọi API hành chính Việt Nam. Dùng chung một HttpClient, Timeout 10 giây.</summary>
public static class DiaChiApi
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(10) };
    private const string BaseUrl = "https://provinces.open-api.vn/api/v2";

    public static async Task<List<DiaDanh>> LayTinhAsync()
    {
        var ds = await Http.GetFromJsonAsync<List<DiaDanh>>($"{BaseUrl}/");
        return ds ?? new List<DiaDanh>();
    }

    public static async Task<List<DiaDanh>> LayPhuongXaAsync(int maTinh)
    {
        var tinh = await Http.GetFromJsonAsync<TinhChiTiet>($"{BaseUrl}/p/{maTinh}?depth=2");
        return tinh?.Wards ?? new List<DiaDanh>();
    }
}
