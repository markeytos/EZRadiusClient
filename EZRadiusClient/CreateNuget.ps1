param (
    [string] $signingCertName = "globalsign",
    [string] $signingAKV = "https://codesigningkeytos.vault.azure.net/",
    [string] $nugetVersion = "1.0.3"
)
msbuild .\SampleApp\SampleApp.csproj /restore /t:publish  /p:Configuration=Release /p:PublishSingleFile=True /p:SelfContained=True /p:Platform=x64 /p:RuntimeIdentifier=win-x64
$akvToken = (az account get-access-token  --resource https://vault.azure.net --query "accessToken").Replace('"','')
azuresigntool sign --azure-key-vault-url $signingAKV -kvc $signingCertName --azure-key-vault-accesstoken $akvToken -tr http://timestamp.digicert.com .\EZRadiusClient\EZRadiusClient\bin\Release\EZRadiusClient.$($nugetVersion).nupkg
dotnet nuget verify --all .\EZRadiusClient\EZRadiusClient\bin\Release\EZRadiusClient.$($nugetVersion).nupkg