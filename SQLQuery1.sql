CREATE DATABASE LaporanKeuanganSeafood
GO;

CREATE TABLE pemasukan (
    id_pemasukan INT PRIMARY KEY,
    tanggal DATE NOT NULL,
    kategori VARCHAR(50),
    deskripsi TEXT,
    jumlah DECIMAL(15,2) NOT NULL
);

CREATE TABLE pengeluaran (
    id_pengeluaran INT PRIMARY KEY,
    tanggal DATE NOT NULL,
    kategori VARCHAR(50),
    deskripsi TEXT,
    jumlah DECIMAL(15,2) NOT NULL
);

CREATE TABLE laporan_keuangan (
    id_transaksi INT PRIMARY KEY,
    tanggal DATE NOT NULL,
    kategori VARCHAR(50),
    deskripsi TEXT,
    jumlah DECIMAL(15,2) NOT NULL,
    tipe VARCHAR(20) CHECK (tipe IN ('Pemasukan', 'Pengeluaran'))
);


INSERT INTO pemasukan (id_pemasukan, tanggal, kategori, deskripsi, jumlah) 
VALUES 

('14', '2025-03-10', 'Penjualan', 'Penjualan ikan segar', 500000),
('15', '2025-03-11', 'Refund', 'Pengembalian dana dari supplier', 100000),
('16', '2024-02-18', 'Suplai Barang', 'Penyuplaian kepiting ke restoran', 300000);

INSERT INTO pengeluaran (id_pengeluaran, tanggal, kategori, deskripsi, jumlah) 
VALUES 
('17', '2025-03-10', 'Beli bahan baku', 'Pembelian ikan dan udang', 300000),
('18', '2025-03-11', 'Transportasi', 'Biaya pengiriman seafood', 50000),	
('19', '2025-01-15', 'Gaji Karywan', 'Upah untuk karyawan seafood', 200000);

select * from pemasukan

SELECT * FROM laporan_keuangan ORDER BY tanggal;

-- Data Dummy Pemasukan
INSERT INTO laporan_keuangan (id_transaksi, tanggal, kategori, deskripsi, jumlah, tipe) VALUES
(21, '2025-03-12', 'Penjualan', 'Penjualan ikan kakap merah', 600000, 'Pemasukan'),
(22, '2025-03-13', 'Penjualan', 'Penjualan udang segar', 450000, 'Pemasukan'),
(23, '2025-03-14', 'Suplai Barang', 'Penyediaan lobster ke hotel', 800000, 'Pemasukan'),
(24, '2025-03-15', 'Refund', 'Pengembalian dana dari pelanggan', 120000, 'Pemasukan'),
(25, '2025-03-16', 'Lain-lain', 'Pendapatan dari kerjasama event kuliner', 300000, 'Pemasukan');

-- Data Dummy Pengeluaran
INSERT INTO laporan_keuangan (id_transaksi, tanggal, kategori, deskripsi, jumlah, tipe) VALUES
(26, '2025-03-12', 'Beli bahan baku', 'Pembelian kerang dan cumi', 350000, 'Pengeluaran'),
(27, '2025-03-13', 'Transportasi', 'Sewa mobil box pendingin', 80000, 'Pengeluaran'),
(28, '2025-03-14', 'Gaji Karyawan', 'Pembayaran gaji harian', 250000, 'Pengeluaran'),
(29, '2025-03-15', 'Perawatan', 'Perbaikan freezer penyimpanan', 150000, 'Pengeluaran'),
(30, '2025-03-16', 'Listrik & Air', 'Pembayaran tagihan bulan Maret', 100000, 'Pengeluaran');

CREATE PROCEDURE sp_TampilkanLaporanKeuangan
AS
BEGIN
    SELECT 
        id_transaksi,
        tanggal,
        kategori,
        deskripsi,
        jumlah,
        tipe
    FROM laporan_keuangan
    ORDER BY tanggal;
END;
GO
