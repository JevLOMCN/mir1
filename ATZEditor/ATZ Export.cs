using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Mir1ATZTool.Main;

namespace Mir1ATZTool
{
    public partial class ATZ_Export : UserControl
    {
        public ATZ_Export(Main main)
        {
            InitializeComponent();
            mainForm = main;
            ExportProgressBar.Minimum = 0;
            ExportProgressBar.Step = 1;
        }
        private Main mainForm;
        private string selectedOutputPath;
        private void SelectOutputATZBtn_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select Output Folder";
                fbd.ShowNewFolderButton = true;

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    selectedOutputPath = fbd.SelectedPath;
                    OutputATZPath.Text = selectedOutputPath;
                }
            }
        }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            //if (currentLibrary == null)
            //{
            //    MessageBox.Show("No ATZ file loaded!", "Error",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //// Fallback: use directory of currentLibrary if no output path selected
            //if (string.IsNullOrEmpty(selectedOutputPath))
            //{
            //    if (!string.IsNullOrEmpty(currentLibrary.LibFilePath) && Directory.Exists(Path.GetDirectoryName(currentLibrary.LibFilePath)))
            //    {
            //        selectedOutputPath = Path.GetDirectoryName(currentLibrary.LibFilePath);
            //        OutputATZPath.Text = selectedOutputPath; // Update UI so user can see it
            //    }
            //    else
            //    {
            //        MessageBox.Show("Please select an output folder!", "Error",
            //                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return;
            //    }
            //}

            //if (!JPGExport.Checked && !PNGExport.Checked && !BMPExport.Checked && !RAWExport.Checked)
            //{
            //    MessageBox.Show("Please select an export format!", "Error",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //if (mainForm == null)
            //{
            //    MessageBox.Show("Main form not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //var selectedItems = mainForm.ExposedATZListView.SelectedItems;
            //if (selectedItems.Count == 0)
            //{
            //    MessageBox.Show("Please select at least one image to export!", "Error",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //try
            //{
            //    bool removeBlanks = ExportExcludeBlanks.Checked;
            //    bool makeTransparent = TransparentExport.Checked;
            //    ImageFormat format = GetSelectedFormat();

            //    List<int> selectedIndices = selectedItems
            //        .Cast<ListViewItem>()
            //        .Select(item => (int)item.Tag)
            //        .ToList();

            //    ExportProgressBar.Maximum = selectedIndices.Count;
            //    ExportProgressBar.Value = 0;

            //    ExportImages(currentLibrary, selectedOutputPath, format, makeTransparent, removeBlanks, selectedIndices);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Export failed:\n{ex.Message}", "Error",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private ImageFormat GetSelectedFormat()
        {
            if (JPGExport.Checked) return ImageFormat.Jpeg;
            if (PNGExport.Checked) return ImageFormat.Png;
            if (BMPExport.Checked) return ImageFormat.Bmp;
            return ImageFormat.Png; // Default for RAW will be handled separately
        }

        private void SaveRawImage(Mir1Image image, string filePath)
        {
            // Save raw pixel data (indexed format)
            //using (FileStream fs = new FileStream(filePath, FileMode.Create))
            //using (BinaryWriter writer = new BinaryWriter(fs))
            //{
            //    for (int y = 0; y < image.Height; y++)
            //    {
            //        for (int x = 0; x < image.Width; x++)
            //        {
            //            Color pixel = image.Image.GetPixel(x, y);
            //            // Save as RGB (ignore alpha for RAW format)
            //            writer.Write(pixel.R);
            //            writer.Write(pixel.G);
            //            writer.Write(pixel.B);
            //        }
            //    }
            //}
        }

        private void ExportImages(Mir1Library library, string outputPath,
                                ImageFormat format, bool transparent, bool removeBlanks, List<int> selectedIndices)
        {
            //string exportDir = Path.Combine(outputPath, library.FileName);
            //Directory.CreateDirectory(exportDir);

            //int exportedCount = 0;
            //int blankCount = 0;

            //for (int count = 0; count < selectedIndices.Count; count++)
            //{
            //    int i = selectedIndices[count];
            //    {
            //        var image = library.Images[i];

            //        ExportProgressBar.Value = count + 1;
            //        Application.DoEvents(); // Allow UI to update

            //        // Skip blank images if requested
            //        if (removeBlanks && IsBlankImage(image))
            //        {
            //            blankCount++;
            //            continue;
            //        }

            //        // Prepare image for export
            //        Bitmap exportBitmap = (Bitmap)image.Image.Clone();

            //        // Handle transparency if requested
            //        if (transparent && format != ImageFormat.Jpeg)
            //        {
            //            exportBitmap.MakeTransparent(Color.Transparent);
            //        }

            //        // Determine file extension
            //        string extension = GetExtension(format);
            //        string fileName = $"{i:0000}.{extension}";
            //        string filePath = Path.Combine(exportDir, fileName);

            //        // Handle RAW format specially
            //        if (RAWExport.Checked)
            //        {
            //            SaveRawImage(image, filePath);
            //        }
            //        else
            //        {
            //            // Save in standard image format
            //            exportBitmap.Save(filePath, format);
            //        }

            //        exportedCount++;
            //    }
            //}
        }
        
        private void FormatCheckChanged(object sender, EventArgs e)
        {
            CheckBox current = sender as CheckBox;
            if (current == null || !current.Checked) return;

            // Uncheck other format checkboxes
            var others = new[] { JPGExport, PNGExport, BMPExport, RAWExport };
            foreach (var cb in others)
            {
                if (cb != current) cb.Checked = false;
            }
        }
        private string GetExtension(ImageFormat format)
        {
            if (RAWExport.Checked) return "raw";
            if (JPGExport.Checked) return "jpg";
            if (PNGExport.Checked) return "png";
            if (BMPExport.Checked) return "bmp";
            return "dat";
        }
    }
}