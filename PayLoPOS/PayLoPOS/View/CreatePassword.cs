using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PayLoPOS.Controller;

namespace PayLoPOS.View
{
    public partial class CreatePassword : Form
    {
        private string mobile;
        private ForgotPassword parent;
        private Login baseView;

        public CreatePassword(Login baseView, ForgotPassword parent,string mobile)
        {
            InitializeComponent();
            this.mobile = mobile;
            this.parent = parent;
            this.baseView = baseView;
            lblMessage.Text = "One Time Password sent to mobile number +91-XXXXXXX" + mobile.Substring(mobile.Length - 3);
        }

        private void textboxMobile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Submit_Click(null, null);
            }

            if (!char.IsControl(e.KeyChar)
               && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void textboxPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Submit_Click(null, null);
            }
        }

        private async void ResendOTP_Click(object sender, EventArgs e)
        {
            try
            {
                ResendOTP.Enabled = false;
                imgLoading.Visible = true;
                var response = await new RestClient().ResendOTP(mobile);
                ResendOTP.Enabled = true;
                imgLoading.Visible = false;
                MessageBox.Show(response.data.msg);
            }
            catch(Exception ex)
            {
                ResendOTP.Enabled = true;
                imgLoading.Visible = false;
                MessageBox.Show(ex.Message);
            }
        }

        private async void Submit_Click(object sender, EventArgs e)
        {
            if(txtPassword.Text == "")
            {
                MessageBox.Show("Please enter a valid password");
                txtPassword.Focus();
            }
            else if(txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password should be greater then or equal to six characters");
                txtPassword.Focus();
            }
            else if(txtOTP.Text.Length != 6)
            {
                MessageBox.Show("Please enter a valid OTP");
                txtOTP.Focus();
            }
            else
            {
                try
                {
                    Submit.Enabled = false;
                    imgLoading.Visible = true;
                    var response = await new RestClient().CreatePassword(mobile, txtPassword.Text, txtOTP.Text);
                    if(response.status == 1)
                    {
                        Properties.Settings.Default.accessToken = response.data.token;
                        Properties.Settings.Default.Save();
                        Close();
                        parent.Close();
                        baseView.checkUserLoggedIn();
                    }
                    else
                    {
                        MessageBox.Show(response.data.msg);
                        txtOTP.Focus();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    txtOTP.Focus();
                }
                finally
                {
                    txtOTP.Text = "";
                    Submit.Enabled = true;
                    imgLoading.Visible = false;
                }
            }
        }
    }
}
