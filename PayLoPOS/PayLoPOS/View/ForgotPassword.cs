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
        public ForgotPassword()
        {
            InitializeComponent();
        }

        private async void RecoverPassword_Click(object sender, EventArgs e)
        {
            if(txtEmail.Text.Length != 10)
            {
                MessageBox.Show("Please enter a valid mobile number");
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
                        CreatePassword cp = new CreatePassword(this, txtEmail.Text);
                        cp.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(response.data.msg);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
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
