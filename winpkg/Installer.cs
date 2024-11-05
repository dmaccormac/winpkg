using Microsoft.Win32;

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
                var uninstallString = GetUninstallString(destinationPath, baseRegistryKey);          
                
                CopyFolder(sourcePath, destinationPath);
                AddEntryToUserPath(destinationPath);

                SetRegistryKeyValue(baseRegistryKey, "DisplayName", displayName);
                SetRegistryKeyValue(baseRegistryKey, "InstallDate", Config.date);
                SetRegistryKeyValue(baseRegistryKey, "Publisher", Config.publisher);
                SetRegistryKeyValue(baseRegistryKey, "HelpLink", Config.link);
                SetRegistryKeyValue(baseRegistryKey, "UninstallString", uninstallString);

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
                    File.Copy(file, destinationFilePath);
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

        public static void AddEntryToUserPath(string entryToAdd)
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

        public static string GetUninstallString(string destinationPath, string baseRegistryKey)
        {

            string s = Config.uninstall;
            s = s.Replace("destinationPath", destinationPath);
            s = s.Replace("baseRegistryKey", baseRegistryKey);
            return s;
            
        }

    }
}

