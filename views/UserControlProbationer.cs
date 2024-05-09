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
using AppManagement.models;
using AppManagement.controllers;
using AppManagement.utils;
using MySql.Data.MySqlClient;

namespace AppManagement.views
{
    public partial class UserControlProbationer : UserControl
    {
        private readonly Form1 form;
        private readonly BunifuSnackbar notification;
        private int id;
        private bool validate = false;

        public UserControlProbationer(Form1 form)
        {
            InitializeComponent();
            this.form = form;
            this.notification = form.notification;
            Init();
        }
        private void Init()
        {
            Reset();
            FillEncadreur();
            FillDropDownFile();
        }
        public void FillEncadreur()
        {
            FillDropDown.FillEncadreur(dropDownEncadreur);
        }
        public void FillDropDownFile()
        {
            FillDropDown.FillFile(dropDownFile);
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage(0);
            btnAdd.Text = "Sauvegarder";
            DisplayProbationer();
            Reset();
            Utils.ClearAllTextBox(bunifuPanel3);
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            Reset();
            bunifuPages1.SetPage(1);
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (Utils.IsNullOrEmptyTextBox(bunifuPanel3))
                {
                    throw new ErrorValidationException("Tous les champs ne doivent pas être vide, veuillez corriger les erreurs.");
                }
                if (validate == false || lblContactErr.Visible == true || lblEtablErr.Visible == true || lblFiliereErr.Visible == true || lblFirstNameErr.Visible == true || lblNameErr.Visible == true || lblNiveauErr.Visible == true || lblNoteErr.Visible == true || lblThemeErr.Visible == true || lblDurationErr.Visible == true)
                {
                    throw new ErrorValidationException("Veuillez vérifier tous les conditions de validation avant la soumission");
                }
                string nom = txtName.Text.Trim();
                string prenom = txtFirstName.Text.Trim();
                string contact = txtContact.Text.Trim();
                string filiere = txtFiliere.Text.Trim();
                string niveau = txtNiveau.Text.Trim();
                string etablissement = txtEtabl.Text.Trim();
                var info_stagiaire = new InfoProbationer(nom, prenom, contact, filiere, niveau, etablissement);

                int idFile = Convert.ToInt32(dropDownFile.SelectedValue.ToString());
                string dateEnter = dateStart.Value.ToString("yyyy-MM-dd");
                int duration = Convert.ToInt32(txtDuration.Text.Trim());
                int encadreur = Convert.ToInt32(dropDownEncadreur.SelectedValue.ToString());
                string theme = txtTheme.Text.Trim();
                string note = txtNote.Text.Trim();
                var probationer = new Probationer(idFile, encadreur, dateEnter, duration, theme, note, info_stagiaire);

                if (btnAdd.Text == "Sauvegarder")
                {
                    ProbationerController.Insert(probationer);
                    Utils.ClearAllTextBox(bunifuPanel3);
                    notification.Show(form, "Stagiaire ajouté(e) avec succès.", BunifuSnackbar.MessageTypes.Success);
                }
                else
                {
                    ProbationerController.Update(probationer, id);
                    notification.Show(form, "Stagiaire modifié(e).", BunifuSnackbar.MessageTypes.Success);
                }
                DisplayProbationer();
                Reset();
            }
            catch (FormatException)
            {
                lblError.Text = "Veuillez bien vérifier vos choix sur les listes déroulantes.";
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
        private void Reset()
        {
            pnlError.Visible = false;
            lblContactErr.Visible = lblEtablErr.Visible = lblFiliereErr.Visible = lblFirstNameErr.Visible = lblNameErr.Visible = lblNiveauErr.Visible = lblNoteErr.Visible = lblThemeErr.Visible = lblDurationErr.Visible = false;
        }
        public void DisplayProbationer()
        {
            string query = "SELECT id_stagiaire, nom_stagiaire, prenom_stagiaire, filiere, niveau, duration, CONCAT(nom, ' ', prenom) AS name, theme, note, nom_fichier FROM stagiaire s " +
                            "JOIN personnel p ON s.id_perso = p.id_perso " +
                            "JOIN info_perso ip ON p.id_info = ip.id_info " +
                            "JOIN info_stagiaire i ON i.id_info_stagiaire = s.id_info_stagiaire " +
                            "JOIN fichier f ON f.id_fichier = s.id_fichier " +
                            "ORDER BY s.id_stagiaire DESC";
            Utils.Display(query, dataGridProbationer);
        }
        private void dataGridProbationer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                btnAdd.Text = "Modifier";
                id = Convert.ToInt32(dataGridProbationer.Rows[e.RowIndex].Cells[2].Value);
                FillInputs(id);
                bunifuPages1.SetPage(1);
            }
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                try
                {
                    id = Convert.ToInt32(dataGridProbationer.Rows[e.RowIndex].Cells[2].Value);
                    ProbationerController.Delete(id);
                    notification.Show(form, "Stagiaire supprimé(e)", BunifuSnackbar.MessageTypes.Success);
                    DisplayProbationer();
                }
                catch (Exception ex)
                {
                    notification.Show(form, "Une erreur s'est produite.", BunifuSnackbar.MessageTypes.Error);
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void FillInputs(int id)
        {
            var con = DatabaseController.GetConnection();
            string query = $"SELECT nom_stagiaire, f.id_fichier, s.id_perso, prenom_stagiaire, filiere, niveau, etablissement, i.contact, duration, date_debut_stage, theme, note FROM stagiaire s " +
                            "JOIN personnel p ON s.id_perso = p.id_perso " +
                            "JOIN info_perso ip ON p.id_info = ip.id_info " +
                            "JOIN info_stagiaire i ON i.id_info_stagiaire = s.id_info_stagiaire " +
                            "JOIN fichier f ON s.id_fichier = f.id_fichier " +
                            $"WHERE s.id_stagiaire = {id}";
            using MySqlCommand cmd = new MySqlCommand(query, con);
            con.Open();
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                txtName.Text = reader.GetString("nom_stagiaire");
                txtFirstName.Text = reader.GetString("prenom_stagiaire");
                txtFiliere.Text = reader.GetString("filiere");
                txtNiveau.Text = reader.GetString("niveau");
                txtEtabl.Text = reader.GetString("etablissement");
                txtContact.Text = reader.GetString("contact");
                int j = reader.GetInt32("id_perso");
                dropDownEncadreur.SelectedValue = j;
                txtDuration.Text = reader.GetString("duration");
                dateStart.Value = Utils.SetDate(reader.GetString("date_debut_stage"));
                txtTheme.Text = reader.GetString("theme");
                txtNote.Text = reader.GetString("note");
                int i = reader.GetInt32("id_fichier");
                dropDownFile.SelectedValue = i;
            }
            con.Close();
        }
        private void txtSearchVacation_TextChanged(object sender, EventArgs e)
        {
            string query = "SELECT id_stagiaire, nom_stagiaire, prenom_stagiaire, filiere, niveau, duration, CONCAT(nom, ' ', prenom) AS name, theme, note, nom_fichier FROM stagiaire s " +
                            "JOIN personnel p ON s.id_perso = p.id_perso " +
                            "JOIN info_perso ip ON p.id_info = ip.id_info " +
                            "JOIN info_stagiaire i ON i.id_info_stagiaire = s.id_info_stagiaire " +
                            "JOIN fichier f ON f.id_fichier = s.id_fichier " +
                            "WHERE nom_stagiaire LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR prenom_stagiaire LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR filiere LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR niveau LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR duration LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR nom LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR prenom LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR theme LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR note LIKE '%" + txtSearchVacation.Text + "%' " +
                            "OR nom_fichier LIKE '%" + txtSearchVacation.Text + "%' " +
                            "ORDER BY s.id_stagiaire DESC";
            Utils.Display(query, dataGridProbationer);
        }
        private void UserControlProbationer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAdd.Focus();
                btnAdd_Click(btnAdd, e);
                e.Handled = true;
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^[A-Za-z\s'\-]+$";
            if (Utils.IsMatch(txtName, pattern))
            {
                lblNameErr.Visible = false;
                txtName.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblNameErr.Visible = true;
                txtName.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblNameErr.Text = "Entrer un nom valide";
                validate = false;
            }
        }
        private void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^[A-Za-z\s'\-]+$";
            if (Utils.IsMatch(txtFirstName, pattern))
            {
                lblFirstNameErr.Visible = false;
                txtFirstName.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblFirstNameErr.Visible = true;
                txtFirstName.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblFirstNameErr.Text = "Entrer un prenom valide";
                validate = false;
            }
        }
        private void txtContact_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^\d{1,10}$";
            if (Utils.IsMatch(txtContact, pattern))
            {
                lblContactErr.Visible = false;
                txtContact.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblContactErr.Visible = true;
                txtContact.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblContactErr.Text = "Entrer un contact valide";
                validate = false;
            }
        }
        private void txtEtabl_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^[A-Za-z0-9\s-]+$";
            if (Utils.IsMatch(txtEtabl, pattern))
            {
                lblEtablErr.Visible = false;
                txtEtabl.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblEtablErr.Visible = true;
                txtEtabl.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblEtablErr.Text = "Entrer un établissement d'étude valide";
                validate = false;
            }
        }
        private void txtFiliere_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^[A-Za-z0-9\s-]+$";
            if (Utils.IsMatch(txtFiliere, pattern))
            {
                lblFiliereErr.Visible = false;
                txtFiliere.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblFiliereErr.Visible = true;
                txtFiliere.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblFiliereErr.Text = "Entrer un filière d'étude valide";
                validate = false;
            }
        }
        private void txtNiveau_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^[LM]\d$";
            if (Utils.IsMatch(txtNiveau, pattern))
            {
                lblNiveauErr.Visible = false;
                txtNiveau.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblNiveauErr.Visible = true;
                txtNiveau.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblNiveauErr.Text = "Entrer un niveau d'étude valide";
                validate = false;
            }
        }
        private void txtDuration_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^\d{1,2}$";
            if (Utils.IsMatch(txtDuration, pattern))
            {
                lblDurationErr.Visible = false;
                txtDuration.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblDurationErr.Visible = true;
                txtDuration.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblDurationErr.Text = "Entrer un nombre de mois valide";
                validate = false;
            }
        }
        private void txtTheme_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^[A-Za-z0-9\s'\-]+$";
            if (Utils.IsMatch(txtTheme, pattern))
            {
                lblThemeErr.Visible = false;
                txtTheme.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblThemeErr.Visible = true;
                txtTheme.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblThemeErr.Text = "Entrer un thème valide";
                validate = false;
            }
        }
        private void txtNote_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^(?:1?[0-9]|20)$";
            if (Utils.IsMatch(txtNote, pattern))
            {
                lblNoteErr.Visible = false;
                txtNote.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblNoteErr.Visible = true;
                txtNote.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblNoteErr.Text = "Le note doit être sur 20";
                validate = false;
            }
        }
    }
}