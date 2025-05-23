﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;

namespace Map_Editor
{
    public static class Libraries
    {
        public static bool Loaded;
        public static int Count, Progress;

        public const string ObjectsPath = @".\Data\Objects\";
        //Map
        public static readonly MLibrary[] MapLibs = new MLibrary[400];

        public static readonly ListItem[] ListItems = new ListItem[400];

        static Libraries()
        {
            //wemade mir2 (allowed from 0-99)
            //MapLibs[99]=new MLibrary(@".\Objects\MyImage");
            MapLibs[1] = new MLibrary(@".\Data\Smtiles");
            ListItems[1] = new ListItem("Smtiles", 1);
            MapLibs[2] = new MLibrary(@".\Data\Objects");
            ListItems[2] = new ListItem("Objects", 2);

            for (int i = 2; i < 27; i++)
            {
                if (File.Exists(@".\Data\Map\WemadeMir2\Objects" + i + ".lib"))
                {
                    MapLibs[i + 1] = new MLibrary(@".\Data\Map\WemadeMir2\Objects" + i);
                    ListItems[i + 1] = new ListItem("Objects" + i, i + 1);
                }
            }
            //Thread thread = new Thread(LoadGameLibraries) { IsBackground = true };
            //thread.Start();
        }


        public static void LoadGameLibraries()
        {
            Count = MapLibs.Length;

            for (int i = 0; i < MapLibs.Length; i++)
            {
                if (MapLibs[i] == null)
                    MapLibs[i] = new MLibrary("");
                else
                    MapLibs[i].Initialize();
                Progress++;
            }
            Loaded = true;
        }

    }
    public sealed class MLibrary
    {
        public const int LibVersion = 1;
        public static bool Load = true;
        public string FileName;

        public List<MImage> Images = new List<MImage>();
        public List<int> IndexList = new List<int>();
        public int Count;
        private bool _initialized;

        private BinaryReader _reader;
        private FileStream _stream;

        public MLibrary(string filename)
        {
            FileName = filename + ".lib";
            Initialize();
        }

        public void Initialize()
        {
            int CurrentVersion;
            _initialized = true;

            if (!File.Exists(FileName))
                return;

            _stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            _reader = new BinaryReader(_stream);
            CurrentVersion = _reader.ReadInt32();
            if (CurrentVersion != 1)
            {
                MessageBox.Show("Wrong version, expecting lib version: " + LibVersion.ToString() + " found version: " + CurrentVersion.ToString() + ".", "Failed to open", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            Count = _reader.ReadInt32();
            Images = new List<MImage>();
            IndexList = new List<int>();

            for (int i = 0; i < Count; i++)
                IndexList.Add(_reader.ReadInt32());

            for (int i = 0; i < Count; i++)
                Images.Add(null);

            //for (int i = 0; i < Count; i++)
            //    CheckImage(i);
        }


        public void Close()
        {
            if (_stream != null)
                _stream.Dispose();
            if (_reader != null)
                _reader.Dispose();
        }

        public void Save()
        {
            Close();

            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            Count = Images.Count;
            IndexList.Clear();

            int offSet = 8 + Count * 4;
            for (int i = 0; i < Count; i++)
            {
                IndexList.Add((int)stream.Length + offSet);
                Images[i].Save(writer);
                //Images[i] = null;
            }

            writer.Flush();
            byte[] fBytes = stream.ToArray();
            //  writer.Dispose();

            _stream = File.Create(FileName);
            writer = new BinaryWriter(_stream);
            writer.Write(LibVersion);
            writer.Write(Count);
            for (int i = 0; i < Count; i++)
                writer.Write(IndexList[i]);

            writer.Write(fBytes);
            writer.Flush();
            writer.Close();
            writer.Dispose();
            Close();
        }

        public void CheckImage(int index)
        {
            if (!_initialized)
                Initialize();

            if (Images == null || index < 0 || index >= Images.Count)
                return;
            if (_stream == null)
            {
                return;
            }
            if (Images[index] == null)
            {
                _stream.Position = IndexList[index];
                Images[index] = new MImage(_reader);
            }

            if (!Load) return;

            //MImage mi = Images[index];
            //if (!mi.TextureValid)
            //{
            //    _stream.Seek(IndexList[index] + 12, SeekOrigin.Begin);
            //    mi.CreateTexture(_reader);
            //}
            if (!Images[index].TextureValid)
            {
                _stream.Seek(IndexList[index] + 12, SeekOrigin.Begin);
                Images[index].CreateBmpTexture(_reader);
            }

            if (!Images[index].TextureValid)
            {

                _stream.Seek(IndexList[index] + 12, SeekOrigin.Begin);
                Images[index].CreateTexture(_reader);
            }
        }

        public Point GetOffSet(int index)
        {
            if (!_initialized)
                Initialize();

            if (Images == null || index < 0 || index >= Images.Count)
                return Point.Empty;

            if (Images[index] == null)
            {
                _stream.Seek(IndexList[index], SeekOrigin.Begin);
                Images[index] = new MImage(_reader);
            }

            return new Point(Images[index].X, Images[index].Y);
        }

        public Size GetSize(int index)
        {
            if (!_initialized)
                Initialize();
            if (Images == null || index < 0 || index >= Images.Count)
                return Size.Empty;

            if (Images[index] == null)
            {
                _stream.Seek(IndexList[index], SeekOrigin.Begin);
                Images[index] = new MImage(_reader);
            }

            return new Size(Images[index].Width, Images[index].Height);
        }

        public MImage GetMImage(int index)
        {
            if (index < 0 || index >= Images.Count)
                return null;
            CheckImage(index);
            return Images[index];
        }

        public Bitmap GetPreview(int index)
        {
            if (index < 0 || index >= Images.Count)
                return new Bitmap(1, 1);

            MImage image = Images[index];

            if (image == null || image.Image == null)
                return new Bitmap(1, 1);

            if (image.Preview == null)
                image.CreatePreview();
            Bitmap preview = image.Preview;
            return preview;
        }

        public void AddImage(Bitmap image, short x, short y)
        {
            MImage mImage = new MImage(image) { X = x, Y = y };

            Count++;
            Images.Add(mImage);
        }

        public void ReplaceImage(int Index, Bitmap image, short x, short y)
        {
            MImage mImage = new MImage(image) { X = x, Y = y };

            Images[Index] = mImage;
        }

        public void InsertImage(int index, Bitmap image, short x, short y)
        {
            MImage mImage = new MImage(image) { X = x, Y = y };

            Count++;
            Images.Insert(index, mImage);
        }

        public void RemoveImage(int index)
        {
            if (Images == null || Count <= 1)
            {
                Count = 0;
                Images = new List<MImage>();
                return;
            }
            Count--;

            Images.RemoveAt(index);
        }

        public static bool CompareBytes(byte[] a, byte[] b)
        {
            if (a == b) return true;

            if (a == null || b == null || a.Length != b.Length) return false;

            for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;

            return true;
        }

        public void RemoveBlanks(bool safe = false)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                if (Images[i].FBytes == null || Images[i].FBytes.Length <= 24)
                {
                    if (!safe)
                        RemoveImage(i);
                    else if (Images[i].X == 0 && Images[i].Y == 0)
                        RemoveImage(i);
                }
            }
        }

