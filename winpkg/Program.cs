using winpkg;

if (args.Length > 0)
{
    string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    for (int i = 0; i < args.Length; i++)
    {
        var displayName = Path.GetFileName(args[i]);
        var sourcePath = args[i];
        var destinationPath = $@"{userProfile}\Apps\{Path.GetFileName(args[i])}";

        Installer.Run(displayName,sourcePath,destinationPath);

    }
}
else
{
    Console.WriteLine(Config.help);

}
