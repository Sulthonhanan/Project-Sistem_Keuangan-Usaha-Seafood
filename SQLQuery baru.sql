CREATE DATABASE Sistem_Keuangan_Usaha_Seafood;
GO


-- Tabel Pemasukan (tanpa kolom kategori)
CREATE TABLE pemasukan (
    id_pemasukan INT IDENTITY(1,1) PRIMARY KEY,   -- Otomatis meningkat
    tanggal DATE NOT NULL,
    deskripsi TEXT,
    jumlah INT NOT NULL,
    harga DECIMAL(15,2) NOT NULL,
    total AS (jumlah * harga) PERSISTED
);

-- Tabel Pengeluaran (tanpa kolom kategori)
CREATE TABLE pengeluaran (
    id_pengeluaran INT IDENTITY(1,1) PRIMARY KEY,
    tanggal DATE NOT NULL,
    deskripsi TEXT,
    jumlah INT NOT NULL,
    harga DECIMAL(15,2) NOT NULL,
    total AS (jumlah * harga) PERSISTED
);

-- Tabel Laporan
CREATE TABLE laporan (
    id_laporan INT IDENTITY(1,1) PRIMARY KEY,
    bulan DATE NOT NULL,
    total_pemasukan DECIMAL(15,2),
    total_pengeluaran DECIMAL(15,2),
    laba_bersih AS (total_pemasukan - total_pengeluaran) PERSISTED
);


-- Insert ke pemasukan
INSERT INTO pemasukan (tanggal, deskripsi, jumlah, harga) 
VALUES 
    ('2025-03-10', 'Penjualan ikan segar', 100, 5000.00),
    ('2025-03-11', 'Pengembalian dana dari supplier', 10, 10000),
    ('2024-02-18', 'Penyuplaian kepiting ke restoran', 30, 10000);

-- Insert ke pengeluaran
INSERT INTO pengeluaran (tanggal, deskripsi, jumlah, harga) 
VALUES 
    ('2025-03-10', 'Pembelian ikan dan udang', 150, 2000.00),
    ('2025-03-11', 'Biaya pengiriman seafood', 1, 50000.00),
    ('2025-01-15', 'Upah untuk karyawan seafood', 4, 50000.00);

-- Insert ke laporan
INSERT INTO laporan (bulan, total_pemasukan, total_pengeluaran)
VALUES
    ('2025-03-01', 600000, 350000),
    ('2025-01-01', 0, 200000),
    ('2024-02-01', 300000, 0);





-------------------------------------------------------------------------------
DROP INDEX idx_pemasukan_tanggal ON pemasukan;
-- Index untuk pencarian berdasarkan tanggal
CREATE NONCLUSTERED INDEX idx_pemasukan_tanggal
ON pemasukan(tanggal);

-- Index untuk pencarian berdasarkan deskripsi
CREATE NONCLUSTERED INDEX idx_pemasukan_deskripsi
ON pemasukan(deskripsi);

-- Index untuk pencarian berdasarkan total (jika digunakan sering)
-- (tidak wajib jika total hanya computed)

-------------------------------------------------------------------------------
DROP INDEX idx_pengeluaran_tanggal ON pengeluaran;
-- Index untuk pencarian berdasarkan tanggal
CREATE NONCLUSTERED INDEX idx_pengeluaran_tanggal
ON pengeluaran(tanggal);

-- Index untuk pencarian berdasarkan deskripsi
CREATE NONCLUSTERED INDEX idx_pengeluaran_deskripsi
ON pengeluaran(deskripsi);


-------------------------------------------------------------------------------
DROP INDEX id_laporan_bulan ON laporan;
	-- Index untuk pencarian berdasarkan bulan
CREATE NONCLUSTERED INDEX id_laporan_bulan
ON laporan(bulan);

-- Index untuk laporan berdasarkan total pemasukan dan pengeluaran
CREATE NONCLUSTERED INDEX idx_laporan_pemasukan_pengeluaran
ON laporan(total_pemasukan, total_pengeluaran);

EXEC sp_helpindex pemasukan;
SELECT 
    OBJECT_NAME(object_id) AS TableName, 
    name AS IndexName 
FROM sys.indexes
WHERE is_primary_key = 0 AND is_unique_constraint = 0
ORDER BY TableName, IndexName;


-------------------------------------------------------------------------------
	
CREATE PROCEDURE sp_HapusLaporanBulanan
    @tahun INT,
    @bulan INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @targetDate DATE = DATEFROMPARTS(@tahun, @bulan, 1);

    IF EXISTS (SELECT 1 FROM laporan WHERE bulan = @targetDate)
    BEGIN
        DELETE FROM laporan WHERE bulan = @targetDate;
        PRINT 'Laporan bulan ' + FORMAT(@targetDate, 'MMMM yyyy') + ' berhasil dihapus.';
    END
    ELSE
    BEGIN
        PRINT 'Laporan bulan ini tidak ditemukan.';
    END
