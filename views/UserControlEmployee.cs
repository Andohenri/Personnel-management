using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppManagement.models;
using AppManagement.controllers;
using AppManagement.utils;
using Bunifu.UI.WinForms;
using MySql.Data.MySqlClient;

namespace AppManagement.views
{
    public partial class UserControlEmployee : UserControl
    {
        private readonly BunifuSnackbar notification;
        private readonly Form1 form;
        private bool validate = false;
        private int id;

        public UserControlEmployee(Form1 form1)
        {
            InitializeComponent();
            form = form1;
            notification = form1.notification;
            Init();
        }

        private void Init()
        {
            Reset();
            DisplayPersonnel();
            FillAttribut();
        }
        private void Reset()
        {
            pnlError.Visible = false;
            lblNameErr.Visible = lblFirstNameErr.Visible = lblAddressErr.Visible = lblContactErr.Visible = lblNumCINErr.Visible = lblLocationErr.Visible = lblDuplicateErr.Visible = lblIMErr.Visible = lblRegionErr.Visible = false;
        }
        public void FillAttribut()
        {
            FillDropDown.FillAttribut(dropDownAttribute);
        }
        public void DisplayPersonnel()
        {
            string query = "SELECT id_perso, immatricule, nom, prenom, attribut, statut, region, nbr_jour_conge FROM personnel p JOIN info_perso i ON p.id_info = i.id_info JOIN attribution a ON a.id_attribution = p.id_attribution ORDER BY id_perso DESC";
            Utils.Display(query, dataGridPersonnel);
        }
        private void txtSearchPersonnel_TextChanged_1(object sender, EventArgs e)
        {
            string query = "SELECT id_perso, immatricule, nom, prenom, attribut, statut, region, nbr_jour_conge FROM personnel p JOIN info_perso i ON p.id_info = i.id_info JOIN attribution a ON a.id_attribution = p.id_attribution " +
                "WHERE nom LIKE '%" + txtSearchPersonnel.Text + "%' " +
                "OR prenom LIKE '%" + txtSearchPersonnel.Text + "%' " +
                "OR immatricule LIKE '%" + txtSearchPersonnel.Text + "%' " +
                "OR attribut LIKE '%" + txtSearchPersonnel.Text + "%' " +
                "OR region LIKE '%" + txtSearchPersonnel.Text + "%' " +
                "OR statut LIKE '%" + txtSearchPersonnel.Text + "%' " +
                "ORDER BY id_perso DESC";
            Utils.Display(query, dataGridPersonnel);
        }
        private void btnNew_Click_1(object sender, EventArgs e)
        {
            FillDropDown.FillAttribut(dropDownAttribute);
            bunifuPages1.SetPage(1);
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage(0);
            DisplayPersonnel();
            Utils.ClearAllTextBox(bunifuPanel3);
            Reset();
            btnAdd.Text = "Sauvegarder";
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (Utils.IsNullOrEmptyTextBox(bunifuPanel3))
                {
                    throw new ErrorValidationException("Tous les champs ne doivent pas être vide, veuillez corriger les erreurs.");
                }
                if (validate == false || lblNameErr.Visible == true || lblFirstNameErr.Visible == true || lblAddressErr.Visible == true || lblContactErr.Visible ==true || lblNumCINErr.Visible == true || lblLocationErr.Visible==true || lblDuplicateErr.Visible==true || lblIMErr.Visible==true || lblRegionErr.Visible == true)
                {
                    throw new ErrorValidationException("Veuillez vérifier tous les conditions de validation avant la soumission");
                }
                string nom = txtName.Text.Trim();
                string prenoms = txtFirstName.Text.Trim();
                string sexe = dropDownSex.Text;
                string adresse = txtAddress.Text.Trim();
                string contact = txtContact.Text.Trim();
                var infoPerso = new InfoPerso(nom, prenoms, sexe, adresse, contact);

                string num_cin = txtNumCIN.Text.Trim();
                string date_delivrance = dateIssue.Value.ToString("yyyy-MM-dd");
                string lieu_delivrance = txtLocation.Text.Trim();
                string duplicata = dateDuplicate.Value.ToString("yyyy-MM-dd");
                string lieu_duplicata = txtDuplicateLoc.Text.Trim();

                var cin = new Cin(num_cin, date_delivrance, lieu_delivrance, duplicata, lieu_duplicata);

                string immatricule = txtIM.Text.Trim();
                string date_entree = dateEnter.Value.ToString("yyyy-MM-dd");
                string statut = dropDownStatus.Text;
                string region = txtRegion.Text.Trim();
                int id_attribut = Convert.ToInt32(dropDownAttribute.SelectedValue.ToString());
                var personnel = new Personnel(immatricule, date_entree, statut, region, id_attribut, infoPerso, cin);

                if (btnAdd.Text == "Sauvegarder")
                {
                    PersonnelController.Insert(personnel);
                    Utils.ClearAllTextBox(bunifuPanel3);
                    notification.Show(form, "Employé ajouté avec succès", BunifuSnackbar.MessageTypes.Success);
                }
                else
                {
                    PersonnelController.Update(personnel, id);
                    notification.Show(form, "Modification réussi.", BunifuSnackbar.MessageTypes.Success);
                }
                DisplayPersonnel();
                Reset();
            }
            catch (FormatException)
            {
                lblError.Text = "Veuillez bien verifier vos choix sur les listes déroulantes.";
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
            }
        }
        private void dataGridPersonnel_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                btnAdd.Text = "Modifier";
                id = Convert.ToInt32(dataGridPersonnel.Rows[e.RowIndex].Cells[2].Value);
                LocationEditionPage(id, bunifuPages1);
            }
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                try
                {
                    id = Convert.ToInt32(dataGridPersonnel.Rows[e.RowIndex].Cells[2].Value);
                    PersonnelController.Delete(id);
                    pnlError.Visible = false;
                    notification.Show(form, "Employé supprimé.", BunifuSnackbar.MessageTypes.Success);
                    DisplayPersonnel();
                }
                catch (ErrorValidationException ex)
                {
                    lblError.Text = ex.Message;
                    pnlError.Visible = true;
                }
                catch (Exception ex)
                {
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
            var con = DatabaseController.GetConnection();
            string query = $"SELECT nom, prenom, sexe, adresse, contact, num_cin, date_delivrance, lieu_delivrance, duplicata, lieu_duplicata, immatricule, date_entree, statut, region, id_attribution " +
                $"FROM personnel p JOIN info_perso i ON p.id_info = i.id_info JOIN cin c ON c.id_cin = p.id_cin WHERE p.id_perso = {id}";
            using MySqlCommand cmd = new MySqlCommand(query, con);

            con.Open();
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                txtName.Text = reader.GetString("nom");
                txtFirstName.Text = reader.GetString("prenom");
                dropDownSex.SelectedItem = reader.GetString("sexe");
                txtAddress.Text = reader.GetString("adresse");
                txtContact.Text = reader.GetString("contact");
                txtNumCIN.Text = reader.GetString("num_cin");
                dateIssue.Value = Utils.SetDate(reader.GetString("date_delivrance"));
                txtLocation.Text = reader.GetString("lieu_delivrance");
                dateDuplicate.Value = Utils.SetDate(reader.GetString("duplicata"));
                txtDuplicateLoc.Text = reader.GetString("lieu_duplicata");
                txtIM.Text = reader.GetString("immatricule");
                dateEnter.Value = Utils.SetDate(reader.GetString("date_entree"));
                dropDownStatus.SelectedItem = reader.GetString("statut");
                txtRegion.Text = reader.GetString("region");
                int i = reader.GetInt32("id_attribution");
                dropDownAttribute.SelectedValue = i;
            }
            con.Close();
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
        private void txtAddress_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^[0-9A-Za-z\s/'-]+$";
            if (Utils.IsMatch(txtAddress, pattern))
            {
                lblAddressErr.Visible = false;
                txtAddress.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblAddressErr.Visible = true;
                txtAddress.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblAddressErr.Text = "Entrer une adresse valide";
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
        private void txtNumCIN_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^\d{1,12}$";
            if (Utils.IsMatch(txtNumCIN, pattern))
            {
                lblNumCINErr.Visible = false;
                txtNumCIN.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblNumCINErr.Visible = true;
                txtNumCIN.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblNumCINErr.Text = "Entrer un n° CIN valide";
                validate = false;
            }
        }
        private void txtLocation_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^[0-9A-Za-z\s,'-]+$";
            if (Utils.IsMatch(txtLocation, pattern))
            {
                lblLocationErr.Visible = false;
                txtLocation.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblLocationErr.Visible = true;
                txtLocation.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblLocationErr.Text = "Entrer un lieu valide";
                validate = false;
            }
        }
        private void txtDuplicateLoc_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^[0-9A-Za-z\s,'-]+$";
            if (Utils.IsMatch(txtDuplicateLoc, pattern))
            {
                lblDuplicateErr.Visible = false;
                txtDuplicateLoc.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblDuplicateErr.Visible = true;
                txtDuplicateLoc.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblDuplicateErr.Text = "Entrer un lieu valide";
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
                txtDuplicateLoc.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblIMErr.Text = "Entrer un immatricule valide";
                validate = false;
            }
        }
        private void txtRegion_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^$|^[A-Za-z\s'-]+$";
            if (Utils.IsMatch(txtRegion, pattern))
            {
                lblRegionErr.Visible = false;
                txtRegion.BorderColorIdle = Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(74)))), ((int)(((byte)(94)))));
                validate = true;
            }
            else
            {
                lblRegionErr.Visible = true;
                txtRegion.BorderColorIdle = Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
                lblRegionErr.Text = "Entrer un lieu valide";
                validate = false;
            }
        }
        private void UserControlEmployee_KeyDown(object sender, KeyEventArgs e)
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
