using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using KeHoachDaNgoai.Data;

namespace KeHoachDaNgoai;

public partial class FormChuyenDi : Form
{
    private static readonly string[] PhuongTienKeys = { "Xe máy", "Ô tô", "Xe khách", "Tàu hỏa" };

    private int? _selectedId;
    private bool _loading = true;
    private Action? _renderWeather;     // vẽ lại khung thời tiết khi đổi ngôn ngữ

    public FormChuyenDi()
    {
        InitializeComponent();
        cboPhuongTien.DisplayMember = "Text";
        cboPhuongTien.ValueMember = "Key";
    }

    private void FormChuyenDi_Load(object? sender, EventArgs e)
    {
        SetupGrid();
        cboNgonNgu.SelectedIndex = Lang.Code == "en" ? 1 : 0;

        dtpTu.Value = DateTime.Today;
        dtpDen.Value = DateTime.Today.AddDays(30);

        ApplyLanguage();
        _loading = false;
        LoadData();
    }

    // =====================================================================
    //  ĐA NGÔN NGỮ
    // =====================================================================
    private void cboNgonNgu_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_loading) return;

        Lang.Set(cboNgonNgu.SelectedIndex == 1 ? "en" : "vi");
        UserSettings.SaveLanguage(Lang.Code);
        ApplyLanguage();
    }

    /// <summary>Đổi toàn bộ nhãn, tiêu đề cột, nút, mô tả thời tiết và định dạng ngày/tiền ngay lập tức.</summary>
    private void ApplyLanguage()
    {
        Text = Lang.T("form");
        lblTieuDe.Text = Lang.T("title");
        lblNgonNgu.Text = Lang.T("lang");

        grpInfo.Text = Lang.T("grpInfo");
        lblTen.Text = Lang.T("lblTen");
        lblDiaDiem.Text = Lang.T("lblDiaDiem");
        lblNgayDi.Text = Lang.T("lblNgayDi");
        lblSoNguoi.Text = Lang.T("lblSoNguoi");
        lblNganSach.Text = Lang.T("lblNganSach");
        lblPhuongTien.Text = Lang.T("lblPhuongTien");
        chkHoanThanh.Text = Lang.T("chkHoanThanh");
        btnThem.Text = Lang.T("btnThem");
        btnSua.Text = Lang.T("btnSua");
        btnXoa.Text = Lang.T("btnXoa");
        btnLamMoi.Text = Lang.T("btnLamMoi");
        btnThoiTiet.Text = Lang.T("btnThoiTiet");

        txtTim.PlaceholderText = Lang.T("timHint");
        chkChuaHT.Text = Lang.T("chkChuaHT");
        chkLocNgay.Text = Lang.T("chkLocNgay");
        lblTu.Text = Lang.T("lblTu");
        lblDen.Text = Lang.T("lblDen");
        btnTim.Text = Lang.T("btnTim");

        var fmt = Lang.Code == "en" ? "MM/dd/yyyy" : "dd/MM/yyyy";
        dtpNgayDi.CustomFormat = fmt;
        dtpTu.CustomFormat = fmt;
        dtpDen.CustomFormat = fmt;

        dgv.Columns["colMa"]!.HeaderText = Lang.T("colMa");
        dgv.Columns["colTen"]!.HeaderText = Lang.T("colTen");
        dgv.Columns["colDiaDiem"]!.HeaderText = Lang.T("colDiaDiem");
        dgv.Columns["colNgay"]!.HeaderText = Lang.T("colNgay");
        dgv.Columns["colSoNguoi"]!.HeaderText = Lang.T("colSoNguoi");
        dgv.Columns["colNganSach"]!.HeaderText = Lang.T("colNganSach");
        dgv.Columns["colPT"]!.HeaderText = Lang.T("colPT");
        dgv.Columns["colHT"]!.HeaderText = Lang.T("colHT");

        BindPhuongTien();

        if (_renderWeather != null) _renderWeather();
        else grpThoiTiet.Text = Lang.T("wDefaultTitle");

        dgv.Invalidate();
    }

    private void BindPhuongTien()
    {
        var sel = cboPhuongTien.SelectedValue as string;
        cboPhuongTien.DataSource = PhuongTienKeys
            .Select(k => new PtItem { Key = k, Text = Lang.T(k) })
            .ToList();
        cboPhuongTien.SelectedValue = sel ?? PhuongTienKeys[0];
    }

    // =====================================================================
    //  LƯỚI
    // =====================================================================
    private void SetupGrid()
    {
        dgv.AutoGenerateColumns = false;
        dgv.ReadOnly = true;
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.MultiSelect = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colMa", DataPropertyName = nameof(ChuyenDi.MaChuyen), Width = 50
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colTen", DataPropertyName = nameof(ChuyenDi.TenChuyen),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 40
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colDiaDiem", DataPropertyName = nameof(ChuyenDi.DiaDiem), Width = 120
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colNgay", DataPropertyName = nameof(ChuyenDi.NgayDi), Width = 100,
            DefaultCellStyle = { Format = "d" }
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colSoNguoi", DataPropertyName = nameof(ChuyenDi.SoNguoi), Width = 80
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colNganSach", DataPropertyName = nameof(ChuyenDi.NganSach), Width = 110,
            DefaultCellStyle = { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colPT", DataPropertyName = nameof(ChuyenDi.PhuongTien), Width = 110
        });
        dgv.Columns.Add(new DataGridViewCheckBoxColumn
        {
            Name = "colHT", DataPropertyName = nameof(ChuyenDi.DaHoanThanh), Width = 90
        });
    }

    /// <summary>Cột phương tiện lưu tiếng Việt trong CSDL nhưng hiển thị theo ngôn ngữ đang chọn.</summary>
    private void dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
        if (dgv.Columns[e.ColumnIndex].Name == "colPT" && e.Value is string key)
        {
            e.Value = Lang.T(key);
            e.FormattingApplied = true;
        }
    }

    private void dgv_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || dgv.Rows[e.RowIndex].DataBoundItem is not ChuyenDi c) return;

        _selectedId = c.MaChuyen;
        txtTen.Text = c.TenChuyen;
        txtDiaDiem.Text = c.DiaDiem;
        dtpNgayDi.Value = c.NgayDi.ToDateTime(TimeOnly.MinValue);
        nudSoNguoi.Value = Math.Clamp(c.SoNguoi, (int)nudSoNguoi.Minimum, (int)nudSoNguoi.Maximum);
        nudNganSach.Value = Math.Min(c.NganSach, nudNganSach.Maximum);
        cboPhuongTien.SelectedValue = PhuongTienKeys.Contains(c.PhuongTien) ? c.PhuongTien : PhuongTienKeys[0];
        chkHoanThanh.Checked = c.DaHoanThanh;
    }

    // =====================================================================
    //  CSDL
    // =====================================================================
    private bool RunDb(Action<QlDaNgoaiContext> act)
    {
        try
        {
            using var db = QlDaNgoaiContext.Create();
            act(db);
            return true;
        }
        catch (Exception)
        {
            MessageBox.Show(Lang.T("dbError"), Lang.T("dbErrTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    private void LoadData()
    {
        if (chkLocNgay.Checked && dtpTu.Value.Date > dtpDen.Value.Date)
        {
            MessageBox.Show(Lang.T("filterErr"), Lang.T("filterErrTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var list = new List<ChuyenDi>();
        var ok = RunDb(db =>
        {
            var q = db.ChuyenDis.AsNoTracking().AsQueryable();

            var kw = txtTim.Text.Trim();
            if (kw.Length > 0)
                q = q.Where(x => x.TenChuyen.Contains(kw) || x.DiaDiem.Contains(kw));

            if (chkChuaHT.Checked)
                q = q.Where(x => !x.DaHoanThanh);

            if (chkLocNgay.Checked)
            {
                var tu = DateOnly.FromDateTime(dtpTu.Value);
                var den = DateOnly.FromDateTime(dtpDen.Value);
                q = q.Where(x => x.NgayDi >= tu && x.NgayDi <= den);
            }

            list = q.OrderBy(x => x.NgayDi).ThenBy(x => x.MaChuyen).ToList();
        });
        if (!ok) return;

        dgv.DataSource = list;
        dgv.ClearSelection();
    }

    // =====================================================================
    //  NHẬP LIỆU
    // =====================================================================
    private bool ValidateInput(bool isAdd)
    {
        if (string.IsNullOrWhiteSpace(txtTen.Text)) return Invalid("vTen", txtTen);
        if (string.IsNullOrWhiteSpace(txtDiaDiem.Text)) return Invalid("vDiaDiem", txtDiaDiem);
        if (cboPhuongTien.SelectedValue is not string) return Invalid("vPhuongTien", cboPhuongTien);
        if (isAdd && dtpNgayDi.Value.Date < DateTime.Today) return Invalid("vNgay", dtpNgayDi);
        if (nudSoNguoi.Value < 1) return Invalid("vSoNguoi", nudSoNguoi);
        if (nudNganSach.Value < 0) return Invalid("vNganSach", nudNganSach);
        return true;
    }

    private static bool Invalid(string key, Control c)
    {
        MessageBox.Show(Lang.T(key), Lang.T("vTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        c.Focus();
        return false;
    }

    private void FillEntity(ChuyenDi c)
    {
        c.TenChuyen = txtTen.Text.Trim();
        c.DiaDiem = txtDiaDiem.Text.Trim();
        c.NgayDi = DateOnly.FromDateTime(dtpNgayDi.Value);
        c.SoNguoi = (int)nudSoNguoi.Value;
        c.NganSach = nudNganSach.Value;
        c.PhuongTien = (string)cboPhuongTien.SelectedValue!;
        c.DaHoanThanh = chkHoanThanh.Checked;
    }

    private void ClearInput()
    {
        _selectedId = null;
        txtTen.Clear();
        txtDiaDiem.Clear();
        dtpNgayDi.Value = DateTime.Today;
        nudSoNguoi.Value = 1;
        nudNganSach.Value = 0;
        cboPhuongTien.SelectedIndex = 0;
        chkHoanThanh.Checked = false;
    }

    private void SelectRowFirst()
    {
        MessageBox.Show(Lang.T("selectRow"), Lang.T("selectTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // =====================================================================
    //  SỰ KIỆN NÚT
    // =====================================================================
    private void btnThem_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput(true)) return;

        var ok = RunDb(db =>
        {
            var c = new ChuyenDi();
            FillEntity(c);
            db.ChuyenDis.Add(c);
            db.SaveChanges();
        });
        if (!ok) return;

        ClearInput();
        LoadData();
    }

    private void btnSua_Click(object? sender, EventArgs e)
    {
        if (_selectedId is null) { SelectRowFirst(); return; }
        if (!ValidateInput(false)) return;

        var id = _selectedId.Value;
        var notFound = false;
        var ok = RunDb(db =>
        {
            var c = db.ChuyenDis.Find(id);
            if (c is null) { notFound = true; return; }
            FillEntity(c);
            db.SaveChanges();
        });
        if (!ok) return;

        if (notFound)
            MessageBox.Show(Lang.T("rowGone"), Lang.T("selectTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);

        ClearInput();
        LoadData();
    }

    private void btnXoa_Click(object? sender, EventArgs e)
    {
        if (_selectedId is null) { SelectRowFirst(); return; }

        var id = _selectedId.Value;
        var confirm = MessageBox.Show(Lang.T("confirmDelete", id), Lang.T("confirmTitle"),
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (confirm != DialogResult.Yes) return;

        var ok = RunDb(db =>
        {
            var c = db.ChuyenDis.Find(id);
            if (c is null) return;
            db.ChuyenDis.Remove(c);
            db.SaveChanges();
        });
        if (!ok) return;

        ClearInput();
        LoadData();
    }

    private void btnLamMoi_Click(object? sender, EventArgs e)
    {
        ClearInput();
        txtTim.Clear();
        chkChuaHT.Checked = false;
        chkLocNgay.Checked = false;
        dtpTu.Value = DateTime.Today;
        dtpDen.Value = DateTime.Today.AddDays(30);
        LoadData();
    }

    private void btnTim_Click(object? sender, EventArgs e) => LoadData();

    private void chkLocNgay_CheckedChanged(object? sender, EventArgs e)
    {
        dtpTu.Enabled = dtpDen.Enabled = chkLocNgay.Checked;
    }

    // =====================================================================
    //  THỜI TIẾT
    // =====================================================================
    private void SetBusy(bool busy)
    {
        btnThoiTiet.Enabled = !busy;
        progress.Visible = busy;
    }

    private void SetWeather(Action render)
    {
        _renderWeather = render;
        render();
    }

    private async void btnThoiTiet_Click(object? sender, EventArgs e)
    {
        var diaDiem = txtDiaDiem.Text.Trim();
        if (diaDiem.Length == 0) { Invalid("vDiaDiem", txtDiaDiem); return; }

        var ngay = DateOnly.FromDateTime(dtpNgayDi.Value);
        var today = DateOnly.FromDateTime(DateTime.Today);

        // Quá 16 ngày (hoặc đã qua) thì không có dữ liệu dự báo, khỏi gọi API
        if (ngay > today.AddDays(16) || ngay < today.AddDays(-1))
        {
            SetWeather(() => RenderMessage("wNoData", ngay, diaDiem));
            return;
        }

        SetBusy(true);
        try
        {
            var geo = await WeatherService.TimToaDoAsync(diaDiem, Lang.Code);
            if (geo is null)
            {
                SetWeather(() => RenderMessage("wNotFound", ngay, diaDiem));
                return;
            }

            var duBao = await WeatherService.LayDuBaoAsync(geo.Latitude, geo.Longitude, ngay);
            if (duBao is null)
                SetWeather(() => RenderMessage("wNoData", ngay, diaDiem));
            else
                SetWeather(() => RenderForecast(duBao, ngay, diaDiem));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            SetWeather(() => RenderMessage("wNetwork", ngay, diaDiem));
        }
        finally
        {
            SetBusy(false);
        }
    }

    private static string IconOf(int code) => code switch
    {
        0 => "☀",
        >= 1 and <= 3 => "⛅",
        45 or 48 => "🌫",
        (>= 51 and <= 67) or (>= 80 and <= 82) => "🌧",
        >= 95 and <= 99 => "⛈",
        _ => "🌡"
    };

    private static string DescOf(int code) => code switch
    {
        0 => Lang.T("c0"),
        >= 1 and <= 3 => Lang.T("c1"),
        45 or 48 => Lang.T("c45"),
        (>= 51 and <= 67) or (>= 80 and <= 82) => Lang.T("c51"),
        >= 95 and <= 99 => Lang.T("c95"),
        _ => Lang.T("cOther")
    };

    private void RenderForecast(DuBaoNgay d, DateOnly ngay, string diaDiem)
    {
        grpThoiTiet.Text = Lang.T("wTitle", ngay.ToString("d", Lang.Culture), diaDiem);
        lblIcon.Text = IconOf(d.WeatherCode);
        lblMoTa.Text = DescOf(d.WeatherCode);
        lblNhietDo.Text = Lang.T("wTemp", d.TempMin.ToString("0", Lang.Culture), d.TempMax.ToString("0", Lang.Culture));
        lblXacSuat.Text = Lang.T("wRain", d.RainProb);

        var mua = d.RainProb >= 60;
        pnlCanhBao.Visible = mua;
        lblCanhBao.Text = mua ? "⚠ " + Lang.T("wAdvice") : "";
    }

    private void RenderMessage(string key, DateOnly ngay, string diaDiem)
    {
        grpThoiTiet.Text = Lang.T("wTitle", ngay.ToString("d", Lang.Culture), diaDiem);
        lblIcon.Text = "";
        lblNhietDo.Text = "";
        lblXacSuat.Text = "";
        pnlCanhBao.Visible = false;
        lblMoTa.Text = key == "wNotFound" ? Lang.T(key, diaDiem) : Lang.T(key);
    }
}
