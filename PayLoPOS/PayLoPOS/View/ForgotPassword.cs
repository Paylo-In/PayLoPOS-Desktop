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
    public partial class ForgotPassword : Form
    {
        Login baseView;

        public ForgotPassword(Login baseView)
        {
            InitializeComponent();
            this.baseView = baseView;
        }

        private void textboxMobile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                RecoverPassword_Click(null, null);
            }

            if (!char.IsControl(e.KeyChar)
               && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private async void RecoverPassword_Click(object sender, EventArgs e)
        {
            if(txtEmail.Text.Length != 10)
            {
                MessageBox.Show("Please enter a valid mobile number");
                txtEmail.Focus();
            }
            else
            {
                try
                {
                    RecoverPassword.Enabled = false;
                    imgLoading.Visible = true;
                    var response = await new RestClient().GenerateOTP(txtEmail.Text);
                    if(response.status == 1)
                    {
                        CreatePassword cp = new CreatePassword(baseView,this, txtEmail.Text);
                        cp.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(response.data.msg);
                        txtEmail.Focus();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    txtEmail.Focus();
                }
                finally
                {
                    RecoverPassword.Enabled = true;
                    imgLoading.Visible = false;
                }
            }
        }
    }
}
