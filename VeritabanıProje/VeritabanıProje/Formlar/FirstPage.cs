using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;
using VeritabanıProje.Formlar;

namespace VeritabanıProje
{
    public partial class FirstPage : Form
    {
        public FirstPage()
        {
            InitializeComponent();
        }

        string StrConnection = "Server=localhost; Port=5432; User Id=postgres; Password=123; Database=proje;";
        NpgsqlConnection Con;
        NpgsqlCommand Cmd;

        private void connection()
        {
            try
            {
                Con = new NpgsqlConnection(StrConnection);

                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bağlantı hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable getData(string sql)
        {
            DataTable dt = new DataTable();

            try
            {
                connection();
                Cmd = new NpgsqlCommand(sql, Con);

                using (NpgsqlDataReader dr = Cmd.ExecuteReader())
                {
                    dt.Load(dr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri alma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (Con != null && Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }

            return dt;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                connection();
                MessageBox.Show("Veritabanı bağlantısı başarılı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Con.Close();
            }
            catch
            {
                MessageBox.Show("Veritabanına bağlanılamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DataTable dtgetdata = new DataTable();

            try
            {
                dtgetdata = getData("SELECT * FROM kazananlar;");
                if (dtgetdata.Rows.Count > 0)
                {
                    MessageBox.Show($"Toplam {dtgetdata.Rows.Count} kayıt getirildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Kayıt bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri getirme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFormGecis_Click(object sender, EventArgs e)
        {
            UserSignUp userSignUpForm = new UserSignUp();

            userSignUpForm.Show();

            this.Hide();
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            UserLogIn userLogIn = new UserLogIn();

            userLogIn.Show();

            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainPage mainPage = new MainPage(1);

            mainPage.Show();

            this.Hide();
        }
    }
}
