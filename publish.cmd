set app=winpkg
dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
copy %app%\bin\Release\net8.0-windows\win-x64\publish\%app%.exe dist\%app%.exe