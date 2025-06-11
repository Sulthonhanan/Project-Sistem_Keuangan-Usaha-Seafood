using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Sistem_Keuangan_Usaha_Seafood
{
    public partial class Laporan_Keuangan : Form
    {
        public Laporan_Keuangan()
        {
            InitializeComponent();
        }


        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        int selectedId = -1;

        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void LoadLaporan()
        {
            string connStr = "Server=MSI\\MSSQLSERVER01;Database=Sistem_Keuangan_Usaha_Seafood;Trusted_Connection=True;";
            string query = "SELECT id_laporan, bulan, total_pemasukan, total_pengeluaran, laba_bersih FROM laporan ORDER BY bulan DESC";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }


        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void HitungLaba()
        {
            if (decimal.TryParse(textBoxHarga.Text, out decimal pemasukan) &&
                decimal.TryParse(textBoxJumlah.Text, out decimal pengeluaran))
            {
                decimal laba = pemasukan - pengeluaran;
                txtTotal.Text = laba.ToString("N0"); // format ribuan
            }
            else
            {
                txtTotal.Text = "";
            }
        }
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ExportToCSV(DataGridView dgv, string filePath)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                // Header
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    sb.Append(dgv.Columns[i].HeaderText);
                    if (i < dgv.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();

                // Data
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            sb.Append(row.Cells[i].Value?.ToString().Replace(",", " "));
                            if (i < dgv.Columns.Count - 1)
                                sb.Append(",");
                        }
                        sb.AppendLine();
                    }
                }

                File.WriteAllText(filePath, sb.ToString());
                MessageBox.Show("Berhasil diekspor ke CSV.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal ekspor CSV: " + ex.Message);
            }
        }

        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void Laporan_Keuangan_Load(object sender, EventArgs e)
        {
            dataGridView1.CellClick += dataGridView1_CellClick;

            // Atur DateTimePicker agar tampil bulan & tahun
            dateTimePickerTanggal.Format = DateTimePickerFormat.Custom;
            dateTimePickerTanggal.CustomFormat = "MMMM yyyy";
            dateTimePickerTanggal.ShowUpDown = true;

            // Isi textboxDeskripsi dengan nilai awal dari date picker
            textBoxDeskripsi.Text = dateTimePickerTanggal.Value.ToString("MMMM yyyy");

            // Muat data laporan
            LoadLaporan();

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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Simpan_Click(object sender, EventArgs e)
        {

            if (selectedId == -1)
            {
                MessageBox.Show("Pilih data yang ingin diubah.");
                return;
            }

            string connStr = "Server=MSI\\MSSQLSERVER01;Database=Sistem_Keuangan_Usaha_Seafood;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("sp_UbahLaporan", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id_laporan", selectedId);
                cmd.Parameters.AddWithValue("@bulan", textBoxDeskripsi.Text);
                cmd.Parameters.AddWithValue("@total_pemasukan", textBoxHarga.Text);
                cmd.Parameters.AddWithValue("@total_pengeluaran", textBoxJumlah.Text);
                // Tidak perlu param @laba_bersih karena dihitung otomatis

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data berhasil diubah.");
                    LoadLaporan();
                    selectedId = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal mengubah data: " + ex.Message);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Tampilkan_Click(object sender, EventArgs e)
        {
            LoadLaporan();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedId == -1)
            {
                MessageBox.Show("Pilih data yang ingin dihapus.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Yakin ingin menghapus data ini?",
                "Konfirmasi Hapus",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                string connStr = "Server=MSI\\MSSQLSERVER01;Database=Sistem_Keuangan_Usaha_Seafood;Trusted_Connection=True;";

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("sp_HapusLaporanById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_laporan", selectedId);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Data berhasil dihapus.");
                        LoadLaporan();
                        selectedId = -1;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal menghapus data: " + ex.Message);
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Gunakan index 0 untuk kolom pertama (id_laporan)
                selectedId = Convert.ToInt32(row.Cells[0].Value);

                textBoxDeskripsi.Text = Convert.ToDateTime(row.Cells[1].Value).ToString("MMMM yyyy");
                textBoxHarga.Text = row.Cells[2].Value.ToString();
                textBoxJumlah.Text = row.Cells[3].Value.ToString();
                txtTotal.Text = row.Cells[4].Value.ToString();
            }

        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            sfd.FileName = "LaporanKeuangan.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ExportToCSV(dataGridView1, sfd.FileName);
            }
        }
    }

}
    

