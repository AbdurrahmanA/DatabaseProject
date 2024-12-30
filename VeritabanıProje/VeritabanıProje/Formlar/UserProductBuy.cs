using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;
using VeritabanıProje.Class;

namespace VeritabanıProje.Formlar
{
    public partial class UserProductBuy : Form
    {
        private int kullanıcıID; 
        string StrConnection = "Server=localhost; Port=5432; User Id=postgres; Password=123; Database=DatabaseProject;";

        public UserProductBuy(int kullanıcıID)
        {
            this.kullanıcıID = kullanıcıID;
            InitializeComponent();
            txtÜrünAdıArama.TextChanged += txtÜrünAdıArama_TextChanged;
            ListeleSatistakiUrunlerForDataGridView2();

            cmbKategori.Items.Add("Tahıl");
            cmbKategori.Items.Add("Sebze");
            cmbKategori.Items.Add("Meyve");
            cmbKategori.Items.Add("Bakliyat");

            InitializeDataGridView3();

            GetBakiyeFromDatabase();
            DisableTextBoxEdits();
        }

        private void DisableTextBoxEdits()
        {
            txtÜrünAdı.ReadOnly = true;
            Miktar.ReadOnly = true;
            txtÜrünID.ReadOnly = true;
            BirimFiyat.ReadOnly = true;
            ToplamFiyat.ReadOnly = true;
            txtSatışID.ReadOnly = true;
            cmbKategori.Enabled = false;
        }

