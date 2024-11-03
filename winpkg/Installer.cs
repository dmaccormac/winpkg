using Microsoft.Win32;

namespace AnyInstall
{
    internal static class Installer
    {
        public static void Run(string applicationName, string sourceFolder, string destinationFolder) 
        {
            try
            {
                CopyFolder(sourceFolder, destinationFolder);
                AddEntryToUserPath(destinationFolder);

                var baseRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + applicationName;

                var uninstallString = @$"cmd.exe /c rd /q /s ""{destinationFolder}""";
                uninstallString += @$" & for /f ""skip=2 tokens=3*"" %a in ('reg query HKCU\Environment /v PATH')";
                uninstallString += @$" do if [%b]==[] (set %pathbak=%a) else (set pathbak=%a %b)";
                uninstallString += @$" & call setx PATH ""%pathbak:{destinationFolder};=%""";
                uninstallString += @$" & reg delete ""HKCU\{baseRegistryKey}"" /f";

                SetRegistryKeyValue(baseRegistryKey, "DisplayName", applicationName);
                SetRegistryKeyValue(baseRegistryKey, "UninstallString", uninstallString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Install error: {ex.Message}" );
            }

        }

        private static void CopyFolder(string sourceFolder, string destinationFolder)
        {
            try
            {
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destinationFilePath = Path.Combine(destinationFolder, fileName);
                    File.Copy(file, destinationFilePath);
                }

                string[] subFolders = Directory.GetDirectories(sourceFolder);
                foreach (string subFolder in subFolders)
                {
                    string folderName = Path.GetFileName(subFolder);
                    string destinationSubFolder = Path.Combine(destinationFolder, folderName);
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


    }
}

