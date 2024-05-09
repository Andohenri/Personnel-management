using AppManagement.controllers;
using MySql.Data.MySqlClient;
using System;               
using System.Windows.Forms;

namespace AppManagement
{
    public partial class SplashScreen : Form
    {
        public MySqlConnection Con { get; set; }

        public SplashScreen()
        {
            InitializeComponent();
            LblError.Visible = false;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            bunifuPanel2.Width += 69;
            if (bunifuPanel2.Width >= 690)
            {
                try
                {
                    Con = DatabaseController.GetConnection();
                    Con.Open();
                    Con.Close();
                    Timer1.Stop();
                    bunifuPages1.SetPage(1);
                }
                catch (Exception)
                {
                    Timer1.Stop();
                    Con.Close();
                    MessageBox.Show("Veuillez verifier votre connexion au base de donnée ou redémarrer le serveur.\nPuis relancer l'application.", "Erreur de connexion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
            }
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            Timer1.Start();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string pseudo = TxtUsername.Text;
            string pwd = TxtPassword.Text;
            BtnLogin.Text = "Chargement...";
            try
            {
                if (LoginController.Authentification(pseudo, pwd))
                {
                    Form1 f = new Form1(this);
                    f.Show();
                    this.Hide();
                    BtnLogin.Text = "Se connecter";
                }
                else
                {
                    TxtPassword.Clear();
                    TxtUsername.Clear();
                    BtnLogin.Text = "Se connecter";
                    LblError.Visible = true;
                    LblError.Text = "Nom d'utilisateur ou mot de passe incorrect.";
                }
            }
            catch (MissingFieldException ex)
            {
                LblError.Visible = true;
                LblError.Text = ex.Message;
                BtnLogin.Text = "Se connecter";
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