        public sealed class MImage
        {
            public short Width, Height, X, Y;
            public byte Shadow;
            public int Length;
            public byte[] FBytes;
            public bool TextureValid;
            public Bitmap Image, Preview;
            public Texture ImageTexture;

            public unsafe byte* Data;

            public MImage(BinaryReader reader)
            {
                //read layer 1
                Width = reader.ReadInt16();
                Height = reader.ReadInt16();
                X = reader.ReadInt16();
                Y = reader.ReadInt16();
                Length = reader.ReadInt32();
                FBytes = reader.ReadBytes(Length);
            }

            public MImage(byte[] image, short Width, short Height)//only use this when converting from old to new type!
            {
                FBytes = image;
                this.Width = Width;
                this.Height = Height;
            }

            public MImage(Bitmap image)
            {
                if (image == null)
                {
                    FBytes = new byte[0];
                    return;
                }

                Width = (short)image.Width;
                Height = (short)image.Height;

                Image = image;// FixImageSize(image);
                FBytes = ConvertBitmapToArray(Image);
            }

            public MImage(Bitmap image, Bitmap Maskimage)
            {
                if (image == null)
                {
                    FBytes = new byte[0];
                    return;
                }

                Width = (short)image.Width;
                Height = (short)image.Height;
                Image = image;// FixImageSize(image);
                FBytes = ConvertBitmapToArray(Image);
            }

            private Bitmap FixImageSize(Bitmap input)
            {
                int w = input.Width + (4 - input.Width % 4) % 4;
                int h = input.Height + (4 - input.Height % 4) % 4;

                if (input.Width != w || input.Height != h)
                {
                    Bitmap temp = new Bitmap(w, h);
                    using (Graphics g = Graphics.FromImage(temp))
                    {
                        g.Clear(Color.Transparent);
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.DrawImage(input, 0, 0);
                        g.Save();
                    }
                    input.Dispose();
                    input = temp;
                }

                return input;
            }

            private unsafe byte[] ConvertBitmapToArray(Bitmap input)
            {
                BitmapData data = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly,
                                                 PixelFormat.Format32bppArgb);

                byte[] pixels = new byte[input.Width * input.Height * 4];

                Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);

                input.UnlockBits(data);

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    if (pixels[i] == 0 && pixels[i + 1] == 0 && pixels[i + 2] == 0)
                        pixels[i + 3] = 0; //Make Transparent
                }

