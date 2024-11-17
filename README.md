# package installer wrapper for Windows

The aim of the project is to provide a lightweight tool to create software installer packages for Windows. 

It can be used as a stand alone program or distributed with your source code as an installer wrapper. 

# Download
You can get the latest portable release [here](https://github.com/dmaccormac/winpkg/releases). 

# Usage 
```
WINPKG <SOURCE ...>

    SOURCE  folder(s) containing app files
```

The installer performs the following actions:
- Copy files to 'Apps' in user's home folder
- Create program shortcuts in the Start Menu 
- Register installation location (PATH)
- Create an uninstaller in Windows setttings

Examples:

`winpkg foo`

`winpkg foo bar`

`winpkg "C:\Users\example\Downloads\baz qux"`

## From the GUI

Drag and drop your source folder onto the winpkg executable.

## From the command line

`winpkg C:\Users\example\Desktop\foo`

`winpkg "C:\Users\example\Desktop\foo bar"`

## Distribution
The batch file version `winpkg.cmd` is suitable for distribution with application source files due to its small size.

At this time the batch file supports a single input parameter.

# Contact
[GitHub](https://github.com/dmaccormac/winpkg)

[Email](mailto:mail@winpkg.org)


