$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$destination = Join-Path $root 'build\wix'
$zip = Join-Path $env:TEMP 'ltool-wix-3.14.1.zip'
$expected = '6ac824e1642d6f7277d0ed7ea09411a508f6116ba6fae0aa5f2c7daa2ff43d31'
Invoke-WebRequest 'https://github.com/wixtoolset/wix3/releases/download/wix3141rtm/wix314-binaries.zip' -OutFile $zip -UseBasicParsing
if ((Get-FileHash -LiteralPath $zip -Algorithm SHA256).Hash.ToLowerInvariant() -ne $expected) { throw 'WiX download hash mismatch.' }
New-Item -ItemType Directory -Path $destination -Force | Out-Null
Expand-Archive -LiteralPath $zip -DestinationPath $destination -Force
Write-Output 'WiX 3.14.1 downloaded and checksum verified. Build-time tools only; not included in app payload.'
