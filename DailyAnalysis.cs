using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TestApp
{
    public partial class DailyAnalysis : Form
    {
        SqlConnection conn;
        Manager mng = new Manager();
        int day;
        public DailyAnalysis()
        {
            InitializeComponent();
        }

        private void DailyAnalysis_Load_1(object sender, EventArgs e)
        {
            day = 0;
            conn = mng.getConn();
            mng.OpenConn(conn);
            DisplayData();
            
        }

        public void DisplayData()
        {
            string query = "select i.Name,p.Quantity,p.Value from purchases as p join items as i on p.productID=i.id where p.date=@Date;";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Date", DateTime.Today.AddDays(day));
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.Columns[2].HeaderText = "Value (Rs)";
            dataGridView1.Columns[0].Width = 170;
            dataGridView1.Columns[1].Width = 120;
            dataGridView1.Columns[2].Width = 160;

            dayLBL.Text = DateTime.Today.AddDays(day).Day.ToString() + " / " + DateTime.Today.AddDays(day).Month.ToString() + " / " + DateTime.Today.AddDays(day).Year.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(day > 1 + -1*((Convert.ToInt32(DateTime.Today.Day.ToString()))))
            {
                day--;
                DisplayData();
            }
            else
            {
                MessageBox.Show("Can't go beyond", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (day < 0)
            {
                day++;
                DisplayData();
            }
            else
            {
                MessageBox.Show("Can't go beyond", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
