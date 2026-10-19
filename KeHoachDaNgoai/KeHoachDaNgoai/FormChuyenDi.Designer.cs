#nullable disable
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KeHoachDaNgoai;

partial class FormChuyenDi
{
    private IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        // ----- Tiêu đề + ngôn ngữ -----
        lblTieuDe = new Label { AutoSize = true, Font = new Font("Segoe UI", 14F, FontStyle.Bold), Location = new Point(12, 8) };
        lblNgonNgu = new Label { AutoSize = true, Location = new Point(770, 14), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        cboNgonNgu = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(850, 10),
            Size = new Size(98, 23),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        cboNgonNgu.Items.AddRange(new object[] { "Tiếng Việt", "English" });
        cboNgonNgu.SelectedIndexChanged += cboNgonNgu_SelectedIndexChanged;

        // ----- Nhóm thông tin chuyến đi -----
        grpInfo = new GroupBox { Location = new Point(12, 44), Size = new Size(480, 290) };

        lblTen = new Label { AutoSize = true, Location = new Point(15, 30) };
        txtTen = new TextBox { MaxLength = 100, Location = new Point(120, 26), Size = new Size(340, 23) };

        lblDiaDiem = new Label { AutoSize = true, Location = new Point(15, 62) };
        txtDiaDiem = new TextBox { MaxLength = 100, Location = new Point(120, 58), Size = new Size(340, 23) };

        lblNgayDi = new Label { AutoSize = true, Location = new Point(15, 94) };
        dtpNgayDi = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Location = new Point(120, 90), Size = new Size(150, 23) };

        lblSoNguoi = new Label { AutoSize = true, Location = new Point(15, 126) };
        nudSoNguoi = new NumericUpDown { Minimum = 1, Maximum = 1000, Value = 1, Location = new Point(120, 122), Size = new Size(150, 23) };

        lblNganSach = new Label { AutoSize = true, Location = new Point(15, 158) };
        nudNganSach = new NumericUpDown
        {
            Minimum = 0, Maximum = 1000000000000M, Increment = 100000, ThousandsSeparator = true,
            Location = new Point(120, 154), Size = new Size(150, 23)
        };

        lblPhuongTien = new Label { AutoSize = true, Location = new Point(15, 190) };
        cboPhuongTien = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(120, 186), Size = new Size(150, 23) };

        chkHoanThanh = new CheckBox { AutoSize = true, Location = new Point(120, 220) };

        btnThem = new Button { Location = new Point(15, 248), Size = new Size(100, 30) };
        btnSua = new Button { Location = new Point(125, 248), Size = new Size(100, 30) };
        btnXoa = new Button { Location = new Point(235, 248), Size = new Size(100, 30) };
        btnLamMoi = new Button { Location = new Point(345, 248), Size = new Size(100, 30) };
        btnThem.Click += btnThem_Click;
        btnSua.Click += btnSua_Click;
        btnXoa.Click += btnXoa_Click;
        btnLamMoi.Click += btnLamMoi_Click;

        grpInfo.Controls.AddRange(new Control[]
        {
            lblTen, txtTen, lblDiaDiem, txtDiaDiem, lblNgayDi, dtpNgayDi, lblSoNguoi, nudSoNguoi,
            lblNganSach, nudNganSach, lblPhuongTien, cboPhuongTien, chkHoanThanh,
            btnThem, btnSua, btnXoa, btnLamMoi
        });

        // ----- Thời tiết -----
        btnThoiTiet = new Button { Location = new Point(510, 44), Size = new Size(170, 30) };
        btnThoiTiet.Click += btnThoiTiet_Click;

        progress = new ProgressBar
        {
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 30,
            Location = new Point(690, 48),
            Size = new Size(258, 22),
            Visible = false
        };

        grpThoiTiet = new GroupBox { Location = new Point(510, 82), Size = new Size(438, 252) };
        lblIcon = new Label
        {
            Font = new Font("Segoe UI Emoji", 40F),
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(15, 40),
            Size = new Size(120, 90)
        };
        lblMoTa = new Label { Font = new Font("Segoe UI", 13F), Location = new Point(150, 40), Size = new Size(275, 55) };
        lblNhietDo = new Label { Location = new Point(150, 98), Size = new Size(275, 22) };
        lblXacSuat = new Label { Location = new Point(150, 124), Size = new Size(275, 22) };

        pnlCanhBao = new Panel { BackColor = Color.FromArgb(255, 224, 178), Location = new Point(15, 170), Size = new Size(408, 50), Visible = false };
        lblCanhBao = new Label { AutoSize = false, ForeColor = Color.FromArgb(150, 70, 0), Location = new Point(10, 14), Size = new Size(390, 24) };
        pnlCanhBao.Controls.Add(lblCanhBao);

        grpThoiTiet.Controls.AddRange(new Control[] { lblIcon, lblMoTa, lblNhietDo, lblXacSuat, pnlCanhBao });

        // ----- Tìm kiếm / lọc -----
        txtTim = new TextBox { Location = new Point(12, 345), Size = new Size(300, 23) };
        chkChuaHT = new CheckBox { AutoSize = true, Location = new Point(325, 347) };

        chkLocNgay = new CheckBox { AutoSize = true, Location = new Point(12, 381) };
        chkLocNgay.CheckedChanged += chkLocNgay_CheckedChanged;
        lblTu = new Label { AutoSize = true, Location = new Point(150, 383) };
        dtpTu = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Enabled = false, Location = new Point(190, 378), Size = new Size(120, 23) };
        lblDen = new Label { AutoSize = true, Location = new Point(325, 383) };
        dtpDen = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Enabled = false, Location = new Point(365, 378), Size = new Size(120, 23) };
        btnTim = new Button { Location = new Point(500, 376), Size = new Size(90, 28) };
        btnTim.Click += btnTim_Click;

        // ----- Lưới -----
        dgv = new DataGridView
        {
            Location = new Point(12, 415),
            Size = new Size(936, 275),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            BackgroundColor = SystemColors.Window
        };
        ((ISupportInitialize)dgv).BeginInit();
        dgv.CellClick += dgv_CellClick;
        dgv.CellFormatting += dgv_CellFormatting;
        ((ISupportInitialize)dgv).EndInit();

        // ----- Form -----
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(960, 700);
        MinimumSize = new Size(976, 739);
        StartPosition = FormStartPosition.CenterScreen;
        Load += FormChuyenDi_Load;
        Controls.AddRange(new Control[]
        {
            lblTieuDe, lblNgonNgu, cboNgonNgu, grpInfo, btnThoiTiet, progress, grpThoiTiet,
            txtTim, chkChuaHT, chkLocNgay, lblTu, dtpTu, lblDen, dtpDen, btnTim, dgv
        });
    }

    private Label lblTieuDe, lblNgonNgu, lblTen, lblDiaDiem, lblNgayDi, lblSoNguoi, lblNganSach, lblPhuongTien;
    private Label lblIcon, lblMoTa, lblNhietDo, lblXacSuat, lblCanhBao, lblTu, lblDen;
    private ComboBox cboNgonNgu, cboPhuongTien;
    private GroupBox grpInfo, grpThoiTiet;
    private TextBox txtTen, txtDiaDiem, txtTim;
    private DateTimePicker dtpNgayDi, dtpTu, dtpDen;
    private NumericUpDown nudSoNguoi, nudNganSach;
    private CheckBox chkHoanThanh, chkChuaHT, chkLocNgay;
    private Button btnThem, btnSua, btnXoa, btnLamMoi, btnThoiTiet, btnTim;
    private ProgressBar progress;
    private Panel pnlCanhBao;
    private DataGridView dgv;
}
