using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VeritabanıProje.Class;

namespace VeritabanıProje.Formlar
{
    public partial class FarmerProductSell : Form
    {
        string StrConnection = "Server=localhost; Port=5432; User Id=postgres; Password=123; Database=DatabaseProject;";
        private int kullanıcıID;

        public FarmerProductSell(int kullanıcıID)
        {
            InitializeComponent();
            this.kullanıcıID = kullanıcıID;
            ListeleDepodakiUrunler();
            ListeleSatistakiUrunlerForDataGridView1();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // İhtiyaç duyulursa ilgili işlemler burada yapılabilir
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            // Panelde görsel bir işlem yapılacaksa burası kullanılabilir
        }

        private void guna2GradientTileButton4_Click(object sender, EventArgs e)
        {
            FarmersStorage farmersStorage = new FarmersStorage(CurrentUser.KullanıcıID);
            farmersStorage.Show();
            this.Hide();
        }

        private void guna2GradientTileButton1_Click(object sender, EventArgs e)
        {
            FarmerProductBuy farmerProductBuy = new FarmerProductBuy(CurrentUser.KullanıcıID);
            farmerProductBuy.Show();
            this.Hide();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

                txtMiktar.Text = row.Cells["stokmiktari"].Value.ToString();
                txtDepoID.Text = row.Cells["depoid"].Value.ToString();
                txtUrunID.Text = row.Cells["urunid"].Value.ToString();
            }
        }

