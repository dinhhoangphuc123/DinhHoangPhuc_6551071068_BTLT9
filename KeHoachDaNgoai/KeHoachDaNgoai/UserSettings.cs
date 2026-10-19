using System.Text.Json;

namespace KeHoachDaNgoai;

/// <summary>Lưu lựa chọn ngôn ngữ vào %LocalAppData%\KeHoachDaNgoai\usersettings.json.</summary>
public class UserSettings
{
    public string Language { get; set; } = "vi";

    private static string FilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "KeHoachDaNgoai", "usersettings.json");

    public static string LoadLanguage()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                var s = JsonSerializer.Deserialize<UserSettings>(File.ReadAllText(FilePath));
                if (s?.Language == "en") return "en";
            }
        }
        catch (Exception)
        {
            // File hỏng hoặc không đọc được: dùng mặc định tiếng Việt
        }
        return "vi";
    }

    public static void SaveLanguage(string code)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            var json = JsonSerializer.Serialize(new UserSettings { Language = code },
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
        catch (Exception)
        {
            // Không lưu được thì thôi, không làm phiền người dùng
        }
    }
}
