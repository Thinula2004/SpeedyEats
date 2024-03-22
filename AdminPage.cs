using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp
{
    public partial class AdminPage : Form
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Add_user_form auf = new Add_user_form();
            auf.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Add_Item_Form aif = new Add_Item_Form();
            aif.Show();
        }

        

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm lf = new LoginForm();
            lf.Show();
        }
    }
}
