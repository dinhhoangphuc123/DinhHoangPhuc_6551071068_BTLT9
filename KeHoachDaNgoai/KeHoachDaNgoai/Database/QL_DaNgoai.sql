
    CREATE DATABASE QL_DaNgoai;
GO

USE QL_DaNgoai;
GO

IF OBJECT_ID(N'dbo.ChuyenDi', N'U') IS NULL
CREATE TABLE dbo.ChuyenDi
(
    MaChuyen     int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChuyenDi PRIMARY KEY,
    TenChuyen    nvarchar(100) NOT NULL,
    DiaDiem      nvarchar(100) NOT NULL,
    NgayDi       date          NOT NULL,
    SoNguoi      int           NOT NULL,
    NganSach     decimal(18,0) NOT NULL,
    PhuongTien   nvarchar(30)  NOT NULL,
    DaHoanThanh  bit           NOT NULL CONSTRAINT DF_ChuyenDi_DaHoanThanh DEFAULT 0,
    GhiChu       nvarchar(200) NULL
);
GO

-- Ngày đi tính theo ngày hiện tại: từ vài ngày đến vài tuần tới (có chuyến cách hơn 16 ngày để thử "Chưa có dữ liệu dự báo")
IF NOT EXISTS (SELECT 1 FROM dbo.ChuyenDi)
INSERT INTO dbo.ChuyenDi (TenChuyen, DiaDiem, NgayDi, SoNguoi, NganSach, PhuongTien, DaHoanThanh, GhiChu) VALUES
(N'Cắm trại đồi chè',      N'Đà Lạt',     DATEADD(DAY,  3, CAST(GETDATE() AS date)), 6,  3500000, N'Ô tô',     0, NULL),
(N'Chinh phục Fansipan',   N'Sa Pa',      DATEADD(DAY,  5, CAST(GETDATE() AS date)), 4,  6000000, N'Tàu hỏa',  0, N'Mang áo ấm'),
(N'Tắm biển cuối tuần',    N'Vũng Tàu',   DATEADD(DAY,  7, CAST(GETDATE() AS date)), 8,  2500000, N'Xe khách', 1, NULL),
(N'Phố cổ về đêm',         N'Hội An',     DATEADD(DAY, 10, CAST(GETDATE() AS date)), 5,  4500000, N'Ô tô',     0, NULL),
(N'Lặn ngắm san hô',       N'Phú Quốc',   DATEADD(DAY, 13, CAST(GETDATE() AS date)), 3,  9000000, N'Ô tô',     0, NULL),
(N'Du thuyền vịnh',        N'Nha Trang',  DATEADD(DAY, 16, CAST(GETDATE() AS date)), 6,  5500000, N'Xe khách', 0, NULL),
(N'Ẩm thực cố đô',         N'Huế',        DATEADD(DAY, 21, CAST(GETDATE() AS date)), 4,  3000000, N'Tàu hỏa',  0, NULL),
(N'Cung đường Hà Giang',   N'Hà Giang',   DATEADD(DAY, 28, CAST(GETDATE() AS date)), 5,  7000000, N'Xe máy',   0, N'Đi theo nhóm');
GO
