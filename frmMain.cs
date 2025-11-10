using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace prjIMS
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void tsbLogOut_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you Sure you want to Log Out?", "Logout Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if(result == DialogResult.Yes)
            {
                this.Hide();
                frmLogin login = new frmLogin();
                login.Show();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void tsbInventoryManagement_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmInventory inventory = new frmInventory();
            inventory.Show();
            
        }

        private void tsbStockHistory_Click(object sender, EventArgs e)
        {
            this.Close();
            frmStockHistory sh = new frmStockHistory();
            sh.Show();
        }
    }
}
