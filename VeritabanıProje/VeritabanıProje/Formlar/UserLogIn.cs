using Npgsql;
using System;
using System.Windows.Forms;
using VeritabanıProje.Class;
using VeritabanıProje.Formlar;

namespace VeritabanıProje
{
    public partial class UserLogIn : Form
    {
        string connectionString = "Server=localhost; Port=5432; User Id=postgres; Password=123; Database=proje;";

        public UserLogIn()
        {
            InitializeComponent();
            this.btnGiris.Click += new System.EventHandler(this.btnGiris_Click);
            this.btnSignUp.Click += new System.EventHandler(this.btnSignUp_Click);
        }

        private string KullaniciTuruBelirle(string email, string sifre)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string queryKullanici = "SELECT kullaniciid FROM public.kullanici WHERE email = @Email AND sifre = @Sifre";
                NpgsqlCommand commandKullanici = new NpgsqlCommand(queryKullanici, connection);
                commandKullanici.Parameters.AddWithValue("Email", email);
                commandKullanici.Parameters.AddWithValue("Sifre", sifre);

                connection.Open();
                object kullaniciIDObj = commandKullanici.ExecuteScalar();
                connection.Close();

                // DEBUGGING MESAJI
                if (kullaniciIDObj != null)
                {
                    MessageBox.Show($"Kullanıcı ID bulundu: {kullaniciIDObj.ToString()}", "Debug");
                }
                else
                {
                    MessageBox.Show("Kullanıcı bulunamadı.", "Debug");
                }

                if (kullaniciIDObj != null)
                {
                    int kullaniciID = Convert.ToInt32(kullaniciIDObj);

                    string queryCiftci = "SELECT COUNT(*) FROM public.ciftci WHERE kullaniciid = @KullaniciID";
                    NpgsqlCommand commandCiftci = new NpgsqlCommand(queryCiftci, connection);
                    commandCiftci.Parameters.AddWithValue("KullaniciID", kullaniciID);

                    connection.Open();
                    int ciftciCount = Convert.ToInt32(commandCiftci.ExecuteScalar());
                    connection.Close();

                    MessageBox.Show($"Çiftçi kayıt sayısı: {ciftciCount}", "Debug");

                    if (ciftciCount > 0)
                    {
                        return "Çiftçi"; 
                    }
                    else
                    {
                        return "Kullanıcı"; 
                    }
                }

                return null; 
            }
        }



        private int GetCiftciID(string email, string sifre)
        {
            int ciftciID = 0;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT kullaniciid FROM public.ciftci WHERE kullaniciid = (SELECT kullaniciid FROM public.kullanici WHERE email = @Email AND sifre = @Sifre)";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("Email", email);
                command.Parameters.AddWithValue("Sifre", sifre);

                connection.Open();
                object result = command.ExecuteScalar();
                connection.Close();

                if (result != null)
                {
                    ciftciID = Convert.ToInt32(result);
                }
            }

            return ciftciID;
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
                    MessageBox.Show("Çiftçi olarak giriş yapıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    int ciftciID = GetCiftciID(email, sifre);
                    int kullaniciID = GetKullaniciID(email, sifre);

                    CurrentFarmer.CiftciID = ciftciID; 
                    CurrentFarmer.KullanıcıID = kullaniciID; 

                    FarmerForm farmerForm = new FarmerForm();
                    farmerForm.Show();
                    this.Hide();
                }
                else if (kullaniciTuru == "Kullanıcı")
                {
                    MessageBox.Show("Kullanıcı olarak giriş yapıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    int kullaniciID = GetKullaniciID(email, sifre);

                    CurrentUser.KullanıcıID = kullaniciID; 

                    MainPage mainForm = new MainPage(kullaniciID);
                    mainForm.Show();
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


        private int GetKullaniciID(string email, string sifre)
        {
            int kullaniciID = 0;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string query = "SELECT kullaniciid FROM public.kullanici WHERE email = @Email AND sifre = @Sifre";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("Email", email);
                command.Parameters.AddWithValue("Sifre", sifre);

                connection.Open();
                object result = command.ExecuteScalar();  
                connection.Close();

                if (result != null)
                {
                    kullaniciID = Convert.ToInt32(result);  
                }
            }

            return kullaniciID; 
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

        private void btnSignUp_Click_1(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
