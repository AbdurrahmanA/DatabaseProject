using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;
using VeritabanıProje.Class; // PostgreSQL için gerekli kütüphane

namespace VeritabanıProje.Formlar
{
    public partial class Settings : Form
    {
        string StrConnection = "Server=localhost; Port=5432; User Id=postgres; Password=123; Database=DatabaseProject;";
        private int kullanıcıID;

        public Settings(int kullanıcıID)
        {
            InitializeComponent();
            this.kullanıcıID = kullanıcıID;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            // Panel için gerekli işlem
        }

        private void label9_Click(object sender, EventArgs e)
        {
            // Label tıklandığında yapılacak işlem
        }

        private void guna2TextBox5_TextChanged(object sender, EventArgs e)
        {
            // Textbox içinde metin değiştiğinde yapılacak işlem
        }

        // Kullanıcı türünü belirleme fonksiyonu
        private string KullaniciTuruBelirle(int kullaniciID)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                string queryCiftci = "SELECT COUNT(*) FROM public.ciftci WHERE kullaniciid = @KullaniciID";
                NpgsqlCommand commandCiftci = new NpgsqlCommand(queryCiftci, connection);
                commandCiftci.Parameters.AddWithValue("KullaniciID", kullaniciID);

                connection.Open();
                int ciftciCount = Convert.ToInt32(commandCiftci.ExecuteScalar());
                connection.Close();

                if (ciftciCount > 0)
                {
                    return "Çiftçi";
                }
                else
                {
                    return "Kullanıcı";
                }
            }
        }

        // Kullanıcı bilgilerini form yüklendiğinde alalım
        private void Settings_Load(object sender, EventArgs e)
        {
            string query = "SELECT ad, soyad, email, telefon, sifre, bakiye FROM kullanici WHERE kullaniciid = @kullaniciid";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@kullaniciid", kullanıcıID);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Kullanıcı bilgilerini text kutularına aktar
                                txtName.Text = reader["ad"].ToString();
                                txtSurname.Text = reader["soyad"].ToString();
                                txtMail.Text = reader["email"].ToString();
                                txtPhone.Text = reader["telefon"].ToString();
                                txtPassword.Text = reader["sifre"].ToString();
                                txtBalance.Text = reader["bakiye"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kullanıcı bilgileri yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Kullanıcı bilgilerini güncelleme fonksiyonu
        private void Kaydet_Click(object sender, EventArgs e)
        {
            // Text kutularındaki verileri alalım
            string name = txtName.Text;
            string surname = txtSurname.Text;
            string mail = txtMail.Text;
            string phone = txtPhone.Text;
            string password = txtPassword.Text;
            decimal balance;

            // Bakiye kısmının geçerli bir sayı olduğundan emin olalım
            if (!decimal.TryParse(txtBalance.Text.Trim(), out balance))
            {
                MessageBox.Show("Geçerli bir bakiye girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kullanıcı bilgilerini güncelleme sorgusu
            string updateQuery = @"
            UPDATE kullanici 
            SET ad = @ad, soyad = @soyad, email = @email, telefon = @telefon, sifre = @sifre, bakiye = @bakiye
            WHERE kullaniciid = @kullaniciid";

            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection))
                    {
                        // Parametreleri ekleyelim
                        command.Parameters.AddWithValue("@ad", name);
                        command.Parameters.AddWithValue("@soyad", surname);
                        command.Parameters.AddWithValue("@email", mail);
                        command.Parameters.AddWithValue("@telefon", phone);
                        command.Parameters.AddWithValue("@sifre", password);
                        command.Parameters.AddWithValue("@bakiye", balance);
                        command.Parameters.AddWithValue("@kullaniciid", kullanıcıID); // Kullanıcı ID'sini ekliyoruz

                        // Sorguyu çalıştırıp veritabanındaki bilgileri güncelle
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Kullanıcı bilgileri başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Kullanıcı türünü belirle
                            string kullaniciTuru = KullaniciTuruBelirle(kullanıcıID);

                            if (kullaniciTuru == "Çiftçi")
                            {
                                // Çiftçi olarak giriş yaptıysa, çiftçiye özel ekranı göster
                                FarmerProductBuy farmerForm = new FarmerProductBuy(kullanıcıID);
                                farmerForm.Show();
                                this.Hide();
                            }
                            else if (kullaniciTuru == "Kullanıcı")
                            {
                                // Kullanıcı olarak giriş yaptıysa, kullanıcıya özel ekranı göster
                                UserProductBuy mainPage = new UserProductBuy(kullanıcıID);
                                mainPage.Show();
                                this.Hide();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Güncelleme sırasında bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kullanıcı bilgileri güncellenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Kullanıcı türünü belirleyip, ona göre yönlendirme yapacak buton
        private void guna2GradientTileButton1_Click(object sender, EventArgs e)
        {
            // Kullanıcı türünü belirle
            string kullaniciTuru = KullaniciTuruBelirle(kullanıcıID);

            // Kullanıcı türüne göre yönlendirme
            if (kullaniciTuru == "Çiftçi")
            {
                // Çiftçi olarak giriş yaptıysa, çiftçiye özel ekranı göster
                FarmerProductBuy farmerForm = new FarmerProductBuy(kullanıcıID);
                farmerForm.Show();
                this.Hide();
            }
            else if (kullaniciTuru == "Kullanıcı")
            {
                // Kullanıcı olarak giriş yaptıysa, kullanıcıya özel ekranı göster
                UserProductBuy mainPage = new UserProductBuy(kullanıcıID);
                mainPage.Show();
                this.Hide();
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
            UserLogIn userLogIn = new UserLogIn();
            userLogIn.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }
    }
}
