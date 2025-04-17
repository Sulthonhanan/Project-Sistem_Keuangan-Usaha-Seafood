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

INSERT INTO pemasukan (id_pemasukan, tanggal, kategori, deskripsi, jumlah) 
VALUES 
('1', '2025-03-10', 'Penjualan', 'Penjualan ikan segar', 500000),
('2', '2025-03-11', 'Refund', 'Pengembalian dana dari supplier', 100000),
('3', '2024-02-18', 'Suplai Barang', 'Penyuplaian kepiting ke restoran', 300000);

INSERT INTO pengeluaran (tanggal, kategori, deskripsi, jumlah) 
VALUES 
('4', '2025-03-10', 'Beli bahan baku', 'Pembelian ikan dan udang', 300000),
('5', '2025-03-11', 'Transportasi', 'Biaya pengiriman seafood', 50000),
('6', '2025-01-15', 'Gaji Karywan', 'Upah untuk karyawan seafood', 200000);