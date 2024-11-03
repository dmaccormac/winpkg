namespace AnyInstall
{
    class Program
    {
        static void Main(string[] args)
        {
            string helpMessage =
                """
                Package installer wrapper for Windows

                WINPKG <SOURCE>

                    SOURCE   folder containing application files

                The following actions are performed on source files:
                - Files are copied to '%USERPROFILE%\Apps\<SOURCE>'               
                - The install path is appended to the user PATH variable
                - An uninstaller is added in Windows Settings

                Examples: 
                    winpkg C:\Users\example\Downloads\foo
                    winpkg "C:\Users\example\Downloads\foo bar"

                Version: 1.1.0
                https://github.com/dmaccormac
                """;

            if (args.Length > 0)
            {
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string applicationName = Path.GetFileName(args[0]);
                string sourceFolder = (args[0]);
                string destinationFolder = $@"{userProfile}\Apps\{applicationName}";

                Installer.Run(applicationName, sourceFolder, destinationFolder);
            }
            else
            {
                Console.WriteLine(helpMessage);

            }

        }
    }
}
