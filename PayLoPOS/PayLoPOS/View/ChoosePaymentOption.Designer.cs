namespace PayLoPOS.View
{
    partial class ChoosePaymentOption
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtPaymentMode = new System.Windows.Forms.ComboBox();
            this.lblSubmit = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtPaymentMode
            // 
            this.txtPaymentMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPaymentMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.txtPaymentMode.Font = new System.Drawing.Font("Arial Unicode MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPaymentMode.ForeColor = System.Drawing.Color.SteelBlue;
            this.txtPaymentMode.FormattingEnabled = true;
            this.txtPaymentMode.Items.AddRange(new object[] {
            "CHOOSE PAYMENT OPTION",
            "CASH",
            "MPOS",
            "SEND LINK",
            "UPI",
            "WALLET"});
            this.txtPaymentMode.Location = new System.Drawing.Point(12, 19);
            this.txtPaymentMode.Name = "txtPaymentMode";
            this.txtPaymentMode.Size = new System.Drawing.Size(320, 29);
            this.txtPaymentMode.TabIndex = 13;
            // 
            // lblSubmit
            // 
            this.lblSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubmit.BackColor = System.Drawing.Color.SeaGreen;
            this.lblSubmit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSubmit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubmit.ForeColor = System.Drawing.Color.White;
            this.lblSubmit.Location = new System.Drawing.Point(0, 72);
            this.lblSubmit.Name = "lblSubmit";
            this.lblSubmit.Size = new System.Drawing.Size(345, 36);
            this.lblSubmit.TabIndex = 14;
            this.lblSubmit.Text = "SUBMIT";
            this.lblSubmit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSubmit.Click += new System.EventHandler(this.lblSubmit_Click);
            // 
            // ChoosePaymentOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 108);
            this.Controls.Add(this.lblSubmit);
            this.Controls.Add(this.txtPaymentMode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChoosePaymentOption";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Payment Option";
            this.Load += new System.EventHandler(this.ChoosePaymentOption_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox txtPaymentMode;
        private System.Windows.Forms.Label lblSubmit;
    }
}