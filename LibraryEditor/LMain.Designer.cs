using System;

namespace LibraryEditor
{
    partial class LMain
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LMain));
            ImageList = new ImageList(components);
            OpenLibraryDialog = new OpenFileDialog();
            SaveLibraryDialog = new SaveFileDialog();
            ImportImageDialog = new OpenFileDialog();
            OpenWeMadeDialog = new OpenFileDialog();
            toolTip = new ToolTip(components);
            pictureBox = new PictureBox();
            statusStrip = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            toolStripProgressBar = new ToolStripProgressBar();
            FolderLibraryDialog = new FolderBrowserDialog();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            openReferenceFileToolStripMenuItem = new ToolStripMenuItem();
            openReferenceImageToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripSeparator();
            closeToolStripMenuItem = new ToolStripMenuItem();
            functionsToolStripMenuItem = new ToolStripMenuItem();
            copyToToolStripMenuItem = new ToolStripMenuItem();
            countBlanksToolStripMenuItem = new ToolStripMenuItem();
            removeBlanksToolStripMenuItem = new ToolStripMenuItem();
            safeToolStripMenuItem = new ToolStripMenuItem();
            convertToolStripMenuItem = new ToolStripMenuItem();
            importShadowsToolStripMenuItem = new ToolStripMenuItem();
            skinToolStripMenuItem = new ToolStripMenuItem();
            MainMenu = new MenuStrip();
            tabControl = new TabControl();
            tabImages = new TabPage();
            PreviewListView = new CustomFormControl.FixedListView();
            splitContainer2 = new SplitContainer();
            BtnYPlusTen = new Button();
            BtnYPlusFive = new Button();
            BtnYPlusOne = new Button();
            BtnYMinusOne = new Button();
            BtnYMinusFive = new Button();
            BtnYMinusTen = new Button();
            BtnXPlusTen = new Button();
            BtnXPlusFive = new Button();
            BtnXPlusOne = new Button();
            BtnXMinusOne = new Button();
            BtnXMinusFive = new Button();
            BtnXMinusTen = new Button();
            MobBackgroundPicBox = new PictureBox();
            MobChkBox = new CheckBox();
            LockOffsetChkBox = new CheckBox();
            numericUpDownY = new NumericUpDown();
            numericUpDownX = new NumericUpDown();
            groupBox1 = new GroupBox();
            RButtonOverlay = new RadioButton();
            RButtonImage = new RadioButton();
            checkboxRemoveBlackOnImport = new CheckBox();
            nudJump = new NumericUpDown();
            checkBoxPreventAntiAliasing = new CheckBox();
            checkBoxQuality = new CheckBox();
            buttonSkipPrevious = new Button();
            buttonSkipNext = new Button();
            buttonReplace = new Button();
            ZoomTrackBar = new TrackBar();
            ExportButton = new Button();
            InsertImageButton = new Button();
            DeleteButton = new Button();
            AddButton = new Button();
            label10 = new Label();
            label8 = new Label();
            HeightLabel = new Label();
            label6 = new Label();
            WidthLabel = new Label();
            label1 = new Label();
            panel = new Panel();
            ImageBox = new PictureBox();
            BackgroundPicBox1 = new PictureBox();
            splitContainer1 = new SplitContainer();
            splitContainer3 = new SplitContainer();
            splitContainer4 = new SplitContainer();
            OpenBtn = new Button();
            PathTxtBox = new TextBox();
            label2 = new Label();
            TreeBrowser = new TreeView();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            statusStrip.SuspendLayout();
            MainMenu.SuspendLayout();
            tabControl.SuspendLayout();
            tabImages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MobBackgroundPicBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownX).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudJump).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ZoomTrackBar).BeginInit();
            panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ImageBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BackgroundPicBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer4).BeginInit();
            splitContainer4.Panel1.SuspendLayout();
            splitContainer4.Panel2.SuspendLayout();
            splitContainer4.SuspendLayout();
            SuspendLayout();
            // 
            // ImageList
            // 
            ImageList.ColorDepth = ColorDepth.Depth32Bit;
            ImageList.ImageSize = new Size(64, 64);
            ImageList.TransparentColor = Color.Transparent;
            // 
            // OpenLibraryDialog
            // 
            OpenLibraryDialog.Filter = "Library|*.Lib";
            // 
            // SaveLibraryDialog
            // 
            SaveLibraryDialog.Filter = "Library|*.Lib";
            // 
            // ImportImageDialog
            // 
            ImportImageDialog.Filter = "Images (*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            ImportImageDialog.Multiselect = true;
            // 
            // OpenWeMadeDialog
            // 
            OpenWeMadeDialog.Filter = "WeMade|*.Wil;*.Wtl|Shanda|*.Wzl;*.Miz|Lib|*.Lib";
            OpenWeMadeDialog.Multiselect = true;
            // 
            // pictureBox
            // 
            pictureBox.Image = (Image)resources.GetObject("pictureBox.Image");
            pictureBox.Location = new Point(10, 9);
            pictureBox.Margin = new Padding(4, 3, 4, 3);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(16, 16);
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox.TabIndex = 14;
            pictureBox.TabStop = false;
            toolTip.SetToolTip(pictureBox, "Switch from Black to White background.");
            pictureBox.Click += pictureBox_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel, toolStripProgressBar });
            statusStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            statusStrip.Location = new Point(0, 706);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(1, 0, 16, 0);
            statusStrip.Size = new Size(1369, 24);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "statusStrip";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(90, 19);
            toolStripStatusLabel.Text = "Selected Image:";
            // 
            // toolStripProgressBar
            // 
            toolStripProgressBar.Alignment = ToolStripItemAlignment.Right;
            toolStripProgressBar.Name = "toolStripProgressBar";
            toolStripProgressBar.Size = new Size(233, 18);
            toolStripProgressBar.Step = 1;
            toolStripProgressBar.Style = ProgressBarStyle.Continuous;
            // 
            // FolderLibraryDialog
            // 
            FolderLibraryDialog.ShowNewFolderButton = false;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, toolStripMenuItem1, openReferenceFileToolStripMenuItem, openReferenceImageToolStripMenuItem, toolStripSeparator1, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripMenuItem2, closeToolStripMenuItem });
            fileToolStripMenuItem.Image = (Image)resources.GetObject("fileToolStripMenuItem.Image");
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(53, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Image = (Image)resources.GetObject("newToolStripMenuItem.Image");
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new Size(194, 22);
            newToolStripMenuItem.Text = "New";
            newToolStripMenuItem.ToolTipText = "New .Lib";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Image = (Image)resources.GetObject("openToolStripMenuItem.Image");
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(194, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.ToolTipText = "Open Shanda or Wemade files.";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(191, 6);
            // 
            // openReferenceFileToolStripMenuItem
            // 
            openReferenceFileToolStripMenuItem.Name = "openReferenceFileToolStripMenuItem";
            openReferenceFileToolStripMenuItem.Size = new Size(194, 22);
            openReferenceFileToolStripMenuItem.Text = "Open Reference File";
            openReferenceFileToolStripMenuItem.Click += openReferenceFileToolStripMenuItem_Click;
            // 
            // openReferenceImageToolStripMenuItem
            // 
            openReferenceImageToolStripMenuItem.Name = "openReferenceImageToolStripMenuItem";
            openReferenceImageToolStripMenuItem.Size = new Size(194, 22);
            openReferenceImageToolStripMenuItem.Text = "Open Reference Image";
            openReferenceImageToolStripMenuItem.Click += openReferenceImageToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(191, 6);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Image = (Image)resources.GetObject("saveToolStripMenuItem.Image");
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(194, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.ToolTipText = "Saves currently open .Lib";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Image = (Image)resources.GetObject("saveAsToolStripMenuItem.Image");
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(194, 22);
            saveAsToolStripMenuItem.Text = "Save As";
            saveAsToolStripMenuItem.ToolTipText = ".Lib Only.";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(191, 6);
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Image = (Image)resources.GetObject("closeToolStripMenuItem.Image");
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(194, 22);
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.ToolTipText = "Exit Application.";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // functionsToolStripMenuItem
            // 
            functionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { copyToToolStripMenuItem, countBlanksToolStripMenuItem, removeBlanksToolStripMenuItem, convertToolStripMenuItem, importShadowsToolStripMenuItem });
            functionsToolStripMenuItem.Image = (Image)resources.GetObject("functionsToolStripMenuItem.Image");
            functionsToolStripMenuItem.Name = "functionsToolStripMenuItem";
            functionsToolStripMenuItem.Size = new Size(87, 20);
            functionsToolStripMenuItem.Text = "Functions";
            // 
            // copyToToolStripMenuItem
            // 
            copyToToolStripMenuItem.Image = (Image)resources.GetObject("copyToToolStripMenuItem.Image");
            copyToToolStripMenuItem.Name = "copyToToolStripMenuItem";
            copyToToolStripMenuItem.Size = new Size(160, 22);
            copyToToolStripMenuItem.Text = "Copy To..";
            copyToToolStripMenuItem.ToolTipText = "Copy to a new .Lib or to the end of an exsisting one.";
            copyToToolStripMenuItem.Click += copyToToolStripMenuItem_Click;
            // 
            // countBlanksToolStripMenuItem
            // 
            countBlanksToolStripMenuItem.Image = (Image)resources.GetObject("countBlanksToolStripMenuItem.Image");
            countBlanksToolStripMenuItem.Name = "countBlanksToolStripMenuItem";
            countBlanksToolStripMenuItem.Size = new Size(160, 22);
            countBlanksToolStripMenuItem.Text = "Count Blanks";
            countBlanksToolStripMenuItem.ToolTipText = "Counts the blank images in the .Lib";
            countBlanksToolStripMenuItem.Click += countBlanksToolStripMenuItem_Click;
            // 
            // removeBlanksToolStripMenuItem
            // 
            removeBlanksToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { safeToolStripMenuItem });
            removeBlanksToolStripMenuItem.Image = (Image)resources.GetObject("removeBlanksToolStripMenuItem.Image");
            removeBlanksToolStripMenuItem.Name = "removeBlanksToolStripMenuItem";
            removeBlanksToolStripMenuItem.Size = new Size(160, 22);
            removeBlanksToolStripMenuItem.Text = "Remove Blanks";
            removeBlanksToolStripMenuItem.ToolTipText = "Quick removal of blanks.";
            removeBlanksToolStripMenuItem.Click += removeBlanksToolStripMenuItem_Click;
            // 
            // safeToolStripMenuItem
            // 
            safeToolStripMenuItem.Image = (Image)resources.GetObject("safeToolStripMenuItem.Image");
            safeToolStripMenuItem.Name = "safeToolStripMenuItem";
            safeToolStripMenuItem.Size = new Size(96, 22);
            safeToolStripMenuItem.Text = "Safe";
            safeToolStripMenuItem.ToolTipText = "Use the safe method of removing blanks.";
            safeToolStripMenuItem.Click += safeToolStripMenuItem_Click;
            // 
            // convertToolStripMenuItem
            // 
            convertToolStripMenuItem.Image = (Image)resources.GetObject("convertToolStripMenuItem.Image");
            convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            convertToolStripMenuItem.Size = new Size(160, 22);
            convertToolStripMenuItem.Text = "Converter";
            convertToolStripMenuItem.ToolTipText = "Convert Wil/Wzl/Miz to .Lib";
            convertToolStripMenuItem.Visible = false;
            convertToolStripMenuItem.Click += convertToolStripMenuItem_Click;
            // 
            // importShadowsToolStripMenuItem
            // 
            importShadowsToolStripMenuItem.Name = "importShadowsToolStripMenuItem";
            importShadowsToolStripMenuItem.Size = new Size(160, 22);
            importShadowsToolStripMenuItem.Text = "Import Shadows";
            importShadowsToolStripMenuItem.Click += importShadowsToolStripMenuItem_Click;
            // 
            // skinToolStripMenuItem
            // 
            skinToolStripMenuItem.Alignment = ToolStripItemAlignment.Right;
            skinToolStripMenuItem.Image = (Image)resources.GetObject("skinToolStripMenuItem.Image");
            skinToolStripMenuItem.Name = "skinToolStripMenuItem";
            skinToolStripMenuItem.Size = new Size(57, 20);
            skinToolStripMenuItem.Text = "Skin";
            skinToolStripMenuItem.Visible = false;
            // 
            // MainMenu
            // 
            MainMenu.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, functionsToolStripMenuItem, skinToolStripMenuItem });
            MainMenu.Location = new Point(0, 0);
            MainMenu.Name = "MainMenu";
            MainMenu.Padding = new Padding(7, 2, 0, 2);
            MainMenu.RenderMode = ToolStripRenderMode.Professional;
            MainMenu.Size = new Size(1369, 24);
            MainMenu.TabIndex = 0;
            MainMenu.Text = "menuStrip1";
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabImages);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 0);
            tabControl.Margin = new Padding(4, 3, 4, 3);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1367, 187);
            tabControl.TabIndex = 0;
            // 
            // tabImages
            // 
            tabImages.Controls.Add(PreviewListView);
            tabImages.Location = new Point(4, 24);
            tabImages.Margin = new Padding(4, 3, 4, 3);
            tabImages.Name = "tabImages";
            tabImages.Padding = new Padding(4, 3, 4, 3);
            tabImages.Size = new Size(1359, 159);
            tabImages.TabIndex = 0;
            tabImages.Text = "Images";
            tabImages.UseVisualStyleBackColor = true;
            // 
            // PreviewListView
            // 
            PreviewListView.Activation = ItemActivation.OneClick;
            PreviewListView.BackColor = Color.GhostWhite;
            PreviewListView.Dock = DockStyle.Fill;
            PreviewListView.ForeColor = Color.FromArgb(142, 152, 156);
            PreviewListView.LargeImageList = ImageList;
            PreviewListView.Location = new Point(4, 3);
            PreviewListView.Margin = new Padding(4, 3, 4, 3);
            PreviewListView.Name = "PreviewListView";
            PreviewListView.Size = new Size(1351, 153);
            PreviewListView.TabIndex = 0;
            PreviewListView.UseCompatibleStateImageBehavior = false;
            PreviewListView.VirtualMode = true;
            PreviewListView.RetrieveVirtualItem += PreviewListView_RetrieveVirtualItem;
            PreviewListView.SelectedIndexChanged += PreviewListView_SelectedIndexChanged;
            PreviewListView.VirtualItemsSelectionRangeChanged += PreviewListView_VirtualItemsSelectionRangeChanged;
            // 
            // splitContainer2
            // 
            splitContainer2.BorderStyle = BorderStyle.FixedSingle;
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.FixedPanel = FixedPanel.Panel1;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Margin = new Padding(4, 3, 4, 3);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(BtnYPlusTen);
            splitContainer2.Panel1.Controls.Add(BtnYPlusFive);
            splitContainer2.Panel1.Controls.Add(BtnYPlusOne);
            splitContainer2.Panel1.Controls.Add(BtnYMinusOne);
            splitContainer2.Panel1.Controls.Add(BtnYMinusFive);
            splitContainer2.Panel1.Controls.Add(BtnYMinusTen);
            splitContainer2.Panel1.Controls.Add(BtnXPlusTen);
            splitContainer2.Panel1.Controls.Add(BtnXPlusFive);
            splitContainer2.Panel1.Controls.Add(BtnXPlusOne);
            splitContainer2.Panel1.Controls.Add(BtnXMinusOne);
            splitContainer2.Panel1.Controls.Add(BtnXMinusFive);
            splitContainer2.Panel1.Controls.Add(BtnXMinusTen);
            splitContainer2.Panel1.Controls.Add(MobBackgroundPicBox);
            splitContainer2.Panel1.Controls.Add(MobChkBox);
            splitContainer2.Panel1.Controls.Add(LockOffsetChkBox);
            splitContainer2.Panel1.Controls.Add(numericUpDownY);
            splitContainer2.Panel1.Controls.Add(numericUpDownX);
            splitContainer2.Panel1.Controls.Add(groupBox1);
            splitContainer2.Panel1.Controls.Add(checkboxRemoveBlackOnImport);
            splitContainer2.Panel1.Controls.Add(nudJump);
            splitContainer2.Panel1.Controls.Add(checkBoxPreventAntiAliasing);
            splitContainer2.Panel1.Controls.Add(checkBoxQuality);
            splitContainer2.Panel1.Controls.Add(buttonSkipPrevious);
            splitContainer2.Panel1.Controls.Add(buttonSkipNext);
            splitContainer2.Panel1.Controls.Add(buttonReplace);
            splitContainer2.Panel1.Controls.Add(pictureBox);
            splitContainer2.Panel1.Controls.Add(ZoomTrackBar);
            splitContainer2.Panel1.Controls.Add(ExportButton);
            splitContainer2.Panel1.Controls.Add(InsertImageButton);
            splitContainer2.Panel1.Controls.Add(DeleteButton);
            splitContainer2.Panel1.Controls.Add(AddButton);
            splitContainer2.Panel1.Controls.Add(label10);
            splitContainer2.Panel1.Controls.Add(label8);
            splitContainer2.Panel1.Controls.Add(HeightLabel);
            splitContainer2.Panel1.Controls.Add(label6);
            splitContainer2.Panel1.Controls.Add(WidthLabel);
            splitContainer2.Panel1.Controls.Add(label1);
            splitContainer2.Panel1.ForeColor = Color.Black;
            splitContainer2.Panel1MinSize = 240;
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(panel);
            splitContainer2.Size = new Size(963, 486);
            splitContainer2.SplitterDistance = 247;
            splitContainer2.SplitterWidth = 5;
            splitContainer2.TabIndex = 0;
            // 
            // BtnYPlusTen
            // 
            BtnYPlusTen.Location = new Point(192, 149);
            BtnYPlusTen.Name = "BtnYPlusTen";
            BtnYPlusTen.Size = new Size(35, 26);
            BtnYPlusTen.TabIndex = 42;
            BtnYPlusTen.Text = "+10";
            BtnYPlusTen.UseVisualStyleBackColor = true;
            BtnYPlusTen.Click += BtnYPlusTen_Click;
            // 
            // BtnYPlusFive
            // 
            BtnYPlusFive.Location = new Point(156, 149);
            BtnYPlusFive.Name = "BtnYPlusFive";
            BtnYPlusFive.Size = new Size(35, 26);
            BtnYPlusFive.TabIndex = 41;
            BtnYPlusFive.Text = "+5";
            BtnYPlusFive.UseVisualStyleBackColor = true;
            BtnYPlusFive.Click += BtnYPlusFive_Click;
            // 
            // BtnYPlusOne
            // 
            BtnYPlusOne.Location = new Point(120, 149);
            BtnYPlusOne.Name = "BtnYPlusOne";
            BtnYPlusOne.Size = new Size(35, 26);
            BtnYPlusOne.TabIndex = 40;
            BtnYPlusOne.Text = "+1";
            BtnYPlusOne.UseVisualStyleBackColor = true;
            BtnYPlusOne.Click += BtnYPlusOne_Click;
            // 
            // BtnYMinusOne
            // 
            BtnYMinusOne.Location = new Point(84, 149);
            BtnYMinusOne.Name = "BtnYMinusOne";
            BtnYMinusOne.Size = new Size(35, 26);
            BtnYMinusOne.TabIndex = 39;
            BtnYMinusOne.Text = "-1";
            BtnYMinusOne.UseVisualStyleBackColor = true;
            BtnYMinusOne.Click += BtnYMinusOne_Click;
            // 
            // BtnYMinusFive
            // 
            BtnYMinusFive.Location = new Point(48, 149);
            BtnYMinusFive.Name = "BtnYMinusFive";
            BtnYMinusFive.Size = new Size(35, 26);
            BtnYMinusFive.TabIndex = 38;
            BtnYMinusFive.Text = "-5";
            BtnYMinusFive.UseVisualStyleBackColor = true;
            BtnYMinusFive.Click += BtnYMinusFive_Click;
            // 
            // BtnYMinusTen
            // 
            BtnYMinusTen.Location = new Point(12, 149);
            BtnYMinusTen.Name = "BtnYMinusTen";
            BtnYMinusTen.Size = new Size(35, 26);
            BtnYMinusTen.TabIndex = 37;
            BtnYMinusTen.Text = "-10";
            BtnYMinusTen.UseVisualStyleBackColor = true;
            BtnYMinusTen.Click += BtnYMinusTen_Click;
            // 
            // BtnXPlusTen
            // 
            BtnXPlusTen.Location = new Point(192, 91);
            BtnXPlusTen.Name = "BtnXPlusTen";
            BtnXPlusTen.Size = new Size(35, 26);
            BtnXPlusTen.TabIndex = 36;
            BtnXPlusTen.Text = "+10";
            BtnXPlusTen.UseVisualStyleBackColor = true;
            BtnXPlusTen.Click += BtnXPlusTen_Click;
            // 
            // BtnXPlusFive
            // 
            BtnXPlusFive.Location = new Point(156, 91);
            BtnXPlusFive.Name = "BtnXPlusFive";
            BtnXPlusFive.Size = new Size(35, 26);
            BtnXPlusFive.TabIndex = 35;
            BtnXPlusFive.Text = "+5";
            BtnXPlusFive.UseVisualStyleBackColor = true;
            BtnXPlusFive.Click += BtnXPlusFive_Click;
            // 
            // BtnXPlusOne
            // 
            BtnXPlusOne.Location = new Point(120, 91);
            BtnXPlusOne.Name = "BtnXPlusOne";
            BtnXPlusOne.Size = new Size(35, 26);
            BtnXPlusOne.TabIndex = 34;
            BtnXPlusOne.Text = "+1";
            BtnXPlusOne.UseVisualStyleBackColor = true;
            BtnXPlusOne.Click += BtnXPlusOne_Click;
            // 
            // BtnXMinusOne
            // 
            BtnXMinusOne.Location = new Point(84, 91);
            BtnXMinusOne.Name = "BtnXMinusOne";
            BtnXMinusOne.Size = new Size(35, 26);
            BtnXMinusOne.TabIndex = 33;
            BtnXMinusOne.Text = "-1";
            BtnXMinusOne.UseVisualStyleBackColor = true;
            BtnXMinusOne.Click += BtnXMinusOne_Click;
            // 
            // BtnXMinusFive
            // 
            BtnXMinusFive.Location = new Point(48, 91);
            BtnXMinusFive.Name = "BtnXMinusFive";
            BtnXMinusFive.Size = new Size(35, 26);
            BtnXMinusFive.TabIndex = 32;
            BtnXMinusFive.Text = "-5";
            BtnXMinusFive.UseVisualStyleBackColor = true;
            BtnXMinusFive.Click += BtnXMinusFive_Click;
            // 
            // BtnXMinusTen
            // 
            BtnXMinusTen.Location = new Point(12, 91);
            BtnXMinusTen.Name = "BtnXMinusTen";
            BtnXMinusTen.Size = new Size(35, 26);
            BtnXMinusTen.TabIndex = 3;
            BtnXMinusTen.Text = "-10";
            BtnXMinusTen.UseVisualStyleBackColor = true;
            BtnXMinusTen.Click += BtnXMinusTen_Click;
            // 
            // MobBackgroundPicBox
            // 
            MobBackgroundPicBox.Image = (Image)resources.GetObject("MobBackgroundPicBox.Image");
            MobBackgroundPicBox.Location = new Point(10, 29);
            MobBackgroundPicBox.Name = "MobBackgroundPicBox";
            MobBackgroundPicBox.Size = new Size(16, 16);
            MobBackgroundPicBox.SizeMode = PictureBoxSizeMode.StretchImage;
            MobBackgroundPicBox.TabIndex = 31;
            MobBackgroundPicBox.TabStop = false;
            MobBackgroundPicBox.Click += MobBackgroundPicBox_Click;
            // 
            // MobChkBox
            // 
            MobChkBox.AutoSize = true;
            MobChkBox.Location = new Point(95, 461);
            MobChkBox.Margin = new Padding(4, 3, 4, 3);
            MobChkBox.Name = "MobChkBox";
            MobChkBox.Size = new Size(56, 19);
            MobChkBox.TabIndex = 30;
            MobChkBox.Text = "Mob?";
            MobChkBox.UseVisualStyleBackColor = true;
            // 
            // LockOffsetChkBox
            // 
            LockOffsetChkBox.AutoSize = true;
            LockOffsetChkBox.Location = new Point(11, 461);
            LockOffsetChkBox.Margin = new Padding(4, 3, 4, 3);
            LockOffsetChkBox.Name = "LockOffsetChkBox";
            LockOffsetChkBox.Size = new Size(86, 19);
            LockOffsetChkBox.TabIndex = 29;
            LockOffsetChkBox.Text = "Lock Offset";
            LockOffsetChkBox.UseVisualStyleBackColor = true;
            // 
            // numericUpDownY
            // 
            numericUpDownY.Location = new Point(79, 123);
            numericUpDownY.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDownY.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            numericUpDownY.Name = "numericUpDownY";
            numericUpDownY.Size = new Size(65, 23);
            numericUpDownY.TabIndex = 27;
            numericUpDownY.ValueChanged += numericUpDownY_ValueChanged;
            // 
            // numericUpDownX
            // 
            numericUpDownX.Location = new Point(79, 65);
            numericUpDownX.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDownX.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            numericUpDownX.Name = "numericUpDownX";
            numericUpDownX.Size = new Size(65, 23);
            numericUpDownX.TabIndex = 26;
            numericUpDownX.ValueChanged += numericUpDownX_ValueChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(RButtonOverlay);
            groupBox1.Controls.Add(RButtonImage);
            groupBox1.Location = new Point(11, 368);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(215, 38);
            groupBox1.TabIndex = 23;
            groupBox1.TabStop = false;
            groupBox1.Text = "View Mode";
            // 
            // RButtonOverlay
            // 
            RButtonOverlay.AutoSize = true;
            RButtonOverlay.Location = new Point(79, 17);
            RButtonOverlay.Margin = new Padding(4, 3, 4, 3);
            RButtonOverlay.Name = "RButtonOverlay";
            RButtonOverlay.Size = new Size(65, 19);
            RButtonOverlay.TabIndex = 1;
            RButtonOverlay.Text = "Overlay";
            RButtonOverlay.UseVisualStyleBackColor = true;
            RButtonOverlay.CheckedChanged += RButtonViewMode_CheckedChanged;
            // 
            // RButtonImage
            // 
            RButtonImage.AutoSize = true;
            RButtonImage.Checked = true;
            RButtonImage.Location = new Point(8, 17);
            RButtonImage.Margin = new Padding(4, 3, 4, 3);
            RButtonImage.Name = "RButtonImage";
            RButtonImage.Size = new Size(58, 19);
            RButtonImage.TabIndex = 0;
            RButtonImage.TabStop = true;
            RButtonImage.Text = "Image";
            RButtonImage.UseVisualStyleBackColor = true;
            RButtonImage.CheckedChanged += RButtonViewMode_CheckedChanged;
            // 
            // checkboxRemoveBlackOnImport
            // 
            checkboxRemoveBlackOnImport.AutoSize = true;
            checkboxRemoveBlackOnImport.Checked = true;
            checkboxRemoveBlackOnImport.CheckState = CheckState.Checked;
            checkboxRemoveBlackOnImport.Location = new Point(11, 415);
            checkboxRemoveBlackOnImport.Margin = new Padding(4, 3, 4, 3);
            checkboxRemoveBlackOnImport.Name = "checkboxRemoveBlackOnImport";
            checkboxRemoveBlackOnImport.Size = new Size(158, 19);
            checkboxRemoveBlackOnImport.TabIndex = 22;
            checkboxRemoveBlackOnImport.Text = "Remove Black On Import";
            checkboxRemoveBlackOnImport.UseVisualStyleBackColor = true;
            // 
            // nudJump
            // 
            nudJump.Location = new Point(77, 300);
            nudJump.Margin = new Padding(4, 3, 4, 3);
            nudJump.Maximum = new decimal(new int[] { 650000, 0, 0, 0 });
            nudJump.Name = "nudJump";
            nudJump.Size = new Size(77, 23);
            nudJump.TabIndex = 21;
            nudJump.ValueChanged += nudJump_ValueChanged;
            nudJump.KeyDown += nudJump_KeyDown;
            // 
            // checkBoxPreventAntiAliasing
            // 
            checkBoxPreventAntiAliasing.AutoSize = true;
            checkBoxPreventAntiAliasing.Location = new Point(95, 438);
            checkBoxPreventAntiAliasing.Margin = new Padding(4, 3, 4, 3);
            checkBoxPreventAntiAliasing.Name = "checkBoxPreventAntiAliasing";
            checkBoxPreventAntiAliasing.Size = new Size(112, 19);
            checkBoxPreventAntiAliasing.TabIndex = 20;
            checkBoxPreventAntiAliasing.Text = "No Anti-aliasing";
            checkBoxPreventAntiAliasing.UseVisualStyleBackColor = true;
            checkBoxPreventAntiAliasing.CheckedChanged += checkBoxPreventAntiAliasing_CheckedChanged;
            // 
            // checkBoxQuality
            // 
            checkBoxQuality.AutoSize = true;
            checkBoxQuality.Location = new Point(11, 438);
            checkBoxQuality.Margin = new Padding(4, 3, 4, 3);
            checkBoxQuality.Name = "checkBoxQuality";
            checkBoxQuality.Size = new Size(87, 19);
            checkBoxQuality.TabIndex = 19;
            checkBoxQuality.Text = "No Blurring";
            checkBoxQuality.UseVisualStyleBackColor = true;
            checkBoxQuality.CheckedChanged += checkBoxQuality_CheckedChanged;
            // 
            // buttonSkipPrevious
            // 
            buttonSkipPrevious.ForeColor = SystemColors.ControlText;
            buttonSkipPrevious.Image = (Image)resources.GetObject("buttonSkipPrevious.Image");
            buttonSkipPrevious.Location = new Point(42, 297);
            buttonSkipPrevious.Margin = new Padding(4, 3, 4, 3);
            buttonSkipPrevious.Name = "buttonSkipPrevious";
            buttonSkipPrevious.Size = new Size(30, 26);
            buttonSkipPrevious.TabIndex = 17;
            buttonSkipPrevious.Tag = "";
            buttonSkipPrevious.TextImageRelation = TextImageRelation.TextBeforeImage;
            buttonSkipPrevious.UseVisualStyleBackColor = true;
            buttonSkipPrevious.Click += buttonSkipPrevious_Click;
            // 
            // buttonSkipNext
            // 
            buttonSkipNext.ForeColor = SystemColors.ControlText;
            buttonSkipNext.Image = (Image)resources.GetObject("buttonSkipNext.Image");
            buttonSkipNext.Location = new Point(159, 297);
            buttonSkipNext.Margin = new Padding(4, 3, 4, 3);
            buttonSkipNext.Name = "buttonSkipNext";
            buttonSkipNext.Size = new Size(30, 26);
            buttonSkipNext.TabIndex = 16;
            buttonSkipNext.Tag = "";
            buttonSkipNext.TextImageRelation = TextImageRelation.TextBeforeImage;
            buttonSkipNext.UseVisualStyleBackColor = true;
            buttonSkipNext.Click += buttonSkipNext_Click;
            // 
            // buttonReplace
            // 
            buttonReplace.ForeColor = SystemColors.ControlText;
            buttonReplace.Image = (Image)resources.GetObject("buttonReplace.Image");
            buttonReplace.ImageAlign = ContentAlignment.TopRight;
            buttonReplace.Location = new Point(10, 225);
            buttonReplace.Margin = new Padding(4, 3, 4, 3);
            buttonReplace.Name = "buttonReplace";
            buttonReplace.Size = new Size(105, 26);
            buttonReplace.TabIndex = 15;
            buttonReplace.Tag = "";
            buttonReplace.Text = "Replace Image";
            buttonReplace.TextImageRelation = TextImageRelation.TextBeforeImage;
            buttonReplace.UseVisualStyleBackColor = true;
            buttonReplace.Click += buttonReplace_Click;
            // 
            // ZoomTrackBar
            // 
            ZoomTrackBar.LargeChange = 1;
            ZoomTrackBar.Location = new Point(42, 329);
            ZoomTrackBar.Margin = new Padding(4, 3, 4, 3);
            ZoomTrackBar.Minimum = 1;
            ZoomTrackBar.Name = "ZoomTrackBar";
            ZoomTrackBar.Size = new Size(147, 45);
            ZoomTrackBar.TabIndex = 4;
            ZoomTrackBar.TickStyle = TickStyle.TopLeft;
            ZoomTrackBar.Value = 1;
            ZoomTrackBar.Scroll += ZoomTrackBar_Scroll;
            // 
            // ExportButton
            // 
            ExportButton.ForeColor = SystemColors.ControlText;
            ExportButton.Image = (Image)resources.GetObject("ExportButton.Image");
            ExportButton.ImageAlign = ContentAlignment.TopRight;
            ExportButton.Location = new Point(121, 257);
            ExportButton.Margin = new Padding(3, 3, 4, 3);
            ExportButton.Name = "ExportButton";
            ExportButton.Size = new Size(105, 26);
            ExportButton.TabIndex = 3;
            ExportButton.Tag = "";
            ExportButton.Text = "Export Images";
            ExportButton.TextImageRelation = TextImageRelation.TextBeforeImage;
            ExportButton.UseVisualStyleBackColor = true;
            ExportButton.Click += ExportButton_Click;
            // 
            // InsertImageButton
            // 
            InsertImageButton.ForeColor = SystemColors.ControlText;
            InsertImageButton.Image = (Image)resources.GetObject("InsertImageButton.Image");
            InsertImageButton.ImageAlign = ContentAlignment.TopRight;
            InsertImageButton.Location = new Point(121, 225);
            InsertImageButton.Margin = new Padding(4, 3, 4, 3);
            InsertImageButton.Name = "InsertImageButton";
            InsertImageButton.Size = new Size(105, 26);
            InsertImageButton.TabIndex = 1;
            InsertImageButton.Tag = "";
            InsertImageButton.Text = "Insert Images";
            InsertImageButton.TextImageRelation = TextImageRelation.TextBeforeImage;
            InsertImageButton.UseVisualStyleBackColor = true;
            InsertImageButton.Click += InsertImageButton_Click;
            // 
            // DeleteButton
            // 
            DeleteButton.ForeColor = SystemColors.ControlText;
            DeleteButton.Image = (Image)resources.GetObject("DeleteButton.Image");
            DeleteButton.ImageAlign = ContentAlignment.TopRight;
            DeleteButton.Location = new Point(121, 193);
            DeleteButton.Margin = new Padding(4, 3, 4, 3);
            DeleteButton.Name = "DeleteButton";
            DeleteButton.Size = new Size(105, 26);
            DeleteButton.TabIndex = 2;
            DeleteButton.Tag = "";
            DeleteButton.Text = "Delete Images";
            DeleteButton.TextImageRelation = TextImageRelation.TextBeforeImage;
            DeleteButton.UseVisualStyleBackColor = true;
            DeleteButton.Click += DeleteButton_Click;
            // 
            // AddButton
            // 
            AddButton.ForeColor = SystemColors.ControlText;
            AddButton.Image = (Image)resources.GetObject("AddButton.Image");
            AddButton.ImageAlign = ContentAlignment.TopRight;
            AddButton.Location = new Point(10, 193);
            AddButton.Margin = new Padding(4, 3, 4, 3);
            AddButton.Name = "AddButton";
            AddButton.Size = new Size(105, 26);
            AddButton.TabIndex = 0;
            AddButton.Tag = "";
            AddButton.Text = "Add Images";
            AddButton.TextImageRelation = TextImageRelation.TextBeforeImage;
            AddButton.UseVisualStyleBackColor = true;
            AddButton.Click += AddButton_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.ForeColor = SystemColors.ControlText;
            label10.Location = new Point(23, 126);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(53, 15);
            label10.TabIndex = 12;
            label10.Text = "OffSet Y:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = SystemColors.ControlText;
            label8.Location = new Point(23, 68);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(53, 15);
            label8.TabIndex = 11;
            label8.Text = "OffSet X:";
            // 
            // HeightLabel
            // 
            HeightLabel.AutoSize = true;
            HeightLabel.ForeColor = SystemColors.ControlText;
            HeightLabel.Location = new Point(123, 30);
            HeightLabel.Margin = new Padding(4, 0, 4, 0);
            HeightLabel.Name = "HeightLabel";
            HeightLabel.Size = new Size(75, 15);
            HeightLabel.TabIndex = 10;
            HeightLabel.Text = "<No Image>";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = SystemColors.ControlText;
            label6.Location = new Point(76, 30);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(46, 15);
            label6.TabIndex = 9;
            label6.Text = "Height:";
            // 
            // WidthLabel
            // 
            WidthLabel.AutoSize = true;
            WidthLabel.ForeColor = SystemColors.ControlText;
            WidthLabel.Location = new Point(123, 12);
            WidthLabel.Margin = new Padding(4, 0, 4, 0);
            WidthLabel.Name = "WidthLabel";
            WidthLabel.Size = new Size(75, 15);
            WidthLabel.TabIndex = 8;
            WidthLabel.Text = "<No Image>";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlText;
            label1.Location = new Point(79, 12);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(42, 15);
            label1.TabIndex = 7;
            label1.Text = "Width:";
            // 
            // panel
            // 
            panel.AutoScroll = true;
            panel.BackColor = Color.Black;
            panel.BorderStyle = BorderStyle.Fixed3D;
            panel.Controls.Add(ImageBox);
            panel.Controls.Add(BackgroundPicBox1);
            panel.Dock = DockStyle.Fill;
            panel.Location = new Point(0, 0);
            panel.Margin = new Padding(4, 3, 4, 3);
            panel.Name = "panel";
            panel.Size = new Size(709, 484);
            panel.TabIndex = 1;
            // 
            // ImageBox
            // 
            ImageBox.BackColor = Color.Transparent;
            ImageBox.Location = new Point(0, 0);
            ImageBox.Margin = new Padding(4, 3, 4, 3);
            ImageBox.Name = "ImageBox";
            ImageBox.Size = new Size(64, 64);
            ImageBox.SizeMode = PictureBoxSizeMode.AutoSize;
            ImageBox.TabIndex = 0;
            ImageBox.TabStop = false;
            // 
            // BackgroundPicBox1
            // 
            BackgroundPicBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BackgroundPicBox1.BackColor = Color.Transparent;
            BackgroundPicBox1.Image = (Image)resources.GetObject("BackgroundPicBox1.Image");
            BackgroundPicBox1.Location = new Point(0, 0);
            BackgroundPicBox1.Name = "BackgroundPicBox1";
            BackgroundPicBox1.Size = new Size(919, 645);
            BackgroundPicBox1.TabIndex = 0;
            BackgroundPicBox1.TabStop = false;
            BackgroundPicBox1.Visible = false;
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.FixedSingle;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 24);
            splitContainer1.Margin = new Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer3);
            splitContainer1.Panel1MinSize = 325;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControl);
            splitContainer1.Size = new Size(1369, 682);
            splitContainer1.SplitterDistance = 488;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 1;
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.Location = new Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(splitContainer4);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(splitContainer2);
            splitContainer3.Size = new Size(1367, 486);
            splitContainer3.SplitterDistance = 400;
            splitContainer3.TabIndex = 1;
            // 
            // splitContainer4
            // 
            splitContainer4.Dock = DockStyle.Fill;
            splitContainer4.Location = new Point(0, 0);
            splitContainer4.Name = "splitContainer4";
            splitContainer4.Orientation = Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            splitContainer4.Panel1.Controls.Add(OpenBtn);
            splitContainer4.Panel1.Controls.Add(PathTxtBox);
            splitContainer4.Panel1.Controls.Add(label2);
            // 
            // splitContainer4.Panel2
            // 
            splitContainer4.Panel2.Controls.Add(TreeBrowser);
            splitContainer4.Size = new Size(400, 486);
            splitContainer4.SplitterDistance = 37;
            splitContainer4.TabIndex = 0;
            // 
            // OpenBtn
            // 
            OpenBtn.Location = new Point(333, 7);
            OpenBtn.Name = "OpenBtn";
            OpenBtn.Size = new Size(59, 23);
            OpenBtn.TabIndex = 2;
            OpenBtn.Text = "Search";
            OpenBtn.UseVisualStyleBackColor = true;
            OpenBtn.Click += OpenBtn_Click;
            // 
            // PathTxtBox
            // 
            PathTxtBox.Location = new Point(73, 7);
            PathTxtBox.Name = "PathTxtBox";
            PathTxtBox.Size = new Size(254, 23);
            PathTxtBox.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 11);
            label2.Name = "label2";
            label2.Size = new Size(61, 15);
            label2.TabIndex = 0;
            label2.Text = "Directory :";
            // 
            // TreeBrowser
            // 
            TreeBrowser.Dock = DockStyle.Fill;
            TreeBrowser.Location = new Point(0, 0);
            TreeBrowser.Name = "TreeBrowser";
            TreeBrowser.Size = new Size(400, 445);
            TreeBrowser.TabIndex = 0;
            // 
            // LMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1369, 730);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip);
            Controls.Add(MainMenu);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = MainMenu;
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(756, 513);
            Name = "LMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Legend of Mir 1 Library Editor";
            Resize += LMain_Resize;
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            MainMenu.ResumeLayout(false);
            MainMenu.PerformLayout();
            tabControl.ResumeLayout(false);
            tabImages.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel1.PerformLayout();
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)MobBackgroundPicBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownX).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudJump).EndInit();
            ((System.ComponentModel.ISupportInitialize)ZoomTrackBar).EndInit();
            panel.ResumeLayout(false);
            panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ImageBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)BackgroundPicBox1).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            splitContainer4.Panel1.ResumeLayout(false);
            splitContainer4.Panel1.PerformLayout();
            splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer4).EndInit();
            splitContainer4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ImageList ImageList;
        private System.Windows.Forms.OpenFileDialog OpenLibraryDialog;
        private System.Windows.Forms.SaveFileDialog SaveLibraryDialog;
        private System.Windows.Forms.OpenFileDialog ImportImageDialog;
        private System.Windows.Forms.OpenFileDialog OpenWeMadeDialog;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.FolderBrowserDialog FolderLibraryDialog;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem openReferenceFileToolStripMenuItem;
        private ToolStripMenuItem openReferenceImageToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripMenuItem functionsToolStripMenuItem;
        private ToolStripMenuItem copyToToolStripMenuItem;
        private ToolStripMenuItem countBlanksToolStripMenuItem;
        private ToolStripMenuItem removeBlanksToolStripMenuItem;
        private ToolStripMenuItem safeToolStripMenuItem;
        private ToolStripMenuItem convertToolStripMenuItem;
        private ToolStripMenuItem importShadowsToolStripMenuItem;
        private ToolStripMenuItem skinToolStripMenuItem;
        private MenuStrip MainMenu;
        private TabControl tabControl;
        private TabPage tabImages;
        private CustomFormControl.FixedListView PreviewListView;
        private SplitContainer splitContainer2;
        private NumericUpDown numericUpDownY;
        private NumericUpDown numericUpDownX;
        private GroupBox groupBox1;
        private RadioButton RButtonOverlay;
        private RadioButton RButtonImage;
        private CheckBox checkboxRemoveBlackOnImport;
        private NumericUpDown nudJump;
        private CheckBox checkBoxPreventAntiAliasing;
        private CheckBox checkBoxQuality;
        private Button buttonSkipPrevious;
        private Button buttonSkipNext;
        private Button buttonReplace;
        private PictureBox pictureBox;
        private TrackBar ZoomTrackBar;
        private Button ExportButton;
        private Button InsertImageButton;
        private Button DeleteButton;
        private Button AddButton;
        private Label label10;
        private Label label8;
        private Label HeightLabel;
        private Label label6;
        private Label WidthLabel;
        private Label label1;
        private PictureBox ImageBox;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer3;
        private SplitContainer splitContainer4;
        private TextBox PathTxtBox;
        private Label label2;
        private Button OpenBtn;
        private Panel panel;
        private PictureBox BackgroundPicBox1;
        private CheckBox MobChkBox;
        private CheckBox LockOffsetChkBox;
        private TreeView TreeBrowser;
        private PictureBox MobBackgroundPicBox;
        private Button BtnYPlusTen;
        private Button BtnYPlusFive;
        private Button BtnYPlusOne;
        private Button BtnYMinusOne;
        private Button BtnYMinusFive;
        private Button BtnYMinusTen;
        private Button BtnXPlusTen;
        private Button BtnXPlusFive;
        private Button BtnXPlusOne;
        private Button BtnXMinusOne;
        private Button BtnXMinusFive;
        private Button BtnXMinusTen;
    }
}

