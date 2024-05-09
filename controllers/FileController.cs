using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppManagement.models;
using MySql.Data.MySqlClient;

namespace AppManagement.controllers
{
    class FileController
    {
        private static readonly MySqlConnection con = DatabaseController.GetConnection();

        internal static void Insert(File file)
        {
            con.Open();
            string insertQuery = "INSERT INTO fichier (nom_fichier, contenu) VALUES (@nomFichier, @contenu)";
            using (MySqlCommand cmd = new MySqlCommand(insertQuery, con))
            {
                cmd.Parameters.AddWithValue("@nomFichier", file.Name);
                cmd.Parameters.AddWithValue("@contenu", file.Content);
                cmd.ExecuteNonQuery();
            }
            con.Close();

        }

        internal static bool DownloadFile(int v)
        {
            con.Open();
            bool res = false;
            string query = $"SELECT contenu from fichier WHERE id_fichier = {v}";
            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataReader r = cmd.ExecuteReader();
            if (r.Read())
            {
                var form = new Form1();

                byte[] content = (byte[])r["contenu"];
                form.saveFileDialog1.Filter = "Fichiers PDF (*.pdf)|*.pdf";
                form.saveFileDialog1.FilterIndex = 1;
                form.saveFileDialog1.Title = "Enregistrer un fichier PDF";
                if (form.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllBytes(form.saveFileDialog1.FileName, content);
                    res = true;
                }
            }
            con.Close();
            return res;
        }

        internal static void DeleteFile(int v)
        {
            con.Open();
            string query = $"DELETE from fichier WHERE id_fichier = {v}";
            using MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
