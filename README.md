# winpkg - package installer wrapper for Windows

This program creates an installer and uninstaller package for any program files.
It copies source files to the 'Apps' folder in the user's home directory.
It adds the installation path to the user PATH variable.
It creates an uninstaller for the app in Windows Settings.
It adds shortcuts for new items to the Start Menu.

# Setup
Download the latest release [here](https://github.com/dmaccormac/winpkg/releases).

# Usage 
WINPKG <SOURCE ...>

    SOURCE   folder(s) containing application files

Examples:
    winpkg foo
    winpkg foo bar
    winpkg "C:\Users\example\Downloads\baz qux"

## From the GUI
Drag and drop your source folder onto the winpkg executable.

## From the command line
`winpkg C:\Users\example\Desktop\foo`

`winpkg "C:\Users\example\Desktop\foo bar"`


# Batch file version
The batch file version `winpkg.cmd` is suitable for distribution with application source files due to its small size.

