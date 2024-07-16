dotnet build -c release
dotnet nuget sign .\bin\release\
dotnet nuget verify --all .\bin\release\