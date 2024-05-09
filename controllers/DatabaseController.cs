using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace AppManagement.controllers
{
    class DatabaseController
    {
        public static MySqlConnection GetConnection()
        {
            var connectionStrings = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            MySqlConnection connection = new MySqlConnection(connectionStrings);
            return connection;
        }
    }
}
