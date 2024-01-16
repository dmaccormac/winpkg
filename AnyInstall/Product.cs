using Microsoft.Win32;
using System;
using System.IO;

namespace AnyInstall
{
    class Product
    {
        private string Name, SourcePath, InstallPath;

        public Product(string Name, string SourcePath, string InstallPath)
        {
            this.Name = Name;
            this.SourcePath = SourcePath;
            this.InstallPath = InstallPath;

        }

        public void Install()
        {
            //copy files
            DirectoryCopy(SourcePath, InstallPath, true);

            //for add/remove programs
            RegistryWrite();

            Console.WriteLine("Installer finished!");
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            Console.WriteLine("Copying files to " + destDirName);

            // Get source sub dirs
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // Get dest sub dirs
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // copy files
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            //  copy sub dirs
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void RegistryWrite()
        {
            Console.WriteLine("Adding registry entries...");

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true);

            key.CreateSubKey(Name);
            key = key.OpenSubKey(Name, true);

            //key.CreateSubKey(Version.ToString());
            //key = key.OpenSubKey(Version.ToString(), true);

            key.SetValue("DisplayName", Name);
            key.SetValue("Publisher", "AnyInstall");
            key.SetValue("DisplayVersion", "1.0");
            key.SetValue("UninstallString", @"cmd.exe /c @echo off && echo AnyInstall -- https://github.com/dmaccormac && echo Uninstall--" + Name + @" && del /s """ + InstallPath + @""" && rmdir """ + InstallPath + @""" && reg delete ""HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + Name + @""" /f && pause"); 

        }

    }
}
