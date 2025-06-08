param (
    [string] $signingCertName = "globalsign",
    [string] $signingAKV = "https://codesigningkeytos.vault.azure.net/"
)
msbuild .\SampleApp\SampleApp.csproj /restore /t:publish  /p:Configuration=Release /p:PublishSingleFile=True /p:SelfContained=True /p:Platform=x64 /p:RuntimeIdentifier=win-x64
pwd
ls
$akvToken = (az account get-access-token  --resource https://vault.azure.net --query "accessToken").Replace('"','')
azuresigntool sign --azure-key-vault-url $signingAKV -kvc $signingCertName --azure-key-vault-accesstoken $akvToken -tr http://timestamp.digicert.com .\EZRadiusClient\SampleApp\bin\Release\net8.0-windows\win-x64\publish\*.exe