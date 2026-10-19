using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaTro.Data;
using Serilog;

namespace QuanLyNhaTro;

public partial class FormKhachThue : Form
{
    private static readonly CultureInfo Vn = new("vi-VN");

    private int? _selectedId;
    private int _wardRequest;                 // đánh số lần gọi API phường/xã để bỏ kết quả cũ
    private bool _tinhManual;                 // true khi không tải được tỉnh/thành -> gõ tay
    private List<DiaDanh> _tinhs = new();

    public FormKhachThue()
    {
        InitializeComponent();

        cboPhong.DisplayMember = "Text";
        cboPhong.ValueMember = "MaPhong";
        cboLocPhong.DisplayMember = "Text";
        cboLocPhong.ValueMember = "MaPhong";
        cboTinh.DisplayMember = "Name";
        cboTinh.ValueMember = "Code";
        cboPhuongXa.DisplayMember = "Name";
        cboPhuongXa.ValueMember = "Code";
    }

    private async void FormKhachThue_Load(object? sender, EventArgs e)
    {
        SetupGrid();
        dtpNgaySinh.Value = DateTime.Today.AddYears(-20);
        LoadPhongCombo(null);
        RefreshFilterCombos();
        LoadGrid();
        await LoadTinhAsync();
    }

    private void FormKhachThue_KeyDown(object? sender, KeyEventArgs e)
    {
        // Ctrl+Shift+E: hiện / ẩn nút "Gây lỗi thử"
        if (e.Control && e.Shift && e.KeyCode == Keys.E)
            btnGayLoi.Visible = !btnGayLoi.Visible;
    }

