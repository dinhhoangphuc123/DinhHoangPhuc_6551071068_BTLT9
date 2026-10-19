namespace KeHoachDaNgoai;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Lang.Set(UserSettings.LoadLanguage());

        try
        {
            AppSettings.Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Không đọc được file appsettings.json cạnh file .exe.\n\n" + ex.Message,
                "Kế hoạch dã ngoại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.Run(new FormChuyenDi());
    }
}