        private void ListeleDepodakiUrunler()
        {
            string query = @"
        SELECT d.stokmiktari, d.urunid, d.kullaniciid, d.depoid, u.kategori, u.urunadi
        FROM depodakiurun d 
        JOIN urun u ON d.urunid = u.urunid
        WHERE d.kullaniciid = @kullaniciid";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@kullaniciid", kullanıcıID);

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
                    MessageBox.Show($"Depodaki ürünleri listeleme sırasında bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int urunID = Convert.ToInt32(txtUrunID.Text);
            int satilanMiktar;

            if (!int.TryParse(txtMiktar.Text.Trim(), out satilanMiktar) || satilanMiktar <= 0)
            {
                MessageBox.Show("Geçerli bir miktar girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal birimFiyat;
            if (!decimal.TryParse(txtBirimFiyat.Text.Trim(), out birimFiyat) || birimFiyat <= 0)
            {
                MessageBox.Show("Geçerli bir birim fiyatı girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal toplamFiyat = satilanMiktar * birimFiyat;
            DateTime satisTarihi = DateTime.Now;

            SatisEkle(urunID, satilanMiktar, birimFiyat, toplamFiyat, satisTarihi);
            DepodakiUrunMiktarGuncelle(urunID, satilanMiktar);
            ListeleDepodakiUrunler();
            ListeleSatistakiUrunlerForDataGridView1();
        }

        private void SatisEkle(int urunID, int satilanMiktar, decimal birimFiyat, decimal toplamFiyat, DateTime satisTarihi)
        {
            string query = @"
    INSERT INTO satistakiurun (urunid, miktar, birimfiyat, toplamfiyat, satistarih, saticiid)
    VALUES (@urunID, @satilanMiktar, @birimFiyat, @toplamFiyat, @satisTarihi, @kullaniciID)";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@urunID", urunID);
                        command.Parameters.AddWithValue("@satilanMiktar", satilanMiktar);
                        command.Parameters.AddWithValue("@birimFiyat", birimFiyat);
                        command.Parameters.AddWithValue("@toplamFiyat", toplamFiyat);
                        command.Parameters.AddWithValue("@satisTarihi", satisTarihi);
                        command.Parameters.AddWithValue("@kullaniciID", kullanıcıID);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Satış başarıyla gerçekleştirildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Satış ekleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DepodakiUrunMiktarGuncelle(int urunID, int satilanMiktar)
        {
            string query = @"
    UPDATE depodakiurun
    SET stokmiktari = stokmiktari - @satilanMiktar
    WHERE urunid = @urunID AND stokmiktari >= @satilanMiktar";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@satilanMiktar", satilanMiktar);
                        command.Parameters.AddWithValue("@urunID", urunID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            SilDepodakiUrunEgerStokSifir(urunID);
                        }
                        else
                        {
                            MessageBox.Show("Depodaki ürün miktarı yetersiz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Depo güncelleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SilDepodakiUrunEgerStokSifir(int urunID)
        {
            string checkQuery = @"
    SELECT stokmiktari FROM depodakiurun WHERE urunid = @urunID";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(checkQuery, connection))
                    {
                        command.Parameters.AddWithValue("@urunID", urunID);

                        var result = command.ExecuteScalar();
                        if (result != null && Convert.ToInt32(result) == 0)
                        {
                            string deleteQuery = @"
                    DELETE FROM depodakiurun
                    WHERE urunid = @urunID";

                            using (NpgsqlCommand deleteCommand = new NpgsqlCommand(deleteQuery, connection))
                            {
                                deleteCommand.Parameters.AddWithValue("@urunID", urunID);
                                deleteCommand.ExecuteNonQuery();
                                MessageBox.Show("Ürün depodan silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Depo silme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtSatisid.Text, out int satisID))
            {
                SatisiDepoyaAktar(satisID);
            }
            else
            {
                MessageBox.Show("Geçerli bir satış ID'si girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SatisiDepoyaAktar(int satisID)
        {
            string query = @"
        SELECT s.urunid, s.miktar, s.birimfiyat, s.toplamfiyat
        FROM satistakiurun s
        WHERE s.satisid = @satisid";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@satisid", satisID);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int urunID = reader.GetInt32(0);
                                int miktar = reader.GetInt32(1);
                                decimal birimFiyat = reader.GetDecimal(2);
                                decimal toplamFiyat = reader.GetDecimal(3);

                                DepoyaAktar(urunID, miktar);
                                SatistakiUrunuSil(satisID);
                            }
                            else
                            {
                                MessageBox.Show("Satış ID'sine ait ürün bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Satışı depoya aktarma sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DepoyaAktar(int urunID, int miktar)
        {
            // İlk olarak, depodaki mevcut ürünü kontrol edelim
            string checkQuery = @"
    SELECT stokmiktari 
    FROM depodakiurun 
    WHERE urunid = @urunID AND kullaniciid = @kullaniciID"; // Kullanıcı ID'yi de ekleyelim

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(checkQuery, connection))
                    {
                        command.Parameters.AddWithValue("@urunID", urunID);
                        command.Parameters.AddWithValue("@kullaniciID", kullanıcıID);

                        var result = command.ExecuteScalar();

                        if (result != null) // Eğer ürün zaten depoda varsa
                        {
                            // Mevcut stok miktarını al
                            int mevcutMiktar = Convert.ToInt32(result);

                            // Stok miktarını güncelle
                            string updateQuery = @"
                    UPDATE depodakiurun
                    SET stokmiktari = stokmiktari + @miktar
                    WHERE urunid = @urunID AND kullaniciid = @kullaniciID";

                            using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@miktar", miktar);
                                updateCommand.Parameters.AddWithValue("@urunID", urunID);
                                updateCommand.Parameters.AddWithValue("@kullaniciID", kullanıcıID);

                                updateCommand.ExecuteNonQuery();
                                MessageBox.Show("Ürün depodaki mevcut miktar ile güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else // Eğer ürün depoda yoksa
                        {
                            // Yeni bir ürün ekleyelim
                            string insertQuery = @"
                    INSERT INTO depodakiurun (urunid, stokmiktari, kullaniciid)
                    VALUES (@urunID, @miktar, @kullaniciID)";

                            using (NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@urunID", urunID);
                                insertCommand.Parameters.AddWithValue("@miktar", miktar);
                                insertCommand.Parameters.AddWithValue("@kullaniciID", kullanıcıID);

                                insertCommand.ExecuteNonQuery();
                                MessageBox.Show("Yeni ürün depoya başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Depoya aktarım sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }




        private void SatistakiUrunuSil(int satisID)
        {
            // Satıştaki ürünü silme işlemi
            string deleteQuery = "DELETE FROM satistakiurun WHERE satisid = @satisid";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@satisid", satisID);
                        command.ExecuteNonQuery();
                        MessageBox.Show("Ürün satıştan başarıyla kaldırıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Satıştan ürün kaldırıldıktan sonra, depodaki ürünleri tekrar listele
                        ListeleDepodakiUrunler();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Satıştan ürün kaldırma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void ListeleSatistakiUrunlerForDataGridView1()
        {
            string query = "SELECT s.satisid, s.urunid, u.urunadi, u.kategori, s.miktar, s.birimfiyat, s.toplamfiyat " +
                           "FROM satistakiurun s " +
                           "JOIN urun u ON s.urunid = u.urunid " +
                           "WHERE s.saticiid = @kullaniciid";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@kullaniciid", kullanıcıID);

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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtSatisid.Text = row.Cells["satisid"].Value.ToString();
            }
        }

        private void guna2GradientTileButton5_Click(object sender, EventArgs e)
        {
            UserLogIn userLogIn = new UserLogIn();
            userLogIn.Show();
            this.Hide();
        }

        private void guna2GradientTileButton2_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(CurrentUser.KullanıcıID);
            settings.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }
    }
}
