namespace VeritabanıProje
{
    partial class FirstPage
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOk = new System.Windows.Forms.Button();
            this.dgDatas = new System.Windows.Forms.DataGridView();
            this.btnFormGecis = new System.Windows.Forms.Button();
            this.btnLogIn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgDatas)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(438, 202);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // dgDatas
            // 
            this.dgDatas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgDatas.Location = new System.Drawing.Point(38, 101);
            this.dgDatas.Name = "dgDatas";
            this.dgDatas.Size = new System.Drawing.Size(505, 150);
            this.dgDatas.TabIndex = 1;
            // 
            // btnFormGecis
            // 
            this.btnFormGecis.Location = new System.Drawing.Point(309, 207);
            this.btnFormGecis.Name = "btnFormGecis";
            this.btnFormGecis.Size = new System.Drawing.Size(75, 23);
            this.btnFormGecis.TabIndex = 2;
            this.btnFormGecis.Text = "Kayıt Ol";
            this.btnFormGecis.UseVisualStyleBackColor = true;
            this.btnFormGecis.Click += new System.EventHandler(this.btnFormGecis_Click);
            // 
            // btnLogIn
            // 
            this.btnLogIn.Location = new System.Drawing.Point(424, 161);
            this.btnLogIn.Name = "btnLogIn";
            this.btnLogIn.Size = new System.Drawing.Size(75, 23);
            this.btnLogIn.TabIndex = 3;
            this.btnLogIn.Text = "GirişYap";
            this.btnLogIn.UseVisualStyleBackColor = true;
            this.btnLogIn.Click += new System.EventHandler(this.btnLogIn_Click);
            // 
            // FirstPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnLogIn);
            this.Controls.Add(this.btnFormGecis);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.dgDatas);
            this.Name = "FirstPage";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgDatas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.DataGridView dgDatas;
        private System.Windows.Forms.Button btnFormGecis;
        private System.Windows.Forms.Button btnLogIn;
    }
}

