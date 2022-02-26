dotnet tool uninstall cpack --global
dotnet pack
dotnet tool install cpack --global --add-source CPack/nupkg