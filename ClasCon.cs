using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Odbc;
using System.Windows.Forms;

namespace prjIMS
{
    class ClasCon
    {
        public OdbcConnection conn;
        public string conStr = "server=localhost;uid=root;password=samantharanish;database=dbInventory;Driver={MySQL ODBC 8.0 ANSI Driver}";

        public ClasCon()
        {
            conn = new OdbcConnection(conStr);
        }
        public void connect()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message, "database");
            }
        }

        public bool ValidateLogin(string username, string password)
        {
            try 
            {
                connect();
                string query = "SELECT * FROM vwuserrole WHERE username = ? AND password = ?";

                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.AddWithValue("?", username);
                cmd.Parameters.AddWithValue("?", password);

                OdbcDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();
                    string userRole = dr["name"].ToString();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("ERROR: " + e.Message, "Database Query");
                return false;
            }
            finally
            {
                if(conn.State == ConnectionState.Open)
                {
                    conn.Close();   
                }
            }
        }

        public string GetUserRole(string username)
        {
            try
            {
                connect();
                string query = "SELECT name FROM vwuserrole WHERE username = ?";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.AddWithValue("?", username);

                OdbcDataReader dr = cmd.ExecuteReader();

                if(dr.HasRows)
                {
                    dr.Read();
                    string role = dr["name"].ToString();
                    return role;
                }
                else
                {
                    MessageBox.Show("No rows found for username: " + username);
                    return string.Empty;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("ERROR: " + e.Message, "Database Query");
                return string.Empty;
            }
            finally
            {
                if(conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        public void loadDGV(string sql, DataGridView dgv)
        {
            connect();
            OdbcDataAdapter da = new OdbcDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgv.DataSource = dt;
            da.Dispose();
            conn.Close();
        }

        public void loadCBO(string sql, ComboBox cbo, string mem, string val)
        {
            connect();
            OdbcDataAdapter da = new OdbcDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cbo.DataSource = dt;
            cbo.DisplayMember = mem;
            cbo.ValueMember = val;
            da.Dispose();
            conn.Close();
        }

        public void inventory(string sql, string name, int stock, int black, int white, int red, int blue, int green, int imid = 0)
        {
            connect();
            try
            {
                OdbcCommand cmd = new OdbcCommand(sql, conn);
                cmd.Parameters.AddWithValue("?", name);
                cmd.Parameters.AddWithValue("?", stock);
                cmd.Parameters.AddWithValue("?", black);
                cmd.Parameters.AddWithValue("?", white);
                cmd.Parameters.AddWithValue("?", red);
                cmd.Parameters.AddWithValue("?", blue);
                cmd.Parameters.AddWithValue("?", green);

                if (imid != 0) // for update
                {
                    cmd.Parameters.AddWithValue("?", imid);
                }

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        public int GetItemIdByName(string itemName)
        {
            try
            {
                connect();
                string query = "SELECT item_id FROM tblItem WHERE item_name = ?";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.AddWithValue("?", itemName);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    MessageBox.Show("Item not found: " + itemName, "Error");
                    return 0;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message, "Database Error");
                return 0;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }


        public void addTransaction(int itemId, string note, string inOut, int black, int white, int red, int blue, int green)
        {
            connect();
            try
            {
                // Ensure that inOut is either 'IN' or 'OUT'
                if (inOut != "IN" && inOut != "OUT")
                {
                    MessageBox.Show("Invalid transaction type. Please choose 'IN' or 'OUT'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ensure non-negative quantities for "IN" transactions
                if (inOut == "IN")
                {
                    black = Math.Max(0, black);  // Ensure black quantity is non-negative for "IN"
                    white = Math.Max(0, white);  // Ensure white quantity is non-negative for "IN"
                    red = Math.Max(0, red);      // Ensure red quantity is non-negative for "IN"
                    blue = Math.Max(0, blue);    // Ensure blue quantity is non-negative for "IN"
                    green = Math.Max(0, green);  // Ensure green quantity is non-negative for "IN"
                }
                else if (inOut == "OUT")
                {
                    // If it's an "OUT" transaction, we store negative quantities in the transaction color table
                    black = black < 0 ? black : -black;
                    white = white < 0 ? white : -white;
                    red = red < 0 ? red : -red;
                    blue = blue < 0 ? blue : -blue;
                    green = green < 0 ? green : -green;
                }

                // Insert into tblInventoryTransaction
                string sqlTransaction = "INSERT INTO tblInventoryTransaction (item_id, transaction_date, in_out, note) VALUES (?, NOW(), ?, ?)";
                OdbcCommand cmd = new OdbcCommand(sqlTransaction, conn);
                cmd.Parameters.AddWithValue("?", itemId);
                cmd.Parameters.AddWithValue("?", inOut);  // Correctly use 'IN' or 'OUT'
                cmd.Parameters.AddWithValue("?", note);
                cmd.ExecuteNonQuery();

                // Get last transaction ID
                cmd = new OdbcCommand("SELECT LAST_INSERT_ID()", conn);
                long transactionId = Convert.ToInt64(cmd.ExecuteScalar());

                // Insert color quantities for the transaction
                var colors = new Dictionary<int, int>()
        {
            {1, black}, {2, white}, {3, red}, {4, blue}, {5, green}
        };

                foreach (var kvp in colors)
                {
                    string sqlColor = "INSERT INTO tblTransactionColor (transaction_id, color_id, quantity) VALUES (?, ?, ?)";
                    cmd = new OdbcCommand(sqlColor, conn);
                    cmd.Parameters.AddWithValue("?", transactionId);
                    cmd.Parameters.AddWithValue("?", kvp.Key);
                    cmd.Parameters.AddWithValue("?", kvp.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }





        public void delInventory(string sql, int imid)
        {
            connect();
            OdbcCommand cmd = new OdbcCommand(sql, conn);
            cmd.Parameters.AddWithValue("?", imid);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public int GetOrCreateItem(string itemName)
        {
            int itemId = GetItemIdByName(itemName);

            if (itemId == 0)
            {
                connect();
                try
                {
                    OdbcCommand cmd = new OdbcCommand("INSERT INTO tblItem(item_name) VALUES (?)", conn);
                    cmd.Parameters.AddWithValue("?", itemName);
                    cmd.ExecuteNonQuery();

                    cmd = new OdbcCommand("SELECT LAST_INSERT_ID()", conn);
                    itemId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error inserting item: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }

            return itemId;
        }

    }
}
