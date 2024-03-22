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

namespace TestApp
{
    public partial class Add_user_form : Form
    {
        SqlConnection conn;
        Manager mng = new Manager();


        public Add_user_form()
        {
            InitializeComponent();
        }

        private void Add_user_form_Load(object sender, EventArgs e)
        {
            conn = mng.getConn();
            mng.OpenConn(conn);
            DisplayData();
            comboBox1.SelectedIndex = 0;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            
            int i = 0;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from registrations where username='" + tb_username.Text + "' ";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            i = Convert.ToInt32(dt.Rows.Count.ToString());
            if (i == 0)
            {
                int isadmin = 0;
                if (comboBox1.Text == "Yes")
                {
                    isadmin = 1;
                }
                else
                {
                    isadmin = 0;
                }

                //Access last primary key

                int currentId = 0;
                int x = 0;
                SqlCommand cmd4 = conn.CreateCommand();
                cmd4.CommandType = CommandType.Text;
                cmd4.CommandText = "select * from registrations";
                cmd4.ExecuteNonQuery();
                DataTable dt3 = new DataTable();
                SqlDataAdapter da3 = new SqlDataAdapter(cmd4);
                da3.Fill(dt3);
                x = Convert.ToInt32(dt3.Rows.Count.ToString());
                if (x > 0)
                {
                    
                    SqlCommand cmd2 = conn.CreateCommand();
                    cmd2.CommandType = CommandType.Text;
                    cmd2.CommandText = "select TOP 1 id from registrations ORDER BY id DESC";
                    cmd2.ExecuteNonQuery();
                    DataTable dt2 = new DataTable();
                    SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                    da2.Fill(dt2);
                    currentId = Convert.ToInt32(dt2.Rows[0]["id"].ToString());
                }
                
                
                
                //Reset primary key
                SqlCommand cmd3 = conn.CreateCommand();
                cmd3.CommandType = CommandType.Text;
                cmd3.CommandText = "DBCC CHECKIDENT(registrations, RESEED, "+ currentId +")";
                cmd3.ExecuteNonQuery();
                
                //Insertion command
                SqlCommand cmd1 = conn.CreateCommand();
                cmd1.CommandType = CommandType.Text;
                cmd1.CommandText = "insert into registrations values('"+ tb_fname.Text +"','"+ tb_lname.Text +"','"+ tb_username.Text +"','"+ tb_password.Text +"','"+ tb_email.Text + "', '"+ tb_email.Text + "', '"+ isadmin +"') ";
                cmd1.ExecuteNonQuery();

                //Making textboxes empty
                tb_fname.Text = "";tb_lname.Text = "";tb_username.Text = "";tb_password.Text = "";tb_email.Text = "";tb_contact.Text = "";

                //Add items to datagrid
                DisplayData();

                //Sending message of success
                MessageBox.Show("New user added successfully!");
            }
            else
            {
                MessageBox.Show("That username is already taken.\nUse a different one!");
            }
        }

        private void pnl_sub1_Paint(object sender, PaintEventArgs e)
        {

        }
        public void DisplayData()
        {
            SqlCommand cmd2 = conn.CreateCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "select * from registrations";
            cmd2.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd2);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        //Delete User
        private void button1_Click(object sender, EventArgs e)
        {
            int i = 0;

            SqlCommand cmd6 = conn.CreateCommand();
            cmd6.CommandType = CommandType.Text;
            cmd6.CommandText = "select * from registrations";
            cmd6.ExecuteNonQuery();
            DataTable dt3 = new DataTable();
            SqlDataAdapter da3 = new SqlDataAdapter(cmd6);
            da3.Fill(dt3);
            i = dt3.Rows.Count;
            if (i > 0)
            {
                int id = Convert.ToInt32(dataGridView1.SelectedCells[0].Value.ToString());
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "delete from registrations where id= " + id + "";
                cmd.ExecuteNonQuery();
                DisplayData();
            }
            
        }

        private void Add_user_form_FormClosed(object sender, FormClosedEventArgs e)
        {
            AdminPage ap = new AdminPage();
            ap.Show();
        }
    }
}
