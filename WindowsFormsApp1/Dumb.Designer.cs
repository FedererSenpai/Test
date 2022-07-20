namespace WindowsFormsApp1
{
    partial class Dumb
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 51F);
            this.label1.Location = new System.Drawing.Point(253, 261);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(494, 78);
            this.label1.TabIndex = 0;
            this.label1.Text = "Are you dumb?";
            // 
            // btnYes
            // 
            this.btnYes.Location = new System.Drawing.Point(200, 600);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(200, 100);
            this.btnYes.TabIndex = 1;
            this.btnYes.Text = "YES";
            this.btnYes.UseVisualStyleBackColor = true;
            this.Font = new System.Drawing.Font("", 25, System.Drawing.FontStyle.Bold);
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.Location = new System.Drawing.Point(600, 600);
            this.btnNo.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(200, 100);
            this.btnNo.TabIndex = 2;
            this.btnNo.Text = "NO";
            this.btnNo.UseVisualStyleBackColor = true;
            this.Font = new System.Drawing.Font("", 25, System.Drawing.FontStyle.Bold);
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // Form4
            // 
            this.ClientSize = new System.Drawing.Size(984, 961);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.label1);
            this.Name = "Form4";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form4";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
    }
}