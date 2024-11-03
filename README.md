# winpkg - package installer wrapper for Windows

This program creates an installer and uninstaller from application files. 

## Setup
Download the latest release [here](https://github.com/dmaccormac/winpkg/releases).

## Usage
Put your source files into a folder and provide that folder path to winpkg.
 
The following actions are performed on source files:
- Files will be copied to '%USERPROFILE%\Apps\<SOURCE>'
- The install path is added to the user PATH variable
- Uninstaller added in Windows Settings

### From the GUI
Drag and drop your source folder onto the winpkg executable.

### From the command line
`winpkg C:\Users\example\Desktop\foo` \

`winpkg "C:\Users\example\Desktop\foo bar"`


