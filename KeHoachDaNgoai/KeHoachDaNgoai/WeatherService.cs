using System.Globalization;
using System.Net.Http.Json;

namespace KeHoachDaNgoai;

/// <summary>Gọi Open-Meteo (miễn phí, không cần API key). Một HttpClient dùng chung, Timeout 8 giây.</summary>
public static class WeatherService
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(8) };

    /// <summary>Bước 1: đổi tên địa điểm thành tọa độ. Trả về null nếu không tìm thấy.</summary>
    public static async Task<GeoResult?> TimToaDoAsync(string diaDiem, string lang)
    {
        var url = "https://geocoding-api.open-meteo.com/v1/search"
                  + $"?name={Uri.EscapeDataString(diaDiem)}&count=1&language={lang}";
        var r = await Http.GetFromJsonAsync<GeocodeResponse>(url);
        return r?.Results?.FirstOrDefault();
    }

    /// <summary>Bước 2: lấy dự báo 16 ngày, tìm đúng chỉ số của ngày đi trong daily.time.</summary>
    public static async Task<DuBaoNgay?> LayDuBaoAsync(double lat, double lon, DateOnly ngayDi)
    {
        var inv = CultureInfo.InvariantCulture;
        var url = "https://api.open-meteo.com/v1/forecast"
                  + $"?latitude={lat.ToString(inv)}&longitude={lon.ToString(inv)}"
                  + "&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_probability_max"
                  + "&timezone=auto&forecast_days=16";

        var r = await Http.GetFromJsonAsync<ForecastResponse>(url);
        var d = r?.Daily;
        if (d is null) return null;

        var i = d.Time.IndexOf(ngayDi.ToString("yyyy-MM-dd", inv));
        if (i < 0) return null;
        if (i >= d.WeatherCode.Count || i >= d.TempMax.Count || i >= d.TempMin.Count || i >= d.RainProb.Count)
            return null;

        return new DuBaoNgay(
            d.WeatherCode[i] ?? -1,
            d.TempMin[i] ?? 0,
            d.TempMax[i] ?? 0,
            (int)Math.Round(d.RainProb[i] ?? 0));
    }
}
