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
    public partial class frmLogin : Form
    {
        ClasCon cls = new ClasCon();
        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtuser.Text;
            string password = txtpass.Text;

            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please Enter both Username and Password.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool loginSuccess = cls.ValidateLogin(username, password);

            if(loginSuccess)
            {
                string userRole = cls.GetUserRole(username);

                if(!string.IsNullOrEmpty(userRole))
                {
                    MessageBox.Show("Login Successfull!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if(userRole == "admin")
                {
                    frmMain main = new frmMain();
                    main.Show();
                }
                else if(userRole == "user")
                {
                    frmMain main = new frmMain();
                    main.Show();
                }
                else
                {
                    MessageBox.Show("Unknown user role. Please contact the System Administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
