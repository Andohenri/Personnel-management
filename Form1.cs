using AppManagement.controllers;
using AppManagement.models;
using AppManagement.utils;
using AppManagement.views;
using Bunifu.UI.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppManagement
{
    public partial class Form1 : Form
    {
        private int id;
        private UserControlEmployee viewEmployee;
        private UserControlDecision viewDecision;
        private UserControlVacation viewVacation;
        private UserControlProbationer viewStage;
        private SplashScreen splashScreen;

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        public Form1(SplashScreen splashScreen)
        {
            InitializeComponent();
            Init();
            this.splashScreen = splashScreen;
        }

        private void Init()
        {
            bunifuPanel1.Visible = false;
            bunifuPanel2.Visible = false;
            bunifuPanel1.BringToFront();
            bunifuPanel2.BringToFront();

            lblError.Visible = false;
            btnEdit.Enabled = false;

            viewEmployee = new UserControlEmployee(this) { Dock = DockStyle.Fill };
            viewDecision = new UserControlDecision(this) { Dock = DockStyle.Fill };
            viewVacation = new UserControlVacation(this) { Dock = DockStyle.Fill };
            viewStage = new UserControlProbationer(this) { Dock = DockStyle.Fill };
            AcceptButton = viewEmployee.btnAdd;
            tabPage1.Controls.Add(viewEmployee);
            tabPage2.Controls.Add(viewDecision);
            tabPage3.Controls.Add(viewVacation);
            tabPage4.Controls.Add(viewStage);
            DisplayAttribution();
            DisplayFile();
        }
        private void DisplayAttribution()
        {
            var query = "SELECT * FROM attribution ORDER BY id_attribution DESC";
            Utils.Display(query, dataGridView1);
        }
        private void DisplayFile()
        {
            string query = "SELECT id_fichier, nom_fichier FROM fichier ORDER BY id_fichier DESC";
            Utils.Display(query, dataGridViewFile);
        }
        private void btn1_Click(object sender, EventArgs e)
        {
            AcceptButton = viewEmployee.btnAdd;
            viewEmployee.DisplayPersonnel();
            pages.SetPage(0);
        }
        private void btn2_Click(object sender, EventArgs e)
        {
            AcceptButton = viewDecision.btnAdd;
            viewDecision.DisplayDecision();
            pages.SetPage(1);
        }
        private void btn3_Click(object sender, EventArgs e)
        {
            AcceptButton = viewVacation.btnAdd;
            viewVacation.DisplayVacation();
            pages.SetPage(2);
        }
        private void btn4_Click(object sender, EventArgs e)
        {
            AcceptButton = viewStage.btnAdd;
            viewStage.DisplayProbationer();
            pages.SetPage(3);
        }
        private void btn6_Click(object sender, EventArgs e)
        {
            if (!bunifuPanel1.Visible)
            {
                bunifuPanel1.BringToFront();
                DisplayAttribution();
                btn6.IdleIconRightImage = Properties.Resources.chevron_up_30px;
                bunifuTransition1.ShowSync(bunifuPanel1, true, Bunifu.UI.WinForms.BunifuAnimatorNS.Animation.VertSlide);
            }
            else
            {
                btn6.IdleIconRightImage = Properties.Resources.chevron_down_30px;
                bunifuTransition1.HideSync(bunifuPanel1, true, Bunifu.UI.WinForms.BunifuAnimatorNS.Animation.VertSlide);
                Reset();
            }
        }
        private void btn7_Click(object sender, EventArgs e)
        {
            if (!bunifuPanel2.Visible)
            {
                bunifuPanel2.BringToFront();
                DisplayFile();
                btn7.IdleIconRightImage = Properties.Resources.chevron_up_30px;
                bunifuTransition1.ShowSync(bunifuPanel2, true, Bunifu.UI.WinForms.BunifuAnimatorNS.Animation.VertSlide);
            }
            else
            {
                btn7.IdleIconRightImage = Properties.Resources.chevron_down_30px;
                bunifuTransition1.HideSync(bunifuPanel2, true, Bunifu.UI.WinForms.BunifuAnimatorNS.Animation.VertSlide);
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            Attribution attribution = new Attribution(bunifuTextBox1.Text.Trim());
            try
            {
                AttributionController.Insert(attribution);
                notification.Show(this, "Attribution ajoutée avec succès.", BunifuSnackbar.MessageTypes.Success);
                Reset();
                DisplayAttribution();
                viewEmployee.FillAttribut();

            }
            catch (ErrorValidationException ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                notification.Show(this, "Une erreur s'est produite.", BunifuSnackbar.MessageTypes.Error);
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                try
                {
                    Attribution attribut = new Attribution
                    {
                        Id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[2].Value),
                        Attribut = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString()
                    };
                    AttributionController.Delete(attribut);
                    notification.Show(this, "Attribution supprimée.", BunifuSnackbar.MessageTypes.Success);
                    DisplayAttribution();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    notification.Show(this, "Une erreur s'est produite.", BunifuSnackbar.MessageTypes.Error);
                }

            }
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                bunifuTextBox1.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[2].Value);
                btnEdit.Enabled = true;
            }
        }
        private void btnEdit_Click(object sender, EventArgs e) 
        { 
            try
            {
                Attribution attribution = new Attribution(id, bunifuTextBox1.Text.Trim());
                AttributionController.Update(attribution);
                notification.Show(this, "Modification réussi.", BunifuSnackbar.MessageTypes.Success);
                Reset();
                DisplayAttribution();
            }
            catch (ErrorValidationException ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                notification.Show(this, "Une erreur s'est produit.", BunifuSnackbar.MessageTypes.Error);
            }
        }
        private void btnUpload_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Fichiers PDF (*.pdf)|*.pdf";
            openFileDialog1.Title = "Choisir un fichier PDF";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string nomFichier = openFileDialog1.SafeFileName;
                    byte[] contenu = System.IO.File.ReadAllBytes(openFileDialog1.FileName);
                    File file = new File(nomFichier, contenu);
                    FileController.Insert(file);
                    DisplayFile();
                    viewStage.FillDropDownFile();
                    notification.Show(this, "Fichier ajoutée avec succès.", BunifuSnackbar.MessageTypes.Success);
                }
                catch (Exception)
                {
                    notification.Show(this, "Une erreur s'est produite.", BunifuSnackbar.MessageTypes.Error);
                }

            }
        }
        private void dataGridViewFile_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                try
                {
                    if (FileController.DownloadFile(Convert.ToInt32(dataGridViewFile.Rows[e.RowIndex].Cells[2].Value))) { 
                        notification.Show(this, "Téléchargement réussi.", BunifuSnackbar.MessageTypes.Success);
                        DisplayFile();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    notification.Show(this, "Une erreur s'est produite.", BunifuSnackbar.MessageTypes.Error);
                }
            }   
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                try
                {
                    FileController.DeleteFile(Convert.ToInt32(dataGridViewFile.Rows[e.RowIndex].Cells[2].Value));
                    notification.Show(this, "Fichier supprimée.", BunifuSnackbar.MessageTypes.Success);
                    DisplayFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    notification.Show(this, "Une erruer s'est produite.", BunifuSnackbar.MessageTypes.Error);
                }
            }
        }
        private void Reset()
        {
            lblError.Visible = false;
            btnEdit.Enabled = false;
            bunifuTextBox1.Clear();
        }
        private void bunifuTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            splashScreen.Close();
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            splashScreen.Close();
        }
    }
}
