using Game_Launcher.Models;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace Game_Launcher.Helpers
{
    public static class DirectoryHelper
    {
        public static List<AppItem> DirFilesToAppItems(string directoryPath)
        {
            List<AppItem> appItems = new List<AppItem>();

            DirectoryInfo directory = new DirectoryInfo(directoryPath);

            if (!directory.Exists) Directory.CreateDirectory(directoryPath);

            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.Name.Contains(".lnk"))
                {
                    string name = Path.GetFileNameWithoutExtension(file.Name);
                    string shortcutPath = file.FullName;
                    Icon icon = IconExtractor.ExtractIconFromShortcut(shortcutPath);

                    appItems.Add(new AppItem()
                    {
                        Name = name,
                        ShortcutPath = shortcutPath,
                        Icon = icon
                    });
                }
            }

            return appItems;
        }
    }
}
