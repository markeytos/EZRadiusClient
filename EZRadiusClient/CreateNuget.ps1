param (
    [string] $signingCertName = "globalsign",
    [string] $signingAKV = "https://codesigningkeytos.vault.azure.net/",
    [string] $nugetVersion = "1.0.3"
)
dotnet build .\EZRadiusClient\EZRadiusClient.csproj -c release -p:Version=$nugetVersion
$akvToken = (az account get-access-token  --resource https://vault.azure.net --query "accessToken").Replace('"','')
azuresigntool sign --azure-key-vault-url $signingAKV -kvc $signingCertName --azure-key-vault-accesstoken $akvToken -tr http://timestamp.digicert.com .\EZRadiusClient\bin\Release\EZRadiusClient.$($nugetVersion).nupkg
dotnet nuget verify --all .\EZRadiusClient\bin\release\EZRadiusClient.$($nugetVersion).nupkg