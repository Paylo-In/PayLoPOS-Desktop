namespace PayLoPOS.View
{
    partial class EnterVPA
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
            this.lblSubmit = new System.Windows.Forms.Label();
            this.txtVPA = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.imgLoading = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSubmit
            // 
            this.lblSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubmit.BackColor = System.Drawing.Color.SeaGreen;
            this.lblSubmit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSubmit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubmit.ForeColor = System.Drawing.Color.White;
            this.lblSubmit.Location = new System.Drawing.Point(0, 83);
            this.lblSubmit.Name = "lblSubmit";
            this.lblSubmit.Size = new System.Drawing.Size(283, 36);
            this.lblSubmit.TabIndex = 7;
            this.lblSubmit.Text = "SUBMIT";
            this.lblSubmit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSubmit.Click += new System.EventHandler(this.lblSubmit_Click);
            // 
            // txtVPA
            // 
            this.txtVPA.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVPA.Location = new System.Drawing.Point(6, 27);
            this.txtVPA.Name = "txtVPA";
            this.txtVPA.Size = new System.Drawing.Size(265, 22);
            this.txtVPA.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(-1, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(285, 33);
            this.label2.TabIndex = 5;
            // 
            // imgLoading
            // 
            this.imgLoading.Image = global::PayLoPOS.Properties.Resources.loading1;
            this.imgLoading.Location = new System.Drawing.Point(68, 59);
            this.imgLoading.Name = "imgLoading";
            this.imgLoading.Size = new System.Drawing.Size(137, 19);
            this.imgLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.imgLoading.TabIndex = 28;
            this.imgLoading.TabStop = false;
            this.imgLoading.Visible = false;
            // 
            // EnterVPA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(284, 119);
            this.Controls.Add(this.imgLoading);
            this.Controls.Add(this.lblSubmit);
            this.Controls.Add(this.txtVPA);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnterVPA";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enter VPA";
            ((System.ComponentModel.ISupportInitialize)(this.imgLoading)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSubmit;
        private System.Windows.Forms.TextBox txtVPA;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox imgLoading;
    }
}