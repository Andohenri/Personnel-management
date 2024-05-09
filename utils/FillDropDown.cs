using AppManagement.controllers;
using AppManagement.models;
using Bunifu.UI.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppManagement.utils
{
    class FillDropDown
    {
        private static readonly MySqlConnection con = DatabaseController.GetConnection();
        public static void FillAttribut(BunifuDropdown d)
        {
            using MySqlCommand cmd = new MySqlCommand("SELECT * FROM attribution", con);
            List<Attribution> attribution = new List<Attribution>();
            try
            {
                con.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        attribution.Add(new Attribution(reader.GetInt32("id_attribution"), reader.GetString("attribut")));
                    }
                }
                d.DataSource = attribution;
                d.DisplayMember = "attribut";
                d.ValueMember = "id";
            }
            catch (Exception)
            {
                MessageBox.Show("Impossible de se connecter à l'un des hôtes MySQL spécifiés \n Veuillez vérifier votre connexion", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            }
            finally
            {
                con.Close();
            }
        }

        internal static void FillFile(BunifuDropdown d)
        {
            using MySqlCommand cmd = new MySqlCommand("SELECT * FROM fichier ORDER BY id_fichier DESC", con);
            List<File> file = new List<File>();
            try
            {
                con.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        file.Add(new File(reader.GetInt32("id_fichier"),reader.GetString("nom_fichier")));
                    }
                }
                d.DataSource = file;
                d.DisplayMember = "Name";
                d.ValueMember = "ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            }
            finally
            {
                con.Close();
            }
        }

        internal static void FillEncadreur(BunifuDropdown d)
        {
            using MySqlCommand cmd = new MySqlCommand("SELECT * FROM personnel p JOIN info_perso i ON i.id_info = p.id_info", con);
            List<Personnel> encadreur = new List<Personnel>();
            try
            {
                con.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        encadreur.Add(new Personnel(reader.GetInt32("id_perso"), $"{reader.GetString("nom")} {reader.GetString("prenom")}"));
                    }
                }
                d.DataSource = encadreur;
                d.DisplayMember = "Nom";
                d.ValueMember = "ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            }
            finally
            {
                con.Close();
            }
        }

        internal static void FillInterim(BunifuDropdown dropDownInterim)
        {
            try
            {
                con.Open();
                using MySqlCommand cmd = new MySqlCommand("SELECT prenom FROM personnel p JOIN info_perso i ON i.id_info = p.id_info", con);
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dropDownInterim.Items.Add(reader.GetString("prenom"));
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

        public static void FillYear(BunifuDropdown d)
        {
            try
            {
                con.Open();
                using MySqlCommand cmd = new MySqlCommand("SELECT * FROM annee", con);
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    d.Items.Add(reader.GetString("id_annee"));
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
    }
}
