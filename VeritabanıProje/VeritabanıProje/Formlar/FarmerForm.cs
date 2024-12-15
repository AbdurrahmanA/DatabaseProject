using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;
using VeritabanıProje.Class;

namespace VeritabanıProje.Formlar
{
    public partial class FarmerForm : Form
    {
        public FarmerForm()
        {
            InitializeComponent();
        }

        string StrConnection = "Server=localhost; Port=5432; User Id=postgres; Password=123; Database=DatabaseProject;";
        string query = "SELECT urunid, urunadi, kategori, miktar, depolamakosullari, ekimtarihi, hasattarihi, stokmiktari FROM depodakiurun";

        private void FarmerForm_Load(object sender, EventArgs e)
        {
            ListeleUrunler(); 
        }

        private void btnListele_Click(object sender, EventArgs e)
        {
            ListeleUrunler();
        }

        private void ListeleUrunler()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı bağlantısı sırasında hata oluştu: " + ex.Message);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kullanılmıyorsa bu metodu kaldırabilirsiniz
        }

        private void btnUrunEkle_Click(object sender, EventArgs e)
        {
            if (CurrentFarmer.CiftciID.HasValue)
            {
                // Nullable değer kesinlikle null değilse, 'Value' özelliği ile erişilebilir.
                FarmersPage productSale = new FarmersPage(CurrentFarmer.CiftciID.Value, CurrentFarmer.KullanıcıID);
                this.Hide();
                productSale.ShowDialog();
            }
            else
            {
                MessageBox.Show("Çiftçi girişi yapılmamış.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
