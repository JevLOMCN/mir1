using Server.MirDatabase;
using Server.MirEnvir;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Server
{
    public partial class NPCInfoForm : Form
    {

        public string NPCListPath = Path.Combine(Settings.ExportPath, "NPCList.txt");

        public Envir Envir => SMain.EditEnvir;

        private List<NPCInfo> _selectedNPCInfos;
        private bool isEditing = false;

        public NPCInfoForm()
        {
            InitializeComponent();

            for (int i = 0; i < Envir.MapInfoList.Count; i++) MapComboBox.Items.Add(Envir.MapInfoList[i]);
            GenderComboBox.Items.AddRange(Enum.GetValues(typeof(MirGender)).Cast<object>().ToArray());
            UpdateInterface();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            Envir.CreateNPCInfo();
            UpdateInterface();
        }
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (_selectedNPCInfos.Count == 0) return;

            if (MessageBox.Show("Are you sure you want to remove the selected NPCs?", "Remove NPCs?", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            for (int i = 0; i < _selectedNPCInfos.Count; i++) Envir.Remove(_selectedNPCInfos[i]);

            if (Envir.NPCInfoList.Count == 0) Envir.NPCIndex = 0;

            UpdateInterface();
        }

        private void UpdateInterface()
        {
            if (NPCInfoListBox.Items.Count != Envir.NPCInfoList.Count)
            {
                NPCInfoListBox.Items.Clear();

                for (int i = 0; i < Envir.NPCInfoList.Count; i++)
                    NPCInfoListBox.Items.Add(Envir.NPCInfoList[i]);
            }

            _selectedNPCInfos = NPCInfoListBox.SelectedItems.Cast<NPCInfo>().ToList();

            if (_selectedNPCInfos.Count == 0)
            {
                tabPage1.Enabled = false;
                tabPage2.Enabled = false;
                NPCIndexTextBox.Text = string.Empty;
                NFileNameTextBox.Text = string.Empty;
                NNameTextBox.Text = string.Empty;
                NXTextBox.Text = string.Empty;
                NYTextBox.Text = string.Empty;
                NImageTextBox.Text = string.Empty;
                GenderComboBox.SelectedItem = null;
                HairTextBox.Text = string.Empty;
                NRateTextBox.Text = string.Empty;
                MapComboBox.SelectedItem = null;
                MinLev_textbox.Text = string.Empty;
                MaxLev_textbox.Text = string.Empty;
                Class_combo.Text = string.Empty;
                Day_combo.Text = string.Empty;
                TimeVisible_checkbox.Checked = false;
                StartHour_combo.Text = string.Empty;
                EndHour_combo.Text = string.Empty;
                StartMin_num.Value = 0;
                EndMin_num.Value = 1;
                Flag_textbox.Text = string.Empty;
                ShowBigMapCheckBox.Checked = false;
                BigMapIconTextBox.Text = string.Empty;
                ConquestVisible_checkbox.Checked = true;
                return;
            }

            NPCInfo info = _selectedNPCInfos[0];

            tabPage1.Enabled = true;
            tabPage2.Enabled = true;

            NPCIndexTextBox.Text = info.Index.ToString();
            NFileNameTextBox.Text = info.FileName;
            NNameTextBox.Text = info.Name;
            NXTextBox.Text = info.Location.X.ToString();
            NYTextBox.Text = info.Location.Y.ToString();
            NImageTextBox.Text = info.Image.ToString();
            GenderComboBox.SelectedItem = info.Gender;
            HairTextBox.Text = info.Hair.ToString();
            NRateTextBox.Text = info.Rate.ToString();
            MapComboBox.SelectedItem = Envir.MapInfoList.FirstOrDefault(x => x.Index == info.MapIndex);
            MinLev_textbox.Text = info.MinLev.ToString();
            MaxLev_textbox.Text = info.MaxLev.ToString();
            Class_combo.Text = info.ClassRequired;
            Day_combo.Text = info.DayofWeek;
            TimeVisible_checkbox.Checked = info.TimeVisible;
            StartHour_combo.Text = info.HourStart.ToString();
            EndHour_combo.Text = info.HourEnd.ToString();
            StartMin_num.Value = info.MinuteStart;
            EndMin_num.Value = info.MinuteEnd;
            Flag_textbox.Text = info.FlagNeeded.ToString();
            ShowBigMapCheckBox.Checked = info.ShowOnBigMap;
            BigMapIconTextBox.Text = info.BigMapIcon.ToString();
            TeleportToCheckBox.Checked = info.CanTeleportTo;


            for (int i = 1; i < _selectedNPCInfos.Count; i++)
            {
                info = _selectedNPCInfos[i];

                if (NFileNameTextBox.Text != info.FileName) NFileNameTextBox.Text = string.Empty;
                if (NNameTextBox.Text != info.Name) NNameTextBox.Text = string.Empty;
                if (NXTextBox.Text != info.Location.X.ToString()) NXTextBox.Text = string.Empty;

                if (NYTextBox.Text != info.Location.Y.ToString()) NYTextBox.Text = string.Empty;
                if (NImageTextBox.Text != info.Image.ToString()) NImageTextBox.Text = string.Empty;
                if (GenderComboBox.SelectedItem == null || (MirGender)GenderComboBox.SelectedItem != info.Gender) GenderComboBox.SelectedItem = null;
                if (HairTextBox.Text != info.Hair.ToString()) HairTextBox.Text = string.Empty;
                if (NRateTextBox.Text != info.Rate.ToString()) NRateTextBox.Text = string.Empty;
                if (BigMapIconTextBox.Text != info.BigMapIcon.ToString()) BigMapIconTextBox.Text = string.Empty;
            }
        }

        private void RefreshNPCList()
        {
            NPCInfoListBox.SelectedIndexChanged -= NPCInfoListBox_SelectedIndexChanged;

            List<bool> selected = new List<bool>();

            for (int i = 0; i < NPCInfoListBox.Items.Count; i++) selected.Add(NPCInfoListBox.GetSelected(i));
            NPCInfoListBox.Items.Clear();

            for (int i = 0; i < Envir.NPCInfoList.Count; i++) NPCInfoListBox.Items.Add(Envir.NPCInfoList[i]);
            for (int i = 0; i < selected.Count; i++) NPCInfoListBox.SetSelected(i, selected[i]);

            NPCInfoListBox.SelectedIndexChanged += NPCInfoListBox_SelectedIndexChanged;
        }

        private void NPCInfoListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInterface();
            UpdateNPCPreview();
            UpdateNPCHairPreview();
            UpdateNPCScriptDisplay();
        }
        private void UpdateNPCScriptDisplay()
        {
            string basePath = "Envir\\NPCs";
            string fileName = NFileNameTextBox.Text;
            fileName = $"{fileName}.txt";

            string filePath = Path.Combine(basePath, fileName);

            try
            {
                string fileContent = File.ReadAllText(filePath);

                NPCScriptTextBox.Text = fileContent;
                NPCScriptTextBox.ReadOnly = !isEditing;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void NFileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].FileName = ActiveControl.Text;

            RefreshNPCList();
            UpdateNPCScriptDisplay();
        }
        private void NNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].Name = ActiveControl.Text;
        }
        private void NXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].Location.X = temp;

            RefreshNPCList();
        }
        private void NYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].Location.Y = temp;

            RefreshNPCList();
        }
        private void NImageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            ushort temp;

            if (!ushort.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].Image = temp;

            UpdateNPCPreview();
        }
        private void GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedNPCInfos.Count; i++)
            {
                _selectedNPCInfos[i].Gender = (MirGender)GenderComboBox.SelectedItem;
            }

            UpdateNPCPreview();
            UpdateNPCHairPreview();
        }

        private void UpdateNPCPreview()
        {
            string gender = GenderComboBox.Text.ToLower();
            string fileName = NImageTextBox.Text;

            // Assuming your images are in folders named "Body/Male" and "Body/Female" inside the "Envir" folder
            string genderFolder = gender.Equals("male", StringComparison.OrdinalIgnoreCase) ? "Male" : "Female";
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "envir", "Previews", "NPC", "Body", genderFolder, $"{fileName}.png");

            try
            {
                if (File.Exists(imagePath))
                {
                    // Load the image from the specified path
                    NPCPreview.Image = Image.FromFile(imagePath);

                    // Invalidate the PictureBox and refresh the form to force a redraw
                    NPCPreview.Invalidate();
                    Refresh();
                }
                else
                {
                    // Handle the case when the image file does not exist
                    NPCPreview.Image = null; // Clear the PictureBox
                }
            }
            catch (Exception ex)
            {
                NPCPreview.Image = null; // Clear the PictureBox
            }
        }
        private void UpdateNPCHairPreview()
        {
            string gender = GenderComboBox.Text.ToLower();
            string fileName = HairTextBox.Text;

            // Assuming your images are in folders named "Hair/Male" and "Hair/Female" inside the "Envir" folder
            string genderFolder = gender.Equals("male", StringComparison.OrdinalIgnoreCase) ? "Male" : "Female";
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "envir", "Previews", "NPC", "Hair", genderFolder, $"{fileName}.png");

            try
            {
                if (File.Exists(imagePath))
                {
                    // Load the image from the specified path
                    NPCHairPreview.Image = Image.FromFile(imagePath);

                    // Invalidate the PictureBox and refresh the form to force a redraw
                    NPCHairPreview.Invalidate();
                    Refresh();
                }
                else
                {
                    // Handle the case when the image file does not exist
                    Console.WriteLine($"Image not found for gender: {gender}, hair filename: {fileName}. No default image provided.");
                    NPCHairPreview.Image = null; // Clear the PictureBox
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image: {ex.Message}");
                NPCHairPreview.Image = null; // Clear the PictureBox
            }
        }
        private void HairTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].Hair = temp;

            UpdateNPCHairPreview();
        }
        private void NRateTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            ushort temp;

            if (!ushort.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].Rate = temp;
        }

        private void MapComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedNPCInfos.Count; i++)
            {
                MapInfo temp = (MapInfo)MapComboBox.SelectedItem;
                _selectedNPCInfos[i].MapIndex = temp.Index;
            }

        }

        private void NPCInfoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Envir.SaveDB();
        }




        private void PasteMButton_Click(object sender, EventArgs e)
        {
            string data = Clipboard.GetText();

            if (!data.StartsWith("NPC", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Cannot Paste, Copied data is not NPC Information.");
                return;
            }


            string[] npcs = data.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);


            //for (int i = 1; i < npcs.Length; i++)
            //    NPCInfo.FromText(npcs[i]);

            UpdateInterface();
        }

        private void ExportAllButton_Click(object sender, EventArgs e)
        {
            ExportNPCs(Envir.NPCInfoList);
        }

        private void ExportSelected_Click(object sender, EventArgs e)
        {
            var list = NPCInfoListBox.SelectedItems.Cast<NPCInfo>().ToList();

            ExportNPCs(list);
        }

        public void ExportNPCs(List<NPCInfo> NPCs)
        {
            if (NPCs.Count == 0) return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Path.Combine(Application.StartupPath, "Exports");
            sfd.Filter = "Text File|*.txt";
            sfd.ShowDialog();

            if (sfd.FileName == string.Empty) return;

            using (StreamWriter sw = File.AppendText(sfd.FileNames[0]))
            {
                for (int j = 0; j < NPCs.Count; j++)
                {
                    sw.WriteLine(NPCs[j].ToText());
                }
            }
            MessageBox.Show("NPC Export complete");
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            string Path = string.Empty;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text File|*.txt";
            ofd.ShowDialog();

            if (ofd.FileName == string.Empty) return;

            Path = ofd.FileName;

            string data;
            using (var sr = new StreamReader(Path))
            {
                data = sr.ReadToEnd();
            }

            var npcs = data.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var m in npcs)
            {
                try
                {
                    NPCInfo.FromText(m);
                }
                catch { }
            }

            UpdateInterface();
            MessageBox.Show("NPC Import complete");
        }

        private void MinLev_textbox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            short temp;

            if (!short.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].MinLev = temp;
        }

        private void HourShow_textbox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].HourStart = temp;
        }

        private void MinutesShow_textbox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].MinuteStart = temp;
        }

        private void Class_textbox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].ClassRequired = ActiveControl.Text;
        }

        private void CopyMButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Envir.Now.DayOfWeek.ToString());
        }

        private void MaxLev_textbox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            short temp;

            if (!short.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].MaxLev = temp;
        }

        private void Class_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;
            string temp = ActiveControl.Text;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].ClassRequired = temp;
        }

        private void Day_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;
            string temp = ActiveControl.Text;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].DayofWeek = temp;
        }

        private void TimeVisible_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].TimeVisible = TimeVisible_checkbox.Checked;
        }

        private void StartHour_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;

            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].HourStart = temp;
        }

        private void EndHour_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            byte temp;

            if (!byte.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;

            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].HourEnd = temp;
        }

        private void StartMin_num_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].MinuteStart = (byte)StartMin_num.Value;
        }

        private void EndMin_num_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].MinuteEnd = (byte)EndMin_num.Value;
        }

        private void Flag_textbox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;

            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].FlagNeeded = temp;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            MessageBox.Show(Envir.Now.TimeOfDay.ToString());
        }

        private void NPCInfoForm_Load(object sender, EventArgs e)
        {

        }

        private void ConquestHidden_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ShowBigMapCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].ShowOnBigMap = ShowBigMapCheckBox.Checked;
        }

        private void BigMapIconTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            int temp;

            if (!int.TryParse(ActiveControl.Text, out temp))
            {
                ActiveControl.BackColor = Color.Red;
                return;
            }
            ActiveControl.BackColor = SystemColors.Window;


            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].BigMapIcon = temp;
        }

        private void TeleportToCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].CanTeleportTo = TeleportToCheckBox.Checked;
        }

        private void ConquestVisible_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveControl != sender) return;

            for (int i = 0; i < _selectedNPCInfos.Count; i++)
                _selectedNPCInfos[i].ConquestVisible = ConquestVisible_checkbox.Checked;
        }

        private void OpenNPCScriptBtn_Click(object sender, EventArgs e)
        {
            if (NFileNameTextBox.Text == string.Empty) return;

            var scriptPath = Path.Combine(Settings.NPCPath, NFileNameTextBox.Text + ".txt");

            if (File.Exists(scriptPath))
            {
                Shared.Helpers.FileIO.OpenScript(scriptPath, true);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(scriptPath));

                string templateFilePath = "Envir\\NPCs\\Template.txt";

                try
                {
                    if (File.Exists(templateFilePath))
                    {
                        string templateContent = File.ReadAllText(templateFilePath);
                        File.WriteAllText(scriptPath, templateContent);

                        Shared.Helpers.FileIO.OpenScript(scriptPath, true);
                    }
                    else
                    {
                        MessageBox.Show("Template file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating or opening script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EditScriptBtn_Click(object sender, EventArgs e)
        {
            isEditing = !isEditing;
            NPCScriptTextBox.ReadOnly = !isEditing;

            if (!isEditing)
            {
                SaveScriptContent();
            }
        }

        private void SaveScriptContent()
        {
            // Get the base folder path
            string basePath = "Envir\\NPCs"; // Adjust as needed

            // Get the file name from the NFileNameTextBox
            string fileName = NFileNameTextBox.Text;

            // Add the ".txt" extension to the file name
            fileName = $"{fileName}.txt";

            // Combine the base path and file name to get the full file path
            string filePath = Path.Combine(basePath, fileName);

            try
            {
                // Save the content of the RichTextBox back to the file
                File.WriteAllText(filePath, NPCScriptTextBox.Text);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during file writing
                MessageBox.Show($"Error writing to file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopyScriptBtn_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                // Set initial directory and suggested file name
                saveFileDialog.InitialDirectory = "Server\\Envir\\NPCs"; // Adjust as needed
                saveFileDialog.FileName = NFileNameTextBox.Text;

                // Set the file filter
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Save the content of the RichTextBox to the selected file
                        File.WriteAllText(saveFileDialog.FileName, NPCScriptTextBox.Text);

                        MessageBox.Show("Script copied successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that might occur during file writing
                        MessageBox.Show($"Error copying script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ReloadScriptBtn_Click(object sender, EventArgs e)
        {
            Envir.ReloadNPCs();
        }
    }
}