    private void btnGayLoi_Click(object? sender, EventArgs e)
    {
        throw new InvalidOperationException("Lỗi thử nghiệm do người dùng bấm nút 'Gây lỗi thử'.");
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
            DataPropertyName = nameof(KhachRow.HoTen), HeaderText = "Họ tên",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 30
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(KhachRow.SoDienThoai), HeaderText = "SĐT", Width = 95
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(KhachRow.TenPhong), HeaderText = "Phòng", Width = 60
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(KhachRow.GiaThue), HeaderText = "Giá thuê", Width = 90,
            DefaultCellStyle = { Format = "N0", FormatProvider = Vn, Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(KhachRow.QueQuan), HeaderText = "Quê quán",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 40
        });
        dgv.Columns.Add(new DataGridViewCheckBoxColumn
        {
            DataPropertyName = nameof(KhachRow.TamTru), HeaderText = "Tạm trú", Width = 60
        });
    }

    private static KhachRow ToRow(KhachThue k) => new()
    {
        MaKhach = k.MaKhach,
        HoTen = k.HoTen,
        SoDienThoai = k.SoDienThoai,
        TenPhong = k.MaPhongNavigation.TenPhong,
        GiaThue = k.MaPhongNavigation.GiaThue,
        QueQuan = string.Join(" – ", new[] { k.TenPhuongXa, k.TenTinh }.Where(s => !string.IsNullOrWhiteSpace(s))),
        TamTru = k.DaDangKyTamTru,
        Khach = k
    };

    private void LoadGrid()
    {
        var rows = new List<KhachRow>();
        var kw = txtTim.Text.Trim();
        var maPhong = cboLocPhong.SelectedValue is int pv ? pv : 0;
        var tinh = cboLocTinh.SelectedIndex > 0 ? cboLocTinh.SelectedItem as string : null;

        var ok = RunDb("tải danh sách khách thuê", db =>
        {
            var q = db.KhachThues.AsNoTracking().Include(k => k.MaPhongNavigation).AsQueryable();

            if (kw.Length > 0)
                q = q.Where(k => k.HoTen.Contains(kw) || k.SoDienThoai.Contains(kw));
            if (maPhong > 0)
                q = q.Where(k => k.MaPhong == maPhong);
            if (tinh != null)
                q = q.Where(k => k.TenTinh == tinh);

            rows = q.OrderBy(k => k.MaKhach).ToList().Select(ToRow).ToList();
        });
        if (!ok) return;

        dgv.DataSource = rows;
        dgv.ClearSelection();
    }

    private async void dgv_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || dgv.Rows[e.RowIndex].DataBoundItem is not KhachRow row) return;

        var k = row.Khach;
        _selectedId = k.MaKhach;

        txtHoTen.Text = k.HoTen;
        rdoNam.Checked = k.GioiTinh == "Nam";
        rdoNu.Checked = k.GioiTinh != "Nam";
        dtpNgaySinh.Value = k.NgaySinh.ToDateTime(TimeOnly.MinValue);
        txtSDT.Text = k.SoDienThoai;
        chkTamTru.Checked = k.DaDangKyTamTru;

        // Khi sửa: danh sách phòng gồm các phòng trống + phòng hiện tại của khách này
        LoadPhongCombo(k.MaPhong);
        cboPhong.SelectedValue = k.MaPhong;

        await ChonDiaChiAsync(k);
    }

    // =====================================================================
    //  COMBOBOX PHÒNG / BỘ LỌC
    // =====================================================================
    /// <summary>Nạp ComboBox phòng: chỉ các phòng "Trống" (cộng thêm phòng includeId nếu đang sửa).</summary>
    private void LoadPhongCombo(int? includeId)
    {
        var items = new List<PhongItem>();
        RunDb("tải danh sách phòng", db =>
        {
            var rooms = db.PhongTros.AsNoTracking()
                .Where(p => p.TinhTrang == "Trống" || p.MaPhong == includeId)
                .OrderBy(p => p.TenPhong)
                .ToList();
            items = rooms.Select(p => new PhongItem
            {
                MaPhong = p.MaPhong,
                Text = $"{p.TenPhong} - {p.GiaThue.ToString("N0", Vn)} đ"
            }).ToList();
        });

        cboPhong.DataSource = items;
        cboPhong.SelectedIndex = -1;
    }

    private void RefreshFilterCombos()
    {
        var phongSel = cboLocPhong.SelectedValue is int pv ? pv : 0;
        var tinhSel = cboLocTinh.SelectedItem as string;

        var phongs = new List<PhongItem> { new() { MaPhong = 0, Text = "Tất cả" } };
        var tinhs = new List<string> { "Tất cả" };

        RunDb("tải bộ lọc", db =>
        {
            phongs.AddRange(db.PhongTros.AsNoTracking()
                .OrderBy(p => p.TenPhong)
                .Select(p => new PhongItem { MaPhong = p.MaPhong, Text = p.TenPhong })
                .ToList());

            tinhs.AddRange(db.KhachThues.AsNoTracking()
                .Select(k => k.TenTinh ?? "")
                .Where(s => s != "")
                .Distinct()
                .OrderBy(s => s)
                .ToList());
        });

        cboLocPhong.DataSource = phongs;
        cboLocPhong.SelectedValue = phongSel;
        if (cboLocPhong.SelectedIndex < 0) cboLocPhong.SelectedIndex = 0;

        cboLocTinh.DataSource = tinhs;
        cboLocTinh.SelectedItem = tinhSel != null && tinhs.Contains(tinhSel) ? tinhSel : "Tất cả";
        if (cboLocTinh.SelectedIndex < 0) cboLocTinh.SelectedIndex = 0;
    }

    // =====================================================================
    //  API TỈNH / PHƯỜNG-XÃ
    // =====================================================================
    private static void ShowLoading(ComboBox c)
    {
        c.DropDownStyle = ComboBoxStyle.DropDownList;
        c.DataSource = new List<DiaDanh> { new() { Name = "Đang tải…", Code = 0 } };
        c.Enabled = false;
    }

    private static void BatNhapTay(ComboBox c)
    {
        c.DataSource = null;
        c.DropDownStyle = ComboBoxStyle.DropDown;
        c.Enabled = true;
        c.Text = "";
    }

    private static void ThongBaoMang()
    {
        MessageBox.Show(
            "Không tải được danh sách địa chỉ, vui lòng kiểm tra mạng.\n\nBạn vẫn có thể gõ tay Tỉnh/Thành và Phường/Xã để lưu khách.",
            "Mất kết nối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private async Task LoadTinhAsync()
    {
        ShowLoading(cboTinh);
        lblStatus.Text = "Đang tải danh sách tỉnh/thành…";
        try
        {
            var ds = await DiaChiApi.LayTinhAsync();
            _tinhs = ds;
            _tinhManual = false;

            cboTinh.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTinh.DataSource = ds;
            cboTinh.SelectedIndex = -1;
            cboTinh.Enabled = true;

            cboPhuongXa.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPhuongXa.DataSource = new List<DiaDanh>();
            cboPhuongXa.Enabled = true;

            lblStatus.Text = $"Đã tải {ds.Count} tỉnh/thành từ provinces.open-api.vn";
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            Log.Warning(ex, "Không tải được danh sách tỉnh/thành từ API");
            _tinhManual = true;
            _tinhs = new List<DiaDanh>();
            BatNhapTay(cboTinh);
            BatNhapTay(cboPhuongXa);
            lblStatus.Text = "Không tải được danh sách địa chỉ - nhập tay Tỉnh/Thành và Phường/Xã";
            ThongBaoMang();
        }
    }

    private async Task LoadPhuongXaAsync(int maTinh, int? chonMa = null, string? chonTen = null)
    {
        var req = ++_wardRequest;
        ShowLoading(cboPhuongXa);
        lblStatus.Text = "Đang tải danh sách phường/xã…";
        try
        {
            var ds = await DiaChiApi.LayPhuongXaAsync(maTinh);
            if (req != _wardRequest) return;     // người dùng đã chọn tỉnh khác

            cboPhuongXa.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPhuongXa.DataSource = ds;
            cboPhuongXa.Enabled = true;

            var w = ds.FirstOrDefault(x => x.Code == chonMa) ?? ds.FirstOrDefault(x => x.Name == chonTen);
            if (w != null) cboPhuongXa.SelectedValue = w.Code;
            else cboPhuongXa.SelectedIndex = -1;

            lblStatus.Text = $"Đã tải {ds.Count} phường/xã";
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            if (req != _wardRequest) return;
            Log.Warning(ex, "Không tải được phường/xã của tỉnh {MaTinh}", maTinh);
            BatNhapTay(cboPhuongXa);
            cboPhuongXa.Text = chonTen ?? "";
            lblStatus.Text = "Không tải được phường/xã - nhập tay";
            ThongBaoMang();
        }
    }

    private async void cboTinh_SelectionChangeCommitted(object? sender, EventArgs e)
    {
        if (cboTinh.SelectedValue is int code && code > 0)
            await LoadPhuongXaAsync(code);
    }

    /// <summary>Đưa tỉnh/phường-xã của khách đang chọn lên các ComboBox.</summary>
    private async Task ChonDiaChiAsync(KhachThue k)
    {
        if (_tinhManual)
        {
            cboTinh.Text = k.TenTinh ?? "";
            cboPhuongXa.Text = k.TenPhuongXa ?? "";
            return;
        }

        var t = _tinhs.FirstOrDefault(x => x.Code == k.MaTinh) ?? _tinhs.FirstOrDefault(x => x.Name == k.TenTinh);
        if (t == null)
        {
            cboTinh.SelectedIndex = -1;
            cboPhuongXa.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPhuongXa.DataSource = new List<DiaDanh>();
            cboPhuongXa.Enabled = true;
            return;
        }

        cboTinh.SelectedValue = t.Code;
        await LoadPhuongXaAsync(t.Code, k.MaPhuongXa, k.TenPhuongXa);
    }

    // =====================================================================
    //  CSDL
    // =====================================================================
    private bool RunDb(string hanhDong, Action<QlNhaTroContext> act)
    {
        try
        {
            using var db = QlNhaTroContext.Create();
            act(db);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Lỗi truy cập CSDL khi {HanhDong}", hanhDong);
            MessageBox.Show(
                "Không thể truy cập cơ sở dữ liệu. Vui lòng kiểm tra lại kết nối trong file appsettings.json rồi thử lại.\n\n(Chi tiết lỗi đã được ghi vào thư mục Logs.)",
                "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    /// <summary>Phòng "Đang thuê" nếu còn khách, ngược lại "Trống".</summary>
    private static void CapNhatTrangThai(QlNhaTroContext db, int maPhong)
    {
        var phong = db.PhongTros.Find(maPhong);
        if (phong is null) return;
        phong.TinhTrang = db.KhachThues.Any(k => k.MaPhong == maPhong) ? "Đang thuê" : "Trống";
        db.SaveChanges();
    }

    // =====================================================================
    //  NHẬP LIỆU
    // =====================================================================
    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            return Invalid("Họ tên không được để trống.", txtHoTen);
        if (dtpNgaySinh.Value.Date > DateTime.Today)
            return Invalid("Ngày sinh không được lớn hơn hôm nay.", dtpNgaySinh);
        if (!Regex.IsMatch(txtSDT.Text.Trim(), @"^\d{9,11}$"))
            return Invalid("Số điện thoại phải gồm 9 đến 11 chữ số.", txtSDT);
        if (cboPhong.SelectedValue is not int)
            return Invalid("Vui lòng chọn phòng.", cboPhong);
        if (!cboTinh.Enabled || !cboPhuongXa.Enabled)
            return Invalid("Danh sách địa chỉ đang được tải, vui lòng chờ giây lát.", cboTinh);
        if (string.IsNullOrWhiteSpace(cboTinh.Text))
            return Invalid("Vui lòng chọn (hoặc nhập) Tỉnh/Thành.", cboTinh);
        if (string.IsNullOrWhiteSpace(cboPhuongXa.Text))
            return Invalid("Vui lòng chọn (hoặc nhập) Phường/Xã.", cboPhuongXa);
        return true;
    }

    private static bool Invalid(string msg, Control c)
    {
        MessageBox.Show(msg, "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        c.Focus();
        return false;
    }

    private static int? CodeOf(ComboBox c) =>
        c.DropDownStyle == ComboBoxStyle.DropDownList && c.SelectedValue is int v && v > 0 ? v : null;

    private void FillEntity(KhachThue k)
    {
        k.HoTen = txtHoTen.Text.Trim();
        k.GioiTinh = rdoNam.Checked ? "Nam" : "Nữ";
        k.NgaySinh = DateOnly.FromDateTime(dtpNgaySinh.Value);
        k.SoDienThoai = txtSDT.Text.Trim();
        k.MaPhong = (int)cboPhong.SelectedValue!;
        k.MaTinh = CodeOf(cboTinh);
        k.TenTinh = cboTinh.Text.Trim();
        k.MaPhuongXa = CodeOf(cboPhuongXa);
        k.TenPhuongXa = cboPhuongXa.Text.Trim();
        k.DaDangKyTamTru = chkTamTru.Checked;
    }

    private void ClearInput()
    {
        _selectedId = null;
        txtHoTen.Clear();
        rdoNam.Checked = true;
        dtpNgaySinh.Value = DateTime.Today.AddYears(-20);
        txtSDT.Clear();
        chkTamTru.Checked = false;

        if (_tinhManual)
        {
            cboTinh.Text = "";
            cboPhuongXa.Text = "";
        }
        else if (cboTinh.Enabled)
        {
            cboTinh.SelectedIndex = -1;
            cboPhuongXa.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPhuongXa.DataSource = new List<DiaDanh>();
            cboPhuongXa.Enabled = true;
        }
    }

    /// <summary>Sau khi thêm/sửa/xóa: làm mới form, danh sách phòng, bộ lọc và lưới.</summary>
    private void LamMoiSauKhiLuu()
    {
        ClearInput();
        LoadPhongCombo(null);
        RefreshFilterCombos();
        LoadGrid();
    }

    // =====================================================================
    //  SỰ KIỆN NÚT
    // =====================================================================
    private void btnThem_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;

        var ok = RunDb("thêm khách thuê", db =>
        {
            using var tx = db.Database.BeginTransaction();
            var k = new KhachThue();
            FillEntity(k);
            db.KhachThues.Add(k);
            db.SaveChanges();
            CapNhatTrangThai(db, k.MaPhong);
            tx.Commit();
            Log.Information("Thêm khách thuê MaKhach={MaKhach} vào phòng MaPhong={MaPhong}", k.MaKhach, k.MaPhong);
        });
        if (ok) LamMoiSauKhiLuu();
    }

    private void btnSua_Click(object? sender, EventArgs e)
    {
        if (_selectedId is null)
        {
            MessageBox.Show("Hãy bấm chọn một dòng trong danh sách để sửa.", "Chưa chọn dòng",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!ValidateInput()) return;

        var id = _selectedId.Value;
        var notFound = false;
        var ok = RunDb("sửa khách thuê", db =>
        {
            using var tx = db.Database.BeginTransaction();
            var k = db.KhachThues.Find(id);
            if (k is null) { notFound = true; return; }

            var phongCu = k.MaPhong;
            FillEntity(k);
            db.SaveChanges();

            CapNhatTrangThai(db, phongCu);
            if (k.MaPhong != phongCu) CapNhatTrangThai(db, k.MaPhong);
            tx.Commit();
            Log.Information("Sửa khách thuê MaKhach={MaKhach}", id);
        });
        if (!ok) return;

        if (notFound)
            MessageBox.Show("Khách thuê này không còn tồn tại (có thể đã bị xóa).", "Không tìm thấy",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

        LamMoiSauKhiLuu();
    }

    private void btnXoa_Click(object? sender, EventArgs e)
    {
        if (_selectedId is null)
        {
            MessageBox.Show("Hãy bấm chọn một dòng trong danh sách để xóa.", "Chưa chọn dòng",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var id = _selectedId.Value;
        var confirm = MessageBox.Show($"Bạn có chắc muốn xóa khách thuê có mã {id}?", "Xác nhận xóa",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (confirm != DialogResult.Yes) return;

        var ok = RunDb("xóa khách thuê", db =>
        {
            using var tx = db.Database.BeginTransaction();
            var k = db.KhachThues.Find(id);
            if (k is null) return;

            var phong = k.MaPhong;
            db.KhachThues.Remove(k);
            db.SaveChanges();
            CapNhatTrangThai(db, phong);      // người cuối cùng rời đi -> phòng "Trống"
            tx.Commit();
            Log.Information("Xóa khách thuê MaKhach={MaKhach}", id);
        });
        if (ok) LamMoiSauKhiLuu();
    }

    private void btnLamMoi_Click(object? sender, EventArgs e)
    {
        ClearInput();
        LoadPhongCombo(null);
    }

    private void btnTim_Click(object? sender, EventArgs e) => LoadGrid();

    private void cboLoc_SelectionChangeCommitted(object? sender, EventArgs e) => LoadGrid();

    private void txtTim_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        LoadGrid();
    }

    private void btnBoLoc_Click(object? sender, EventArgs e)
    {
        txtTim.Clear();
        cboLocPhong.SelectedIndex = 0;
        cboLocTinh.SelectedIndex = 0;
        LoadGrid();
    }
}
