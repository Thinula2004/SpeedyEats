using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace TestApp
{
    public partial class LoginForm : Form 
    {
        Manager mng = new Manager();

        SqlConnection conn;
        
        
        public LoginForm()
        {
            InitializeComponent();
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conn = mng.getConn();
            mng.OpenConn(conn);
            Console.WriteLine("Hi");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            int i = 0;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from registrations where username='"+ tb_username.Text +"' and password='"+ tb_password.Text +"'";
            cmd.ExecuteNonQuery();

            // Accessing selected data

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            
            i = Convert.ToInt32(dt.Rows.Count.ToString());
            if(i == 0)
            {
                MessageBox.Show("Invalid username or password. Please try again!","Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {
                
                this.Hide();
                
                
                if(Convert.ToInt32(dt.Rows[0]["isAdmin"].ToString()) == 1)
                {
                    AdminPage ap = new AdminPage();
                    ap.Show();
                }
                else
                {
                    MainPage mp = new MainPage();
                    mp.uName = dt.Rows[0]["firstname"].ToString();
                    mp.Show();
                }
                
                
                
            }
        }

        private void tb_username_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                tb_password.Focus();
                
            }
        }

        private void tb_password_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                button1_Click(sender, e);
            }
        }
    }
}
