#nullable disable
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SoChiTieu;

partial class FormChiTieu
{
    private IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        grpInfo = new GroupBox();
        lblNgay = new Label(); dtpNgay = new DateTimePicker();
        lblLoai = new Label(); cboLoai = new ComboBox();
        lblNoiDung = new Label(); txtNoiDung = new TextBox();
        lblSoTien = new Label(); nudSoTien = new NumericUpDown();
        lblGhiChu = new Label(); txtGhiChu = new TextBox();
        grpSearch = new GroupBox();
        txtTim = new TextBox();
        lblLoc = new Label(); cboLoc = new ComboBox();
        chkLocNgay = new CheckBox();
        lblTu = new Label(); dtpTu = new DateTimePicker();
        lblDen = new Label(); dtpDen = new DateTimePicker();
        btnTim = new Button();
        btnThem = new Button(); btnSua = new Button(); btnXoa = new Button(); btnLamMoi = new Button();
        dgv = new DataGridView();
        lblTong = new Label();

        ((ISupportInitialize)nudSoTien).BeginInit();
        ((ISupportInitialize)dgv).BeginInit();
        grpInfo.SuspendLayout();
        grpSearch.SuspendLayout();
        SuspendLayout();

        // grpInfo
        grpInfo.Text = "Thông tin khoản chi";
        grpInfo.Location = new Point(12, 12);
        grpInfo.Size = new Size(560, 195);
        grpInfo.Controls.AddRange(new Control[] { lblNgay, dtpNgay, lblLoai, cboLoai, lblNoiDung, txtNoiDung, lblSoTien, nudSoTien, lblGhiChu, txtGhiChu });

        lblNgay.AutoSize = true; lblNgay.Text = "Ngày chi"; lblNgay.Location = new Point(15, 32);
        dtpNgay.Format = DateTimePickerFormat.Custom; dtpNgay.CustomFormat = "dd/MM/yyyy";
        dtpNgay.Location = new Point(100, 28); dtpNgay.Size = new Size(150, 23);

        lblLoai.AutoSize = true; lblLoai.Text = "Loại chi"; lblLoai.Location = new Point(280, 32);
        cboLoai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLoai.Location = new Point(350, 28); cboLoai.Size = new Size(190, 23);

        lblNoiDung.AutoSize = true; lblNoiDung.Text = "Nội dung"; lblNoiDung.Location = new Point(15, 67);
        txtNoiDung.MaxLength = 100;
        txtNoiDung.Location = new Point(100, 63); txtNoiDung.Size = new Size(440, 23);

        lblSoTien.AutoSize = true; lblSoTien.Text = "Số tiền"; lblSoTien.Location = new Point(15, 102);
        nudSoTien.Minimum = 0; nudSoTien.Maximum = 1000000000000M; nudSoTien.Increment = 1000;
        nudSoTien.ThousandsSeparator = true;
        nudSoTien.Location = new Point(100, 98); nudSoTien.Size = new Size(150, 23);

        lblGhiChu.AutoSize = true; lblGhiChu.Text = "Ghi chú"; lblGhiChu.Location = new Point(15, 137);
        txtGhiChu.Multiline = true; txtGhiChu.MaxLength = 200; txtGhiChu.ScrollBars = ScrollBars.Vertical;
        txtGhiChu.Location = new Point(100, 133); txtGhiChu.Size = new Size(440, 45);

        // grpSearch
        grpSearch.Text = "Tìm kiếm";
        grpSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        grpSearch.Location = new Point(590, 12);
        grpSearch.Size = new Size(318, 195);
        grpSearch.Controls.AddRange(new Control[] { txtTim, lblLoc, cboLoc, chkLocNgay, lblTu, dtpTu, lblDen, dtpDen, btnTim });

        txtTim.PlaceholderText = "Nhập nội dung...";
        txtTim.Location = new Point(15, 28); txtTim.Size = new Size(285, 23);

