using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace VeritabanıProje.Formlar
{
    public partial class MainPage : Form
    {
        private readonly int KullanıcıID;
        private readonly string StrConnection = "Server=localhost; Port=5432; User Id=postgres; Password=123; Database=DatabaseProject;";

        public MainPage(int KullanıcıID)
        {
            InitializeComponent();
            this.KullanıcıID = KullanıcıID;

            MessageBox.Show($"MainPage'e aktarılan Kullanıcı ID: {KullanıcıID}", "Debug");
            KullaniciBilgileriniGoster();
            ListeleSatistakiUrunler();
        }


        private void MainPage_Load(object sender, EventArgs e)
        {
        }

        private void ListeleSatistakiUrunler()
        {
            string query = "SELECT urunid, urunadi, miktar, birimfiyati, toplamfiyat, depolamakosullari FROM satistakiurun";

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
                            dataGridView1.DataSource = dataTable; 
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Satıştaki ürünleri listeleme sırasında bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void KullaniciBilgileriniGoster()
        {
            string query = "SELECT ad, soyad, bakiye FROM public.kullanici WHERE kullaniciid = @KullaniciID";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@KullaniciID", KullanıcıID);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string ad = reader["ad"].ToString();
                                string soyad = reader["soyad"].ToString();
                                decimal bakiye = Convert.ToDecimal(reader["bakiye"]);

                                lblKullaniciBilgi.Text = $"Ad: {ad} {soyad} | Bakiye: {bakiye:C}";
                            }
                            else
                            {
                                MessageBox.Show("Kullanıcı bilgileri bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                lblKullaniciBilgi.Text = "Kullanıcı bilgileri bulunamadı.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kullanıcı bilgilerini getirirken bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void lblKullaniciBilgi_Click(object sender, EventArgs e)
        {
        }

        private void btnSatinAl_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                int urunID = Convert.ToInt32(selectedRow.Cells["urunid"].Value);
                string urunAdi = selectedRow.Cells["urunadi"].Value.ToString();
                decimal toplamFiyat = Convert.ToDecimal(selectedRow.Cells["toplamfiyat"].Value);
                decimal miktar = Convert.ToDecimal(selectedRow.Cells["miktar"].Value);
                string depolamaKosullari = selectedRow.Cells["depolamakosullari"].Value.ToString();

                decimal mevcutBakiye = GetKullaniciBakiyesi(KullanıcıID);

                if (mevcutBakiye >= toplamFiyat)
                {
                    bool basarili = SatinAlUrun(urunID, urunAdi, toplamFiyat, miktar, depolamaKosullari);

                    if (basarili)
                    {
                        MessageBox.Show("Ürün başarıyla satın alındı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ListeleSatistakiUrunler(); 
                        KullaniciBilgileriniGoster(); 
                    }
                    else
                    {
                        MessageBox.Show("Satın alma işlemi sırasında bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Bakiyeniz bu ürünü satın almak için yeterli değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir ürün seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private decimal GetKullaniciBakiyesi(int kullaniciID)
        {
            string query = "SELECT bakiye FROM public.kullanici WHERE kullaniciid = @KullaniciID";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@KullaniciID", kullaniciID);

                        object result = command.ExecuteScalar();
                        return result != null ? Convert.ToDecimal(result) : 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kullanıcı bakiyesini getirirken bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
            }
        }

        //SATISIDYE GORE ÇALIŞMASI LAZIM 
        private bool SatinAlUrun(int urunID, string urunAdi, decimal toplamFiyat, decimal miktar, string depolamaKosullari)
        {
            string insertQuery = @"
    INSERT INTO SatilanUrun (CiftciID, UrunAdi, Kategori, Miktar, DepolamaKosullari, SatilanMiktar, ToplamFiyat, AliciID)
    SELECT ciftciid, @UrunAdi, kategori, miktar, @DepolamaKosullari, @Miktar, @ToplamFiyat, @AliciID
    FROM satistakiurun
    WHERE urunid = @UrunID;
";


            string updateBakiyeQuery = "UPDATE public.kullanici SET bakiye = bakiye - @ToplamFiyat WHERE kullaniciid = @KullaniciID";
            string deleteUrunQuery = "DELETE FROM satistakiurun WHERE urunid = @UrunID";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@UrunID", urunID);
                            command.Parameters.AddWithValue("@UrunAdi", urunAdi);
                            command.Parameters.AddWithValue("@DepolamaKosullari", depolamaKosullari);
                            command.Parameters.AddWithValue("@Miktar", miktar);
                            command.Parameters.AddWithValue("@ToplamFiyat", toplamFiyat);
                            command.Parameters.AddWithValue("@AliciID", KullanıcıID);

                            command.ExecuteNonQuery();
                        }

                        using (NpgsqlCommand command = new NpgsqlCommand(updateBakiyeQuery, connection))
                        {
                            command.Parameters.AddWithValue("@ToplamFiyat", toplamFiyat);
                            command.Parameters.AddWithValue("@KullaniciID", KullanıcıID);

                            command.ExecuteNonQuery();
                        }

                        using (NpgsqlCommand command = new NpgsqlCommand(deleteUrunQuery, connection))
                        {
                            command.Parameters.AddWithValue("@UrunID", urunID);

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Satın alma işlemi sırasında bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
    }
}
