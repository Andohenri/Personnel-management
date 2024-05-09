using AppManagement.controllers;
using Bunifu.UI.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppManagement.utils
{
    class Utils
    {
        private static MySqlConnection con = DatabaseController.GetConnection();
        public static void Display(string query, DataGridView dgv)
        {
            try
            {
                con.Open();
                using MySqlCommand cmd = new MySqlCommand(query, con);
                using MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adp.Fill(table);
                dgv.DataSource = table;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                con.Close();
            }
        }
        public static bool IsMatch(BunifuTextBox textBox, string regexPattern)
        {
            Regex regex = new Regex(regexPattern);
            return regex.IsMatch(textBox.Text);
        }
        public static void ClearAllTextBox(BunifuPanel pnl)
        {
            foreach (Control item in pnl.Controls)
            {
                if(item is BunifuTextBox)
                {
                    BunifuTextBox txtBox = (BunifuTextBox)item;
                    txtBox.Clear();
                }
            }
        }
        public static bool IsNullOrEmptyTextBox(BunifuPanel pnl)
        {
            bool res = false;
            foreach (Control item in pnl.Controls)
            {
                if (item is BunifuTextBox txtBox)
                {
                    if (string.IsNullOrEmpty(txtBox.Text) && txtBox.Name != "txtDuplicateLoc")
                    {
                        res = true;
                    }
                }
            }
            return res;
        }
        public static DateTime SetDate(string v)
        {
            string[] date1 = v.Split('-');
            int[] dateInt = date1.Select(int.Parse).ToArray();
            return new DateTime(dateInt[0], dateInt[1], dateInt[2]);
        }
        public static bool IsFound(int IM)
        {
            try
            {
                con.Open();
                using MySqlCommand cmd = new MySqlCommand($"SELECT * FROM personnel WHERE immatricule = {IM}", con);
                using MySqlDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
            }
            catch(Exception)
            {
                throw new Exception("Impossible de se connecter à l'un des hôtes MySQL spécifiés \n Veuillez vérifier votre connexion");
            }
            finally
            {
                con.Close();
            }
        }
    }
}
