namespace Map_Editor
{
    partial class FrmSetLight
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
            this.txtLight = new System.Windows.Forms.TextBox();
            this.btnSetLight = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Light Brightness";
            // 
            // txtLight
            // 
            this.txtLight.Location = new System.Drawing.Point(111, 26);
            this.txtLight.Name = "txtLight";
            this.txtLight.Size = new System.Drawing.Size(43, 20);
            this.txtLight.TabIndex = 1;
            this.txtLight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLight_KeyDown);
            // 
            // btnSetLight
            // 
            this.btnSetLight.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSetLight.Location = new System.Drawing.Point(79, 65);
            this.btnSetLight.Name = "btnSetLight";
            this.btnSetLight.Size = new System.Drawing.Size(63, 25);
            this.btnSetLight.TabIndex = 2;
            this.btnSetLight.Text = "OK";
            this.btnSetLight.UseVisualStyleBackColor = true;
            this.btnSetLight.Click += new System.EventHandler(this.btnSetLight_Click);
            // 
            // FrmSetLight
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(199, 100);
            this.Controls.Add(this.btnSetLight);
            this.Controls.Add(this.txtLight);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSetLight";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SetLight 0-9, Fishing 100-119";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLight;
        private System.Windows.Forms.Button btnSetLight;
    }
}