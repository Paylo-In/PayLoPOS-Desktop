﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PayLoPOS.View
{
    public partial class EnterVPA : Form
    {
        Dashboard parent;

        long billId;
        string email;

        public EnterVPA(Dashboard parent, long billId, string email)
        {
            this.parent = parent;
            this.billId = billId;
            this.email = email;
            InitializeComponent();
        }

        private void lblSubmit_Click(object sender, EventArgs e)
        {
            if(txtVPA.Text == "")
            {
                MessageBox.Show("Please enter a valid VPA");
            }
            else
            {
                this.parent.submitVPA(txtVPA.Text, this, billId, email);
            }
        }

        public void showLoading(bool isShow)
        {
            lblSubmit.Enabled = !isShow;
            imgLoading.Visible = isShow;
        }
    }
}
