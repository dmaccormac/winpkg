set app=winpkg
dotnet publish
copy %app%\bin\Release\net8.0-windows\publish\win-x64\%app%.exe dist\%app%.exe