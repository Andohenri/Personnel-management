using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bunifu.UI.WinForms;
using AppManagement.utils;
using AppManagement.models;
using AppManagement.controllers;
using MySql.Data.MySqlClient;

namespace AppManagement.views
{
    public partial class UserControlDecision : UserControl
    {
        private readonly BunifuSnackbar notification;
        private readonly MySqlConnection con = DatabaseController.GetConnection();
        private readonly Form1 form;
        private bool validate = false;
        private int id;
        private int prevNbrJour;
        private string prevIM;
        private int idPersoDelete;
        private byte[] content;

        public UserControlDecision(Form1 form1)
        {
            InitializeComponent();
            form = form1;
            notification = form1.notification;
            Init();
        }
        private void Init()
        {
            Reset();
            DisplayDecision();
        }
        private void Reset()
        {
            pnlError.Visible = false;
            lblNumDecErr.Visible = lblIMErr.Visible = lblDayNbrErr.Visible = false;
            content = null;
            if (content == null)
            {
                label3.Visible = false;
            }
        }
        public void DisplayDecision()
        {
            string query = "SELECT id_decision, num_decision, nom, prenom, nbr_jour_acc, date_decision FROM decision d " +
                            "JOIN personnel p ON p.id_perso = d.id_perso " +
                            "JOIN info_perso i ON p.id_info = i.id_info ORDER BY d.id_decision DESC";
            Utils.Display(query, dataGridDecision);
        }
        private void btnBack_Click_1(object sender, EventArgs e)
        {
            bunifuPages1.SetPage(0);
            DisplayDecision();
            Reset();
            Utils.ClearAllTextBox(bunifuPanel3);
            btnAdd.Text = "Sauvegarder";
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage(1);
        }
        private void txtSearchDecision_TextChanged_1(object sender, EventArgs e)
        {
            string query = "SELECT id_decision, num_decision, nom, prenom, nbr_jour_acc, date_decision FROM decision d " +
                "JOIN personnel p ON p.id_perso = d.id_perso " +
                "JOIN info_perso i ON i.id_info = p.id_info " +
                "WHERE nom LIKE '%" + txtSearchDecision.Text + "%' " +
                "OR prenom LIKE '%" + txtSearchDecision.Text + "%' " +
                "OR immatricule LIKE '%" + txtSearchDecision.Text + "%' " +
                "OR num_decision LIKE '%" + txtSearchDecision.Text + "%' ";
            Utils.Display(query, dataGridDecision);
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (validate == false || lblNumDecErr.Visible == true || lblIMErr.Visible == true || lblDayNbrErr.Visible == true)
                {
                    throw new ErrorValidationException("Veuillez vérifier tous les conditions de validation avant la soumission.");
                }
                string numDecision = txtNumDecision.Text.Trim();
                string immatricule = txtIM.Text.Trim();
                string decisionDate = dateDecision.Value.ToString("yyyy-MM-dd");
                int nbrJourAcc = Convert.ToInt32(txtDayNbr.Text.Trim());
                string id_annee = dropDownYear.SelectedItem.ToString();
                byte[] proof = content;
                var decision = new Decision(numDecision, immatricule, decisionDate, nbrJourAcc, id_annee, proof);
                if (btnAdd.Text == "Sauvegarder")
                {
                    DecisionController.Insert(decision);
                    Utils.ClearAllTextBox(bunifuPanel1);
                    notification.Show(form, "Décision de congé ajoutée avec succès.", BunifuSnackbar.MessageTypes.Success);
                }
                else
                {
                    DecisionController.Update(decision, id, prevNbrJour, prevIM);
                    notification.Show(form, "Décision de congé modifiée.", BunifuSnackbar.MessageTypes.Success);
                }
                Reset();
                DisplayDecision();
            }
            catch (FormatException)
            {
                lblError.Text = "Veuillez vérifier vos donnés entrants ou selectionés.";
                pnlError.Visible = true;
                notification.Show(form, "Une erreur s'est produite.", BunifuSnackbar.MessageTypes.Error);
            }

