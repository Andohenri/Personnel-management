using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppManagement.controllers;
using MySql.Data.MySqlClient;
using Bunifu.UI.WinForms;
using AppManagement.models;
using AppManagement.utils;

namespace AppManagement.views
{
    public partial class UserControlVacation : UserControl
    {
        private readonly MySqlConnection con = DatabaseController.GetConnection();
        private readonly BunifuSnackbar notification;
        private readonly Form1 form;
        private bool validate = false;
        private int id;
        private int prevNbrJour;
        private int idPersoOnDelete;
        private string prevIM;

        public UserControlVacation(Form1 form1)
        {
            InitializeComponent();
            form = form1;
            notification = form1.notification;
            Init();
        }

        private void Init()
        {
            Reset();
            DisplayVacation();
            FillDropDown.FillInterim(dropDownInterim);
        }
        private void btnNew_Click_1(object sender, EventArgs e)
        {
            bunifuPages1.SetPage(1);
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            DisplayVacation();
            bunifuPages1.SetPage(0);
            Utils.ClearAllTextBox(bunifuPanel3);
            Reset();
            btnAdd.Text = "Sauvegarder";
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (validate == false || lblPlaceErr.Visible == true || lblIMErr.Visible == true || lblDayNbrErr.Visible == true)
                {
                    throw new ErrorValidationException("Veuillez vérifier tous les conditions de validation avant la soumission");
                }
                string immatricule = txtIM.Text.Trim();
                string startDate = dateStart.Value.ToString("yyyy-MM-dd");
                int nbrJour = Convert.ToInt32(txtDayNbr.Text);
                string endDate = dateStart.Value.AddDays(nbrJour).ToString("yyyy-MM-dd");
                string motif = dropDownMotif.Text;
                string interim = dropDownInterim.Text;
                string lieu = txtPlace.Text.Trim();
                var vacation = new Vacation(immatricule, startDate, nbrJour, endDate, motif, interim, lieu);
                
                if (btnAdd.Text == "Sauvegarder")
                {
                    VacationController.Insert(vacation);
                    Utils.ClearAllTextBox(bunifuPanel3);
                    notification.Show(form, "Congé pris avec succès", BunifuSnackbar.MessageTypes.Success);
                }
                else
                {
                    VacationController.Update(vacation, id, prevIM, prevNbrJour);
                    notification.Show(form, "Congé modifié", BunifuSnackbar.MessageTypes.Success);
                }
                DisplayVacation();
                Reset();
            }
            catch (FormatException)
            {
                lblError.Text = "Veuillez bien verifier vos données entrant ou selectionné";
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
                MessageBox.Show(ex.Message);
                notification.Show(form, "Une erreur s'est produite.", BunifuSnackbar.MessageTypes.Error);
            }
        }
        private void dataGridVacation_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                btnAdd.Text = "Modifier";
                id = Convert.ToInt32(dataGridVacation.Rows[e.RowIndex].Cells[2].Value);
                prevNbrJour = Convert.ToInt32(dataGridVacation.Rows[e.RowIndex].Cells[5].Value);
                LocationEditionPage(id, bunifuPages1);
            }
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                try
                {
                    id = Convert.ToInt32(dataGridVacation.Rows[e.RowIndex].Cells[2].Value);
                    prevNbrJour = Convert.ToInt32(dataGridVacation.Rows[e.RowIndex].Cells[5].Value);
                    string motif = dataGridVacation.Rows[e.RowIndex].Cells[7].Value.ToString();
                    using MySqlCommand cmd = new MySqlCommand($"SELECT id_perso FROM conge WHERE id_conge = {id}", con);
                    con.Open();
                    using MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        idPersoOnDelete = Convert.ToInt32(reader.GetString("id_perso"));
                    }
                    con.Close();
                    VacationController.Delete(id, prevNbrJour, idPersoOnDelete, motif);
                    notification.Show(form, "Congé supprimé.", BunifuSnackbar.MessageTypes.Success);
                    DisplayVacation();
                }
                catch (Exception ex)
                {
                    notification.Show(form, "Une erreur s'est produite.", BunifuSnackbar.MessageTypes.Error);
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void DisplayVacation()
        {
            string query = "SELECT id_conge, nom, prenom, nbr_jour, date_debut, date_fin, motif, remplacent, lieu_conge, etat_conge FROM conge c " +
                            "JOIN personnel p ON p.id_perso = c.id_perso " +
                            "JOIN info_perso i ON p.id_info = i.id_info ORDER BY c.id_conge DESC";
            Utils.Display(query, dataGridVacation);
        }
        private void LocationEditionPage(int id, BunifuPages bunifuPage)
        {
            FillInputs(id);
            bunifuPage.SetPage(1);
        }
        private void FillInputs(int id)
        {
            var con = DatabaseController.GetConnection();
            string query = $"SELECT immatricule, nbr_jour, date_debut, motif, remplacent, lieu_conge FROM conge c " +
                $"JOIN personnel p ON  c.id_perso = p.id_perso WHERE c.id_conge = {id}";
            using MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                txtIM.Text = prevIM = reader.GetString("immatricule");
                dateStart.Value = Utils.SetDate(reader.GetString("date_debut"));
                txtDayNbr.Text = reader.GetString("nbr_jour");
                dropDownMotif.SelectedItem = reader.GetString("motif");
                dropDownInterim.SelectedItem = reader.GetString("remplacent");
                txtPlace.Text = reader.GetString("lieu_conge");
            }
            con.Close();
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
        private void txtNumPlace_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^[a-zA-Z]+$";
            if (Utils.IsMatch(txtPlace, pattern))
            {
                lblPlaceErr.Visible = false;
                txtPlace.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblPlaceErr.Visible = true;
                txtPlace.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblPlaceErr.Text = "Entrer un lieu valide";
                validate = false;
            }
        }
        private void dataGridVacation_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 11 && e.RowIndex >= 0)
            {
                int id = Convert.ToInt32(dataGridVacation.Rows[e.RowIndex].Cells[2].Value);
                DateTime startDate = Utils.SetDate(dataGridVacation.Rows[e.RowIndex].Cells[6].Value.ToString());
                DateTime endDate = Utils.SetDate(dataGridVacation.Rows[e.RowIndex].Cells[7].Value.ToString());
                if (startDate <= DateTime.Now)
                {
                    e.Value = "En cours";
                    VacationController.UpdateStatut("En cours", id);
                }
                if (endDate < DateTime.Now)
                {
                    e.Value = "Terminé";
                    VacationController.UpdateStatut("Terminé", id);
                }
            }
        }
        private void Reset()
        {
            pnlError.Visible = false;
            lblPlaceErr.Visible = lblIMErr.Visible = lblDayNbrErr.Visible = false;
        }
        private void txtSearchVacation_TextChanged(object sender, EventArgs e)
        {
            string query = "SELECT id_conge, nom, prenom, nbr_jour, date_debut, date_fin, motif, remplacent, lieu_conge, etat_conge FROM conge c " +
                            "JOIN personnel p ON p.id_perso = c.id_perso " +
                            "JOIN info_perso i ON p.id_info = i.id_info " +
                            "WHERE nom LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR prenom LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR motif LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR etat_conge LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR lieu_conge LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR remplacent LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR date_debut LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR date_fin LIKE '%" + txtSearchVacation.Text + "%' ORDER BY c.id_conge DESC ";
            Utils.Display(query, dataGridVacation);
        }
        private void UserControlVacation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAdd.Focus();
                btnAdd_Click(btnAdd, e);
                e.Handled = true;
            }
        }
    }
}
