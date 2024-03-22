using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp
{
    public partial class MainPage : Form
    {

        SqlConnection conn;
        Manager mng = new Manager();
         

        public int isAdmin;
        public string uName;

        public MainPage()
        {
            InitializeComponent();
        }


        private void MainPage_Load(object sender, EventArgs e)
        {
            conn = mng.getConn();
            mng.OpenConn(conn);
            lbl_uname.Text = uName;

            DisplayProducts("Food");
            MakeBill();

            



        }

        //User control access
        private void userControlToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            
            
        }


        

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        

        private void btn_food_Click(object sender, EventArgs e)
        {
            DisplayProducts("Food");
        }

        private void btn_drinks_Click(object sender, EventArgs e)
        {
            DisplayProducts("Drink");
        }

        private void btn_desert_Click(object sender, EventArgs e)
        {
            
        }

        private void Add_Item_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            DisplayProducts("Food");
        }

        private void inventoryManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }


        void AddButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string tag = btn.Tag.ToString();
            int id = Convert.ToInt32(tag);

            string query = "select * from items where id='"+ id +"'";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            string query2 = "select * from bill where productID='" + id + "'";
            SqlCommand cmd2 = new SqlCommand(query2, conn);
            cmd2.ExecuteNonQuery();
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);

            string name = dt.Rows[0]["name"].ToString();
            int amount = Convert.ToInt32(dt.Rows[0]["price"].ToString());
            int productID = id;
            int quantity;
            
            if(dt2.Rows.Count > 0)
            {
                int resetAmount = Convert.ToInt32(dt2.Rows[0]["Amount"].ToString()) + amount;
                quantity = Convert.ToInt32(dt2.Rows[0]["Quantity"].ToString()) + 1;
                string query3 = "update bill set Quantity="+ quantity +",Amount="+ resetAmount +" where productID="+ id +"";
                SqlCommand cmd3 = new SqlCommand(query3, conn);
                cmd3.ExecuteNonQuery();
            }
            else
            {
                quantity = 1;
                string query3 = "insert into bill values('" + name + "', " + quantity + ", " + amount + ", " + productID + ")";
                SqlCommand cmd3 = new SqlCommand(query3, conn);
                cmd3.ExecuteNonQuery();
            }

            MakeBill();
        }
        //Reset bill
        private void button1_Click(object sender, EventArgs e)
        {

            ResetBill();
            
            MakeBill();
        }
        private bool isDataThere()
        {
            bool isDataThere;
            if (dataGridView1.Rows.Count > 0)
            {
                isDataThere = true;
            }
            else
            {
                isDataThere = false;
            }

            return isDataThere;
        }
        //Remove Item
        private void button2_Click(object sender, EventArgs e)
        {

            if(isDataThere())
            {
                int id = Convert.ToInt32(dataGridView1.SelectedCells[0].Value.ToString());

                string query = "delete from bill where productID=" + id + "";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();

                MakeBill();
            }
            else
            {
                MessageBox.Show("There is no item to remove!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
            
        }

        int total = 0;
        //Purchase
        private void button3_Click(object sender, EventArgs e)
        {
            if (isDataThere())
            {
                MessageBox.Show("Puchase successful. Total is " + total, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdatePurchases();
                ResetBill();

                MakeBill();


            }
            else
            {
                MessageBox.Show("Add items to make the purchase!", "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            
            
        }
        private void button4_Click(object sender, EventArgs e)
        {
            DailyAnalysis da = new DailyAnalysis();
            da.DialogResult = DialogResult.None;
            da.ShowDialog();
            
        }

        public void MakeBill()
        {
            string query = "select ProductID,Name,Quantity,Amount from bill";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].Visible = false;
            foreach(DataGridViewColumn column in dataGridView1.Columns)
            {
                column.Resizable = DataGridViewTriState.False;
            }
            dataGridView1.Columns[1].Width = 200;
            dataGridView1.Columns[2].Width = 90;
            dataGridView1.Columns[3].Width = 130;
            dataGridView1.Columns[3].HeaderText = "Amount (Rs)";
            
            total = 0;
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                total+= Convert.ToInt32(row.Cells[3].Value.ToString()) ;

            }
            lbl_total.Text = "Rs " +  total.ToString() + " /=";


        }

        public void DisplayProducts(string type)
        {
            //Clearing every card
            flowLayoutPanel1.Controls.Clear();

            string query2 = "select * from items where type='" + type + "'";
            mng.OpenConn(conn);
            using (SqlCommand cmd2 = new SqlCommand(query2,conn))
            {
                using(SqlDataReader reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //Change
                        string name = reader["name"].ToString();
                        string price = reader["price"].ToString();
                        int id = Convert.ToInt32(reader["id"].ToString());

                        
                        byte[] imageData = (byte[])reader["image"];
                        Image img;
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            img = Image.FromStream(ms);
                        }

                        Panel pnl1 = new Panel();
                        pnl1.Width = 305;
                        pnl1.Height = 210;
                        pnl1.BackColor = Color.White;
                        pnl1.BorderStyle = BorderStyle.FixedSingle;
                        pnl1.Margin = new Padding(5);


                        Color orngClr = ColorTranslator.FromHtml("#FB7D13");
                        FontFamily font = label1.Font.FontFamily;


                        // image panel
                        Panel pnl2 = new Panel();
                        pnl2.Width = 140;
                        pnl2.Height = 120;
                        pnl2.BackgroundImage = img; //Change
                        pnl2.BackgroundImageLayout = ImageLayout.Zoom;
                        pnl2.Dock = DockStyle.Left;
                        pnl2.Margin = new Padding(5);
                        pnl1.Controls.Add(pnl2);

                        // price add panel
                        Panel pnl3 = new Panel();
                        pnl3.Width = 145;
                        pnl3.Height = 145;
                        pnl3.Dock = DockStyle.Right;
                        //pnl3.BackColor = Color.Red;
                        pnl1.Controls.Add(pnl3);

                        //price label
                        Label lblPrice = new Label();
                        lblPrice.Text = "Rs. " + price + "/=";
                        lblPrice.Font = new Font(font, 10f, FontStyle.Bold);
                        lblPrice.ForeColor = orngClr;
                        lblPrice.TextAlign = ContentAlignment.MiddleCenter;
                        lblPrice.Location = new Point(25, 20);
                        pnl3.Controls.Add(lblPrice);


                        //add button
                        Button btnAdd = new Button();
                        btnAdd.Text = "Add";
                        btnAdd.Width = 127;
                        btnAdd.Height = 94;
                        btnAdd.BackColor = orngClr;
                        btnAdd.ForeColor = Color.White;
                        btnAdd.Font = new Font("Russo One", 20f, FontStyle.Regular);
                        btnAdd.TextAlign = ContentAlignment.MiddleCenter;
                        btnAdd.Location = new Point(10, 50);
                        btnAdd.Tag = id; //change
                        btnAdd.Click += AddButton_Click;
                        pnl3.Controls.Add(btnAdd);


                        // name panel
                        Panel pnl4 = new Panel();
                        pnl4.Width = 305;
                        pnl4.Height = 44;
                        pnl4.BackColor = Color.White;
                        pnl4.Dock = DockStyle.Bottom;
                        pnl4.BorderStyle = BorderStyle.FixedSingle;
                        pnl1.Controls.Add(pnl4);

                        Label lblName = new Label();
                        lblName.Text = name; //Change
                        lblName.Font = new System.Drawing.Font(font, 15f, FontStyle.Bold);
                        lblName.ForeColor = orngClr;
                        lblName.Width = 100;
                        lblName.Height = 40;
                        lblName.Dock = DockStyle.Fill;
                        lblName.TextAlign = ContentAlignment.MiddleCenter;
                        pnl4.Controls.Add(lblName);


                        flowLayoutPanel1.Controls.Add(pnl1);
                    }
                }
            }


            

        }

        private void MainPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            ResetBill();
        }
        public void ResetBill()
        {
            string query = "truncate table bill";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            total = 0;
            

        }
        public void UpdateHistory()
        {
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                int productID = Convert.ToInt32(row.Cells[0].Value);

                string query2 = "select quantity,value from purchase where productID="+ productID +"";
                SqlCommand cmd2 = new SqlCommand(query2,conn);
                cmd2.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd2);
                DataTable dt = new DataTable();
                da.Fill(dt);
                int quantity = Convert.ToInt32(dt.Rows[0]["quantity"].ToString()) + Convert.ToInt32(row.Cells[2].Value);

                
                
                int value = Convert.ToInt32(dt.Rows[0]["value"].ToString()) + Convert.ToInt32(row.Cells[3].Value);

                
                string query = "update purchase set quantity=" + quantity + ",value="+ value +" where productID=" + productID + "";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdatePurchases()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int productID = Convert.ToInt32(row.Cells[0].Value);

                string query2 = "select * from purchases where productID="+ productID +" and date= CAST(GETDATE() AS DATE)";
                SqlCommand cmd2 = new SqlCommand(query2, conn);
                cmd2.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd2);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if(dt.Rows.Count > 0)
                {
                    int _quantity = Convert.ToInt32(dt.Rows[0]["quantity"].ToString()) + Convert.ToInt32(row.Cells[2].Value);
                    int _value = Convert.ToInt32(dt.Rows[0]["value"].ToString()) + Convert.ToInt32(row.Cells[3].Value);
                    string query3 = "update purchases set quantity="+ _quantity +",value="+ _value +" where productID="+productID+ " and date=CAST(GETDATE() AS DATE);";
                    SqlCommand cmd3 = new SqlCommand(query3 , conn);
                    cmd3.ExecuteNonQuery();
                }
                else
                {
                    int _quantity = Convert.ToInt32(row.Cells[2].Value);
                    int _value = Convert.ToInt32(row.Cells[3].Value);
                    string query3 = "insert into purchases(date,productID,quantity,value) values(GETDATE(), "+productID+","+_quantity +", "+ _value +");";
                    SqlCommand cmd3 = new SqlCommand( query3 , conn);
                    cmd3.ExecuteNonQuery();
                }

            }


        }
        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm lf = new LoginForm();
            lf.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MonthlyAnalysis ma = new MonthlyAnalysis();
            ma.DialogResult = DialogResult.None;
            ma.ShowDialog();
        }
    }
}
