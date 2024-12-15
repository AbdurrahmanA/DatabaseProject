using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;
using VeritabanıProje.Class;

namespace VeritabanıProje.Formlar
{
    public partial class FarmersPage : Form
    {
        private int ciftciID;
        private int KullanıcıID;
        private int hiddenSelectedUrunID = 0;
        private int hiddenSelectedSatistakiUrunID = 0;
        public FarmersPage(int ciftciID,int KullanıcıID)
        {
            InitializeComponent();
            this.ciftciID = ciftciID;
            this.KullanıcıID = KullanıcıID;
            ListeleUrunler();
            ListeleSatistakiUrunler();
            dataGridView2.SelectionChanged += dataGridView2_SelectionChanged;

        }

        string StrConnection = "Server=localhost; Port=5432; User Id=postgres; Password=123; Database=DatabaseProject;";

        private void ListeleUrunler()
        {
            string query = "SELECT urunid, urunadi, kategori, miktar, depolamakosullari, ekimtarihi, hasattarihi, stokmiktari " +
                           "FROM depodakiurun WHERE ciftciid = @CiftciID";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CiftciID", ciftciID);

                    NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command);
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
        private void ListeleSatistakiUrunler()
        {
            string query = "SELECT urunid, urunadi, miktar, birimfiyati, depolamakosullari FROM satistakiurun WHERE ciftciid = @CiftciID";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CiftciID", ciftciID);

                    NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dataGridView2.DataSource = dataTable; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Satıştaki ürünleri listeleme sırasında hata oluştu: " + ex.Message);
                }
            }
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

