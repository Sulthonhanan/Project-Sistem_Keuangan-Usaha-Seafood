using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistem_Keuangan_Usaha_Seafood
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonInputPemasukan_Click(object sender, EventArgs e)
        {
            Input_Pemasukan pemasukanForm = new Input_Pemasukan();
            pemasukanForm.Show();
            this.Hide();
        }

        private void buttonInputPengeluaran_Click(object sender, EventArgs e)
        {
            Input_Pengeluaran pengeluaranForm = new Input_Pengeluaran();
            pengeluaranForm.Show();
            this.Hide();
        }

        private void buttonLaporanKeuangan_Click(object sender, EventArgs e)
        {
            Laporan_Keuangan laporanForm = new Laporan_Keuangan();
            laporanForm.Show();
            this.Hide();
        }

        private void buttonUbahData_Click(object sender, EventArgs e)
        {
            Edit_Data laporanForm = new Edit_Data();
            laporanForm.Show();
            this.Hide();
        }
    }
}
