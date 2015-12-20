$ErrorActionPreference = 'Stop'

#Install IIS and set up the website
Enable-WindowsOptionalFeature -Online -FeatureName 'IIS-ASPNET45' -All
ipmo WebAdministration
New-Website -Force -PhysicalPath (resolve-path $PSScriptRoot\..) -Name HttpHandlerApp -Port 81
New-WebApplication -Site HttpHandlerApp -PhysicalPath $PSScriptRoot -Name HttpHandlerWebSite
icacls $PSScriptRoot\.. /grant "everyone:(OI)(CI)(RA,RD,X,S,GR)" /t
iisreset

#Install build tools
$buildToolsExe = Join-Path $env:TEMP BuildTools_Full.exe
Invoke-WebRequest -UseBasicParsing https://download.microsoft.com/download/E/E/D/EEDF18A8-4AED-4CE0-BEBE-70A83094FC5A/BuildTools_Full.exe -OutFile $buildToolsExe
Start-Process -Wait -FilePath $buildToolsExe -ArgumentList '/Full', '/NoRestart', '/Passive'

#Build the Reproducer project
& 'C:\Program Files (x86)\MSBuild\14.0\Bin\amd64\msbuild.exe' $PSScriptRoot\..\IISHttpHandlerRepro.sln /v:q

#List next instructions
$reproducerPath = Resolve-Path $PSScriptRoot\..\Reproducer\bin\Debug\Reproducer.exe
Write-Host -ForegroundColor Cyan "Setup complete."
Write-Host -ForegroundColor Cyan "Now run Reproducer.exe in $reproducerPath as many times as you need to diagnose the issue."