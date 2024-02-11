namespace AnyInstall
{
    class Program
    {
        static void Main(string[] args)
        {
            string helpMessage =
                """
                AnyInstall - Installer for any program files that you provide.

                Add any program files to a folder, then drop that folder onto this executable.
                Files will be copied to '%username%\Programs' and an uninstaller added in Windows Settings.

                https://github.com/dmaccormac
                """;

            if (args.Length > 0)
            {
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string applicationName = Path.GetFileName(args[0]);
                string sourceFolder = (args[0]);
                string destinationFolder = $@"{userProfile}\Programs\{applicationName}";

                Installer.Run(applicationName, sourceFolder, destinationFolder);
            }
            else
            {
                Console.WriteLine(helpMessage);

            }

        }
    }
}
