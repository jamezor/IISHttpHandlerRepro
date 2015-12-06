$siteName = 'HttpHandlerWebSite'
$appName = 'HttpHandlerApp'
$sitePath = Resolve-Path $PSScriptRoot\..
$appPath = $PSScriptRoot

$currentUser = "$($env:userdomain)\$($env:username)"

$unmanagedString = [System.IntPtr]::Zero;
try
{
    $unmanagedString = [Runtime.InteropServices.Marshal]::SecureStringToGlobalAllocUnicode((Read-Host -AsSecureString "Please enter your current user's password so we can run the app pool as you"))
    $password = [Runtime.InteropServices.Marshal]::PtrToStringUni($unmanagedString)
}
finally
{
    [Runtime.InteropServices.Marshal]::ZeroFreeGlobalAllocUnicode($unmanagedString)
}

Set-AppPool $appName $currentUser $password

Set-WebSite $siteName $appName $sitePath
Set-WebSiteBindings $siteName @{protocol="http"; port=81}

if(-not (Get-WebApplication -Site $siteName -Name $appName)) {
    New-WebApplication -Site $siteName -Name $appName -PhysicalPath $appPath | Out-Null
}
$webAppPath = "IIS:\Sites\$($siteName)\$($appName)"
Set-ItemProperty $webAppPath -Name applicationPool -Value $appName

Set-WebConfigurationProperty -filter /system.WebServer/security/authentication/AnonymousAuthentication -name username -value "" -location $siteName

iisreset