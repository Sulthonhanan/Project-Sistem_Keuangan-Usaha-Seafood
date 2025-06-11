    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    namespace Sistem_Keuangan_Usaha_Seafood
    {
        public partial class Input_Pemasukan : Form
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



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void LoadData()
            {
                string connectionString = "Server=MSI\\MSSQLSERVER01;Database=Sistem_Keuangan_Usaha_Seafood;Trusted_Connection=True;";
                string query = "SELECT id_pemasukan, tanggal, deskripsi, jumlah, harga FROM pemasukan ORDER BY tanggal DESC";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
            }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ClearForm()
            {
                dateTimePickerTanggal.Value = DateTime.Today;
                textBoxHarga.Clear();
                textBoxDeskripsi.Clear();
                textBoxJumlah.Clear();
                txtTotal.Clear();
            }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void LoadDataPemasukan()
            {
                string connStr = "Server=MSI\\MSSQLSERVER01;Database=Sistem_Keuangan_Usaha_Seafood;Trusted_Connection=True;";
                string query = "SELECT id_pemasukan, tanggal, deskripsi, jumlah, harga FROM pemasukan ORDER BY tanggal DESC";

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateLaporanPemasukan(DateTime tanggal, decimal totalPemasukan)
            {
                string connStr = "Server=MSI\\MSSQLSERVER01;Database=Sistem_Keuangan_Usaha_Seafood;Trusted_Connection=True;";
                DateTime bulan = new DateTime(tanggal.Year, tanggal.Month, 1);

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Cek apakah bulan sudah ada
                    string checkQuery = "SELECT COUNT(*) FROM laporan WHERE bulan = @bulan";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@bulan", bulan);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // Update total_pemasukan
                            string updateQuery = "UPDATE laporan SET total_pemasukan = ISNULL(total_pemasukan,0) + @pemasukan WHERE bulan = @bulan";
                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@bulan", bulan);
                                updateCmd.Parameters.AddWithValue("@pemasukan", totalPemasukan);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Insert baru
                            string insertQuery = "INSERT INTO laporan (bulan, total_pemasukan, total_pengeluaran) VALUES (@bulan, @pemasukan, 0)";
                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@bulan", bulan);
                                insertCmd.Parameters.AddWithValue("@pemasukan", totalPemasukan);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
    

                /// <summary>
                /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// </summary>



        public Input_Pemasukan()
            {
                InitializeComponent();
            }

            private void Input_Pemasukan_Load(object sender, EventArgs e)
            {
                LoadDataPemasukan();
            }

            private void Simpan_Click(object sender, EventArgs e)
            {
           
            }

            private void textBoxHarga_TextChanged(object sender, EventArgs e)
            {
                HitungTotal();
            }

            private void textBoxJumlah_TextChanged(object sender, EventArgs e)
            {
                HitungTotal();
            }

            private void textBoxDeskripsi_TextChanged(object sender, EventArgs e)
            {

            }

            private void Simpan_Click_1(object sender, EventArgs e)
            {
            // Validasi input
            if (!decimal.TryParse(textBoxHarga.Text, out decimal harga) || !int.TryParse(textBoxJumlah.Text, out int jumlah))
            {
                MessageBox.Show("Masukkan jumlah dan harga yang valid.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string deskripsi = textBoxDeskripsi.Text.Trim();
            DateTime tanggal = dateTimePickerTanggal.Value;

            string connStr = "Server=MSI\\MSSQLSERVER01;Database=Sistem_Keuangan_Usaha_Seafood;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("sp_InsertPemasukan", conn))
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

                    // Hitung total untuk update laporan
                    decimal total = harga * jumlah;
                    UpdateLaporanPemasukan(tanggal, total);

                    MessageBox.Show("Data pemasukan berhasil disimpan.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadDataPemasukan();
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

            private void txtTotal_TextChanged(object sender, EventArgs e)
            {

            }

            private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
            {
                LoadData();
            }
        }
    }
