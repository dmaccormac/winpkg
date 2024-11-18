# Packaging tool for Windows

The aim of the project is to provide a tool to easily create software installation packages for Windows.

It can be used to manage ad hoc application installs or distributed with your application as an installer wrapper.

The source code is available in the project repository [here](https://github.com/dmaccormac/winpkg).
 
# Download
You can get the latest release [here](https://github.com/dmaccormac/winpkg/releases). 

The self contained executable is targeted for Windows x64 and .NET 8.0 framework.

# Usage 
The application executable `winpkg.exe` can be used from the desktop or the command line. 

It does not require installation or administrative privileges to run. 

When application files are provided to winpkg it performs the following actions: 
- Install application to user's Apps folder
- Create program shortcuts in Start Menu 
- Register environment variables (PATH)
- Create an uninstaller for the program
  
## From the desktop
Drag and drop your source folder onto the winpkg executable.

## From the command line
Provide the application source path as an argument to winpkg:

`winpkg c:\install\foobar`

## Distribution
The batch file version `winpkg.cmd` is suitable for distribution with application source files due to its small size. The batch file supports a single input parameter.

# Documentation
To view help on the command line, run winpkg without arguments.

```
C:\>winpkg
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

Version: 1.1.9
Website: https://github.com/dmaccormac/winpkg
```

See the [CHANGELOG](https://github.com/dmaccormac/winpkg/blob/main/CHANGELOG.md) for more information on release versions and features.

# Contact
[GitHub](https://github.com/dmaccormac)

[Email](mailto:mail@winpkg.org)



