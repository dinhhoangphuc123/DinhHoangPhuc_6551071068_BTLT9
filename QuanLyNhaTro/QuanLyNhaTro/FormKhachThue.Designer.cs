#nullable disable
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyNhaTro;

partial class FormKhachThue
{
    private IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        // ----- Nhóm thông tin khách thuê -----
        grpInfo = new GroupBox { Text = "Thông tin khách thuê", Location = new Point(12, 12), Size = new Size(380, 430) };

        lblHoTen = new Label { Text = "Họ tên", AutoSize = true, Location = new Point(15, 38) };
        txtHoTen = new TextBox { MaxLength = 100, Location = new Point(120, 34), Size = new Size(245, 23) };

        lblGioiTinh = new Label { Text = "Giới tính", AutoSize = true, Location = new Point(15, 77) };
        grpGioiTinh = new GroupBox { Location = new Point(120, 60), Size = new Size(245, 42) };
        rdoNam = new RadioButton { Text = "Nam", Checked = true, AutoSize = true, Location = new Point(15, 15) };
        rdoNu = new RadioButton { Text = "Nữ", AutoSize = true, Location = new Point(100, 15) };
        grpGioiTinh.Controls.AddRange(new Control[] { rdoNam, rdoNu });

        lblNgaySinh = new Label { Text = "Ngày sinh", AutoSize = true, Location = new Point(15, 120) };
        dtpNgaySinh = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Location = new Point(120, 116), Size = new Size(245, 23) };

        lblSDT = new Label { Text = "Số điện thoại", AutoSize = true, Location = new Point(15, 155) };
        txtSDT = new TextBox { MaxLength = 15, Location = new Point(120, 151), Size = new Size(245, 23) };

        lblPhong = new Label { Text = "Phòng", AutoSize = true, Location = new Point(15, 190) };
        cboPhong = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(120, 186), Size = new Size(245, 23) };

        lblTinh = new Label { Text = "Tỉnh/Thành", AutoSize = true, Location = new Point(15, 225) };
        cboTinh = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(120, 221), Size = new Size(245, 23) };
        cboTinh.SelectionChangeCommitted += cboTinh_SelectionChangeCommitted;

        lblPhuongXa = new Label { Text = "Phường/Xã", AutoSize = true, Location = new Point(15, 260) };
        cboPhuongXa = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(120, 256), Size = new Size(245, 23) };

        chkTamTru = new CheckBox { Text = "Đã đăng ký tạm trú", AutoSize = true, Location = new Point(120, 292) };

        btnThem = new Button { Text = "Thêm", Location = new Point(15, 337), Size = new Size(80, 30) };
        btnSua = new Button { Text = "Sửa", Location = new Point(105, 337), Size = new Size(80, 30) };
        btnXoa = new Button { Text = "Xóa", Location = new Point(195, 337), Size = new Size(80, 30) };
        btnLamMoi = new Button { Text = "Làm mới", Location = new Point(285, 337), Size = new Size(80, 30) };
        btnThem.Click += btnThem_Click;
        btnSua.Click += btnSua_Click;
        btnXoa.Click += btnXoa_Click;
        btnLamMoi.Click += btnLamMoi_Click;

        // Nút ẩn: bấm Ctrl+Shift+E để hiện / ẩn
        btnGayLoi = new Button { Text = "Gây lỗi thử", Location = new Point(15, 387), Size = new Size(200, 28), Visible = false };
        btnGayLoi.Click += btnGayLoi_Click;

        grpInfo.Controls.AddRange(new Control[]
        {
            lblHoTen, txtHoTen, lblGioiTinh, grpGioiTinh, lblNgaySinh, dtpNgaySinh,
            lblSDT, txtSDT, lblPhong, cboPhong, lblTinh, cboTinh, lblPhuongXa, cboPhuongXa,
            chkTamTru, btnThem, btnSua, btnXoa, btnLamMoi, btnGayLoi
        });

        // ----- Khu tìm kiếm -----
        txtTim = new TextBox { PlaceholderText = "Tìm theo họ tên hoặc SĐT", Location = new Point(410, 14), Size = new Size(578, 23), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        txtTim.KeyDown += txtTim_KeyDown;

        lblLocPhong = new Label { Text = "Phòng", AutoSize = true, Location = new Point(410, 50) };
        cboLocPhong = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(460, 46), Size = new Size(110, 23) };
        cboLocPhong.SelectionChangeCommitted += cboLoc_SelectionChangeCommitted;

        lblLocTinh = new Label { Text = "Tỉnh", AutoSize = true, Location = new Point(585, 50) };
        cboLocTinh = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(625, 46), Size = new Size(180, 23) };
        cboLocTinh.SelectionChangeCommitted += cboLoc_SelectionChangeCommitted;

        btnTim = new Button { Text = "Tìm", Location = new Point(815, 44), Size = new Size(80, 28) };
        btnBoLoc = new Button { Text = "Bỏ lọc", Location = new Point(903, 44), Size = new Size(85, 28) };
        btnTim.Click += btnTim_Click;
        btnBoLoc.Click += btnBoLoc_Click;

        // ----- Lưới -----
        dgv = new DataGridView
        {
            Location = new Point(410, 82),
            Size = new Size(578, 500),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            BackgroundColor = SystemColors.Window
        };
        ((ISupportInitialize)dgv).BeginInit();
        dgv.CellClick += dgv_CellClick;
        ((ISupportInitialize)dgv).EndInit();

        // ----- Thanh trạng thái -----
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel { Text = "Sẵn sàng" };
        statusStrip.Items.Add(lblStatus);

        // ----- Form -----
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 620);
        MinimumSize = new Size(1016, 659);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Quản lý nhà trọ - Khách thuê";
        KeyPreview = true;
        KeyDown += FormKhachThue_KeyDown;
        Load += FormKhachThue_Load;
        Controls.AddRange(new Control[]
        {
            grpInfo, txtTim, lblLocPhong, cboLocPhong, lblLocTinh, cboLocTinh, btnTim, btnBoLoc, dgv, statusStrip
        });
    }

    private GroupBox grpInfo, grpGioiTinh;
    private Label lblHoTen, lblGioiTinh, lblNgaySinh, lblSDT, lblPhong, lblTinh, lblPhuongXa, lblLocPhong, lblLocTinh;
    private TextBox txtHoTen, txtSDT, txtTim;
    private RadioButton rdoNam, rdoNu;
    private DateTimePicker dtpNgaySinh;
    private ComboBox cboPhong, cboTinh, cboPhuongXa, cboLocPhong, cboLocTinh;
    private CheckBox chkTamTru;
    private Button btnThem, btnSua, btnXoa, btnLamMoi, btnGayLoi, btnTim, btnBoLoc;
    private DataGridView dgv;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
}
