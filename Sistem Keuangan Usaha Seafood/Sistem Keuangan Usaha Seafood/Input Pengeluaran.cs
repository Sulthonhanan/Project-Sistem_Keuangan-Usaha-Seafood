using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistem_Keuangan_Usaha_Seafood
{


   

    public partial class Input_Pengeluaran : Form
    {

        private void HitungTotal()
        {
            if (decimal.TryParse(textBoxHarga.Text, out decimal harga) &&
                int.TryParse(textBoxJumlah.Text, out int jumlah))
            {
                decimal total = harga * jumlah;
                txtTotal.Text = total.ToString("N2"); // Format dua desimal
            }
            else
            {
                txtTotal.Text = "0.00"; // Reset jika input tidak valid
            }
        }

        private void UpdateLaporanPengeluaran(DateTime tanggal, decimal totalPengeluaran)
        {
            string connStr = "Server=MSI\\MSSQLSERVER01;Database=Sistem_Keuangan_Usaha_Seafood;Trusted_Connection=True;";
            DateTime bulan = new DateTime(tanggal.Year, tanggal.Month, 1);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string checkQuery = "SELECT COUNT(*) FROM laporan WHERE bulan = @bulan";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@bulan", bulan);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        string updateQuery = "UPDATE laporan SET total_pengeluaran = ISNULL(total_pengeluaran,0) + @pengeluaran WHERE bulan = @bulan";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@bulan", bulan);
                            updateCmd.Parameters.AddWithValue("@pengeluaran", totalPengeluaran);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO laporan (bulan, total_pemasukan, total_pengeluaran) VALUES (@bulan, 0, @pengeluaran)";
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@bulan", bulan);
                            insertCmd.Parameters.AddWithValue("@pengeluaran", totalPengeluaran);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }


        private void LoadDataPengeluaran()
        {
            string connStr = "Server=MSI\\MSSQLSERVER01;Database=Sistem_Keuangan_Usaha_Seafood;Trusted_Connection=True;";
            string query = "SELECT id_pengeluaran, tanggal, deskripsi, jumlah, harga FROM pengeluaran ORDER BY tanggal DESC";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }
        private void ClearForm()
        {
            dateTimePickerTanggal.Value = DateTime.Today;
            textBoxHarga.Clear();
            textBoxDeskripsi.Clear();
            textBoxJumlah.Clear();
            txtTotal.Clear();
        }

        public Input_Pengeluaran()
        {
            InitializeComponent();
        }

        private void Input_Pengeluaran_Load(object sender, EventArgs e)
        {
            LoadDataPengeluaran();
            ClearForm();

        }

        private void textBoxHarga_TextChanged(object sender, EventArgs e)
        {
            HitungTotal();
        }

        private void dateTimePickerTanggal_ValueChanged(object sender, EventArgs e)
        {

        }

        private void textBoxDeskripsi_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxJumlah_TextChanged(object sender, EventArgs e)
        {
            HitungTotal();
        }

        private void txtTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void Simpan_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBoxHarga.Text, out decimal harga) || !int.TryParse(textBoxJumlah.Text, out int jumlah))
            {
                MessageBox.Show("Masukkan jumlah dan harga yang valid.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string deskripsi = textBoxDeskripsi.Text.Trim();
            DateTime tanggal = dateTimePickerTanggal.Value;

            string connStr = "Server=MSI\\MSSQLSERVER01;Database=Sistem_Keuangan_Usaha_Seafood;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("sp_InsertPengeluaran", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tanggal", tanggal);
                cmd.Parameters.AddWithValue("@deskripsi", deskripsi);
                cmd.Parameters.AddWithValue("@jumlah", jumlah);
                cmd.Parameters.AddWithValue("@harga", harga);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    // Hitung total pengeluaran untuk update laporan
                    decimal total = harga * jumlah;
                    UpdateLaporanPengeluaran(tanggal, total);

                    MessageBox.Show("Data pengeluaran berhasil disimpan.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadDataPengeluaran();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Batal_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            LoadDataPengeluaran();
        }
    }
}
