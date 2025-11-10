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
    public partial class frmDEInventory : Form
    {
        ClasCon cls = new ClasCon();
        clsInventoryManagement u;
        public frmDEInventory(clsInventoryManagement user)
        {
            
            InitializeComponent();
            this.u = user;
        }

        private void frmDEInventory_Load(object sender, EventArgs e)
        {

            if(u == null)
            {
                clearField();
            }
            else
            {
                txtItemName.Text = u.itemName;
                txtStock.Text = u.stock.ToString();
                txtBlack.Text = u.black.ToString();
                txtWhite.Text = u.white.ToString();
                txtRed.Text = u.red.ToString();
                txtBlue.Text = u.blue.ToString();
                txtGreen.Text = u.green.ToString();

            }
        }

        public void clearField()
        {
            txtItemName.Clear();
            txtStock.Clear();
            txtBlack.Clear();
            txtWhite.Clear();
            txtRed.Clear();
            txtBlue.Clear();
            txtGreen.Clear();
        }



        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int itemId = 0;

            // 1. Insert new item if this is a new record
            if (u == null)
            {
                cls.connect();
                try
                {
                    OdbcCommand cmd = new OdbcCommand("INSERT INTO tblItem(item_name) VALUES (?)", cls.conn);
                    cmd.Parameters.AddWithValue("?", txtItemName.Text);
                    cmd.ExecuteNonQuery();

                    cmd = new OdbcCommand("SELECT LAST_INSERT_ID()", cls.conn);
                    itemId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error inserting item: " + ex.Message);
                    return;
                }
                finally
                {
                    cls.conn.Close();
                }
            }
            else
            {
                itemId = u.itemId;
            }

            // 2. Parse color quantities
            int black, white, red, blue, green;
            int.TryParse(txtBlack.Text, out black);
            int.TryParse(txtWhite.Text, out white);
            int.TryParse(txtRed.Text, out red);
            int.TryParse(txtBlue.Text, out blue);
            int.TryParse(txtGreen.Text, out green);

            // 3. Insert transaction with colors
            cls.addTransaction(itemId, "Added via frmDEInventory", "IN", black, white, red, blue, green);
                
            MessageBox.Show("Inventory saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }



    }
}
