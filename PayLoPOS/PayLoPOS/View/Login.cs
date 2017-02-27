using System;
using System.Windows.Forms;
using PayLoPOS.Controller;
using PayLoPOS.Model;
using System.Diagnostics;

namespace PayLoPOS.View
{

    public partial class Login : Form
    {

        public Login()
        {
            InitializeComponent();
            imgLoading.Visible = false;
            if(Properties.Settings.Default.accessToken != "")
            {
                loginPanel.Visible = false;
                loadingPanel.Visible = true;
            }
            else
            {
                loginPanel.Visible = true;
                loadingPanel.Visible = false;
            }
        }

        private async void SignIn_Click(object sender, EventArgs e)
        {
            if(validate())
            {
                SignIn.Enabled = false;
                try
                {
                    imgLoading.Visible = true;
                    User user = await new RestClient().Login(txtEmail.Text, txtPassword.Text);
                    if (user.status == 1)
                    {
                        txtEmail.Text = "";
                        txtPassword.Text = "";
                        Properties.Settings.Default.accessToken = user.data.token;
                        Properties.Settings.Default.Save();
                        Global.currentUser = user.data;
                        openDashboard();
                    }
                    else
                    {
                        MessageBox.Show(user.data.msg);
                        txtEmail.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    txtEmail.Focus();
                }
                finally
                {
                    imgLoading.Visible = false;
                }
                SignIn.Enabled = true;
            }
        }

        private void ForgotPassword_Click(object sender, EventArgs e)
        {
            ForgotPassword forgot = new ForgotPassword(this);
            forgot.ShowDialog();
        }

        private void openDashboard()
        {
            Dashboard dashboard = new Dashboard();
            dashboard.login = this;
            dashboard.Show();
            this.Hide();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            checkUserLoggedIn();
        }

        public async void checkUserLoggedIn()
        {
            if (Properties.Settings.Default.accessToken != "")
            {
                loginPanel.Visible = false;
                loadingPanel.Visible = true;
            }
            else
            {
                loginPanel.Visible = true;
                loadingPanel.Visible = false;
            }
            if (Properties.Settings.Default.accessToken != "")
            {
                try
                {
                    User user = await new RestClient().GetProfile();
                    if (user.status == 1)
                    {
                        Global.currentUser = user.data;
                        openDashboard();
                    }
                    else
                    {
                        MessageBox.Show(user.data.msg);
                        Properties.Settings.Default.accessToken = "";
                        Properties.Settings.Default.Save();
                        loginPanel.Visible = true;
                        loadingPanel.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Properties.Settings.Default.accessToken = "";
                    Properties.Settings.Default.Save();
                    loginPanel.Visible = true;
                    loadingPanel.Visible = false;
                }
            }
        }

        public void showLoginPanel()
        {
            loginPanel.Visible = true;
            loadingPanel.Visible = false;
        }

        private void textboxMobile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (validate())
                {
                    SignIn_Click(null, null);
                }
            }

            if (!char.IsControl(e.KeyChar)
               && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txtEmail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (validate())
                {
                    SignIn_Click(null, null);
                }
            }
        }

        private bool validate()
        {
            if(txtEmail.Text.Length != 10)
            {
                MessageBox.Show("Please enter a valid mobile number");
                txtEmail.Focus();
                return false;
            }
            else if(txtPassword.Text == "")
            {
                MessageBox.Show("Please enter a valid password.");
                txtPassword.Focus();
                return false;
            }
            return true;
        }
    }
}