END;
GO

EXEC sp_HapusLaporanBulanan @tahun = 2025, @bulan = 8;

	
-------------------------------------------------------------------------------
CREATE PROCEDURE sp_InsertPemasukan
    @tanggal DATE,
    @deskripsi NVARCHAR(255),
    @jumlah INT,
    @harga DECIMAL(15,2)
AS
BEGIN
    BEGIN TRY
        INSERT INTO pemasukan (tanggal, deskripsi, jumlah, harga)
        VALUES (@tanggal, @deskripsi, @jumlah, @harga);
    END TRY
    BEGIN CATCH
        PRINT 'Terjadi error saat input pemasukan: ' + ERROR_MESSAGE();
    END CATCH
END


-------------------------------------------------------------------------------

CREATE PROCEDURE sp_InsertPengeluaran
    @tanggal DATE,
    @deskripsi NVARCHAR(255),
    @jumlah INT,
    @harga DECIMAL(15,2)
AS
BEGIN
    BEGIN TRY
        INSERT INTO pengeluaran (tanggal, deskripsi, jumlah, harga)
        VALUES (@tanggal, @deskripsi, @jumlah, @harga);
    END TRY
    BEGIN CATCH
        PRINT 'Terjadi error saat input pengeluaran: ' + ERROR_MESSAGE();
    END CATCH
END


-------------------------------------------------------------------------------
CREATE PROCEDURE sp_UbahPemasukan
    @id_pemasukan INT,
    @tanggal DATE,
    @deskripsi NVARCHAR(255),
    @harga DECIMAL(18,2),
    @jumlah INT
    
AS
BEGIN
    UPDATE pemasukan
    SET
        tanggal = @tanggal,
        deskripsi = @deskripsi,
        harga = @harga,
        jumlah = @jumlah
    WHERE id_pemasukan = @id_pemasukan
END


-------------------------------------------------------------------------------
CREATE PROCEDURE sp_UbahPengeluaran
    @id_pengeluaran INT,
    @tanggal DATE,
    @deskripsi NVARCHAR(255),
    @harga DECIMAL(18,2),
    @jumlah INT
AS
BEGIN
    UPDATE pengeluaran
    SET
        tanggal = @tanggal,
        deskripsi = @deskripsi,
        harga = @harga,
        jumlah = @jumlah
    WHERE id_pengeluaran = @id_pengeluaran
END

-------------------------------------------------------------------------------
CREATE PROCEDURE sp_HapusPemasukan
    @id_pemasukan INT
AS
BEGIN
    DELETE FROM pemasukan
    WHERE id_pemasukan = @id_pemasukan
END


-------------------------------------------------------------------------------
CREATE PROCEDURE sp_HapusPengeluaran
    @id_pengeluaran INT
AS
BEGIN
    DELETE FROM pengeluaran
    WHERE id_pengeluaran = @id_pengeluaran
END

-------------------------------------------------------------------------------
Create PROCEDURE sp_UbahLaporan
    @id_laporan INT,
    @bulan NVARCHAR(50),
    @total_pemasukan DECIMAL(18, 2),
    @total_pengeluaran DECIMAL(18, 2)
AS
BEGIN
    UPDATE laporan
    SET
        bulan = @bulan,
        total_pemasukan = @total_pemasukan,
        total_pengeluaran = @total_pengeluaran
    WHERE id_laporan = @id_laporan
END
-------------------------------------------------------------------------------

CREATE PROCEDURE sp_HapusLaporanById
    @id_laporan INT
AS
BEGIN
    DELETE FROM laporan WHERE id_laporan = @id_laporan
END
-------------------------------------------------------------------------------

CREATE VIEW vw_TransaksiGabungan AS
SELECT 
    id_pemasukan AS id,
    tanggal,
    deskripsi,
    harga,
    jumlah,
    total,
    'Pemasukan' AS jenis
FROM pemasukan

UNION ALL

SELECT 
    id_pengeluaran AS id,
    tanggal,
    deskripsi,
    harga,
    jumlah,
    total,
    'Pengeluaran' AS jenis
FROM pengeluaran;


DROP PROCEDURE IF EXISTS sp_InsertLaporanBulanan;
DROP PROCEDURE IF EXISTS sp_HapusLaporanBulanan;
DROP PROCEDURE IF EXISTS sp_InsertPemasukan;
DROP PROCEDURE IF EXISTS sp_InsertPengeluaran;
DROP PROCEDURE IF EXISTS sp_UpdatePemasukan;
DROP PROCEDURE IF EXISTS sp_UpdatePengeluaran;
DROP PROCEDURE IF EXISTS sp_DeletePemasukan;
DROP PROCEDURE IF EXISTS sp_DeletePengeluaran;



SELECT * FROM vw_TransaksiGabungan;

SELECT TOP 1 * FROM vw_TransaksiGabungan
