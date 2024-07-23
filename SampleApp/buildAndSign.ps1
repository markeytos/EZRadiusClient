$env:Path += ";C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin" 
$env:Path += ";C:\Program Files (x86)\Windows Kits\10\App Certification Kit"
msbuild /restore /t:publish  /p:Configuration=Release /p:PublishSingleFile=True /p:SelfContained=True /p:Platform=x64 /p:RuntimeIdentifier=win-x64
SignTool sign /fd SHA256 /a /t http://timestamp.digicert.com  /n "Keytos LLC" bin\x64\Release\net8.0\win-x64\publish\RADIUSConsole.exe