using Microsoft.Win32;
using System.Diagnostics;

namespace winpkg
{
    internal static class Installer
    {
        public static void Run(string displayName, string sourcePath, string destinationPath) 
        {
            try
            {
                Console.WriteLine("winpkg " + Config.version);
                Console.WriteLine("Installing " + sourcePath + " ==> " + destinationPath);

                var baseRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + displayName;
                var uninstallString = GetUninstallString(displayName, destinationPath, baseRegistryKey);          
                
                CopyFolder(sourcePath, destinationPath);
                AddEntryToUserPath(destinationPath);

                SetRegistryKeyValue(baseRegistryKey, "DisplayName", displayName);
                SetRegistryKeyValue(baseRegistryKey, "InstallDate", Config.date);
                SetRegistryKeyValue(baseRegistryKey, "Publisher", Config.publisher);
                SetRegistryKeyValue(baseRegistryKey, "HelpLink", Config.link);
                SetRegistryKeyValue(baseRegistryKey, "UninstallString", uninstallString);

                CreateStartMenuShortcuts(displayName, destinationPath);

                Console.WriteLine("The operation completed successfully.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Install error: {ex.Message}" );
            }

        }

        private static void CopyFolder(string sourcePath, string destinationPath)
        {
            try
            {
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                string[] files = Directory.GetFiles(sourcePath);
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destinationFilePath = Path.Combine(destinationPath, fileName);
                    System.IO.File.Copy(file, destinationFilePath);
                }

                string[] subFolders = Directory.GetDirectories(sourcePath);
                foreach (string subFolder in subFolders)
                {
                    string folderName = Path.GetFileName(subFolder);
                    string destinationSubFolder = Path.Combine(destinationPath, folderName);
                    CopyFolder(subFolder, destinationSubFolder);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error copying folder: {ex.Message}", ex);
            }
        }


        private static void SetRegistryKeyValue(string keyName, string valueName, string valueData)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(keyName))
                {
                    if (key != null)
                    {
                        key.SetValue(valueName, valueData);
                    }
                    else
                    {
                        throw new Exception("Failed to create or open registry key.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error setting registry key value: {ex.Message}", ex);
            }
        }

        private static void AddEntryToUserPath(string entryToAdd)
        {
            try
            {
                var currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ??
                    throw new NullReferenceException("Error getting PATH variable");

                if (!currentPath.Split(';').Contains(entryToAdd, StringComparer.OrdinalIgnoreCase))
                {
                    string newPath = currentPath + entryToAdd + ';';
                    Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.User);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding entry to the PATH variable: {ex.Message}");
            }
        }

        private static string GetUninstallString(string displayName, string destinationPath, string baseRegistryKey)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);  // For Roaming AppData
            string newFolderName = @"Microsoft\Windows\Start Menu\Programs\" + displayName;
            string startMenuPath = Path.Combine(appDataPath, newFolderName);


            string s = Config.uninstall;
            s = s.Replace("destinationPath", destinationPath);
            s = s.Replace("baseRegistryKey", baseRegistryKey);
            s = s.Replace("startMenuPath", startMenuPath);
            return s;
            
        }

        private static void CreateStartMenuShortcuts(string displayName, string destinationPath)
        {


            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // Roaming AppData
            // string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); // Local AppData
            string newFolderName = @"Microsoft\Windows\Start Menu\Programs\" + displayName;
            string newFolderPath = Path.Combine(appDataPath, newFolderName);

            if (!Directory.Exists(newFolderPath))
            {
                Directory.CreateDirectory(newFolderPath);
                Console.WriteLine("Folder created successfully at: " + newFolderPath);
            }
            else
            {
                Console.WriteLine("The folder already exists: " + newFolderPath);
            }


            var exeFiles = Directory.GetFiles(destinationPath, "*.exe");
            foreach (string exeFile in exeFiles)
            {
                var link = newFolderPath + $@"\{Path.GetFileNameWithoutExtension(exeFile)}.lnk";
                var target = Path.GetFullPath(exeFile);
                CreateShortcut(link, target);
            }
        }

        private static void CreateShortcut(string shortcutPath, string targetPath)
        {
            try
            {
                string vbScriptCode = $@"
                ' VBScript to create a shortcut

                Dim objShell, objShortcut
                Dim shortcutPath, targetPath, iconPath

                shortcutPath = ""{shortcutPath}""
                targetPath = ""{targetPath}""
                iconPath = ""{targetPath}""

                Set objShell = CreateObject(""WScript.Shell"")
                Set objShortcut = objShell.CreateShortcut(shortcutPath)

                objShortcut.TargetPath = targetPath  
                objShortcut.IconLocation = iconPath 
                objShortcut.WindowStyle = 1         ' Normal window (1 = Normal, 7 = Minimized, 3 = Maximized)

                objShortcut.Save
                Set objShortcut = Nothing
                Set objShell = Nothing
                ";

                string tempFilePath = System.IO.Path.GetTempFileName() + ".vbs";
                System.IO.File.WriteAllText(tempFilePath, vbScriptCode);

                var process = new Process();
                process.StartInfo.FileName = "cscript.exe";
                process.StartInfo.Arguments = tempFilePath;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Console.WriteLine("VBScript Output: " + output);

                System.IO.File.Delete(tempFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    }
}