    CREATE DATABASE QL_NhaTro;
GO

USE QL_NhaTro;
GO

IF OBJECT_ID(N'dbo.PhongTro', N'U') IS NULL
CREATE TABLE dbo.PhongTro
(
    MaPhong   int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PhongTro PRIMARY KEY,
    TenPhong  nvarchar(20)  NOT NULL,
    GiaThue   decimal(18,0) NOT NULL,
    TinhTrang nvarchar(20)  NOT NULL CONSTRAINT DF_PhongTro_TinhTrang DEFAULT N'Trống'
);
GO

IF OBJECT_ID(N'dbo.KhachThue', N'U') IS NULL
CREATE TABLE dbo.KhachThue
(
    MaKhach         int IDENTITY(1,1) NOT NULL CONSTRAINT PK_KhachThue PRIMARY KEY,
    HoTen           nvarchar(100) NOT NULL,
    GioiTinh        nvarchar(5)   NOT NULL,
    NgaySinh        date          NOT NULL,
    SoDienThoai     varchar(15)   NOT NULL,
    MaTinh          int           NULL,
    TenTinh         nvarchar(100) NULL,
    MaPhuongXa      int           NULL,
    TenPhuongXa     nvarchar(100) NULL,
    DaDangKyTamTru  bit           NOT NULL CONSTRAINT DF_KhachThue_TamTru DEFAULT 0,
    MaPhong         int           NOT NULL CONSTRAINT FK_KhachThue_PhongTro REFERENCES dbo.PhongTro(MaPhong)
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PhongTro)
INSERT INTO dbo.PhongTro (TenPhong, GiaThue, TinhTrang) VALUES
(N'P01', 1500000, N'Đang thuê'),
(N'P02', 1600000, N'Đang thuê'),
(N'P03', 1800000, N'Đang thuê'),
(N'P04', 2000000, N'Đang thuê'),
(N'P05', 1700000, N'Trống'),
(N'P06', 1900000, N'Trống');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.KhachThue)
INSERT INTO dbo.KhachThue (HoTen, GioiTinh, NgaySinh, SoDienThoai, TenTinh, TenPhuongXa, DaDangKyTamTru, MaPhong) VALUES
(N'Nguyễn Văn An',   N'Nam', '2004-05-12', '0912345678', N'Thành phố Hà Nội', N'Phường Ba Đình',  1, (SELECT MaPhong FROM dbo.PhongTro WHERE TenPhong = N'P03')),
(N'Trần Thị Bình',   N'Nữ',  '2003-08-20', '0876543210', N'Tỉnh Ninh Bình',   N'Phường Nam Định', 0, (SELECT MaPhong FROM dbo.PhongTro WHERE TenPhong = N'P01')),
(N'Lê Văn Cường',    N'Nam', '2002-11-03', '0901234567', N'Tỉnh Thanh Hóa',   N'Phường Hạc Thành',0, (SELECT MaPhong FROM dbo.PhongTro WHERE TenPhong = N'P02')),
(N'Phạm Minh Đức',   N'Nam', '2001-02-27', '0987654321', N'Tỉnh Thanh Hóa',   N'Phường Hàm Rồng', 1, (SELECT MaPhong FROM dbo.PhongTro WHERE TenPhong = N'P04')),
(N'Hoàng Thị Dung',  N'Nữ',  '2004-09-15', '0765432109', N'Tỉnh Bắc Ninh',    N'Phường Bắc Giang',0, (SELECT MaPhong FROM dbo.PhongTro WHERE TenPhong = N'P03'));
GO
