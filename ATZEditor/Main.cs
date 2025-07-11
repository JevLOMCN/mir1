using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mir1ATZTool
{
    public partial class Main : Form
    {
        private ImageList imageListThumbnails = new ImageList();
        private bool viewPageInitialized = false;
        private Color[] _globalPalette = new Color[256];
        private CancellationTokenSource? _thumbnailCancellationSource;

        public class Mir1Image
        {
            public int Width;
            public int Height;
            public int X;
            public int Y;
            public int Reserved;
            public long Position;
            public byte[] PixelData;
            private Bitmap? _cachedBitmap;

            public Bitmap GetBitmap(Color[] palette)
            {
                if (_cachedBitmap != null)
                    return _cachedBitmap;

                // Validate dimensions before creating bitmap
                if (Width <= 0 || Height <= 0)
                {
                    // Create a minimal placeholder image
                    var placeholder = new Bitmap(1, 1);
                    placeholder.SetPixel(0, 0, Color.Magenta);
                    _cachedBitmap = placeholder;
                    return placeholder;
                }

                var bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
                var rect = new Rectangle(0, 0, Width, Height);
                var bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);

                unsafe
                {
                    byte* scan0 = (byte*)bitmapData.Scan0;
                    int stride = bitmapData.Stride;

                    fixed (byte* pPixelData = PixelData)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            int srcOffset = y * Width;
                            int destOffset = y * stride;

                            for (int x = 0; x < Width; x++)
                            {
                                byte index = pPixelData[srcOffset + x];

                                // Validate palette index
                                if (index >= palette.Length)
                                    index = 0;

                                Color color = palette[index];
                                int pos = destOffset + x * 4;

                                scan0[pos] = color.B;
                                scan0[pos + 1] = color.G;
                                scan0[pos + 2] = color.R;
                                scan0[pos + 3] = color.A;
                            }
                        }
                    }
                }

                bitmap.UnlockBits(bitmapData);
                _cachedBitmap = bitmap;
                return bitmap;
            }

            public void ClearCache()
            {
                if (_cachedBitmap != null)
                {
                    _cachedBitmap.Dispose();
                    _cachedBitmap = null;
                }
            }
        }

        public class Mir1Library : IDisposable
        {
            public string LibFilePath;
            public int ImageCount;
            public List<Mir1Image> Images = new List<Mir1Image>();
            public string FileName => Path.GetFileNameWithoutExtension(LibFilePath);

            public Mir1Library(string filepath)
            {
                LibFilePath = filepath;
                if (File.Exists(LibFilePath))
                    LoadImages();
            }

            private void LoadImages()
            {
                using (FileStream input = File.OpenRead(LibFilePath))
                using (BinaryReader reader = new BinaryReader(input, Encoding.Default))
                {
                    reader.ReadChars(4); // Skip header
                    ImageCount = reader.ReadInt32();
                    reader.ReadInt32(); // Skip unknown

                    // Skip palette (256 colors * 4 bytes)
                    reader.BaseStream.Seek(256 * 4, SeekOrigin.Current);

                    for (int j = 0; j < ImageCount; j++)
                    {
                        int width = reader.ReadInt32();
                        int height = reader.ReadInt32();

                        var image = new Mir1Image()
                        {
                            Position = reader.BaseStream.Position,
                            Width = width,
                            Height = height,
                            X = reader.ReadInt32(),
                            Y = reader.ReadInt32(),
                            Reserved = reader.ReadInt32()
                        };

                        // Only read pixel data if dimensions are valid
                        if (width > 0 && height > 0)
                        {
                            image.PixelData = reader.ReadBytes(width * height);
                        }
                        else
                        {
                            image.PixelData = Array.Empty<byte>();
                            reader.BaseStream.Seek(width * height, SeekOrigin.Current);
                        }

                        Images.Add(image);
                    }
                }
            }

            public void Dispose()
            {
                foreach (var image in Images)
                    image.ClearCache();
            }
        }

        public static Mir1Library? currentLibrary;

        public Main()
        {
            InitializeComponent();
            InitializeViewPageComponents();
            viewPageInitialized = true;
        }

        private void LoadATZFile(string filePath)
        {
            try
            {
                currentLibrary?.Dispose();
                currentLibrary = new Mir1Library(filePath);

                // Update UI
                ATZSpriteCount.Text = currentLibrary.ImageCount.ToString();

                FileInfo fi = new FileInfo(filePath);
                ATZFileSize.Text = $"{fi.Length / 1024} KB";

                // Read palette and dimensions
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var reader = new BinaryReader(fs))
                {
                    fs.Seek(12, SeekOrigin.Begin); // Skip header

                    // Load palette
                    for (int i = 0; i < 256; i++)
                    {
                        byte[] colorBytes = reader.ReadBytes(4);
                        _globalPalette[i] = (colorBytes[0] == 0 &&
                                             colorBytes[1] == 0 &&
                                             colorBytes[2] == 0)
                            ? Color.Transparent
                            : Color.FromArgb(colorBytes[0], colorBytes[1], colorBytes[2]);
                    }

                    // Read atlas dimensions
                    int atlasWidth = reader.ReadInt32();
                    int atlasHeight = reader.ReadInt32();
                    ATZWValue.Text = atlasWidth.ToString();
                    ATZHValue.Text = atlasHeight.ToString();
                }

                IndexUpDown.Minimum = 0;
                IndexUpDown.Maximum = currentLibrary.Images.Count > 0 ? currentLibrary.Images.Count - 1 : 0;
                IndexUpDown.Value = 0;

                // Count blank images
                int blankCount = 0;
                foreach (var image in currentLibrary.Images)
                {
                    if (IsBlankImage(image))
                        blankCount++;
                }
                ATZBlankCount.Text = blankCount.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading ATZ file:\n{ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool IsBlankImage(Mir1Image image)
        {
            return (image.Width == 4 && image.Height == 4) ||
                   (image.Width == 8 && image.Height == 5);
        }

        private void SelectATZFolderBtn_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select Folder with ATZ Files";
                fbd.ShowNewFolderButton = false;

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    ViewATZPath.Text = fbd.SelectedPath;
                    LoadATZFolder(fbd.SelectedPath);
                }
            }
        }

        private void LoadATZFolder(string folderPath)
        {
            // Safely clear folder list view
            if (ATZFolderListView != null)
            {
                ATZFolderListView.Items.Clear();
            }

            // Clear thumbnail images
            imageListThumbnails.Images.Clear();

            // Safely clear ATZListView if it exists
            if (ATZListView != null)
            {
                ATZListView.Items.Clear();
            }

            string[] atzFiles = Directory.GetFiles(folderPath, "*.atz");
            string[] atkFiles = Directory.GetFiles(folderPath, "*.atk");
            string[] allFiles = atzFiles.Concat(atkFiles).ToArray();

            // Safely configure progress bar
            if (FileLoadProgressBar != null)
            {
                FileLoadProgressBar.Minimum = 0;
                FileLoadProgressBar.Maximum = allFiles.Length;
                FileLoadProgressBar.Value = 0;
                FileLoadProgressBar.Visible = true;
            }

            foreach (string file in allFiles)
            {
                ListViewItem item = new ListViewItem(Path.GetFileName(file))
                {
                    Tag = file
                };

                // Safely add to folder list
                if (ATZFolderListView != null)
                {
                    ATZFolderListView.Items.Add(item);
                }

                // Update progress if available
                if (FileLoadProgressBar != null)
                {
                    FileLoadProgressBar.Value++;
                }
            }

            if (FileLoadProgressBar != null)
            {
                FileLoadProgressBar.Visible = false;
            }

            // Update file count if control exists
            if (ATZFileCount != null)
            {
                ATZFileCount.Text = allFiles.Length.ToString();
            }
        }

        private async void ATZFolderListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ATZFolderListView.SelectedItems.Count == 0) return;

            string selectedFile = ATZFolderListView.SelectedItems[0].Tag.ToString();

            if (File.Exists(selectedFile))
            {
                FileInfo fi = new FileInfo(selectedFile);
                ATZSelectedFileSize.Text = $"{fi.Length / 1024} KB";
            }

            LoadATZFile(selectedFile);
            await PopulateImageListAsync();

            PreviewImage.Image = null;
        }

        private async Task PopulateImageListAsync()
        {
            if (ATZListView == null || currentLibrary == null)
                return;

            // Cancel any previous thumbnail generation
            _thumbnailCancellationSource?.Cancel();
            _thumbnailCancellationSource = new CancellationTokenSource();
            var cancellationToken = _thumbnailCancellationSource.Token;

            // Clear existing items
            imageListThumbnails.Images.Clear();
            ATZListView.BeginUpdate();
            ATZListView.Items.Clear();
            ATZListView.EndUpdate();

            // Force UI update
            Application.DoEvents();

            // Pre-calculate visible indices
            var visibleIndices = new List<int>();
            for (int i = 0; i < currentLibrary.Images.Count; i++)
            {
                if (HideBlanksCheckBox.Checked && IsBlankImage(currentLibrary.Images[i]))
                    continue;
                visibleIndices.Add(i);
            }

            // Configure progress bar
            if (FileLoadProgressBar != null)
            {
                FileLoadProgressBar.Minimum = 0;
                FileLoadProgressBar.Maximum = visibleIndices.Count;
                FileLoadProgressBar.Value = 0;
                FileLoadProgressBar.Visible = true;
            }

            // Process in batches
            const int batchSize = 30;
            int processedCount = 0;

            for (int i = 0; i < visibleIndices.Count; i += batchSize)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                int endIndex = Math.Min(i + batchSize, visibleIndices.Count);
                await ProcessThumbnailBatchAsync(visibleIndices, i, endIndex, cancellationToken);

                processedCount = Math.Min(processedCount + batchSize, visibleIndices.Count);

                if (FileLoadProgressBar != null)
                {
                    FileLoadProgressBar.Value = processedCount;
                }

                await Task.Delay(10); // Allow UI to respond
            }

            if (FileLoadProgressBar != null)
            {
                FileLoadProgressBar.Visible = false;
            }
            DebugSaveThumbnails();
        }
        private void DebugSaveThumbnails()
        {
            if (!Directory.Exists(@"C:\ATZDebug"))
                Directory.CreateDirectory(@"C:\ATZDebug");

            for (int i = 0; i < imageListThumbnails.Images.Count; i++)
            {
                try
                {
                    imageListThumbnails.Images[i].Save($@"C:\ATZDebug\thumb_{i}.png");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error saving thumb {i}: {ex.Message}");
                }
            }
            MessageBox.Show($"Saved {imageListThumbnails.Images.Count} thumbnails to C:\\ATZDebug");
        }

        private async Task ProcessThumbnailBatchAsync(List<int> indices, int start, int end, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                for (int i = start; i < end; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    int actualIndex = indices[i];
                    var image = currentLibrary.Images[actualIndex];

                    try
                    {
                        using (var fullBitmap = image.GetBitmap(_globalPalette))
                        {
                            var thumbnail = new Bitmap(fullBitmap, 64, 64);

                            // Use BeginInvoke for better performance
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                if (cancellationToken.IsCancellationRequested || ATZListView == null)
                                    return;

                                int imageIndex = imageListThumbnails.Images.Count;
                                imageListThumbnails.Images.Add(thumbnail);

                                var item = new ListViewItem($"{actualIndex:00000}")
                                {
                                    ImageIndex = imageIndex,
                                    Tag = actualIndex
                                };

                                // Add item without redrawing
                                ATZListView.BeginUpdate();
                                ATZListView.Items.Add(item);
                                ATZListView.EndUpdate();
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error creating thumbnail for image {actualIndex}: {ex.Message}");
                    }
                }
            });
        }

        private void InitializeViewPageComponents()
        {
            if (imageListThumbnails == null)
                imageListThumbnails = new ImageList();

            imageListThumbnails.ImageSize = new Size(64, 64);
            imageListThumbnails.ColorDepth = ColorDepth.Depth32Bit;

            if (ATZListView != null)
            {
                ATZListView.LargeImageList = imageListThumbnails;
                ATZListView.View = View.LargeIcon;
                ATZListView.Scrollable = true;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "ATZ Files|*.atz;*.atk";
                ofd.Title = "Select ATZ File";
                ofd.CheckFileExists = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = ofd.FileName;
                    LoadATZFile(selectedFile);

                    // Add to folder list if not exists
                    bool exists = false;
                    foreach (ListViewItem item in ATZFolderListView.Items)
                    {
                        if (item.Tag.ToString() == selectedFile)
                        {
                            exists = true;
                            item.Selected = true;
                            item.Focused = true;
                            item.EnsureVisible();
                            break;
                        }
                    }

                    if (!exists)
                    {
                        ListViewItem newItem = new ListViewItem(Path.GetFileName(selectedFile))
                        {
                            Tag = selectedFile
                        };
                        ATZFolderListView.Items.Add(newItem);
                        newItem.Selected = true;
                        newItem.Focused = true;
                        newItem.EnsureVisible();
                    }

                    if (!viewPageInitialized)
                    {
                        InitializeViewPageComponents();
                        viewPageInitialized = true;
                    }

                    _ = PopulateImageListAsync();
                }
            }
        }

        private void Export_Button_Click(object sender, EventArgs e)
        {
            // Export implementation would go here
            MessageBox.Show("Export functionality not implemented yet");
        }

        private void ATZListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ATZListView == null || ATZListView.SelectedItems.Count == 0) return;

            int selectedIndex = (int)ATZListView.SelectedItems[0].Tag;
            IndexUpDown.Value = selectedIndex;

            if (currentLibrary != null &&
                selectedIndex >= 0 &&
                selectedIndex < currentLibrary.Images.Count)
            {
                try
                {
                    var image = currentLibrary.Images[selectedIndex].GetBitmap(_globalPalette);
                    if (image != null)
                    {
                        PreviewImage.Image = image;
                    }
                }
                catch (Exception ex)
                {
                    // Handle any remaining exceptions
                    PreviewImage.Image = null;
                    Debug.WriteLine($"Error loading image: {ex.Message}");
                }
            }
        }

        private void IndexUpDown_ValueChanged(object sender, EventArgs e)
        {
            int index = (int)IndexUpDown.Value;

            if (index >= 0 && index < ATZListView.Items.Count)
            {
                ATZListView.SelectedIndices.Clear();
                ATZListView.SelectedIndices.Add(index);
                ATZListView.EnsureVisible(index);
            }
        }

        private void IndexSlider_Scroll(object sender, EventArgs e)
        {
            if (currentLibrary == null || ATZListView.SelectedItems.Count == 0)
                return;

            int selectedIndex = (int)ATZListView.SelectedItems[0].Tag;
            var originalImage = currentLibrary.Images[selectedIndex].GetBitmap(_globalPalette);

            int zoomLevel = Math.Max(1, IndexSlider.Value);
            var zoomedImage = new Bitmap(originalImage.Width * zoomLevel, originalImage.Height * zoomLevel);

            using (var g = Graphics.FromImage(zoomedImage))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(originalImage, 0, 0, zoomedImage.Width, zoomedImage.Height);
            }

            PreviewImage.Image = zoomedImage;
        }

        private void PreviousFrameBtn_Click(object sender, EventArgs e)
        {
            if (IndexUpDown.Value > IndexUpDown.Minimum)
                IndexUpDown.Value -= 1;
        }

        private void NextFrameBtn_Click(object sender, EventArgs e)
        {
            if (IndexUpDown.Value < IndexUpDown.Maximum)
                IndexUpDown.Value += 1;
        }

        private void HideBlanksCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _ = PopulateImageListAsync();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _thumbnailCancellationSource?.Cancel();
            currentLibrary?.Dispose();
            base.OnFormClosing(e);
        }
    }

    public class DoubleBufferedListView : ListView
    {
        public DoubleBufferedListView()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }
    }
}