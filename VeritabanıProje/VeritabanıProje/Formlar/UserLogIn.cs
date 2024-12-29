using Npgsql;
using System;
using System.Windows.Forms;
using VeritabanıProje.Class;
using VeritabanıProje.Formlar;

namespace VeritabanıProje
{
    public partial class UserLogIn : Form
    {
        private readonly string connectionString = "Server=localhost; Port=5432; User Id=postgres; Password=123; Database=DatabaseProject;";

        public UserLogIn()
        {
            InitializeComponent();
            this.btnGiris.Click += btnGiris_Click;
            this.btnSignUp.Click += btnSignUp_Click;
        }

        private string KullaniciTuruBelirle(string email, string sifre)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    CASE 
                        WHEN EXISTS (SELECT 1 FROM public.ciftci c WHERE c.kullaniciid = k.kullaniciid) THEN 'Çiftçi'
                        ELSE 'Kullanıcı'
                    END AS KullaniciTuru
                FROM public.kullanici k
                WHERE k.email = @Email AND k.sifre = @Sifre";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("Email", email);
                    command.Parameters.AddWithValue("Sifre", sifre);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    return result?.ToString();
                }
            }
        }

        private int GetKullaniciID(string email, string sifre)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT kullaniciid FROM public.kullanici WHERE email = @Email AND sifre = @Sifre";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("Email", email);
                    command.Parameters.AddWithValue("Sifre", sifre);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        private int GetCiftciID(int kullaniciID)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT kullaniciid FROM public.ciftci WHERE kullaniciid = @KullaniciID";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("KullaniciID", kullaniciID);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                string sifre = txtSifre.Text.Trim();

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre))
                {
                    MessageBox.Show("E-mail ve şifre boş olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string kullaniciTuru = KullaniciTuruBelirle(email, sifre);

                if (kullaniciTuru == "Çiftçi")
                {
                    int kullaniciID = GetKullaniciID(email, sifre);
                    int ciftciID = GetCiftciID(kullaniciID);

                    CurrentFarmer.CiftciID = ciftciID;
                    CurrentFarmer.KullanıcıID = kullaniciID;

                    MessageBox.Show("Çiftçi olarak giriş yapıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FarmerForm farmerForm = new FarmerForm();
                    farmerForm.Show();
                    this.Hide();
                }
                else if (kullaniciTuru == "Kullanıcı")
                {
                    int kullaniciID = GetKullaniciID(email, sifre);
                    CurrentUser.KullanıcıID = kullaniciID;

                    MessageBox.Show("Kullanıcı olarak giriş yapıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MainPage mainPage = new MainPage(kullaniciID);
                    mainPage.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("E-mail veya şifre hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Giriş sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserLogIn_Load(object sender, EventArgs e)
        {
            btnGiris.Enabled = true;
            btnSignUp.Enabled = true;
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            UserSignUp userSignUp = new UserSignUp();
            userSignUp.Show();
            this.Hide();
        }
    }
}
