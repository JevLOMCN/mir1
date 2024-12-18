﻿using System;
using System.IO;
using System.Windows.Forms;

namespace Map_Editor
{
    static class Program
    {
        public static string openFileWith { get; set; } = string.Empty;
        /// <summary>
        /// The main entry point for the application。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                FileInfo file = new FileInfo(args[0]);
                if (file.Exists)
                {
                    openFileWith = args[0];
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
