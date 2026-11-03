$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$build = Join-Path $root 'build'
New-Item -ItemType Directory -Path $build -Force | Out-Null
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
& $compiler /nologo /target:exe /platform:x64 /main:Tests "/out:$build\Tests.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Core.dll /reference:System.Data.dll "$root\src\Launcher.cs" "$root\tests\Tests.cs"
if ($LASTEXITCODE -ne 0) { throw 'Test build failed.' }
& "$build\Tests.exe"
if ($LASTEXITCODE -ne 0) { throw 'Tests failed.' }
