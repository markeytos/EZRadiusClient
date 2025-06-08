param (
    [string] $signingCertName = "globalsign",
    [string] $signingAKV = "https://codesigningkeytos.vault.azure.net/"
)
# $env:Path += ";C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin" 
# $env:Path += ";C:\Program Files (x86)\Windows Kits\10\App Certification Kit"
msbuild .\SampleApp\SampleApp.csproj /restore /t:publish  /p:Configuration=Release /p:PublishSingleFile=True /p:SelfContained=True /p:Platform=x64 /p:RuntimeIdentifier=win-x64
pwd
ls
$akvToken = (az account get-access-token  --resource https://vault.azure.net --query "accessToken").Replace('"','')
azuresigntool sign --azure-key-vault-url $signingAKV -kvc $signingCertName --azure-key-vault-accesstoken $akvToken -tr http://timestamp.digicert.com .\SampleApp\bin\x64\Release\net8.0\win-x64\publish\*.exe