            catch (ErrorValidationException ex)
            {
                lblError.Text = ex.Message;
                pnlError.Visible = true;
                notification.Show(form, "Une erreur s'est produite.", BunifuSnackbar.MessageTypes.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur Insert:" + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                notification.Show(form, "Une erreur s'est produite.", BunifuSnackbar.MessageTypes.Error);
            }
        }
        private void dataGridDecision_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                try
                {
                    if (DecisionController.DownloadProof(Convert.ToInt32(dataGridDecision.Rows[e.RowIndex].Cells[3].Value)))
                    {
                        notification.Show(form, "Téléchargement réussi.", BunifuSnackbar.MessageTypes.Success);
                    }
                }
                catch (Exception ex)
                {
                    notification.Show(form, "Erreur de téléchargement.", BunifuSnackbar.MessageTypes.Error);
                }
            }
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                btnAdd.Text = "Modifier";
                id = Convert.ToInt32(dataGridDecision.Rows[e.RowIndex].Cells[3].Value);
                prevNbrJour = Convert.ToInt32(dataGridDecision.Rows[e.RowIndex].Cells[7].Value);
                LocationEditionPage(id, bunifuPages1);
            }
            if(e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                try
                {
                    id = Convert.ToInt32(dataGridDecision.Rows[e.RowIndex].Cells[3].Value);
                    prevNbrJour = prevNbrJour = Convert.ToInt32(dataGridDecision.Rows[e.RowIndex].Cells[7].Value);
                    using MySqlCommand cmd = new MySqlCommand($"SELECT id_perso FROM decision WHERE id_decision = {id}", con);
                    con.Open();
                    using MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        idPersoDelete = Convert.ToInt32(reader.GetString("id_perso"));
                    }
                    con.Close();
                    DecisionController.Delete(id, prevNbrJour, idPersoDelete);
                    notification.Show(form, "Décsion de congé supprimée.", BunifuSnackbar.MessageTypes.Success);
                    DisplayDecision();
                }
                catch (Exception ex)
                {
                    notification.Show(form, "Une erreur s'est produite.", BunifuSnackbar.MessageTypes.Error);
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void LocationEditionPage(int id, BunifuPages bunifuPage)
        {
            FillInputs(id);
            bunifuPage.SetPage(1);
        }
        private void FillInputs(int id)
        {
            try
            {
                var con = DatabaseController.GetConnection();
                string query = $"SELECT num_decision, immatricule, nbr_jour_acc, date_decision, id_annee, preuve FROM decision d " +
                    $"JOIN personnel p ON  d.id_perso = p.id_perso WHERE d.id_decision = {id}";
                using MySqlCommand cmd = new MySqlCommand(query, con);
                con.Open();
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    txtNumDecision.Text = reader.GetString("num_decision");
                    txtIM.Text = prevIM = reader.GetString("immatricule");
                    txtDayNbr.Text = reader.GetString("nbr_jour_acc");
                    dateDecision.Value = Utils.SetDate(reader.GetString("date_decision"));
                    dropDownYear.SelectedItem = reader.GetString("id_annee");
                    content = (byte[])reader["preuve"];
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                con.Close();
            }
        }
        private void txtNumDecision_TextChanged(object sender, EventArgs e)
        {
            //TO DO
            string pattern = @"^$|^[0-9]+$";
            if (Utils.IsMatch(txtNumDecision, pattern))
            {
                lblNumDecErr.Visible = false;
                txtNumDecision.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblNumDecErr.Visible = true;
                txtNumDecision.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblNumDecErr.Text = "Entrer un numéro valide";
                validate = false;
            }
        }
        private void txtIM_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^[0-9]+$";
            if (Utils.IsMatch(txtIM, pattern))
            {
                lblIMErr.Visible = false;
                txtIM.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblIMErr.Visible = true;
                txtIM.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblIMErr.Text = "Entrer un immatricule valide";
                validate = false;
            }
        }
        private void txtDayNbr_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^[0-9]+$";
            if (Utils.IsMatch(txtDayNbr, pattern))
            {
                lblDayNbrErr.Visible = false;
                txtDayNbr.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblDayNbrErr.Visible = true;
                txtDayNbr.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblDayNbrErr.Text = "Entrer un nombre de jours";
                validate = false;
            }
        }
        private void txtIM_Leave(object sender, EventArgs e)
        {
            try
            {
                if (Utils.IsFound(Convert.ToInt32(txtIM.Text)))
                {
                    lblIMErr.Visible = false;
                    txtIM.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                    validate = true;
                }
                else
                {
                    lblIMErr.Visible = true;
                    txtIM.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                    lblIMErr.Text = "Entrer un matricule qui existe";
                    validate = false;
                }
            }
            catch (FormatException)
            {
                lblIMErr.Visible = true;
                txtIM.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblIMErr.Text = "Ce champ doit comporter seulement des chiffres";
                validate = false;
            }
        }
        private void UserControlDecision_Load(object sender, EventArgs e)
        {
            try
            {
                int current_year = DateTime.Now.Year;
                int year = 0;
                var con = DatabaseController.GetConnection();
                MySqlCommand cmd = new MySqlCommand("SELECT id_annee FROM annee ORDER BY id_annee DESC LIMIT 1", con);
                con.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        year = Convert.ToInt32(reader.GetString("id_annee"));
                    }
                    reader.Close();
                }

                if (year < current_year)
                {
                    MySqlCommand cmd1 = new MySqlCommand($"INSERT INTO annee (id_annee) VALUES ({current_year})", con);
                    cmd1.ExecuteNonQuery();
                }

                dropDownYear.Items.Clear();
                con.Close();
                FillDropDown.FillYear(dropDownYear);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void UserControlDecision_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAdd.Focus();
                btnAdd_Click(btnAdd, e);
                e.Handled = true;
            }
        }
        private void BtnProof_Click(object sender, EventArgs e)
        {
            var form = new Form1();
            form.openFileDialog1.Filter = "Fichiers PDF (*.pdf)|*.pdf";
            form.openFileDialog1.Title = "Choisir un fichier PDF";
            if (form.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                content = System.IO.File.ReadAllBytes(form.openFileDialog1.FileName);
                label3.Visible = true;
            }
        }
    }
}