                byte[] compressedBytes;
                compressedBytes = Compress(pixels);

                return compressedBytes;
            }

            public unsafe void CreateBmpTexture(BinaryReader reader)
            {

                //bmp
                int w = Width;// +(4 - Width % 4) % 4;
                int h = Height;// +(4 - Height % 4) % 4;

                if (w == 0 || h == 0)
                    return;
                if ((w < 2) || (h < 2)) return;
                Image = new Bitmap(w, h);

                BitmapData data = Image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite,
                                                 PixelFormat.Format32bppArgb);

                byte[] dest = Decompress(FBytes);

                Marshal.Copy(dest, 0, data.Scan0, dest.Length);

                Image.UnlockBits(data);

                //if (Image.Width > 0 && Image.Height > 0)
                //{
                //    Guid id = Guid.NewGuid();
                //    Image.Save(id + ".bmp", ImageFormat.Bmp);
                //}

                dest = null;
            }

            public unsafe void CreateTexture(BinaryReader reader)
            {
                int w = Width;// + (4 - Width % 4) % 4;
                int h = Height;// + (4 - Height % 4) % 4;
                if (w == 0 || h == 0)
                    return;
                if ((w < 2) || (h < 2)) return;

                DataRectangle stream = null;

                ImageTexture = new Texture(DXManager.Device, w, h, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                stream = ImageTexture.LockRectangle(0, LockFlags.Discard);
                Data = (byte*)stream.Data.DataPointer;

                byte[] decomp = DecompressImage(reader.ReadBytes(Length));

                stream.Data.Write(decomp, 0, decomp.Length);

                stream.Data.Dispose();
                ImageTexture.UnlockRectangle(0);

                /*if (HasMask)
                {
                    reader.ReadBytes(12);
                    w = Width;// + (4 - Width % 4) % 4;
                    h = Height;// + (4 - Height % 4) % 4;

                    MaskImageTexture = new Texture(DXManager.Device, w, h, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                    stream = MaskImageTexture.LockRectangle(0, LockFlags.Discard);

                    decomp = DecompressImage(reader.ReadBytes(Length));

                    stream.Data.Write(decomp, 0, decomp.Length);

                    stream.Data.Dispose();
                    MaskImageTexture.UnlockRectangle(0);
                }
                */

                //DXManager.TextureList.Add(this);
                TextureValid = true;
                //Image.Disposing += (o, e) =>
                //{
                //    TextureValid = false;
                //    Image = null;
                //    MaskImage = null;
                //    Data = null;
                //    DXManager.TextureList.Remove(this);
                //};
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(X);
                writer.Write(Y);
                writer.Write(FBytes.Length);
                writer.Write(FBytes);
            }

            public static byte[] Compress(byte[] raw)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    using (GZipStream gzip = new GZipStream(memory,
                    CompressionMode.Compress, true))
                    {
                        gzip.Write(raw, 0, raw.Length);
                    }
                    return memory.ToArray();
                }
            }

            static byte[] Decompress(byte[] gzip)
            {
                // Create a GZIP stream with decompression mode.
                // ... Then create a buffer and write into while reading from the GZIP stream.
                using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        return memory.ToArray();
                    }
                }
            }

            private static byte[] DecompressImage(byte[] image)
            {
                using (GZipStream stream = new GZipStream(new MemoryStream(image), CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        return memory.ToArray();
                    }
                }
            }

            public void CreatePreview()
            {
                if (Image == null)
                {
                    Preview = new Bitmap(1, 1);
                    return;
                }

                Preview = new Bitmap(64, 64);

                using (Graphics g = Graphics.FromImage(Preview))
                {
                    g.InterpolationMode = InterpolationMode.Low;//HighQualityBicubic
                    g.Clear(Color.Transparent);
                    int w = Math.Min((int)Width, 64);
                    int h = Math.Min((int)Height, 64);
                    g.DrawImage(Image, new Rectangle((64 - w) / 2, (64 - h) / 2, w, h), new Rectangle(0, 0, Width, Height), GraphicsUnit.Pixel);

                    g.Save();
                }
            }
            public unsafe void DisposeTexture()
            {
                DXManager.TextureList.Remove(this);

                if (Image != null)
                {
                    Image.Dispose();
                }

                //if (MaskImage != null)
                //{
                //    MaskImage.Dispose();
                //}

                if (ImageTexture != null && !ImageTexture.Disposed)
                {
                    ImageTexture.Dispose();
                }

                //if (MaskImageTexture != null && !MaskImageTexture.Disposed)
                //{
                //    MaskImageTexture.Dispose();
                //}

                TextureValid = false;
                Image = null;
                //MaskImage = null;
                ImageTexture = null;
                //MaskImageTexture = null;
                Data = null;
            }
        }
    }
}