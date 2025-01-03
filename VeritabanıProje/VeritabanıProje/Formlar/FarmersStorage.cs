﻿using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;
using VeritabanıProje.Class;

namespace VeritabanıProje.Formlar
{
    public partial class FarmersStorage : Form
    {
        string StrConnection = "Server=localhost; Port=5432; User Id=postgres; Password=123; Database=DatabaseProject;";
        private int kullanıcıID;

        public FarmersStorage(int kullanıcıID)
        {
            this.kullanıcıID = kullanıcıID;
            InitializeComponent();

            Console.WriteLine("Kullanıcı ID: " + kullanıcıID); // 

            ListeleDepodakiUrunler();
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
                        Console.WriteLine("Running query with kullanıcıID: " + kullanıcıID);

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
                    MessageBox.Show($"Depodaki ürünleri listeleme sırasında bir hata oluştu:\n{ex.Message}",
                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string urunAdi = txtUrunAdi.Text.Trim(); 
            int stokMiktari;

            if (!int.TryParse(txtStokMiktari.Text.Trim(), out stokMiktari) || stokMiktari <= 0)
            {
                MessageBox.Show("Geçerli bir stok miktarı girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int urunID = GetUrunIDByUrunAdi(urunAdi);
            if (urunID == -1)
            {
                MessageBox.Show("Ürün adı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (IsUrunDepodaVar(urunID))
            {
                UpdateDepodakiUrun(stokMiktari, urunID);
            }
            else
            {
                AddDepodakiUrun(stokMiktari, urunID);
            }

            ListeleDepodakiUrunler();
        }

        private int GetUrunIDByUrunAdi(string urunAdi)
        {
            string query = "SELECT urunid FROM urun WHERE urunadi = @urunAdi";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@urunAdi", urunAdi);

                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            return Convert.ToInt32(result); 
                        }
                        else
                        {
                            return -1; 
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
            }
        }

        private bool IsUrunDepodaVar(int urunID)
        {
            string query = "SELECT COUNT(*) FROM depodakiurun WHERE urunid = @urunID AND kullaniciid = @kullaniciID";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@urunID", urunID);
                        command.Parameters.AddWithValue("@kullaniciID", kullanıcıID);

                        var result = command.ExecuteScalar();
                        return Convert.ToInt32(result) > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Veri kontrol hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        private void AddDepodakiUrun(int stokMiktari, int urunID)
        {
            string query = "INSERT INTO depodakiurun (stokmiktari, urunid, kullaniciid) VALUES (@stokMiktari, @urunID, @kullaniciID)";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@stokMiktari", stokMiktari);
                        command.Parameters.AddWithValue("@urunID", urunID);
                        command.Parameters.AddWithValue("@kullaniciID", kullanıcıID);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Ürün başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Veri ekleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateDepodakiUrun(int stokMiktari, int urunID)
        {
            string queryGetCurrentStock = "SELECT stokmiktari FROM depodakiurun WHERE urunid = @urunID AND kullaniciid = @kullaniciID";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();

                    using (NpgsqlCommand commandGetCurrentStock = new NpgsqlCommand(queryGetCurrentStock, connection))
                    {
                        commandGetCurrentStock.Parameters.AddWithValue("@urunID", urunID);
                        commandGetCurrentStock.Parameters.AddWithValue("@kullaniciID", kullanıcıID);
                        var currentStock = commandGetCurrentStock.ExecuteScalar();

                        if (currentStock != null)
                        {
                            int newStock = Convert.ToInt32(currentStock) + stokMiktari;

                            string updateQuery = "UPDATE depodakiurun SET stokmiktari = @stokMiktari WHERE urunid = @urunID AND kullaniciid = @kullaniciID";
                            using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@stokMiktari", newStock);
                                updateCommand.Parameters.AddWithValue("@urunID", urunID);
                                updateCommand.Parameters.AddWithValue("@kullaniciID", kullanıcıID);

                                updateCommand.ExecuteNonQuery();
                                MessageBox.Show("Stok miktarı başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Veri güncelleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
                txtUrunID.Text = row.Cells["urunid"].Value.ToString(); 
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtUrunID.Text, out int urunID) && urunID > 0)
            {
                if (int.TryParse(txtMiktar.Text, out int miktar) && miktar > 0)
                {
                    ReduceOrRemoveProduct(urunID, miktar);
                }
                else
                {
                    MessageBox.Show("Geçerli bir miktar girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Geçerli bir Ürün ID'si girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ReduceOrRemoveProduct(int urunID, int miktar)
        {
            string queryGetCurrentStock = "SELECT stokmiktari FROM depodakiurun WHERE urunid = @urunID AND kullaniciid = @kullaniciID";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();

                    using (NpgsqlCommand commandGetCurrentStock = new NpgsqlCommand(queryGetCurrentStock, connection))
                    {
                        commandGetCurrentStock.Parameters.AddWithValue("@urunID", urunID);
                        commandGetCurrentStock.Parameters.AddWithValue("@kullaniciID", kullanıcıID);
                        var currentStock = commandGetCurrentStock.ExecuteScalar();

                        if (currentStock != null)
                        {
                            int currentStockAmount = Convert.ToInt32(currentStock);

                            if (miktar > currentStockAmount)
                            {
                                MessageBox.Show("Azaltmak istediğiniz miktar, mevcut stok miktarından fazla.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                int newStockAmount = currentStockAmount - miktar;

                                if (newStockAmount == 0)
                                {
                                    string deleteQuery = "DELETE FROM depodakiurun WHERE urunid = @urunID AND kullaniciid = @kullaniciID";
                                    using (NpgsqlCommand deleteCommand = new NpgsqlCommand(deleteQuery, connection))
                                    {
                                        deleteCommand.Parameters.AddWithValue("@urunID", urunID);
                                        deleteCommand.Parameters.AddWithValue("@kullaniciID", kullanıcıID);

                                        deleteCommand.ExecuteNonQuery();
                                        MessageBox.Show("Ürün depodan başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                else
                                {
                                    string updateQuery = "UPDATE depodakiurun SET stokmiktari = @stokMiktari WHERE urunid = @urunID AND kullaniciid = @kullaniciID";
                                    using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, connection))
                                    {
                                        updateCommand.Parameters.AddWithValue("@stokMiktari", newStockAmount);
                                        updateCommand.Parameters.AddWithValue("@urunID", urunID);
                                        updateCommand.Parameters.AddWithValue("@kullaniciID", kullanıcıID);

                                        updateCommand.ExecuteNonQuery();
                                        MessageBox.Show("Stok miktarı başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ürün depoda bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    ListeleDepodakiUrunler();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Veri işleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void guna2GradientTileButton1_Click(object sender, EventArgs e)
        {
            FarmerProductBuy farmerProductBuy = new FarmerProductBuy(CurrentUser.KullanıcıID);
            farmerProductBuy.Show();
            this.Hide();
        }

        private void guna2GradientTileButton3_Click(object sender, EventArgs e)
        {
            FarmerProductSell farmerProductSell = new FarmerProductSell(CurrentUser.KullanıcıID);
            farmerProductSell.Show();
            this.Hide();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

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

        private void FarmersStorage_Load(object sender, EventArgs e)
        {

        }
    }
}
