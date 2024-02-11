set app=AnyInstall
dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
copy %app%\bin\Release\net8.0-windows\win-x64\publish\*.exe %app%\dist