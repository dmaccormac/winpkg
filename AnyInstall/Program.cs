using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace AnyInstall
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args == null || args.Length == 0) 
            {
                string version = (FileVersionInfo.GetVersionInfo(Assembly.GetCallingAssembly().Location).ProductVersion).ToString();

                Console.WriteLine("AnyInstall " + version.Substring(0,3) + " -- https://github.com/dmaccormac \n" +
                    "This tool acts as a makeshift installer for any program files you provide. \n" +
                    "Add any program files to a folder, then drop that folder onto this executable. \n" +
                    @"The folder will be copied to '%username%\Programs' and an uninstaller added under 'Add/Remove Programs'." 
                    );

                Console.ReadKey();

            }

            else
            {
                if (Directory.Exists(args[0]))
                {
                    string SourcePath = (args[0]);
                    string ProductName = Path.GetFileName(args[0]);
                    string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    string InstallPath = userPath + @"\Programs\" + ProductName;

                    Console.WriteLine("AnyInstall -- https://github.com/dmaccormac");
                    Console.WriteLine ("Install " + ProductName + " to " + InstallPath);

                    Product p = new Product(ProductName, SourcePath, InstallPath);
                    p.Install();
              
                    Console.ReadKey();
                }

                else
                {
                    Console.WriteLine("A directory must be supplied as install source.");
                    Console.ReadKey();
                }
            }

        }
    }
}
