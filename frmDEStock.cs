using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Odbc;
namespace prjIMS
{
    public partial class frmDEStock : Form
    {
        ClasCon cls = new ClasCon();
        public frmDEStock()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmDEStock_Load(object sender, EventArgs e)
        {
            cls.loadCBO("SELECT DISTINCT in_out from tblInventoryTransaction", cboInOut, "in_out", "in_out");
            cboInOut.SelectedIndex = -1;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string itemName = txtItemName.Text.Trim();
            string note = txtNote.Text.Trim();

            if (string.IsNullOrEmpty(itemName))
            {
                MessageBox.Show("Please enter the item name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Use ClasCon to get or create the item
            int itemId = cls.GetOrCreateItem(itemName);

            // Parse color quantities
            int black = int.TryParse(txtBlack.Text, out black) ? black : 0;
            int white = int.TryParse(txtWhite.Text, out white) ? white : 0;
            int red = int.TryParse(txtRed.Text, out red) ? red : 0;
            int blue = int.TryParse(txtBlue.Text, out blue) ? blue : 0;
            int green = int.TryParse(txtGreen.Text, out green) ? green : 0;

            // Determine IN/OUT as full strings "IN" or "OUT"
            string inOut = "IN"; // default to "IN"
            if (cboInOut.SelectedItem != null)
            {
                string selected = cboInOut.SelectedItem.ToString().ToUpper(); // Ensure case-insensitivity
                if (selected == "OUT")
                {
                    inOut = "OUT";  // Correctly use "OUT"

                    // Negate quantities for OUT
                    black = -black;
                    white = -white;
                    red = -red;
                    blue = -blue;
                    green = -green;
                }
            }

            // Add transaction
            cls.addTransaction(itemId, note, inOut, black, white, red, blue, green);

            MessageBox.Show("Stock history saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Clear fields
            txtItemName.Clear();
            txtBlack.Clear();
            txtWhite.Clear();
            txtRed.Clear();
            txtBlue.Clear();
            txtGreen.Clear();
            txtNote.Clear();
            cboInOut.SelectedIndex = -1;

            this.Close();
        }




    }
}
