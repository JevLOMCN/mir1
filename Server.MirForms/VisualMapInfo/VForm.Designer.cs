namespace Server.MirForms.VisualMapInfo
{
    partial class VForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VForm));
            Tool = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            EndFocus = new ToolStripButton();
            FocusBreak = new ToolStripSeparator();
            SelectButton = new ToolStripButton();
            AddButton = new ToolStripButton();
            MoveButton = new ToolStripButton();
            ResizeButton = new ToolStripButton();
            splitter1 = new Splitter();
            tabPage2 = new TabPage();
            RespawnPanel = new Panel();
            toolStrip3 = new ToolStrip();
            RespawnsRemoveSelected = new ToolStripButton();
            ResapwnsHideRegion = new ToolStripButton();
            ResapwnsShowRegion = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            ResapwnsFocusRegion = new ToolStripButton();
            RespawnsFilter = new ComboBox();
            RespawnTools = new ToolStrip();
            RespawnsSelectAll = new ToolStripButton();
            RespawnsSelectNone = new ToolStripButton();
            RespawnsInvertSelection = new ToolStripButton();
            RegionTabs = new TabControl();
            statusStrip1 = new StatusStrip();
            MapDetailsLabel = new ToolStripStatusLabel();
            mapContainer1 = new MirForms.Control.MapContainer();
            MapImage = new PictureBox();
            tabPage1 = new TabPage();
            MiningPanel = new Panel();
            MiningFilter = new ComboBox();
            MiningTools = new ToolStrip();
            MiningSelectAll = new ToolStripButton();
            MiningSelectNone = new ToolStripButton();
            MiningInvertSelection = new ToolStripButton();
            MiningRemoveSelected = new ToolStripButton();
            toolStrip2 = new ToolStrip();
            MiningHideRegion = new ToolStripButton();
            MiningShowRegion = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            MiningFocusRegion = new ToolStripButton();
            Tool.SuspendLayout();
            tabPage2.SuspendLayout();
            toolStrip3.SuspendLayout();
            RespawnTools.SuspendLayout();
            RegionTabs.SuspendLayout();
            statusStrip1.SuspendLayout();
            mapContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MapImage).BeginInit();
            tabPage1.SuspendLayout();
            MiningTools.SuspendLayout();
            toolStrip2.SuspendLayout();
            SuspendLayout();
            // 
            // Tool
            // 
            Tool.AutoSize = false;
            Tool.Dock = DockStyle.Left;
            Tool.GripStyle = ToolStripGripStyle.Hidden;
            Tool.ImageScalingSize = new Size(28, 28);
            Tool.Items.AddRange(new ToolStripItem[] { toolStripLabel1, EndFocus, FocusBreak, SelectButton, AddButton, MoveButton, ResizeButton });
            Tool.Location = new Point(0, 0);
            Tool.Name = "Tool";
            Tool.Size = new Size(52, 545);
            Tool.TabIndex = 1;
            Tool.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(50, 15);
            toolStripLabel1.Text = " ";
            // 
            // EndFocus
            // 
            EndFocus.AutoSize = false;
            EndFocus.DisplayStyle = ToolStripItemDisplayStyle.Image;
            EndFocus.Image = (Image)resources.GetObject("EndFocus.Image");
            EndFocus.ImageScaling = ToolStripItemImageScaling.None;
            EndFocus.ImageTransparentColor = Color.Magenta;
            EndFocus.Name = "EndFocus";
            EndFocus.Size = new Size(28, 28);
            EndFocus.Text = "End Focus";
            EndFocus.Visible = false;
            EndFocus.Click += EndFocus_Click;
            // 
            // FocusBreak
            // 
            FocusBreak.Name = "FocusBreak";
            FocusBreak.Size = new Size(50, 6);
            FocusBreak.Visible = false;
            // 
            // SelectButton
            // 
            SelectButton.AutoSize = false;
            SelectButton.Checked = true;
            SelectButton.CheckOnClick = true;
            SelectButton.CheckState = CheckState.Checked;
            SelectButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            SelectButton.Image = (Image)resources.GetObject("SelectButton.Image");
            SelectButton.ImageAlign = ContentAlignment.MiddleLeft;
            SelectButton.ImageScaling = ToolStripItemImageScaling.None;
            SelectButton.ImageTransparentColor = Color.Magenta;
            SelectButton.Name = "SelectButton";
            SelectButton.Size = new Size(28, 28);
            SelectButton.Text = "Select Region";
            SelectButton.Click += ToolSelectedChanged;
            // 
            // AddButton
            // 
            AddButton.AutoSize = false;
            AddButton.CheckOnClick = true;
            AddButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            AddButton.Image = (Image)resources.GetObject("AddButton.Image");
            AddButton.ImageAlign = ContentAlignment.MiddleLeft;
            AddButton.ImageScaling = ToolStripItemImageScaling.None;
            AddButton.ImageTransparentColor = Color.Magenta;
            AddButton.Name = "AddButton";
            AddButton.Size = new Size(28, 28);
            AddButton.Text = "Add Region";
            AddButton.Click += ToolSelectedChanged;
            // 
            // MoveButton
            // 
            MoveButton.AutoSize = false;
            MoveButton.CheckOnClick = true;
            MoveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MoveButton.Image = (Image)resources.GetObject("MoveButton.Image");
            MoveButton.ImageScaling = ToolStripItemImageScaling.None;
            MoveButton.ImageTransparentColor = Color.Magenta;
            MoveButton.Name = "MoveButton";
            MoveButton.Size = new Size(28, 28);
            MoveButton.Text = "Move Region";
            MoveButton.Click += ToolSelectedChanged;
            // 
            // ResizeButton
            // 
            ResizeButton.AutoSize = false;
            ResizeButton.CheckOnClick = true;
            ResizeButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ResizeButton.Image = (Image)resources.GetObject("ResizeButton.Image");
            ResizeButton.ImageScaling = ToolStripItemImageScaling.None;
            ResizeButton.ImageTransparentColor = Color.Magenta;
            ResizeButton.Name = "ResizeButton";
            ResizeButton.Size = new Size(28, 28);
            ResizeButton.Text = "Resize Region";
            ResizeButton.Click += ToolSelectedChanged;
            // 
            // splitter1
            // 
            splitter1.BackColor = SystemColors.ActiveCaption;
            splitter1.Dock = DockStyle.Right;
            splitter1.Location = new Point(803, 0);
            splitter1.Margin = new Padding(4, 3, 4, 3);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(6, 545);
            splitter1.TabIndex = 3;
            splitter1.TabStop = false;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(RespawnPanel);
            tabPage2.Controls.Add(toolStrip3);
            tabPage2.Controls.Add(RespawnsFilter);
            tabPage2.Controls.Add(RespawnTools);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Margin = new Padding(4, 3, 4, 3);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(284, 517);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Respawns";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // RespawnPanel
            // 
            RespawnPanel.AutoScroll = true;
            RespawnPanel.Dock = DockStyle.Fill;
            RespawnPanel.Location = new Point(0, 48);
            RespawnPanel.Margin = new Padding(4, 3, 4, 3);
            RespawnPanel.Name = "RespawnPanel";
            RespawnPanel.Size = new Size(284, 444);
            RespawnPanel.TabIndex = 3;
            // 
            // toolStrip3
            // 
            toolStrip3.Dock = DockStyle.Bottom;
            toolStrip3.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip3.Items.AddRange(new ToolStripItem[] { RespawnsRemoveSelected, ResapwnsHideRegion, ResapwnsShowRegion, toolStripSeparator2, ResapwnsFocusRegion });
            toolStrip3.Location = new Point(0, 492);
            toolStrip3.Name = "toolStrip3";
            toolStrip3.RenderMode = ToolStripRenderMode.Professional;
            toolStrip3.Size = new Size(284, 25);
            toolStrip3.TabIndex = 7;
            toolStrip3.Text = "toolStrip3";
            // 
            // RespawnsRemoveSelected
            // 
            RespawnsRemoveSelected.Alignment = ToolStripItemAlignment.Right;
            RespawnsRemoveSelected.DisplayStyle = ToolStripItemDisplayStyle.Image;
            RespawnsRemoveSelected.Image = (Image)resources.GetObject("RespawnsRemoveSelected.Image");
            RespawnsRemoveSelected.ImageTransparentColor = Color.Magenta;
            RespawnsRemoveSelected.Name = "RespawnsRemoveSelected";
            RespawnsRemoveSelected.Size = new Size(23, 22);
            RespawnsRemoveSelected.Text = "Remove Selected";
            RespawnsRemoveSelected.Click += RespawnsRemoveSelected_Click;
            // 
            // ResapwnsHideRegion
            // 
            ResapwnsHideRegion.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ResapwnsHideRegion.Image = (Image)resources.GetObject("ResapwnsHideRegion.Image");
            ResapwnsHideRegion.ImageTransparentColor = Color.Magenta;
            ResapwnsHideRegion.Name = "ResapwnsHideRegion";
            ResapwnsHideRegion.Size = new Size(23, 22);
            ResapwnsHideRegion.Text = "Hide Region";
            ResapwnsHideRegion.Click += ResapwnsHideRegion_Click;
            // 
            // ResapwnsShowRegion
            // 
            ResapwnsShowRegion.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ResapwnsShowRegion.Image = (Image)resources.GetObject("ResapwnsShowRegion.Image");
            ResapwnsShowRegion.ImageTransparentColor = Color.Magenta;
            ResapwnsShowRegion.Name = "ResapwnsShowRegion";
            ResapwnsShowRegion.Size = new Size(23, 22);
            ResapwnsShowRegion.Text = "Show Region";
            ResapwnsShowRegion.Click += ResapwnsShowRegion_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // ResapwnsFocusRegion
            // 
            ResapwnsFocusRegion.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ResapwnsFocusRegion.Image = (Image)resources.GetObject("ResapwnsFocusRegion.Image");
            ResapwnsFocusRegion.ImageTransparentColor = Color.Magenta;
            ResapwnsFocusRegion.Name = "ResapwnsFocusRegion";
            ResapwnsFocusRegion.Size = new Size(23, 22);
            ResapwnsFocusRegion.Text = "Focus Region";
            ResapwnsFocusRegion.Click += ResapwnsFocusRegion_Click;
            // 
            // RespawnsFilter
            // 
            RespawnsFilter.Dock = DockStyle.Top;
            RespawnsFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            RespawnsFilter.FormattingEnabled = true;
            RespawnsFilter.Location = new Point(0, 25);
            RespawnsFilter.Margin = new Padding(4, 3, 4, 3);
            RespawnsFilter.Name = "RespawnsFilter";
            RespawnsFilter.Size = new Size(284, 23);
            RespawnsFilter.TabIndex = 6;
            RespawnsFilter.SelectedIndexChanged += RespawnsFilter_SelectedIndexChanged;
            // 
            // RespawnTools
            // 
            RespawnTools.GripStyle = ToolStripGripStyle.Hidden;
            RespawnTools.Items.AddRange(new ToolStripItem[] { RespawnsSelectAll, RespawnsSelectNone, RespawnsInvertSelection });
            RespawnTools.Location = new Point(0, 0);
            RespawnTools.Name = "RespawnTools";
            RespawnTools.RenderMode = ToolStripRenderMode.Professional;
            RespawnTools.Size = new Size(284, 25);
            RespawnTools.TabIndex = 2;
            RespawnTools.Text = "toolStrip1";
            // 
            // RespawnsSelectAll
            // 
            RespawnsSelectAll.DisplayStyle = ToolStripItemDisplayStyle.Image;
            RespawnsSelectAll.Image = (Image)resources.GetObject("RespawnsSelectAll.Image");
            RespawnsSelectAll.ImageTransparentColor = Color.Magenta;
            RespawnsSelectAll.Name = "RespawnsSelectAll";
            RespawnsSelectAll.Size = new Size(23, 22);
            RespawnsSelectAll.Text = "Select All";
            RespawnsSelectAll.Click += RespawnsSelectAll_Click;
            // 
            // RespawnsSelectNone
            // 
            RespawnsSelectNone.DisplayStyle = ToolStripItemDisplayStyle.Image;
            RespawnsSelectNone.Image = (Image)resources.GetObject("RespawnsSelectNone.Image");
            RespawnsSelectNone.ImageTransparentColor = Color.Magenta;
            RespawnsSelectNone.Name = "RespawnsSelectNone";
            RespawnsSelectNone.Size = new Size(23, 22);
            RespawnsSelectNone.Text = "Select None";
            RespawnsSelectNone.Click += RespawnsSelectNone_Click;
            // 
            // RespawnsInvertSelection
            // 
            RespawnsInvertSelection.DisplayStyle = ToolStripItemDisplayStyle.Image;
            RespawnsInvertSelection.Image = (Image)resources.GetObject("RespawnsInvertSelection.Image");
            RespawnsInvertSelection.ImageTransparentColor = Color.Magenta;
            RespawnsInvertSelection.Name = "RespawnsInvertSelection";
            RespawnsInvertSelection.Size = new Size(23, 22);
            RespawnsInvertSelection.Text = "Invert Selection";
            RespawnsInvertSelection.Click += RespawnsInvertSelection_Click;
            // 
            // RegionTabs
            // 
            RegionTabs.Controls.Add(tabPage2);
            RegionTabs.Controls.Add(tabPage1);
            RegionTabs.Dock = DockStyle.Right;
            RegionTabs.Location = new Point(809, 0);
            RegionTabs.Margin = new Padding(4, 3, 4, 3);
            RegionTabs.Multiline = true;
            RegionTabs.Name = "RegionTabs";
            RegionTabs.SelectedIndex = 0;
            RegionTabs.Size = new Size(292, 545);
            RegionTabs.SizeMode = TabSizeMode.FillToRight;
            RegionTabs.TabIndex = 3;
            RegionTabs.SelectedIndexChanged += RegionTabs_SelectedIndexChanged;
            RegionTabs.Selecting += RegionTabs_Selecting;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { MapDetailsLabel });
            statusStrip1.Location = new Point(0, 545);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(1101, 24);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "statusStrip1";
            // 
            // MapDetailsLabel
            // 
            MapDetailsLabel.BorderSides = ToolStripStatusLabelBorderSides.Right;
            MapDetailsLabel.BorderStyle = Border3DStyle.Sunken;
            MapDetailsLabel.Name = "MapDetailsLabel";
            MapDetailsLabel.Size = new Size(216, 19);
            MapDetailsLabel.Text = "Map Name: {0}   Width: {1}   Height: {2}";
            // 
            // mapContainer1
            // 
            mapContainer1.AutoScroll = true;
            mapContainer1.Controls.Add(MapImage);
            mapContainer1.Dock = DockStyle.Fill;
            mapContainer1.Location = new Point(52, 0);
            mapContainer1.Margin = new Padding(4, 3, 4, 3);
            mapContainer1.Name = "mapContainer1";
            mapContainer1.Size = new Size(751, 545);
            mapContainer1.TabIndex = 2;
            // 
            // MapImage
            // 
            MapImage.Location = new Point(0, 0);
            MapImage.Margin = new Padding(4, 3, 4, 3);
            MapImage.Name = "MapImage";
            MapImage.Size = new Size(0, 0);
            MapImage.SizeMode = PictureBoxSizeMode.AutoSize;
            MapImage.TabIndex = 0;
            MapImage.TabStop = false;
            MapImage.Click += MapImage_Click;
            MapImage.MouseDown += MapImage_MouseDown;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(MiningPanel);
            tabPage1.Controls.Add(MiningFilter);
            tabPage1.Controls.Add(MiningTools);
            tabPage1.Controls.Add(toolStrip2);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(284, 517);
            tabPage1.TabIndex = 2;
            tabPage1.Text = "Mining";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // MiningPanel
            // 
            MiningPanel.AutoScroll = true;
            MiningPanel.Dock = DockStyle.Fill;
            MiningPanel.Location = new Point(0, 48);
            MiningPanel.Margin = new Padding(4, 3, 4, 3);
            MiningPanel.Name = "MiningPanel";
            MiningPanel.Size = new Size(284, 444);
            MiningPanel.TabIndex = 9;
            // 
            // MiningFilter
            // 
            MiningFilter.Dock = DockStyle.Top;
            MiningFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            MiningFilter.FormattingEnabled = true;
            MiningFilter.Location = new Point(0, 25);
            MiningFilter.Margin = new Padding(4, 3, 4, 3);
            MiningFilter.Name = "MiningFilter";
            MiningFilter.Size = new Size(284, 23);
            MiningFilter.TabIndex = 10;
            // 
            // MiningTools
            // 
            MiningTools.GripStyle = ToolStripGripStyle.Hidden;
            MiningTools.Items.AddRange(new ToolStripItem[] { MiningSelectAll, MiningSelectNone, MiningInvertSelection });
            MiningTools.Location = new Point(0, 0);
            MiningTools.Name = "MiningTools";
            MiningTools.RenderMode = ToolStripRenderMode.Professional;
            MiningTools.Size = new Size(284, 25);
            MiningTools.TabIndex = 8;
            MiningTools.Text = "toolStrip1";
            // 
            // MiningSelectAll
            // 
            MiningSelectAll.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MiningSelectAll.Image = (Image)resources.GetObject("MiningSelectAll.Image");
            MiningSelectAll.ImageTransparentColor = Color.Magenta;
            MiningSelectAll.Name = "MiningSelectAll";
            MiningSelectAll.Size = new Size(23, 22);
            MiningSelectAll.Text = "Select All";
            // 
            // MiningSelectNone
            // 
            MiningSelectNone.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MiningSelectNone.Image = (Image)resources.GetObject("MiningSelectNone.Image");
            MiningSelectNone.ImageTransparentColor = Color.Magenta;
            MiningSelectNone.Name = "MiningSelectNone";
            MiningSelectNone.Size = new Size(23, 22);
            MiningSelectNone.Text = "Select None";
            // 
            // MiningInvertSelection
            // 
            MiningInvertSelection.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MiningInvertSelection.Image = (Image)resources.GetObject("MiningInvertSelection.Image");
            MiningInvertSelection.ImageTransparentColor = Color.Magenta;
            MiningInvertSelection.Name = "MiningInvertSelection";
            MiningInvertSelection.Size = new Size(23, 22);
            MiningInvertSelection.Text = "Invert Selection";
            // 
            // MiningRemoveSelected
            // 
            MiningRemoveSelected.Alignment = ToolStripItemAlignment.Right;
            MiningRemoveSelected.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MiningRemoveSelected.Image = (Image)resources.GetObject("MiningRemoveSelected.Image");
            MiningRemoveSelected.ImageTransparentColor = Color.Magenta;
            MiningRemoveSelected.Name = "MiningRemoveSelected";
            MiningRemoveSelected.Size = new Size(23, 22);
            MiningRemoveSelected.Text = "Remove Selected";
            // 
            // toolStrip2
            // 
            toolStrip2.Dock = DockStyle.Bottom;
            toolStrip2.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip2.Items.AddRange(new ToolStripItem[] { MiningRemoveSelected, MiningHideRegion, MiningShowRegion, toolStripSeparator1, MiningFocusRegion });
            toolStrip2.Location = new Point(0, 492);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.RenderMode = ToolStripRenderMode.Professional;
            toolStrip2.Size = new Size(284, 25);
            toolStrip2.TabIndex = 11;
            toolStrip2.Text = "toolStrip2";
            // 
            // MiningHideRegion
            // 
            MiningHideRegion.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MiningHideRegion.Image = (Image)resources.GetObject("MiningHideRegion.Image");
            MiningHideRegion.ImageTransparentColor = Color.Magenta;
            MiningHideRegion.Name = "MiningHideRegion";
            MiningHideRegion.Size = new Size(23, 22);
            MiningHideRegion.Text = "Hide Region";
            // 
            // MiningShowRegion
            // 
            MiningShowRegion.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MiningShowRegion.Image = (Image)resources.GetObject("MiningShowRegion.Image");
            MiningShowRegion.ImageTransparentColor = Color.Magenta;
            MiningShowRegion.Name = "MiningShowRegion";
            MiningShowRegion.Size = new Size(23, 22);
            MiningShowRegion.Text = "Show Region";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // MiningFocusRegion
            // 
            MiningFocusRegion.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MiningFocusRegion.Image = (Image)resources.GetObject("MiningFocusRegion.Image");
            MiningFocusRegion.ImageTransparentColor = Color.Magenta;
            MiningFocusRegion.Name = "MiningFocusRegion";
            MiningFocusRegion.Size = new Size(23, 22);
            MiningFocusRegion.Text = "Focus Region";
            // 
            // VForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1101, 569);
            Controls.Add(mapContainer1);
            Controls.Add(splitter1);
            Controls.Add(RegionTabs);
            Controls.Add(Tool);
            Controls.Add(statusStrip1);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "VForm";
            Text = "Visualizer";
            FormClosing += VForm_FormClosing;
            Load += VForm_Load;
            Tool.ResumeLayout(false);
            Tool.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            toolStrip3.ResumeLayout(false);
            toolStrip3.PerformLayout();
            RespawnTools.ResumeLayout(false);
            RespawnTools.PerformLayout();
            RegionTabs.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            mapContainer1.ResumeLayout(false);
            mapContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)MapImage).EndInit();
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            MiningTools.ResumeLayout(false);
            MiningTools.PerformLayout();
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip Tool;
        private System.Windows.Forms.ToolStripButton SelectButton;
        private System.Windows.Forms.ToolStripButton AddButton;
        private System.Windows.Forms.ToolStripButton MoveButton;
        private System.Windows.Forms.ToolStripButton ResizeButton;
        private Server.MirForms.Control.MapContainer mapContainer1;
        private System.Windows.Forms.PictureBox MapImage;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel RespawnPanel;
        private System.Windows.Forms.ToolStrip RespawnTools;
        private System.Windows.Forms.TabControl RegionTabs;
        private System.Windows.Forms.ToolStripButton EndFocus;
        private System.Windows.Forms.ToolStripSeparator FocusBreak;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel MapDetailsLabel;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton RespawnsRemoveSelected;
        private System.Windows.Forms.ToolStripButton ResapwnsHideRegion;
        private System.Windows.Forms.ToolStripButton ResapwnsShowRegion;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton ResapwnsFocusRegion;
        private System.Windows.Forms.ComboBox RespawnsFilter;
        private System.Windows.Forms.ToolStripButton RespawnsInvertSelection;
        private System.Windows.Forms.ToolStripButton RespawnsSelectAll;
        private System.Windows.Forms.ToolStripButton RespawnsSelectNone;
        private TabPage tabPage1;
        private Panel MiningPanel;
        private ComboBox MiningFilter;
        private ToolStrip MiningTools;
        private ToolStripButton MiningSelectAll;
        private ToolStripButton MiningSelectNone;
        private ToolStripButton MiningInvertSelection;
        private ToolStrip toolStrip2;
        private ToolStripButton MiningRemoveSelected;
        private ToolStripButton MiningHideRegion;
        private ToolStripButton MiningShowRegion;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton MiningFocusRegion;
    }
}