                hiddenSelectedSatistakiUrunID = Convert.ToInt32(row.Cells["urunid"].Value);
            }
        }

        private void ProductSale_Load(object sender, EventArgs e)
        {
            ListeleUrunler();
        }
        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0) 
            {
                DataGridViewRow row = dataGridView2.SelectedRows[0];
                hiddenSelectedSatistakiUrunID = Convert.ToInt32(row.Cells["urunid"].Value);
            }
        }

        private void btnEkle_Click_1(object sender, EventArgs e)
        {
            
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz ürünü seçiniz.");
                return;
            }
            int urunID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["urunid"].Value);
            string deleteQuery = "DELETE FROM depodakiurun WHERE urunid = @UrunID AND ciftciid = @CiftciID";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();

                    NpgsqlCommand command = new NpgsqlCommand(deleteQuery, connection);
                    command.Parameters.AddWithValue("@UrunID", urunID);
                    command.Parameters.AddWithValue("@CiftciID", ciftciID);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Ürün Başarıyla Silindi");
                        ListeleUrunler();
                    }
                    else
                    {
                        MessageBox.Show("Ürün Silinemedi.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanından silerken hata oluştu: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void btnGüncelle_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtUrunAdi.Text = row.Cells["urunadi"].Value.ToString();
                txtKategori.Text = row.Cells["kategori"].Value.ToString();
                txtMiktar.Text = row.Cells["miktar"].Value.ToString();
                txtDepolamaKosullari.Text = row.Cells["depolamakosullari"].Value.ToString();
                dateTimePickerEkimTarihi.Value = Convert.ToDateTime(row.Cells["ekimtarihi"].Value);
                dateTimePickerHasatTarihi.Value = Convert.ToDateTime(row.Cells["hasattarihi"].Value);

                hiddenSelectedUrunID = Convert.ToInt32(row.Cells["urunid"].Value);
            }
        }

        private void btnSatisaEkle_Click_1(object sender, EventArgs e)
        {
            if (hiddenSelectedUrunID == 0)
            {
                MessageBox.Show("Satışa eklemek için bir ürün seçiniz.");
                return;
            }

            if (!decimal.TryParse(txtSatilacakMiktar.Text, out decimal satilacakMiktar) || satilacakMiktar <= 0)
            {
                MessageBox.Show("Geçerli bir miktar giriniz.");
                return;
            }

            if (!decimal.TryParse(txtBirimFiyati.Text, out decimal birimFiyati) || birimFiyati <= 0)
            {
                MessageBox.Show("Geçerli bir birim fiyatı giriniz.");
                return;
            }

            string depolamaKosullari = txtDepolamaKosullari.Text;
            if (string.IsNullOrEmpty(depolamaKosullari))
            {
                depolamaKosullari = "Depo koşulları belirsiz";
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand("CALL satis_ekle(@UrunID, @CiftciID, @Miktar, @BirimFiyati, @DepolamaKosullari)", connection))
                    {
                        command.Parameters.AddWithValue("@UrunID", hiddenSelectedUrunID);
                        command.Parameters.AddWithValue("@CiftciID", ciftciID);
                        command.Parameters.AddWithValue("@Miktar", satilacakMiktar);
                        command.Parameters.AddWithValue("@BirimFiyati", birimFiyati);
                        command.Parameters.AddWithValue("@DepolamaKosullari", depolamaKosullari);

                        command.ExecuteNonQuery();

                        MessageBox.Show("Ürün başarıyla satışa eklendi.");
                        ListeleSatistakiUrunler();
                        ListeleUrunler();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Satışa ekleme sırasında hata oluştu: " + ex.Message);
                }
            }
        }
        //URUNIDYE GÖRE SİLMEMELİ
        private void btnSatistakiUrunuGeriAl_Click(object sender, EventArgs e)
        {
            if (hiddenSelectedSatistakiUrunID == 0)
            {
                MessageBox.Show("Depoya geri eklemek için bir satıştaki ürünü seçiniz.");
                return;
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        string deleteSatistakiUrunQuery = "DELETE FROM satistakiurun WHERE urunid = @UrunID AND ciftciid = @CiftciID";

                        using (NpgsqlCommand deleteCommand = new NpgsqlCommand(deleteSatistakiUrunQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@UrunID", hiddenSelectedSatistakiUrunID);
                            deleteCommand.Parameters.AddWithValue("@CiftciID", ciftciID);

                            int rowsAffected = deleteCommand.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                MessageBox.Show("Seçilen ürün silinemedi. Lütfen tekrar deneyiniz.");
                                transaction.Rollback();
                                return;
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show("Seçilen ürün başarıyla silindi.");
                        ListeleSatistakiUrunler(); 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme işlemi sırasında hata oluştu: " + ex.Message);
                }
            }
        }



        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUrunAdi.Text) || string.IsNullOrEmpty(txtKategori.Text) ||
               string.IsNullOrEmpty(txtMiktar.Text) || string.IsNullOrEmpty(txtDepolamaKosullari.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return;
            }

            if (!decimal.TryParse(txtMiktar.Text, out decimal miktar))
            {
                MessageBox.Show("Lütfen geçerli bir miktar girin.");
                return;
            }

            string urunAdi = txtUrunAdi.Text.Trim();
            string kategori = txtKategori.Text.Trim();
            string depolamaKosullari = txtDepolamaKosullari.Text.Trim();
            DateTime ekimTarihi = dateTimePickerEkimTarihi.Value;
            DateTime hasatTarihi = dateTimePickerHasatTarihi.Value;

            string insertQuery = "INSERT INTO depodakiurun (CiftciID, UrunAdi, Kategori, Miktar, DepolamaKosullari, EkimTarihi, HasatTarihi, StokMiktari) " +
                                 "VALUES (@CiftciID, @UrunAdi, @Kategori, @Miktar, @DepolamaKosullari, @EkimTarihi, @HasatTarihi, @StokMiktari)";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();

                    NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection);
                    command.Parameters.AddWithValue("@CiftciID", ciftciID);
                    command.Parameters.AddWithValue("@UrunAdi", urunAdi);
                    command.Parameters.AddWithValue("@Kategori", kategori);
                    command.Parameters.AddWithValue("@Miktar", miktar);
                    command.Parameters.AddWithValue("@DepolamaKosullari", depolamaKosullari);
                    command.Parameters.AddWithValue("@EkimTarihi", NpgsqlTypes.NpgsqlDbType.Date, ekimTarihi);
                    command.Parameters.AddWithValue("@HasatTarihi", NpgsqlTypes.NpgsqlDbType.Date, hasatTarihi);
                    command.Parameters.AddWithValue("@StokMiktari", miktar);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Ürün başarıyla eklendi!");
                        ListeleUrunler();
                    }
                    else
                    {
                        MessageBox.Show("Kayıt yapılamadı.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanına eklerken hata oluştu: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void btnGüncelle_Click_1(object sender, EventArgs e)
        {

            if (hiddenSelectedUrunID == 0)
            {
                MessageBox.Show("Güncellenecek bir ürün seçilmedi.");
                return;
            }

            if (string.IsNullOrEmpty(txtUrunAdi.Text) || string.IsNullOrEmpty(txtKategori.Text) ||
                string.IsNullOrEmpty(txtMiktar.Text) || string.IsNullOrEmpty(txtDepolamaKosullari.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return;
            }

            if (!decimal.TryParse(txtMiktar.Text, out decimal miktar))
            {
                MessageBox.Show("Lütfen geçerli bir miktar girin.");
                return;
            }

            string urunAdi = txtUrunAdi.Text.Trim();
            string kategori = txtKategori.Text.Trim();
            string depolamaKosullari = txtDepolamaKosullari.Text.Trim();
            DateTime ekimTarihi = dateTimePickerEkimTarihi.Value;
            DateTime hasatTarihi = dateTimePickerHasatTarihi.Value;

            string updateQuery = "UPDATE depodakiurun " +
                                 "SET urunadi = @UrunAdi, kategori = @Kategori, miktar = @Miktar, depolamakosullari = @DepolamaKosullari, " +
                                 "ekimtarihi = @EkimTarihi, hasattarihi = @HasatTarihi " +
                                 "WHERE urunid = @UrunID AND ciftciid = @CiftciID";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();

                    NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection);
                    command.Parameters.AddWithValue("@UrunAdi", urunAdi);
                    command.Parameters.AddWithValue("@Kategori", kategori);
                    command.Parameters.AddWithValue("@Miktar", miktar);
                    command.Parameters.AddWithValue("@DepolamaKosullari", depolamaKosullari);
                    command.Parameters.AddWithValue("@EkimTarihi", ekimTarihi);
                    command.Parameters.AddWithValue("@HasatTarihi", hasatTarihi);
                    command.Parameters.AddWithValue("@UrunID", hiddenSelectedUrunID);
                    command.Parameters.AddWithValue("@CiftciID", ciftciID);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Ürün başarıyla güncellendi!");
                        ListeleUrunler();
                    }
                    else
                    {
                        MessageBox.Show("Ürün güncellenemedi. Bu ürünü güncelleme yetkiniz olmayabilir.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ürünü güncellerken hata oluştu: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void FarmersPage_Load(object sender, EventArgs e)
        {

        }

        private void btnAnaSayfa_Click(object sender, EventArgs e)
        {
            MainPage mainPage = new MainPage(KullanıcıID);
            this.Hide();
            mainPage.Show();
        }
    }
}
