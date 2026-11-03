param([string]$Version = '0.1.1')
$ErrorActionPreference = 'Stop'
if ($Version -notmatch '^\d+\.\d+\.\d+$') { throw 'Version must have the form 0.1.1.' }
$root = Split-Path $PSScriptRoot -Parent
$dist = Join-Path $root 'dist'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
New-Item -ItemType Directory -Path $dist -Force | Out-Null
$refs = @('/reference:System.Windows.Forms.dll','/reference:System.Drawing.dll','/reference:System.Core.dll','/reference:System.Data.dll')
& $compiler /nologo /target:winexe /platform:x64 "/win32manifest:$root\src\app.manifest" "/out:$dist\Ltool.exe" @refs "$root\src\Launcher.cs" "$root\src\AssemblyInfo.cs"
if ($LASTEXITCODE -ne 0) { throw 'Application build failed.' }
Copy-Item "$root\src\Ltool.exe.config" "$dist\Ltool.exe.config" -Force
$payload = @('Ltool.exe','Ltool.exe.config','LICENSE','NOTICE','README.md','PRIVACY.md','SECURITY_REVIEW.md')
foreach ($name in @('LICENSE','NOTICE','README.md','PRIVACY.md','SECURITY_REVIEW.md')) { Copy-Item (Join-Path $root $name) (Join-Path $dist $name) -Force }
$wix = Join-Path $root 'build\wix'
if (-not (Test-Path "$wix\candle.exe")) { throw 'WiX 3.14.1 build tools required. See scripts/get-wix.ps1.' }
$licenseText = [IO.File]::ReadAllText("$root\LICENSE").Replace('\','\\').Replace('{','\{').Replace('}','\}').Replace("`r",'').Replace("`n",'\par ')
[IO.File]::WriteAllText("$dist\License.rtf",('{\rtf1\ansi\deff0 {\fonttbl{\f0 Segoe UI;}}\f0\fs18 '+$licenseText+'}'),[Text.Encoding]::ASCII)
$setup = Join-Path $dist "Ltool-Setup-$Version-windows-x64.msi"
& "$wix\candle.exe" -nologo -arch x64 "-dPayloadDir=$dist" -out "$dist\Product.wixobj" "$root\installer\Product.wxs"
if ($LASTEXITCODE -ne 0) { throw 'MSI compilation failed.' }
& "$wix\light.exe" -nologo -ext WixUIExtension -out $setup "$dist\Product.wixobj"
if ($LASTEXITCODE -ne 0) { throw 'MSI linking or validation failed.' }
$portable = Join-Path $dist "Ltool-$Version-windows-x64-portable.zip"
Compress-Archive -LiteralPath @($payload | ForEach-Object { Join-Path $dist $_ }) -DestinationPath $portable -Force
@($setup,$portable) | ForEach-Object { $hash = Get-FileHash -LiteralPath $_ -Algorithm SHA256; '{0}  {1}' -f $hash.Hash.ToLowerInvariant(),(Split-Path $_ -Leaf) } | Set-Content -LiteralPath "$dist\SHA256SUMS.txt" -Encoding ASCII
Write-Output "Built: $setup"
Write-Output "Built: $portable"
