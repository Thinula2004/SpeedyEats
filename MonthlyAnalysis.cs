using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp
{
    public partial class MonthlyAnalysis : Form
    {
        SqlConnection conn;
        Manager mng = new Manager();
        int month;
        public MonthlyAnalysis()
        {
            InitializeComponent();
        }

        private void MonthlyAnalysis_Load(object sender, EventArgs e)
        {
            month = 0;
            conn = mng.getConn();
            mng.OpenConn(conn);
            DisplayData(true);
            
        }

        public void DisplayData(bool initial)
        {
            string query = "select i.Name,SUM(p.Quantity),SUM(p.Value) from purchases as p join items as i on p.productID=i.id where MONTH(p.date)=MONTH(@Date) and YEAR(p.date)=YEAR(@Date) GROUP BY i.name;";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Date", DateTime.Today.AddMonths(month));
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.Columns[2].HeaderText = "Value (Rs)";
            dataGridView1.Columns[0].Width = 170;
            dataGridView1.Columns[1].Width = 120;
            dataGridView1.Columns[2].Width = 160;

            string query2 = "select i.Name as ProductName,SUM(p.Quantity) as TotalQuantity from purchases as p join items as i on p.productID=i.id where MONTH(p.date)=MONTH(@Date) and YEAR(p.date)=YEAR(@Date) GROUP BY i.name;";
            SqlCommand cmd2 = new SqlCommand(query2, conn);
            cmd2.Parameters.AddWithValue("@Date", DateTime.Today.AddMonths(month));
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);

            
            chart1.DataSource = dt2;
            if (initial)
            {
                chart1.Series.Add("TotalQuantity");
                chart1.Series["TotalQuantity"].XValueMember = "ProductName";
                chart1.Series["TotalQuantity"].YValueMembers = "TotalQuantity";
                chart1.Series["TotalQuantity"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            }
            else
            {
                chart1.Series["TotalQuantity"].XValueMember = "ProductName";
                chart1.Series["TotalQuantity"].YValueMembers = "TotalQuantity";
                chart1.Series["TotalQuantity"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            }
            
            

            monthLBL.Text = DateTime.Today.AddMonths(month).Month.ToString() + " / "+ DateTime.Today.Year.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(month > 1+(-1*(Convert.ToInt32(DateTime.Today.Month.ToString()))))
            {
                month--;
                DisplayData(false);
            }
            else
            {
                MessageBox.Show("Can't go beyond!","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (month < 0)
            {
                month++;
                DisplayData(false);
            }
            else
            {
                MessageBox.Show("Can't go beyond!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
