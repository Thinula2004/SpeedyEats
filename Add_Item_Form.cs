using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TestApp
{
    public partial class Add_Item_Form : Form
    {
        OpenFileDialog openFileDialog_1 = new OpenFileDialog();
        Manager mng = new Manager();
        SqlConnection conn;
        public byte[] imageData;
        private void Add_Item_Form_Load(object sender, EventArgs e)
        {
            conn = mng.getConn();
            mng.OpenConn(conn);
            cb_type.SelectedIndex = 0;
            DisplayData();
        }
        public Add_Item_Form()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        public bool isImageSelected = false;
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog_1.Filter = "Image files (*.png; *.jpg; *.jpeg; *.gif)|*jpg; *jpeg; *.png; *.gif";
            if (openFileDialog_1.ShowDialog() == DialogResult.OK)
            {
                string imagePath = openFileDialog_1.FileName;
                label4.Text = imagePath;
                imageData = File.ReadAllBytes(imagePath);
                isImageSelected = true;
            }
        }

        private void btn_addProduct_Click(object sender, EventArgs e)
        {
            int i = 0;
            int currentId=0;
            SqlCommand cmd4 = conn.CreateCommand();
            cmd4.CommandType = CommandType.Text;
            cmd4.CommandText = "select * from items";
            cmd4.ExecuteNonQuery();
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(cmd4);
            da2.Fill(dt2);
            i = dt2.Rows.Count;
            if(i > 0)
            {
                SqlCommand cmd1 = conn.CreateCommand();
                cmd1.CommandType = CommandType.Text;
                cmd1.CommandText = "select TOP 1 id from items ORDER BY id DESC";
                cmd1.ExecuteNonQuery();
                DataTable dt1 = new DataTable();
                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                da1.Fill(dt1);
                currentId = Convert.ToInt32(dt1.Rows[0]["id"].ToString());
            }


            

            SqlCommand cmd2 = conn.CreateCommand();
            cmd2.CommandType = CommandType.Text;
            cmd2.CommandText = "DBCC CHECKIDENT(items, RESEED, "+ currentId +")";
            cmd2.ExecuteNonQuery();

            int price = 0;
            if(int.TryParse(tb_price.Text, out price))
            {
                price = Convert.ToInt32(tb_price.Text);
            }
            else
            {
                price = 0;
            }
            if(isImageSelected == true)
            {
                //Inserting into items table
                SqlCommand cmd3 = conn.CreateCommand();
                cmd3.CommandType = CommandType.Text;
                cmd3.CommandText = "insert into items values('" + tb_pname.Text + "', " + price + " ,@ImageData,'" + cb_type.Text + "')";
                cmd3.Parameters.AddWithValue("@ImageData", imageData);
                cmd3.ExecuteNonQuery();

                // Inserting into purchase table
                string query = "select top 1 id from items order by id desc";
                SqlCommand cmd5 = new SqlCommand(query, conn);
                cmd5.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd5);
                DataTable dt = new DataTable();
                da.Fill(dt);
                int productID = Convert.ToInt32(dt.Rows[0]["id"].ToString());
                


                string query2 = "insert into purchase values(" + productID + ",'" + tb_pname.Text + "',0, 0 )";
                SqlCommand cmd6 = new SqlCommand(query2, conn);
                cmd6.ExecuteNonQuery();

                DisplayData();
                isImageSelected = false;
                //Clearing text boxes
                tb_pname.Text = ""; tb_price.Text = ""; label4.Text = "";
            }
            else
            {
                MessageBox.Show("Image Not selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            


            

        }

        public void DisplayData()
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select id,name,price,type from items";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Name";
            dataGridView1.Columns[2].HeaderText = "Price";
            dataGridView1.Columns[3].HeaderText = "Type";
            dataGridView1.Columns[1].Width = 150;
            dataGridView1.Columns[2].Width = 95;
            dataGridView1.Columns[3].Width = 105;




        }

        private void btn_deleteProduct_Click(object sender, EventArgs e)
        {
            int i = 0;
            
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from items";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            i = dt.Rows.Count;
            if (i > 0)
            {
                int id = Convert.ToInt32(dataGridView1.SelectedCells[0].Value.ToString());
                SqlCommand cmd2 = conn.CreateCommand();
                cmd2.CommandType = CommandType.Text;
                cmd2.CommandText = "delete from items where id= " + id + "";
                cmd2.ExecuteNonQuery();
                DisplayData();

                string query = "delete from purchase where productID="+ id +"";
                SqlCommand cmd3 = new SqlCommand(query, conn);
                cmd3.ExecuteNonQuery();

            }
            

        }

        private void Add_Item_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            AdminPage ap = new AdminPage();
            ap.Show();
        }
    }
}

        
