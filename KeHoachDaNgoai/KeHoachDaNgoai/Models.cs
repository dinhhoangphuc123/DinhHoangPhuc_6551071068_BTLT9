using System.Text.Json.Serialization;

namespace KeHoachDaNgoai;

/// <summary>Mục hiển thị trong ComboBox phương tiện (Key = giá trị lưu CSDL, Text = chữ theo ngôn ngữ).</summary>
public class PtItem
{
    public string Key { get; set; } = "";
    public string Text { get; set; } = "";
}

public class GeocodeResponse
{
    [JsonPropertyName("results")]
    public List<GeoResult>? Results { get; set; }
}

public class GeoResult
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}

public class ForecastResponse
{
    [JsonPropertyName("daily")]
    public DailyData? Daily { get; set; }
}

public class DailyData
{
    [JsonPropertyName("time")]
    public List<string> Time { get; set; } = new();

    [JsonPropertyName("weather_code")]
    public List<int?> WeatherCode { get; set; } = new();

    [JsonPropertyName("temperature_2m_max")]
    public List<double?> TempMax { get; set; } = new();

    [JsonPropertyName("temperature_2m_min")]
    public List<double?> TempMin { get; set; } = new();

    [JsonPropertyName("precipitation_probability_max")]
    public List<double?> RainProb { get; set; } = new();
}

/// <summary>Dự báo của đúng ngày đi.</summary>
public record DuBaoNgay(int WeatherCode, double TempMin, double TempMax, int RainProb);
