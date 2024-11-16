:: winpkg v.1.1.9b (batch) 
:: https://github.com/dmaccormac/winpkg
::
:: Package installer wrapper for Windows
::
:: This program creates an installer and uninstaller package for any program files.
:: It copies source files to the 'Apps' folder in the user's home directory.
:: It adds the installation path to the user PATH variable.
:: It creates an uninstaller for the app in Windows Settings.
:: It adds shortcuts for new items to the Start Menu.
::
:: WINPKG <SOURCE>
::
::    SOURCE   folder containing application files
:: Examples:
::     winpkgb foo
:: 
:: VARIABLES

@echo off
set src=%~1
set app=%~nx1
set dst=%USERPROFILE%\Apps\%app%
set key=HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\%app%
set unx=cmd /q /c ""for /f ""skip=2 tokens=3*"" %%a in ('reg query HKCU\Environment /v PATH') do set p=""%%a %%b"" ^& call set p=%%p:; =;%% ^& call setx PATH %%p:%dst%;=%% ^& reg delete ""%key%"" /f ^& rd /q /s ""%dst%"" ^& rd /q /s ""%APPDATA%\Microsoft\Windows\Start Menu\Programs\%app%""
set ver=1.1.9b

echo winpkg %ver%
echo Installing %src% ==^> %dst%

:: COPY FILES
xcopy "%src%" "%dst%" /E /I /H /Y /Q 1>nul

:: REGISTRY KEYS
reg add "%key%" /v "DisplayName" /t REG_SZ /d "%app%" /f 1>nul
reg add "%key%" /v "UninstallString" /t REG_SZ /d "%unx%" /f 1>nul
reg add "%key%" /v "Publisher" /t REG_SZ /d "winpkg" /f 1>nul
reg add "%key%" /v "HelpLink" /t REG_SZ /d "https://github.com/dmaccormac/winpkg" /f 1>nul

:: SET PATH
for /f "skip=2 tokens=3*" %%a in ('reg query HKCU\Environment /v PATH') do (set p="%%~a %%~b")
set p=%p:"=%
set p=%p:; =;%
setx PATH "%p%%dst%;" 1>nul
::setx PATH "%p: =%%dst%;" 1>nul

:: START MENU
mkdir "%APPDATA%\Microsoft\Windows\Start Menu\Programs\%app%" 2>nul

for /f "delims=" %%a in ('dir /S /B "%dst%\*.exe"') do (
	echo Set objShell = CreateObject^("WScript.Shell"^) > winpkg.vbs
	echo Set objShortcut = objShell.CreateShortcut^("%APPDATA%\Microsoft\Windows\Start Menu\Programs\%app%\%%~na.lnk"^) >> winpkg.vbs
	echo objShortcut.TargetPath = "%%a" >> winpkg.vbs
	echo objShortcut.IconLocation = "%%a%" >> winpkg.vbs
	echo objShortcut.Save >> winpkg.vbs
	cscript //nologo //b winpkg.vbs
	del /q winpkg.vbs
	)