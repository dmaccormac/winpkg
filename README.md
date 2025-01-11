# Package builder for Windows

The aim of the project is to provide a command line tool to quickly build software installation packages for Windows.

It can be used to create software packages for distribution or as a utility to manage ad hoc application installations.
    
The project is built using C++ and is designed to be portable, compact and fast. 

You can download the latest release [here](https://github.com/dmaccormac/winpkg/releases).

# Usage 
The stand-alone executable `winpkg.exe` can be used from the desktop or the command line.

Provide the application source folder as a parameter and winpkg will build a stand-alone executable installer package. 

Add your own `setup.bat` to your application source folder to customize the application installer.

## From the desktop
Drag and drop your application folder onto the winpkg executable.

## From the command line
Provide the application folder path as an argument to winpkg:

`winpkg c:\install\foobar`

# Documentation
To view help on the command line, run `winpkg` without arguments.

View the CHANGELOG file [here](https://github.com/dmaccormac/winpkg/blob/main/CHANGELOG.md).

# Contact
[GitHub](https://github.com/dmaccormac)

[Email](mailto:mail@winpkg.org)