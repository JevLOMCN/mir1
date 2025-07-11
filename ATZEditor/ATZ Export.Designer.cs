using static Mir1ATZTool.Main;

namespace Mir1ATZTool
{
    partial class ATZ_Export
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ExportProgressBar = new ProgressBar();
            ExportExcludeBlanks = new CheckBox();
            TransparentExport = new CheckBox();
            OutputATZPath = new TextBox();
            label3 = new Label();
            label2 = new Label();
            SelectOutputATZBtn = new Button();
            RAWExport = new CheckBox();
            BMPExport = new CheckBox();
            JPGExport = new CheckBox();
            PNGExport = new CheckBox();
            ExportBtn = new Button();
            SuspendLayout();
            // 
            // ExportProgressBar
            // 
            ExportProgressBar.Location = new Point(12, 99);
            ExportProgressBar.Name = "ExportProgressBar";
            ExportProgressBar.Size = new Size(414, 23);
            ExportProgressBar.Style = ProgressBarStyle.Continuous;
            ExportProgressBar.TabIndex = 20;
            // 
            // ExportExcludeBlanks
            // 
            ExportExcludeBlanks.AutoSize = true;
            ExportExcludeBlanks.Location = new Point(13, 67);
            ExportExcludeBlanks.Name = "ExportExcludeBlanks";
            ExportExcludeBlanks.Size = new Size(103, 19);
            ExportExcludeBlanks.TabIndex = 19;
            ExportExcludeBlanks.Text = "Exclude Blanks";
            ExportExcludeBlanks.UseVisualStyleBackColor = true;
            // 
            // TransparentExport
            // 
            TransparentExport.AutoSize = true;
            TransparentExport.Location = new Point(287, 13);
            TransparentExport.Name = "TransparentExport";
            TransparentExport.Size = new Size(124, 19);
            TransparentExport.TabIndex = 18;
            TransparentExport.Text = "Transparent Image";
            TransparentExport.UseVisualStyleBackColor = true;
            // 
            // OutputATZPath
            // 
            OutputATZPath.BorderStyle = BorderStyle.FixedSingle;
            OutputATZPath.Location = new Point(69, 38);
            OutputATZPath.Name = "OutputATZPath";
            OutputATZPath.PlaceholderText = "C:\\Mir 1\\Client\\data\\Output";
            OutputATZPath.ReadOnly = true;
            OutputATZPath.Size = new Size(269, 23);
            OutputATZPath.TabIndex = 13;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(11, 42);
            label3.Name = "label3";
            label3.Size = new Size(48, 15);
            label3.TabIndex = 17;
            label3.Text = "Output:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(11, 14);
            label2.Name = "label2";
            label2.Size = new Size(48, 15);
            label2.TabIndex = 15;
            label2.Text = "Format:";
            // 
            // SelectOutputATZBtn
            // 
            SelectOutputATZBtn.Location = new Point(344, 38);
            SelectOutputATZBtn.Name = "SelectOutputATZBtn";
            SelectOutputATZBtn.Size = new Size(75, 23);
            SelectOutputATZBtn.TabIndex = 16;
            SelectOutputATZBtn.Text = "Select";
            SelectOutputATZBtn.UseVisualStyleBackColor = true;
            SelectOutputATZBtn.Click += SelectOutputATZBtn_Click;
            // 
            // RAWExport
            // 
            RAWExport.AutoSize = true;
            RAWExport.Location = new Point(233, 13);
            RAWExport.Name = "RAWExport";
            RAWExport.Size = new Size(48, 19);
            RAWExport.TabIndex = 14;
            RAWExport.Text = "Raw";
            RAWExport.UseVisualStyleBackColor = true;
            // 
            // BMPExport
            // 
            BMPExport.AutoSize = true;
            BMPExport.Location = new Point(176, 13);
            BMPExport.Name = "BMPExport";
            BMPExport.Size = new Size(51, 19);
            BMPExport.TabIndex = 12;
            BMPExport.Text = "BMP";
            BMPExport.UseVisualStyleBackColor = true;
            // 
            // JPGExport
            // 
            JPGExport.AutoSize = true;
            JPGExport.Location = new Point(125, 13);
            JPGExport.Name = "JPGExport";
            JPGExport.Size = new Size(45, 19);
            JPGExport.TabIndex = 11;
            JPGExport.Text = "JPG";
            JPGExport.UseVisualStyleBackColor = true;
            // 
            // PNGExport
            // 
            PNGExport.AutoSize = true;
            PNGExport.Location = new Point(69, 13);
            PNGExport.Name = "PNGExport";
            PNGExport.Size = new Size(50, 19);
            PNGExport.TabIndex = 10;
            PNGExport.Text = "PNG";
            PNGExport.UseVisualStyleBackColor = true;
            // 
            // ExportBtn
            // 
            ExportBtn.Location = new Point(344, 70);
            ExportBtn.Name = "ExportBtn";
            ExportBtn.Size = new Size(75, 23);
            ExportBtn.TabIndex = 21;
            ExportBtn.Text = "Export";
            ExportBtn.UseVisualStyleBackColor = true;
            ExportBtn.Click += ExportBtn_Click;
            // 
            // ATZ_Export
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ExportBtn);
            Controls.Add(ExportProgressBar);
            Controls.Add(ExportExcludeBlanks);
            Controls.Add(TransparentExport);
            Controls.Add(OutputATZPath);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(SelectOutputATZBtn);
            Controls.Add(RAWExport);
            Controls.Add(BMPExport);
            Controls.Add(JPGExport);
            Controls.Add(PNGExport);
            Name = "ATZ_Export";
            Size = new Size(440, 135);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar ExportProgressBar;
        private CheckBox ExportExcludeBlanks;
        private CheckBox TransparentExport;
        private TextBox OutputATZPath;
        private Label label3;
        private Label label2;
        private Button SelectOutputATZBtn;
        private CheckBox RAWExport;
        private CheckBox BMPExport;
        private CheckBox JPGExport;
        private CheckBox PNGExport;
        private Button ExportBtn;
        public DoubleBufferedListView ATZListView;
    }
}
