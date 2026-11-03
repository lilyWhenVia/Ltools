param([string]$Version = '0.1.0')
$ErrorActionPreference = 'Stop'
if ($Version -notmatch '^\d+\.\d+\.\d+$') { throw 'Version must have the form 0.1.0.' }
$root = Split-Path $PSScriptRoot -Parent
$dist = Join-Path $root 'dist'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
New-Item -ItemType Directory -Path $dist -Force | Out-Null
$refs = @('/reference:System.Windows.Forms.dll','/reference:System.Drawing.dll','/reference:System.Core.dll','/reference:System.Data.dll')
& $compiler /nologo /target:winexe /platform:x64 "/win32manifest:$root\src\app.manifest" "/out:$dist\LilyTools.exe" @refs "$root\src\Launcher.cs" "$root\src\AssemblyInfo.cs"
if ($LASTEXITCODE -ne 0) { throw 'Application build failed.' }
Copy-Item "$root\src\LilyTools.exe.config" "$dist\LilyTools.exe.config" -Force
$payload = @('LilyTools.exe','LilyTools.exe.config','LICENSE','NOTICE','README.md','PRIVACY.md')
foreach ($name in @('LICENSE','NOTICE','README.md','PRIVACY.md')) { Copy-Item (Join-Path $root $name) (Join-Path $dist $name) -Force }
$resources = foreach ($name in $payload) { "/resource:$dist\$name,payload.$name" }
$setup = Join-Path $dist "LilyTools-Setup-$Version-windows-x64.exe"
& $compiler /nologo /target:winexe /platform:x64 "/win32manifest:$root\src\app.manifest" "/out:$setup" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Core.dll /reference:Microsoft.CSharp.dll @resources "$root\installer\Setup.cs"
if ($LASTEXITCODE -ne 0) { throw 'Installer build failed.' }
$portable = Join-Path $dist "LilyTools-$Version-windows-x64-portable.zip"
Compress-Archive -LiteralPath @($payload | ForEach-Object { Join-Path $dist $_ }) -DestinationPath $portable -Force
@($setup,$portable) | ForEach-Object { $hash = Get-FileHash -LiteralPath $_ -Algorithm SHA256; '{0}  {1}' -f $hash.Hash.ToLowerInvariant(),(Split-Path $_ -Leaf) } | Set-Content -LiteralPath "$dist\SHA256SUMS.txt" -Encoding ASCII
Write-Output "Built: $setup"
Write-Output "Built: $portable"
