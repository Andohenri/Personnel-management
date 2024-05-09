using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppManagement.controllers
{
    class LoginController
    {
        private static bool auth;

        internal static bool Authentification(string pseudo, string pwd)
        {
            MySqlConnection con = DatabaseController.GetConnection();
            string pseudoCopy = pseudo;
            string pwdCopy = pwd;
            try
            {
                using MySqlCommand cmd = new MySqlCommand("SELECT * FROM admin WHERE username=@pseudo and password=@password", con);
                con.Open();
                cmd.Parameters.AddWithValue("@pseudo", pseudo);
                cmd.Parameters.AddWithValue("@password", pwd);
                using MySqlDataReader r = cmd.ExecuteReader();
                if (string.IsNullOrEmpty(pseudoCopy) || string.IsNullOrEmpty(pwdCopy))
                {
                    throw new MissingFieldException("Veuillez remplir tous les champs.");
                }
                auth = r.HasRows ? true : false;
            }
            catch (MySqlException)
            {
                throw new MissingFieldException("Veuillez vérifier votre connexion au base de donnée.");
            }
            finally
            {
                con.Close();
            }
            return auth;
        }
    }
}
