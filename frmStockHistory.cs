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
    public partial class frmStockHistory : Form{
        ClasCon cls = new ClasCon();
        public int itemId;
        public string itemName, in_out, note;
        public DateTime date;
        public int black, blackBalance, white, whiteBalance, red, redBalance, blue, blueBalance, green, greenBalance;
        public frmStockHistory()
        {
            InitializeComponent();
        }

        private void frmStockHistory_Load(object sender, EventArgs e)
        {
                LoadStockHistory();
        }

        private void LoadStockHistory()
        {
            cls.loadDGV("SELECT * FROM vwstockhistory", dgvStockHistory);
            dgvStockHistory.ClearSelection();
        }


        private void dgvStockHistory_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvStockHistory.Rows[e.RowIndex];

                itemName = row.Cells[1].Value.ToString();
                date = Convert.ToDateTime(row.Cells[2].Value);
                in_out = row.Cells[3].Value.ToString();
                black = Convert.ToInt32(row.Cells[4].Value);
                blackBalance = Convert.ToInt32(row.Cells[5].Value);
                white = Convert.ToInt32(row.Cells[6].Value);
                whiteBalance = Convert.ToInt32(row.Cells[7].Value);
                red = Convert.ToInt32(row.Cells[8].Value);
                redBalance = Convert.ToInt32(row.Cells[9].Value);
                blue = Convert.ToInt32(row.Cells[10].Value);
                blueBalance = Convert.ToInt32(row.Cells[11].Value);
                green = Convert.ToInt32(row.Cells[12].Value);
                greenBalance = Convert.ToInt32(row.Cells[13].Value);
                note = row.Cells[14].Value.ToString();


                dgvStockHistory.Tag = itemId;
            }
        }

        private void tsbLogOut_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you Sure you want to Log Out?", "Logout Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Hide();
                frmLogin login = new frmLogin();
                login.Show();
            }
        }

        private void tsbBack_Click(object sender, EventArgs e)
        {
            this.Close();
            frmMain main = new frmMain();
            main.Show();

        }

        private void tsbAdd_Click(object sender, EventArgs e)
        {
            frmDEStock stock = new frmDEStock();
            stock.ShowDialog();
        }

        private void frmStockHistory_Activated(object sender, EventArgs e)
        {
            LoadStockHistory();
        }

        private void tsbDelete_Click(object sender, EventArgs e)
{
    // Check if a row is selected
    if (dgvStockHistory.SelectedRows.Count > 0)
    {
        // Get the selected row
        DataGridViewRow row = dgvStockHistory.SelectedRows[0];

        // Retrieve the item_name from the selected row (used to find the item_id)
        string selectedItemName = row.Cells["item_name"].Value.ToString(); // Assuming "item_name" is the column in your view

        // Confirm the delete action
        var result = MessageBox.Show("Are you sure you want to delete this item?", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            try
            {
                // Get the item_id from tblItem using the item_name
                int itemId = cls.GetItemIdByName(selectedItemName);

                // Now delete the item from tblItem using the item_id
                if (itemId > 0)
                {
                    cls.delInventory("DELETE FROM tblItem WHERE item_id=?", itemId);
                    MessageBox.Show("Item deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload stock history to reflect changes
                    LoadStockHistory();
                }
                else
                {
                    MessageBox.Show("Item not found in the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    else
    {
        MessageBox.Show("Please select an item to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}

        }
    }
