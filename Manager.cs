using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Manager
    {

        private SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Projects\C#\SpeedyEats\Inventory.mdf;Integrated Security=True");

        
        // Use of Encapsulation
        public SqlConnection getConn()
        {
            return conn;
        }

        public void OpenConn(SqlConnection conn)
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
        }

        
    }
}
