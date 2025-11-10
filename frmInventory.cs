using System;
using System.Data;
using System.Windows.Forms;

namespace prjIMS
{
    public partial class frmInventory : Form
    {
        ClasCon cls = new ClasCon();
        public int itemId;
        public string itemName;
        public int stock, black, white, red, blue, green;

        public frmInventory()
        {
            InitializeComponent();
        }

        private void frmInventory_Load(object sender, EventArgs e)
        {
            LoadInventory();
        }
        private void LoadInventory()
        {
            cls.loadDGV("SELECT * FROM vwCurrentStock", dgvInventory);
            dgvInventory.ClearSelection();
        }


        private void dgvInventory_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvInventory.Rows[e.RowIndex];

                itemId = Convert.ToInt32(row.Cells[0].Value);
                itemName = row.Cells[1].Value.ToString();
                stock = Convert.ToInt32(row.Cells[2].Value);
                black = Convert.ToInt32(row.Cells[3].Value);
                white = Convert.ToInt32(row.Cells[4].Value);
                red = Convert.ToInt32(row.Cells[5].Value);
                blue = Convert.ToInt32(row.Cells[6].Value);
                green = Convert.ToInt32(row.Cells[7].Value);
                

                dgvInventory.Tag = itemId;
            }
        }

        private void tsbAdd_Click(object sender, EventArgs e)
        {
            // Open form to add a new transaction (IN/OUT)
            frmDEInventory invent = new frmDEInventory(null);
            invent.ShowDialog();
            LoadInventory();
        }

        private void tsbEdit_Click(object sender, EventArgs e)
        {
            if (dgvInventory.Tag != null && Convert.ToInt32(dgvInventory.Tag) > 0)
            {
                clsInventoryManagement u = new clsInventoryManagement
                {
                    itemId = itemId,
                    itemName = itemName,
                    stock = stock,
                    black = black,
                    white = white,
                    red = red,
                    blue = blue,
                    green = green
                };
                frmDEInventory invent = new frmDEInventory(u);
                invent.ShowDialog();
                LoadInventory();
            }
            else
            {
                MessageBox.Show("Please select an item to edit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvInventory.SelectedRows[0];
                int selectedId = Convert.ToInt32(row.Cells["item_id"].Value);

                var result = MessageBox.Show("Are you sure you want to delete this item?", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    cls.delInventory("DELETE FROM tblItem WHERE item_id=?", selectedId);
                    MessageBox.Show("Deleted Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadInventory();
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void tsbLogOut_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

        private void frmInventory_Activated(object sender, EventArgs e)
        {
            LoadInventory();
            dgvInventory.RowEnter += dgvInventory_RowEnter;
        }

        private void frmInventory_Load_1(object sender, EventArgs e)
        {

        }
    }
}
