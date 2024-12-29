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
using VeritabanıProje.Formlar;

namespace VeritabanıProje
{
    public partial class UserSignUp : Form
    {
        string connectionString = "Server=localhost; Port=5432; User Id=postgres; Password=123; Database=proje;";
        public UserSignUp()
        {

            InitializeComponent();
            cmbKullaniciTipi.Items.Add("Kullanıcı");
            cmbKullaniciTipi.Items.Add("Çiftçi");
            label1.Width = 200;    
            label1.Height = 50;
        }
        private void KullaniciKaydet(string ad, string soyad, string email, string telefon, string sifre, decimal bakiye)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = @"
        INSERT INTO public.kullanici (ad, soyad, email, telefon, sifre, kayittarihi, bakiye)
        VALUES (@ad, @soyad, @email, @telefon, @sifre, @kayittarihi, @bakiye)";

                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("ad", ad);
                command.Parameters.AddWithValue("soyad", soyad);
                command.Parameters.AddWithValue("email", email);
                command.Parameters.AddWithValue("telefon", telefon);
                command.Parameters.AddWithValue("sifre", sifre);
                command.Parameters.AddWithValue("kayittarihi", DateTime.Now);
                command.Parameters.AddWithValue("bakiye", bakiye);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void CiftciKaydet(string ad, string soyad, string email, string telefon, string sifre, decimal bakiye)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = @"
        INSERT INTO public.kullanici (ad, soyad, email, telefon, sifre, kayittarihi, bakiye)
        VALUES (@ad, @soyad, @email, @telefon, @sifre, @kayittarihi, @bakiye)
        RETURNING kullaniciid";

                connection.Open();

                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("ad", ad);
                command.Parameters.AddWithValue("soyad", soyad);
                command.Parameters.AddWithValue("email", email);
                command.Parameters.AddWithValue("telefon", telefon);
                command.Parameters.AddWithValue("sifre", sifre);
                command.Parameters.AddWithValue("kayittarihi", DateTime.Now);
                command.Parameters.AddWithValue("bakiye", bakiye);

                // Kullanıcı kaydını al
                int kullaniciId = (int)command.ExecuteScalar();

                // Çiftçi tablosuna kayıt yap
                string farmerQuery = @"
        INSERT INTO public.ciftci (kullaniciid, toplamkazanc)
        VALUES (@kullaniciid, @toplamkazanc)";

                NpgsqlCommand farmerCommand = new NpgsqlCommand(farmerQuery, connection);
                farmerCommand.Parameters.AddWithValue("kullaniciid", kullaniciId);
                farmerCommand.Parameters.AddWithValue("toplamkazanc", 0); // Başlangıç kazancı 0

                farmerCommand.ExecuteNonQuery();
            }
        }

        private void btnKayıt_Click(object sender, EventArgs e)
        {
            try
            {
                string ad = txtAd.Text.Trim();
                string soyad = txtSoyad.Text.Trim();
                string email = txtEmail.Text.Trim();
                string telefon = txtTelefon.Text.Trim();
                string sifre = txtSifre.Text.Trim();
                decimal bakiye;

                if (string.IsNullOrWhiteSpace(ad) ||
                    string.IsNullOrWhiteSpace(soyad) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(telefon) ||
                    string.IsNullOrWhiteSpace(sifre) ||
                    !decimal.TryParse(txtBakiye.Text.Trim(), out bakiye))
                {
                    MessageBox.Show("Tüm alanları doğru ve eksiksiz doldurunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kullanıcı tipini kontrol et
                if (cmbKullaniciTipi.SelectedItem == null)
                {
                    MessageBox.Show("Lütfen kullanıcı tipini seçiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string kullaniciTipi = cmbKullaniciTipi.SelectedItem.ToString();

                if (kullaniciTipi == "Kullanıcı")
                {
                    KullaniciKaydet(ad, soyad, email, telefon, sifre, bakiye);
                    MessageBox.Show("Kullanıcı başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (kullaniciTipi == "Çiftçi")
                {
                    CiftciKaydet(ad, soyad, email, telefon, sifre, bakiye);
                    MessageBox.Show("Çiftçi başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }


                // Form temizleme
                txtAd.Clear();
                txtSoyad.Clear();
                txtEmail.Clear();
                txtTelefon.Clear();
                txtSifre.Clear();
                txtBakiye.Clear();
                cmbKullaniciTipi.SelectedIndex = -1;

                UserLogIn userLogIn = new UserLogIn();

                userLogIn.Show();

                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt yapılamadı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            FirstPage firstPage = new FirstPage();

            firstPage.Show();

            this.Hide();
        }

        private void UserSignUp_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
