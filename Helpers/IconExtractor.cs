using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;

namespace Game_Launcher.Helpers
{
    public static class IconExtractor
    {
        // COM interfaces
        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        private class ShellLink { }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        private interface IShellLinkW
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, IntPtr pfd, int fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            void Resolve(IntPtr hwnd, int fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("0000010b-0000-0000-C000-000000000046")]
        private interface IPersistFile
        {
            void GetClassID(out Guid pClassID);
            [PreserveSig]
            int IsDirty();
            void Load([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);
            void Save([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [In, MarshalAs(UnmanagedType.Bool)] bool fRemember);
            void SaveCompleted([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
            void GetCurFile([In, MarshalAs(UnmanagedType.LPWStr)] string ppszFileName);
        }

        // Win32 API
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        public static Icon ExtractIconFromShortcut(string shortcutPath)
        {
            if (!File.Exists(shortcutPath))
                throw new FileNotFoundException("Shortcut file not found", shortcutPath);

            // Load the shortcut
            ShellLink link = new ShellLink();
            ((IPersistFile)link).Load(shortcutPath, 0);
            IShellLinkW shellLink = (IShellLinkW)link;

            // Get target path
            StringBuilder targetPath = new StringBuilder(260);
            shellLink.GetPath(targetPath, targetPath.Capacity, IntPtr.Zero, 0);

            // Get icon location
            StringBuilder iconPath = new StringBuilder(260);
            int iconIndex;
            shellLink.GetIconLocation(iconPath, iconPath.Capacity, out iconIndex);

            string iconFile = iconPath.ToString();
            string target = targetPath.ToString();

            // Release COM objects
            Marshal.ReleaseComObject(shellLink);
            Marshal.ReleaseComObject(link);

            // Determine which file to extract icon from
            string fileToExtract = string.IsNullOrEmpty(iconFile) ? target : iconFile;

            if (string.IsNullOrEmpty(fileToExtract) || !File.Exists(fileToExtract))
                return null;

            // Handle .ico files directly
            if (fileToExtract.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
            {
                return new Icon(fileToExtract);
            }

            // Extract icon from exe/dll
            IntPtr hIcon = ExtractIcon(IntPtr.Zero, fileToExtract, iconIndex);

            if (hIcon == IntPtr.Zero || hIcon == new IntPtr(1))
            {
                // Try to get associated icon as fallback
                if (File.Exists(target))
                {
                    return Icon.ExtractAssociatedIcon(target);
                }
                return null;
            }

            Icon icon = Icon.FromHandle(hIcon);
            Icon clonedIcon = (Icon)icon.Clone();

            DestroyIcon(hIcon);

            return clonedIcon;
        }

        public static BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Position = 0;

                BitmapImage bitmapSource = new BitmapImage();
                bitmapSource.BeginInit();
                bitmapSource.StreamSource = memoryStream;
                bitmapSource.CacheOption = BitmapCacheOption.OnLoad;
                bitmapSource.EndInit();
                bitmapSource.Freeze();

                return bitmapSource;
            }
        }
    }
}