        lblLoc.AutoSize = true; lblLoc.Text = "Loại chi"; lblLoc.Location = new Point(15, 67);
        cboLoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLoc.Location = new Point(90, 63); cboLoc.Size = new Size(210, 23);

        chkLocNgay.AutoSize = true; chkLocNgay.Text = "Lọc theo ngày";
        chkLocNgay.Location = new Point(15, 98);
        chkLocNgay.CheckedChanged += chkLocNgay_CheckedChanged;

        lblTu.AutoSize = true; lblTu.Text = "Từ ngày"; lblTu.Location = new Point(15, 130);
        dtpTu.Format = DateTimePickerFormat.Custom; dtpTu.CustomFormat = "dd/MM/yyyy"; dtpTu.Enabled = false;
        dtpTu.Location = new Point(90, 126); dtpTu.Size = new Size(130, 23);

        lblDen.AutoSize = true; lblDen.Text = "Đến ngày"; lblDen.Location = new Point(15, 162);
        dtpDen.Format = DateTimePickerFormat.Custom; dtpDen.CustomFormat = "dd/MM/yyyy"; dtpDen.Enabled = false;
        dtpDen.Location = new Point(90, 158); dtpDen.Size = new Size(130, 23);

        btnTim.Text = "Tìm"; btnTim.Location = new Point(230, 157); btnTim.Size = new Size(70, 26);
        btnTim.Click += btnTim_Click;

        // Nút CRUD
        btnThem.Text = "Thêm"; btnThem.Location = new Point(200, 215); btnThem.Size = new Size(90, 30); btnThem.Click += btnThem_Click;
        btnSua.Text = "Sửa"; btnSua.Location = new Point(300, 215); btnSua.Size = new Size(90, 30); btnSua.Click += btnSua_Click;
        btnXoa.Text = "Xóa"; btnXoa.Location = new Point(400, 215); btnXoa.Size = new Size(90, 30); btnXoa.Click += btnXoa_Click;
        btnLamMoi.Text = "Làm mới"; btnLamMoi.Location = new Point(500, 215); btnLamMoi.Size = new Size(90, 30); btnLamMoi.Click += btnLamMoi_Click;

        // dgv
        dgv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgv.Location = new Point(12, 256);
        dgv.Size = new Size(896, 300);
        dgv.BackgroundColor = SystemColors.Window;
        dgv.CellClick += dgv_CellClick;

        // lblTong
        lblTong.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        lblTong.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblTong.ForeColor = Color.Red;
        lblTong.TextAlign = ContentAlignment.MiddleRight;
        lblTong.Location = new Point(508, 562);
        lblTong.Size = new Size(400, 28);
        lblTong.Text = "Tổng chi: 0 đ";

        // Form
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(920, 600);
        MinimumSize = new Size(936, 639);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Sổ chi tiêu cá nhân";
        Controls.AddRange(new Control[] { grpInfo, grpSearch, btnThem, btnSua, btnXoa, btnLamMoi, dgv, lblTong });
        Load += FormChiTieu_Load;

        ((ISupportInitialize)nudSoTien).EndInit();
        ((ISupportInitialize)dgv).EndInit();
        grpInfo.ResumeLayout(false); grpInfo.PerformLayout();
        grpSearch.ResumeLayout(false); grpSearch.PerformLayout();
        ResumeLayout(false);
    }

    private GroupBox grpInfo, grpSearch;
    private Label lblNgay, lblLoai, lblNoiDung, lblSoTien, lblGhiChu, lblLoc, lblTu, lblDen, lblTong;
    private DateTimePicker dtpNgay, dtpTu, dtpDen;
    private ComboBox cboLoai, cboLoc;
    private TextBox txtNoiDung, txtGhiChu, txtTim;
    private NumericUpDown nudSoTien;
    private CheckBox chkLocNgay;
    private Button btnTim, btnThem, btnSua, btnXoa, btnLamMoi;
    private DataGridView dgv;
}
