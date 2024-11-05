using winpkg;


if (args.Length > 0)
{
    string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    for (int i = 0; i < args.Length; i++)
    {
        Installer.Run(Path.GetFileName(args[i]), args[i], $@"{userProfile}\Apps\{Path.GetFileName(args[i])}");
    }
}
else
{
    Console.WriteLine(Config.help);

}
