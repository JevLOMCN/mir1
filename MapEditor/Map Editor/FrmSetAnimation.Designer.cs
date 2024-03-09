namespace Map_Editor
{
    partial class FrmSetAnimation
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
            this.btnSetAnimation = new System.Windows.Forms.Button();
            this.txtAnimationTick = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAnimationFrame = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkBlend = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnSetAnimation
            // 
            this.btnSetAnimation.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSetAnimation.Location = new System.Drawing.Point(99, 93);
            this.btnSetAnimation.Name = "btnSetAnimation";
            this.btnSetAnimation.Size = new System.Drawing.Size(75, 25);
            this.btnSetAnimation.TabIndex = 11;
            this.btnSetAnimation.Text = "OK";
            this.btnSetAnimation.UseVisualStyleBackColor = true;
            this.btnSetAnimation.Click += new System.EventHandler(this.btnSetAnimation_Click);
            // 
            // txtAnimationTick
            // 
            this.txtAnimationTick.Location = new System.Drawing.Point(200, 53);
            this.txtAnimationTick.Name = "txtAnimationTick";
            this.txtAnimationTick.Size = new System.Drawing.Size(50, 20);
            this.txtAnimationTick.TabIndex = 10;
            this.txtAnimationTick.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAnimationTick_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(97, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Animation Interval ";
            // 
            // txtAnimationFrame
            // 
            this.txtAnimationFrame.Location = new System.Drawing.Point(200, 24);
            this.txtAnimationFrame.Name = "txtAnimationFrame";
            this.txtAnimationFrame.Size = new System.Drawing.Size(50, 20);
            this.txtAnimationFrame.TabIndex = 8;
            this.txtAnimationFrame.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAnimationFrame_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(97, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Animation Frames ";
            // 
            // chkBlend
            // 
            this.chkBlend.AutoSize = true;
            this.chkBlend.Location = new System.Drawing.Point(28, 40);
            this.chkBlend.Name = "chkBlend";
            this.chkBlend.Size = new System.Drawing.Size(53, 17);
            this.chkBlend.TabIndex = 6;
            this.chkBlend.Text = "Blend";
            this.chkBlend.UseVisualStyleBackColor = true;
            // 
            // FrmSetAnimation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 127);
            this.Controls.Add(this.btnSetAnimation);
            this.Controls.Add(this.txtAnimationTick);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAnimationFrame);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkBlend);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSetAnimation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Set Front/Middle Animation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSetAnimation;
        private System.Windows.Forms.TextBox txtAnimationTick;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAnimationFrame;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkBlend;
    }
}