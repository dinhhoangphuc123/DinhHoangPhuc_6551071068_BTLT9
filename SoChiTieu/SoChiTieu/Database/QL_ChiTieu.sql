
    CREATE DATABASE QL_ChiTieu;
GO

USE QL_ChiTieu;
GO

IF OBJECT_ID(N'dbo.ChiTieu', N'U') IS NULL
CREATE TABLE dbo.ChiTieu
(
    MaCT    int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChiTieu PRIMARY KEY,
    NgayChi date          NOT NULL,
    NoiDung nvarchar(100) NOT NULL,
    LoaiChi nvarchar(30)  NOT NULL,
    SoTien  decimal(18,0) NOT NULL,
    GhiChu  nvarchar(200) NULL
);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ChiTieu)
INSERT INTO dbo.ChiTieu (NgayChi, NoiDung, LoaiChi, SoTien, GhiChu) VALUES
(DATEADD(DAY, -11, CAST(GETDATE() AS date)), N'Cơm trưa',           N'Ăn uống', 35000,  NULL),
(DATEADD(DAY, -10, CAST(GETDATE() AS date)), N'Xăng xe',            N'Đi lại',  60000,  NULL),
(DATEADD(DAY,  -9, CAST(GETDATE() AS date)), N'Photo giáo trình',   N'Học tập', 25000,  NULL),
(DATEADD(DAY,  -8, CAST(GETDATE() AS date)), N'Xem phim',           N'Giải trí',90000,  N'Đi cùng bạn'),
(DATEADD(DAY,  -7, CAST(GETDATE() AS date)), N'Trà sữa',            N'Ăn uống', 40000,  NULL),
(DATEADD(DAY,  -6, CAST(GETDATE() AS date)), N'Mua sách lập trình', N'Học tập', 150000, NULL),
(DATEADD(DAY,  -5, CAST(GETDATE() AS date)), N'Vé xe buýt',         N'Đi lại',  12000,  NULL),
(DATEADD(DAY,  -4, CAST(GETDATE() AS date)), N'Ăn sáng',            N'Ăn uống', 25000,  NULL),
(DATEADD(DAY,  -3, CAST(GETDATE() AS date)), N'Cà phê với bạn',     N'Ăn uống', 45000,  NULL),
(DATEADD(DAY,  -2, CAST(GETDATE() AS date)), N'Chơi game',          N'Giải trí',50000,  NULL),
(DATEADD(DAY,  -1, CAST(GETDATE() AS date)), N'Tiền điện',          N'Khác',    500000, N'Tiền điện tháng này'),
(CAST(GETDATE() AS date),                    N'Cơm tối',            N'Ăn uống', 40000,  NULL);
GO