        private void GetBakiyeFromDatabase()
        {
            string query = "SELECT bakiye FROM kullanici WHERE kullaniciid = @kullaniciid";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@kullaniciid", kullanıcıID);
                        object result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            decimal bakiye = Convert.ToDecimal(result);
                            txtBakiye.Text = bakiye.ToString("C2");
                        }
                        else
                        {
                            MessageBox.Show("Bakiye bulunamadı. Lütfen kullanıcı ID'sini kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Bakiye getirirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InitializeDataGridView3()
        {
            dataGridView3.Columns.Add("urunadi", "Ürün Adı");
            dataGridView3.Columns.Add("miktar", "Miktar");
            dataGridView3.Columns.Add("urunid", "Ürün ID");
            dataGridView3.Columns.Add("birimfiyat", "Birim Fiyat");
            dataGridView3.Columns.Add("toplamfiyat", "Toplam Fiyat");
            dataGridView3.Columns.Add("satisid", "Satış ID");
            dataGridView3.Columns.Add("kategori", "Kategori");
            dataGridView3.Columns.Add("saticiid", "Satıcı ID");
        }

        private void ListeleSatistakiUrunlerForDataGridView2()
        {
            string query = "SELECT s.satisid, s.urunid, u.urunadi, u.kategori, s.miktar, s.birimfiyat, s.toplamfiyat, s.saticiid " +
                           "FROM satistakiurun s " +
                           "JOIN urun u ON s.urunid = u.urunid";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            dataAdapter.Fill(dataTable);
                            dataGridView2.DataSource = dataTable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Satıştaki ürünleri listeleme sırasında bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtÜrünAdı.Text) ||
                 string.IsNullOrWhiteSpace(Miktar.Text) ||
                 string.IsNullOrWhiteSpace(txtÜrünID.Text) ||
                 string.IsNullOrWhiteSpace(BirimFiyat.Text) ||
                 string.IsNullOrWhiteSpace(ToplamFiyat.Text) ||
                 string.IsNullOrWhiteSpace(txtSatışID.Text) ||
                 string.IsNullOrWhiteSpace(cmbKategori.SelectedItem?.ToString()) ||
                 string.IsNullOrWhiteSpace(txtSaticiid.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }

            string urunAdi = txtÜrünAdı.Text;
            string miktar = Miktar.Text;
            string urunID = txtÜrünID.Text;
            string birimFiyat = BirimFiyat.Text;
            string toplamFiyat = ToplamFiyat.Text;
            string satisID = txtSatışID.Text;
            string kategori = cmbKategori.SelectedItem?.ToString() ?? string.Empty;
            string saticiid = txtSaticiid.Text;

            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                if (row.Cells["satisid"].Value != null && row.Cells["satisid"].Value.ToString() == satisID)
                {
                    MessageBox.Show("Bu ürün zaten eklenmiş!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            int rowIndex = dataGridView3.Rows.Add();
            dataGridView3.Rows[rowIndex].Cells["urunadi"].Value = urunAdi;
            dataGridView3.Rows[rowIndex].Cells["miktar"].Value = miktar;
            dataGridView3.Rows[rowIndex].Cells["urunid"].Value = urunID;
            dataGridView3.Rows[rowIndex].Cells["birimfiyat"].Value = birimFiyat;
            dataGridView3.Rows[rowIndex].Cells["toplamfiyat"].Value = toplamFiyat;
            dataGridView3.Rows[rowIndex].Cells["satisid"].Value = satisID;
            dataGridView3.Rows[rowIndex].Cells["kategori"].Value = kategori;
            dataGridView3.Rows[rowIndex].Cells["saticiid"].Value = saticiid;

            ToplamTutarHesapla();

            txtSatışID.Clear();
            txtÜrünAdı.Clear();
            txtÜrünID.Clear();
            Miktar.Clear();
            BirimFiyat.Clear();
            ToplamFiyat.Clear();
            txtSaticiid.Clear();
        }

        private void ToplamTutarHesapla()
        {
            decimal toplamTutar = 0;

            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                if (row.Cells["toplamfiyat"].Value != null &&
                    decimal.TryParse(row.Cells["toplamfiyat"].Value.ToString(), out decimal fiyat))
                {
                    toplamTutar += fiyat;
                }
            }

            txtToplamTutar.Text = toplamTutar.ToString("C2");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (DataGridViewRow row in dataGridView3.Rows)
                        {
                            if (row.Cells["satisid"].Value != null)
                            {
                                int satisID = Convert.ToInt32(row.Cells["satisid"].Value);
                                int urunID = Convert.ToInt32(row.Cells["urunid"].Value);
                                decimal miktar = Convert.ToDecimal(row.Cells["miktar"].Value);
                                decimal toplamFiyat = Convert.ToDecimal(row.Cells["toplamfiyat"].Value);
                                int saticiID = Convert.ToInt32(row.Cells["saticiid"].Value);

                                using (NpgsqlCommand command = new NpgsqlCommand("CALL urun_satin_al_v2(@p_satisid, @p_urunid, @p_satilanmiktar, @p_toplamfiyat, @p_aliciid)", connection))
                                {
                                    command.Parameters.AddWithValue("@p_satisid", satisID);
                                    command.Parameters.AddWithValue("@p_urunid", urunID);
                                    command.Parameters.AddWithValue("@p_satilanmiktar", miktar);
                                    command.Parameters.AddWithValue("@p_toplamfiyat", toplamFiyat);
                                    command.Parameters.AddWithValue("@p_aliciid", kullanıcıID);
                                    command.ExecuteNonQuery();
                                }

                                using (NpgsqlCommand kazancCommand = new NpgsqlCommand("CALL guncelle_toplamkazanc(@p_kullaniciid, @p_kazanc)", connection))
                                {
                                    kazancCommand.Parameters.AddWithValue("@p_kullaniciid", saticiID);
                                    kazancCommand.Parameters.AddWithValue("@p_kazanc", toplamFiyat);
                                    kazancCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }

                    MessageBox.Show("Satış işlemi başarıyla tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ListeleSatistakiUrunlerForDataGridView2();
                    dataGridView3.Rows.Clear();
                    txtToplamTutar.Text = "0";
                    GetBakiyeFromDatabase();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

                txtÜrünAdı.Text = row.Cells["urunadi"].Value?.ToString() ?? string.Empty;
                Miktar.Text = row.Cells["miktar"].Value?.ToString() ?? string.Empty;
                txtÜrünID.Text = row.Cells["urunid"].Value?.ToString() ?? string.Empty;
                BirimFiyat.Text = row.Cells["birimfiyat"].Value?.ToString() ?? string.Empty;
                ToplamFiyat.Text = row.Cells["toplamfiyat"].Value?.ToString() ?? string.Empty;
                txtSatışID.Text = row.Cells["satisid"].Value?.ToString() ?? string.Empty;
                txtSaticiid.Text = row.Cells["saticiid"].Value?.ToString() ?? string.Empty;


                string kategoriValue = row.Cells["kategori"].Value?.ToString() ?? string.Empty;

                if (cmbKategori.Items.Contains(kategoriValue))
                {
                    cmbKategori.SelectedItem = kategoriValue;
                }
                else
                {
                    cmbKategori.SelectedIndex = -1;
                }
            }
        }

        private void guna2GradientTileButton2_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(CurrentUser.KullanıcıID);
            settings.Show();
            this.Hide();
        }

        private void guna2GradientTileButton5_Click(object sender, EventArgs e)
        {
            UserLogIn userLogIn = new UserLogIn();
            userLogIn.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
        private void txtÜrünAdıArama_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = txtÜrünAdıArama.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                ListeleSatistakiUrunlerForDataGridView2(); 
                return;
            }

            string query = "SELECT s.satisid, s.urunid, u.urunadi, u.kategori, s.miktar, s.birimfiyat, s.toplamfiyat, s.saticiid " +
                           "FROM satistakiurun s " +
                           "JOIN urun u ON s.urunid = u.urunid " +
                           "WHERE u.urunadi ILIKE @searchTerm";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@searchTerm", searchTerm + "%"); 

                        using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            dataAdapter.Fill(dataTable);
                            dataGridView2.DataSource = dataTable; 
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ürün arama sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UserProductBuy_Load(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
