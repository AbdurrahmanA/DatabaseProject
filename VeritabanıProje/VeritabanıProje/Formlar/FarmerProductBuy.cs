using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;
using VeritabanıProje.Class;

namespace VeritabanıProje.Formlar
{
    public partial class FarmerProductBuy : Form
    {
        private int kullanıcıID; // Kullanıcı ID
        string StrConnection = "Server=localhost; Port=5432; User Id=postgres; Password=123; Database=DatabaseProject;";

        public FarmerProductBuy(int kullanıcıID)
        {
            this.kullanıcıID = kullanıcıID;
            InitializeComponent();
            ListeleSatistakiUrunlerForDataGridView2();

            // Manually add items to cmbKategori
            cmbKategori.Items.Add("Tahıl");
            cmbKategori.Items.Add("Sebze");
            cmbKategori.Items.Add("Meyve");
            cmbKategori.Items.Add("Bakliyat");

            // Initialize DataGridView3 with columns
            InitializeDataGridView3();

            // Get user's balance (bakiye) when form loads
            GetBakiyeFromDatabase();
            DisableTextBoxEdits();

        }
        private void DisableTextBoxEdits()
        {
            // Disable editing (make them read-only)
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
                        // Kullanıcı ID'sini parametre olarak ekle
                        command.Parameters.AddWithValue("@kullaniciid", kullanıcıID);

                        // Sorguyu çalıştır ve sonucu al
                        object result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            // Gelen bakiye değerini decimal'e dönüştür ve TextBox'a yaz
                            decimal bakiye = Convert.ToDecimal(result);
                            txtBakiye.Text = bakiye.ToString("C2"); // Para formatında gösterim
                        }
                        else
                        {
                            // Bakiye bulunamazsa hata göster
                            MessageBox.Show("Bakiye bulunamadı. Lütfen kullanıcı ID'sini kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Hata durumunda mesaj göster
                    MessageBox.Show($"Bakiye getirirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Method to initialize columns for dataGridView3
        private void InitializeDataGridView3()
        {
            // Manually add columns to dataGridView3
            dataGridView3.Columns.Add("urunadi", "Ürün Adı");
            dataGridView3.Columns.Add("miktar", "Miktar");
            dataGridView3.Columns.Add("urunid", "Ürün ID");
            dataGridView3.Columns.Add("birimfiyat", "Birim Fiyat");
            dataGridView3.Columns.Add("toplamfiyat", "Toplam Fiyat");
            dataGridView3.Columns.Add("satisid", "Satış ID");
            dataGridView3.Columns.Add("kategori", "Kategori");
            dataGridView3.Columns.Add("saticiid", "Satıcı ID");
        }

        // Method to populate dataGridView2
        private void ListeleSatistakiUrunlerForDataGridView2()
        {
            // SQL sorgusunu güncelle ve saticiid'yi de dahil et
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
                            dataGridView2.DataSource = dataTable; // Veriler dataGridView2'ye atanır
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Satıştaki ürünleri listeleme sırasında bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        // Button click event to add data from textboxes to dataGridView3
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
                return; // Stop further execution if validation fails
            }
            // TextBox'lardan veri al
            string urunAdi = txtÜrünAdı.Text;
            string miktar = Miktar.Text;
            string urunID = txtÜrünID.Text;
            string birimFiyat = BirimFiyat.Text;
            string toplamFiyat = ToplamFiyat.Text;
            string satisID = txtSatışID.Text;
            string kategori = cmbKategori.SelectedItem?.ToString() ?? string.Empty;
            string saticiid = txtSaticiid.Text; // txtSaticiid'deki değeri al

            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                if (row.Cells["satisid"].Value != null && row.Cells["satisid"].Value.ToString() == satisID)
                {
                    MessageBox.Show("Bu ürün zaten eklenmiş!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Eğer zaten mevcutsa, eklemeyi iptal et
                }
            }

            // dataGridView3'e yeni bir satır ekle
            int rowIndex = dataGridView3.Rows.Add(); // Yeni bir satır ekler ve satırın indeksini alır

            // Yeni satıra verileri yaz
            dataGridView3.Rows[rowIndex].Cells["urunadi"].Value = urunAdi;
            dataGridView3.Rows[rowIndex].Cells["miktar"].Value = miktar;
            dataGridView3.Rows[rowIndex].Cells["urunid"].Value = urunID;
            dataGridView3.Rows[rowIndex].Cells["birimfiyat"].Value = birimFiyat;
            dataGridView3.Rows[rowIndex].Cells["toplamfiyat"].Value = toplamFiyat;
            dataGridView3.Rows[rowIndex].Cells["satisid"].Value = satisID;
            dataGridView3.Rows[rowIndex].Cells["kategori"].Value = kategori;
            dataGridView3.Rows[rowIndex].Cells["saticiid"].Value = saticiid; // saticiid'yi de aktar

            // Call to recalculate the total price when a new row is added
            ToplamTutarHesapla();

            // Formu sıfırlama
            txtSatışID.Clear();
            txtÜrünAdı.Clear();
            txtÜrünID.Clear();
            Miktar.Clear();
            BirimFiyat.Clear();
            ToplamFiyat.Clear();
            txtSaticiid.Clear(); // txtSaticiid'yi de temizle
        }


        private void dataGridView2_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

                // Set the values in the textboxes
                txtÜrünAdı.Text = row.Cells["urunadi"].Value?.ToString() ?? string.Empty;
                Miktar.Text = row.Cells["miktar"].Value?.ToString() ?? string.Empty;
                txtÜrünID.Text = row.Cells["urunid"].Value?.ToString() ?? string.Empty;
                BirimFiyat.Text = row.Cells["birimfiyat"].Value?.ToString() ?? string.Empty;
                ToplamFiyat.Text = row.Cells["toplamfiyat"].Value?.ToString() ?? string.Empty;
                txtSatışID.Text = row.Cells["satisid"].Value?.ToString() ?? string.Empty;
                txtSaticiid.Text = row.Cells["saticiid"].Value?.ToString() ?? string.Empty;


                // Set the ComboBox value by finding the corresponding item in cmbKategori
                string kategoriValue = row.Cells["kategori"].Value?.ToString() ?? string.Empty;

                // Try to find the item in the ComboBox
                if (cmbKategori.Items.Contains(kategoriValue))
                {
                    cmbKategori.SelectedItem = kategoriValue;
                }
                else
                {
                    cmbKategori.SelectedIndex = -1; // Reset the ComboBox if the value is not found
                }
            }
        }

