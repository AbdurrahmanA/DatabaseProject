using Npgsql;
using System;
using System.Windows.Forms;
using VeritabanıProje.Class;
using VeritabanıProje.Formlar;

namespace VeritabanıProje
{
    public partial class UserLogIn : Form
    {
        string StrConnection = "Server=localhost; Port=5432; User Id=postgres; Password=123; Database=DatabaseProject;";

        public UserLogIn()
        {
            InitializeComponent();
            this.btnGiris.Click += new System.EventHandler(this.btnGiris_Click);
        }

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

        private int GetKullaniciID(string email, string sifre)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(StrConnection))
            {
                string query = "SELECT kullaniciid FROM public.kullanici WHERE email = @Email AND sifre = @Sifre";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("Email", email);
                command.Parameters.AddWithValue("Sifre", sifre);

                connection.Open();
                object result = command.ExecuteScalar();
                connection.Close();

                return result != null ? Convert.ToInt32(result) : 0;
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

                int kullaniciID = GetKullaniciID(email, sifre);

                if (kullaniciID == 0)
                {
                    MessageBox.Show("E-mail veya şifre hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                CurrentUser.KullanıcıID = kullaniciID;

                string kullaniciTuru = KullaniciTuruBelirle(kullaniciID);

                if (kullaniciTuru == "Çiftçi")
                {
                    MessageBox.Show("Çiftçi olarak giriş yapıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    FarmerProductBuy farmerForm = new FarmerProductBuy(CurrentUser.KullanıcıID); 
                    farmerForm.Show();
                    this.Hide();
                }
                else if (kullaniciTuru == "Kullanıcı")
                {
                    MessageBox.Show("Kullanıcı olarak giriş yapıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UserProductBuy mainPage = new UserProductBuy(CurrentUser.KullanıcıID); 
                    mainPage.Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Giriş sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnSignUp_Click(object sender, EventArgs e)
        {
            UserSignUp userSignUp = new UserSignUp();
            userSignUp.Show();
            this.Hide();
        }

        private void UserLogIn_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void label5_Click(object sender, EventArgs e)
        {
            UserSignUp firstPage = new UserSignUp();

            firstPage.Show();

            this.Hide();
        }
    }
}
