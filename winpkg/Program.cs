namespace winpkg
{
    public enum BuildType
    {
        PowerShell,
        Batch,
        Compact,
        Executable,

    }
    class Program
    {
        static void Main(string[] args)
        {
            // Check if no arguments are provided
            if (args.Length == 0)
            {
                Console.WriteLine(Config.help);
                return; // Exit the program
            }

            Console.WriteLine("winpkg " + Config.version);
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "/i":
                        while (i + 1 < args.Length && !args[i + 1].StartsWith("/"))
                        {

                            RunInstaller(args, ++i);
                        }
                        break;

                    case "/b":
                        while (i + 1 < args.Length && !args[i + 1].StartsWith("/"))
                        {
                            HandleBuild(args, ++i, BuildType.PowerShell);
                        }
                        break;
                    case "/c":
                        while (i + 1 < args.Length && !args[i + 1].StartsWith("/"))
                        {
                            HandleBuild(args, ++i, BuildType.Compact); 
                        }
                        break;
                    case "/t":
                        while (i + 1 < args.Length && !args[i + 1].StartsWith("/"))
                        {
                            HandleBuild(args, ++i, BuildType.Batch);
                        }
                        break;
                    case "/x":
                        while (i + 1 < args.Length && !args[i + 1].StartsWith("/"))
                        {
                            HandleBuild(args, ++i, BuildType.Executable);
                        }
                        break;
                    case "/?":
                    case "/h":
                    case "/help":
                    case "help":
                        Console.Write(Config.help);
                        break;
                    default:
                        if (args.Length == 1 & Directory.Exists(args[0]))
                            RunInstaller(args, i);
                        else
                            Console.WriteLine($"Unknown argument: {args[i]}");
                        break;


                }
            }
        }

        static void RunInstaller(string[] args, int curr)
        {
            Console.WriteLine($"Install: {args[curr]}");
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var displayName = Path.GetFileName(args[curr]);
            var sourcePath = args[curr];
            var destinationPath = $@"{userProfile}\Apps\{Path.GetFileName(args[curr])}";
            Installer.Run(displayName, sourcePath, destinationPath);

        }

        static void HandleBuild(string[] args, int curr, BuildType buildType)
        {
            string installerContent = "";

            switch (buildType)
            {
                case BuildType.PowerShell:
                    Console.WriteLine("Add code for ps");
                    return;
                case BuildType.Batch:
                    installerContent = Config.batchFile;
                    break;
                case BuildType.Compact:
                    installerContent = Config.compactBatchFile;
                    break;
                case BuildType.Executable:
                    Console.WriteLine("Add code for exe");
                    return;
                default:
                    throw new ArgumentException("Invalid BuildType provided");
            }


            Console.WriteLine($"Build {buildType.ToString()}: {args[curr]}");
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var displayName = Path.GetFileName(args[curr]);
            var sourcePath = args[curr];
            var destinationPath = $@"{userProfile}\Apps\{Path.GetFileName(args[curr])}";

            
            installerContent = installerContent.Replace("destinationPath", destinationPath);
            installerContent = installerContent.Replace("sourcePath", sourcePath);
            installerContent = installerContent.Replace("displayName", displayName);

            //string trimmedBatchFile = string.Join(Environment.NewLine, installerContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(line => line.TrimStart()));
            string outFileName = displayName + ".cmd";
            //System.IO.File.WriteAllText(batchFileName, trimmedBatchFile);
            System.IO.File.WriteAllText(outFileName, installerContent);
            Console.WriteLine("Created installer " + outFileName);
        }

    }

}