        // Event triggered when a new row is added to dataGridView3
        private void DataGridView3_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            ToplamTutarHesapla(); // Her yeni satır eklendiğinde toplam tutarı hesapla
        }

        // Method to calculate the total of 'toplamfiyat' column in dataGridView3
        private void ToplamTutarHesapla()
        {
            decimal toplamTutar = 0; // Toplam tutar için değişken

            // dataGridView3'teki her satır için toplamfiyat hücresini al
            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                // Eğer satır boş değilse ve toplamfiyat hücresinin değeri sayısal ise
                if (row.Cells["toplamfiyat"].Value != null)
                {
                    // Veriyi 'decimal' formatında almayı deneyelim
                    if (decimal.TryParse(row.Cells["toplamfiyat"].Value.ToString(), out decimal fiyat))
                    {
                        toplamTutar += fiyat; // Toplam tutarı güncelle
                    }
                }
            }

            // Hesaplanan toplam tutarı txtToplamTutar TextBox'ına yaz
            txtToplamTutar.Text = toplamTutar.ToString("C2"); // İsteğe bağlı: "C2" formatı ile para birimi olarak gösterme
        }

        // Inside your button3_Click event method

        private void button3_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) // Start a transaction
                    {
                        foreach (DataGridViewRow row in dataGridView3.Rows)
                        {
                            if (row.Cells["satisid"].Value != null)
                            {
                                int satisID = Convert.ToInt32(row.Cells["satisid"].Value);
                                int urunID = Convert.ToInt32(row.Cells["urunid"].Value);
                                decimal miktar = Convert.ToDecimal(row.Cells["miktar"].Value);
                                decimal toplamFiyat = Convert.ToDecimal(row.Cells["toplamfiyat"].Value);
                                int saticiID = Convert.ToInt32(row.Cells["saticiid"].Value); // Satıcı ID'yi alıyoruz

                                // Satış işlemini gerçekleştiren prosedürü çağır
                                using (NpgsqlCommand command = new NpgsqlCommand("CALL urun_satin_al_v2(@p_satisid, @p_urunid, @p_satilanmiktar, @p_toplamfiyat, @p_aliciid)", connection))
                                {
                                    command.Parameters.AddWithValue("@p_satisid", satisID);
                                    command.Parameters.AddWithValue("@p_urunid", urunID);
                                    command.Parameters.AddWithValue("@p_satilanmiktar", miktar);
                                    command.Parameters.AddWithValue("@p_toplamfiyat", toplamFiyat);
                                    command.Parameters.AddWithValue("@p_aliciid", kullanıcıID);
                                    command.ExecuteNonQuery();
                                }

                                // Çiftçinin toplam kazancını güncelle
                                using (NpgsqlCommand kazancCommand = new NpgsqlCommand("CALL guncelle_toplamkazanc(@p_kullaniciid, @p_kazanc)", connection))
                                {
                                    kazancCommand.Parameters.AddWithValue("@p_kullaniciid", saticiID);  // Satıcı ID'sini gönder
                                    kazancCommand.Parameters.AddWithValue("@p_kazanc", toplamFiyat);  // Kazancı toplamFiyat kadar artır
                                    kazancCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit(); // Commit the transaction
                    }

                    MessageBox.Show("Satış işlemi başarıyla tamamlandı ve çiftçi kazancı güncellendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ListeleSatistakiUrunlerForDataGridView2();
                    dataGridView3.Rows.Clear();
                    txtToplamTutar.Text = "0"; // Satış sonrası toplam tutarı sıfırla
                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show($"Veritabanı hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Satış işlemi sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void guna2GradientTileButton4_Click(object sender, EventArgs e)
        {
            FarmersStorage farmersStorage = new FarmersStorage(CurrentUser.KullanıcıID);
            farmersStorage.Show();
            this.Hide();
        }

        private void guna2GradientTileButton3_Click(object sender, EventArgs e)
        {
            FarmerProductSell farmerProductSell = new FarmerProductSell(CurrentUser.KullanıcıID);
            farmerProductSell.Show();
            this.Hide();
        }

        private void guna2GradientTileButton1_Click(object sender, EventArgs e)
        {
            FarmerProductBuy farmerProductBuy = new FarmerProductBuy(CurrentUser.KullanıcıID);
            farmerProductBuy.Show();
            this.Hide();
        }

        private void guna2GradientTileButton2_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(CurrentUser.KullanıcıID);
            settings.Show();
            this.Hide();
        }

        private void FarmerProductBuy_Load(object sender, EventArgs e)
        {

        }

        private void guna2GradientTileButton5_Click(object sender, EventArgs e)
        {
            UserLogIn userLogIn = new UserLogIn();
            userLogIn.Show();
            this.Hide();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }
    }
}
