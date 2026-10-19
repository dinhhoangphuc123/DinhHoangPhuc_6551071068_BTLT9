using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SoChiTieu.Data;

namespace SoChiTieu;

public partial class FormChiTieu : Form
{
    private static readonly CultureInfo Vn = new("vi-VN");
    private static readonly string[] DanhSachLoai = { "Ăn uống", "Đi lại", "Học tập", "Giải trí", "Khác" };

    private int? _selectedId;

    public FormChiTieu()
    {
        InitializeComponent();
    }

    private void FormChiTieu_Load(object? sender, EventArgs e)
    {
        cboLoai.Items.AddRange(DanhSachLoai);
        cboLoai.SelectedIndex = 0;

        cboLoc.Items.Add("Tất cả");
        cboLoc.Items.AddRange(DanhSachLoai);
        cboLoc.SelectedIndex = 0;

        var today = DateTime.Today;
        dtpTu.Value = new DateTime(today.Year, today.Month, 1);
        dtpDen.Value = today;

        SetupGrid();
        LoadData();
    }

    // ---------- Lưới ----------
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
            DataPropertyName = nameof(ChiTieu.MaCt),
            HeaderText = "Mã",
            Width = 60
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(ChiTieu.NgayChi),
            HeaderText = "Ngày chi",
            Width = 100,
            DefaultCellStyle = { Format = "dd/MM/yyyy" }
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(ChiTieu.NoiDung),
            HeaderText = "Nội dung",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            FillWeight = 40
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(ChiTieu.LoaiChi),
            HeaderText = "Loại chi",
            Width = 100
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(ChiTieu.SoTien),
            HeaderText = "Số tiền",
            Width = 110,
            DefaultCellStyle = { Format = "N0", FormatProvider = Vn, Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(ChiTieu.GhiChu),
            HeaderText = "Ghi chú",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            FillWeight = 40
        });
    }

    private void dgv_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || dgv.Rows[e.RowIndex].DataBoundItem is not ChiTieu ct) return;

        _selectedId = ct.MaCt;
        dtpNgay.Value = ct.NgayChi.ToDateTime(TimeOnly.MinValue);
        txtNoiDung.Text = ct.NoiDung;
        var idx = cboLoai.Items.IndexOf(ct.LoaiChi);
        cboLoai.SelectedIndex = idx >= 0 ? idx : DanhSachLoai.Length - 1;
        nudSoTien.Value = Math.Min(ct.SoTien, nudSoTien.Maximum);
        txtGhiChu.Text = ct.GhiChu ?? "";
    }

    // ---------- Truy cập CSDL (bọc try/catch tập trung) ----------
    private bool RunDb(string hanhDong, Action<QlChiTieuContext> act)
    {
        try
        {
            using var db = QlChiTieuContext.Create();
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

    /// <summary>Tải danh sách theo bộ lọc hiện tại và cập nhật Label tổng chi.</summary>
    private void LoadData()
    {
        if (chkLocNgay.Checked && dtpTu.Value.Date > dtpDen.Value.Date)
        {
            MessageBox.Show("\"Từ ngày\" không được lớn hơn \"Đến ngày\".", "Bộ lọc chưa hợp lệ",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var list = new List<ChiTieu>();
        var ok = RunDb("tải danh sách chi tiêu", db =>
        {
            var q = db.ChiTieus.AsNoTracking().AsQueryable();

            var kw = txtTim.Text.Trim();
            if (kw.Length > 0)
                q = q.Where(x => x.NoiDung.Contains(kw));

            if (cboLoc.SelectedIndex > 0)
            {
                var loai = (string)cboLoc.SelectedItem!;
                q = q.Where(x => x.LoaiChi == loai);
            }

            if (chkLocNgay.Checked)
            {
                var tu = DateOnly.FromDateTime(dtpTu.Value);
                var den = DateOnly.FromDateTime(dtpDen.Value);
                q = q.Where(x => x.NgayChi >= tu && x.NgayChi <= den);
            }

            list = q.OrderBy(x => x.MaCt).ToList();
        });
        if (!ok) return;

        dgv.DataSource = list;
        dgv.ClearSelection();
        lblTong.Text = $"Tổng chi: {list.Sum(x => x.SoTien).ToString("N0", Vn)} đ";
    }

    // ---------- Nhập liệu ----------
    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtNoiDung.Text))
            return Invalid("Nội dung không được để trống.", txtNoiDung);
        if (cboLoai.SelectedIndex < 0)
            return Invalid("Vui lòng chọn loại chi.", cboLoai);
        if (nudSoTien.Value <= 0)
            return Invalid("Số tiền phải lớn hơn 0.", nudSoTien);
        if (dtpNgay.Value.Date > DateTime.Today)
            return Invalid("Ngày chi không được lớn hơn hôm nay.", dtpNgay);
        return true;
    }

    private static bool Invalid(string msg, Control c)
    {
        MessageBox.Show(msg, "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        c.Focus();
        return false;
    }

    private void FillEntity(ChiTieu ct)
    {
        ct.NgayChi = DateOnly.FromDateTime(dtpNgay.Value);
        ct.NoiDung = txtNoiDung.Text.Trim();
        ct.LoaiChi = (string)cboLoai.SelectedItem!;
        ct.SoTien = nudSoTien.Value;
        var ghiChu = txtGhiChu.Text.Trim();
        ct.GhiChu = ghiChu.Length == 0 ? null : ghiChu;
    }

    private void ClearInput()
    {
        _selectedId = null;
        dtpNgay.Value = DateTime.Today;
        txtNoiDung.Clear();
        cboLoai.SelectedIndex = 0;
        nudSoTien.Value = 0;
        txtGhiChu.Clear();
    }

    // ---------- Sự kiện nút ----------
    private void btnThem_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;

        var ok = RunDb("thêm chi tiêu", db =>
        {
            var ct = new ChiTieu();
            FillEntity(ct);
            db.ChiTieus.Add(ct);
            db.SaveChanges();
            Log.Information("Thêm chi tiêu MaCT={MaCT}", ct.MaCt);
        });
        if (!ok) return;

        ClearInput();
        LoadData();
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
        var ok = RunDb("sửa chi tiêu", db =>
        {
            var ct = db.ChiTieus.Find(id);
            if (ct is null) { notFound = true; return; }
            FillEntity(ct);
            db.SaveChanges();
            Log.Information("Sửa chi tiêu MaCT={MaCT}", id);
        });
        if (!ok) return;

        if (notFound)
            MessageBox.Show("Khoản chi này không còn tồn tại (có thể đã bị xóa).", "Không tìm thấy",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

        ClearInput();
        LoadData();
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
        var confirm = MessageBox.Show($"Bạn có chắc muốn xóa khoản chi có mã {id}?", "Xác nhận xóa",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (confirm != DialogResult.Yes) return;

        var ok = RunDb("xóa chi tiêu", db =>
        {
            var ct = db.ChiTieus.Find(id);
            if (ct is null) return;
            db.ChiTieus.Remove(ct);
            db.SaveChanges();
            Log.Information("Xóa chi tiêu MaCT={MaCT}", id);
        });
        if (!ok) return;

        ClearInput();
        LoadData();
    }

    private void btnTim_Click(object? sender, EventArgs e) => LoadData();

    private void btnLamMoi_Click(object? sender, EventArgs e)
    {
        ClearInput();
        txtTim.Clear();
        cboLoc.SelectedIndex = 0;
        chkLocNgay.Checked = false;
        var today = DateTime.Today;
        dtpTu.Value = new DateTime(today.Year, today.Month, 1);
        dtpDen.Value = today;
        LoadData();
    }

    private void chkLocNgay_CheckedChanged(object? sender, EventArgs e)
    {
        dtpTu.Enabled = dtpDen.Enabled = chkLocNgay.Checked;
    }
}