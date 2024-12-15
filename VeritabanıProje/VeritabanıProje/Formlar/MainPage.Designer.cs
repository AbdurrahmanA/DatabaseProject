namespace VeritabanıProje.Formlar
{
    partial class MainPage
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lblKullaniciBilgi = new System.Windows.Forms.Label();
            this.btnSatinAl = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(309, 82);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(467, 150);
            this.dataGridView1.TabIndex = 0;
            // 
            // lblKullaniciBilgi
            // 
            this.lblKullaniciBilgi.AutoSize = true;
            this.lblKullaniciBilgi.Location = new System.Drawing.Point(57, 67);
            this.lblKullaniciBilgi.Name = "lblKullaniciBilgi";
            this.lblKullaniciBilgi.Size = new System.Drawing.Size(35, 13);
            this.lblKullaniciBilgi.TabIndex = 1;
            this.lblKullaniciBilgi.Text = "label1";
            this.lblKullaniciBilgi.Click += new System.EventHandler(this.lblKullaniciBilgi_Click);
            // 
            // btnSatinAl
            // 
            this.btnSatinAl.Location = new System.Drawing.Point(75, 129);
            this.btnSatinAl.Name = "btnSatinAl";
            this.btnSatinAl.Size = new System.Drawing.Size(75, 23);
            this.btnSatinAl.TabIndex = 2;
            this.btnSatinAl.Text = "Satın Al";
            this.btnSatinAl.UseVisualStyleBackColor = true;
            this.btnSatinAl.Click += new System.EventHandler(this.btnSatinAl_Click);
            // 
            // MainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnSatinAl);
            this.Controls.Add(this.lblKullaniciBilgi);
            this.Controls.Add(this.dataGridView1);
            this.Name = "MainPage";
            this.Text = "MainPage";
            this.Load += new System.EventHandler(this.MainPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label lblKullaniciBilgi;
        private System.Windows.Forms.Button btnSatinAl;
    }
}