using System;
using System.Windows.Forms;
using PayLoPOS.Controller;
using PayLoPOS.Model;

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
            if(txtEmail.Text == "")
            {
                MessageBox.Show("Please enter a valid email address");
            }
            else if(txtPassword.Text == "")
            {
                MessageBox.Show("Please enter a valid password");
            }
            else
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
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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
            ForgotPassword forgot = new ForgotPassword();
            forgot.ShowDialog();
        }

        private void openDashboard()
        {
            Dashboard dashboard = new Dashboard();
            dashboard.login = this;
            dashboard.Show();
            this.Hide();
        }

        private async void Login_Load(object sender, EventArgs e)
        {
            if(Properties.Settings.Default.accessToken != "") {
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
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    /*Properties.Settings.Default.accessToken = "";
                    Properties.Settings.Default.Save();
                    loginPanel.Visible = true;
                    loadingPanel.Visible = false;*/
                }
            }
        }

        public void showLoginPanel()
        {
            loginPanel.Visible = true;
            loadingPanel.Visible = false;
        }
    }
}
