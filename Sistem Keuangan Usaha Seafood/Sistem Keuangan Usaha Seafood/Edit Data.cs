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

    public partial class Edit_Data : Form
    {
        SqlConnection conn = new SqlConnection("Server=MSI\\MSSQLSERVER01;Database=Sistem_Keuangan_Usaha_Seafood;Trusted_Connection=True;");

        int selectedId = -1;
        string selectedJenis = "";

        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void LoadGabunganData()
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM vw_TransaksiGabungan ORDER BY tanggal DESC", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridViewGabungan.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data gabungan: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void HitungLaba()
        {
            if (decimal.TryParse(textBoxHarga.Text, out decimal harga) &&
                 int.TryParse(textBoxJumlah.Text, out int jumlah))
            {
                decimal total = harga * jumlah;
                txtTotal.Text = total.ToString("N0");
            }
            else
            {
                txtTotal.Text = "";
            }
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Edit_Data()
        {
            InitializeComponent();
        }

        private void dateTimePickerTanggal_ValueChanged(object sender, EventArgs e)
        {

        }

        private void textBoxHarga_TextChanged(object sender, EventArgs e)
        {
            HitungLaba();
        }

        private void textBoxDeskripsi_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxJumlah_TextChanged(object sender, EventArgs e)
        {
            HitungLaba();
        }

        private void txtTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void EditData_Click(object sender, EventArgs e)
        {
            if (selectedId == -1)
            {
                MessageBox.Show("Pilih data yang ingin diedit terlebih dahulu.");
                return;
            }

            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }



                // Hitung total
                decimal harga = decimal.Parse(textBoxHarga.Text);
                int jumlah = int.Parse(textBoxJumlah.Text);
                decimal total = harga * jumlah;

                // Deteksi jenis dari DataGridView langsung
                string jenis = dataGridViewGabungan.CurrentRow.Cells[6].Value.ToString(); // indeks kolom ke-6 (dimulai dari 0)


                SqlCommand cmd;

                if (jenis == "Pemasukan")
                {
                    cmd =  new SqlCommand("EXEC sp_UbahPemasukan @id, @tanggal, @deskripsi, @harga, @jumlah", conn);
                }
                else if (jenis == "Pengeluaran")
                {
                    cmd = new SqlCommand("EXEC sp_UbahPengeluaran @id, @tanggal, @deskripsi, @harga, @jumlah", conn);

                }
                else
                {
                    MessageBox.Show("Jenis transaksi tidak valid.");
                    return;
                }

                cmd.Parameters.AddWithValue("@id", selectedId);
                cmd.Parameters.AddWithValue("@tanggal", dateTimePickerTanggal.Value);
                cmd.Parameters.AddWithValue("@deskripsi", textBoxDeskripsi.Text);
                cmd.Parameters.AddWithValue("@harga", harga);
                cmd.Parameters.AddWithValue("@jumlah", jumlah);
                

                cmd.ExecuteNonQuery();

                MessageBox.Show("Data berhasil diperbarui.");
                LoadGabunganData();
                selectedId = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengedit data: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void Hapus_Click(object sender, EventArgs e)
        {
            if (selectedId == -1 || string.IsNullOrEmpty(selectedJenis))
            {
                MessageBox.Show("Pilih data yang ingin dihapus terlebih dahulu.");
                return;
            }

            // Konfirmasi dari user
            DialogResult confirm = MessageBox.Show("Yakin ingin menghapus data ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Tentukan stored procedure sesuai jenis
                    if (selectedJenis == "Pemasukan")
                    {
                        cmd.CommandText = "sp_HapusPemasukan";
                        cmd.Parameters.AddWithValue("@id_pemasukan", selectedId);
                    }
                    else if (selectedJenis == "Pengeluaran")
                    {
                        cmd.CommandText = "sp_HapusPengeluaran";
                        cmd.Parameters.AddWithValue("@id_pengeluaran", selectedId);
                    }
                    else
                    {
                        MessageBox.Show("Jenis transaksi tidak dikenali.");
                        return;
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data berhasil dihapus.");
                }

                // Reset & refresh
                LoadGabunganData();
                selectedId = -1;
                selectedJenis = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menghapus data: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

        }

        private void dataGridViewGabungan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void Edit_Data_Load(object sender, EventArgs e)
        {
            LoadGabunganData();
            dataGridViewGabungan.CellClick += dataGridViewGabungan_CellClick;

        }

        private void dataGridViewGabungan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewGabungan.Rows[e.RowIndex];

                selectedId = Convert.ToInt32(row.Cells[0].Value); // kolom id
                selectedJenis = row.Cells[6].Value.ToString();    // perbaikan: pakai variabel global

                dateTimePickerTanggal.Value = Convert.ToDateTime(row.Cells[1].Value);
                textBoxDeskripsi.Text = row.Cells[2].Value.ToString();
                textBoxHarga.Text = row.Cells[3].Value.ToString();
                textBoxJumlah.Text = row.Cells[4].Value.ToString();
                txtTotal.Text = row.Cells[5].Value.ToString();
            }
        }
    }
}

