using System.Reflection;

namespace winpkg
{
    internal static class Config
    {
        internal static readonly string version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0,5);
        internal static readonly string publisher =  "winpkg";
        internal static readonly string link = "https://github.com/dmaccormac/winpkg";
        internal static readonly string date = DateTime.Now.ToString("yyyyMMdd");
        internal static readonly string help =
        $"""
        Package installer wrapper for Windows

        This program creates an installer and uninstaller package for any program files. 
        It copies source files to the 'Apps' folder in the user's home directory. 
        It adds the installation path to the user PATH variable.
        It creates an uninstaller for the app in Windows Settings.
        It adds shortcuts for new items to the Start Menu.

        WINPKG <SOURCE ...>

            SOURCE   folder(s) containing application files

        Examples: 
            winpkg foo
            winpkg foo bar
            winpkg "C:\Users\example\Downloads\baz qux"

        Version: {version}
        Website: {link}
        """;

        internal static readonly string uninstall =
        @"cmd.exe /q /c ""for /f ""skip=2 tokens=3*"" %a in ('reg query HKCU\Environment /v PATH')" +
        @" do set p=""%a %b""" +
        @" & call set p=%p:; =;%" + //trim possible trailing whitespace
        @" & call setx PATH %p:destinationPath;=%" +
        @" & reg delete ""HKCU\baseRegistryKey"" /f" +
        @" & rd /q /s ""destinationPath""" +
        @" & rd /q /s ""startMenuPath""""";

    }
}