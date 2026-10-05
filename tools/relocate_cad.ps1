param(
    [string]$CadRoot=(Join-Path $PSScriptRoot '..\cad'),
    [string]$SolidWorksDir='C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS'
)
$ErrorActionPreference='Stop'
$target=(Resolve-Path -LiteralPath $CadRoot).Path
$expected=(Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..\cad')).Path
if ($target -ne $expected) { throw 'This helper only updates copied CAD inside this repository.' }
Add-Type -Path "$SolidWorksDir\SolidWorks.Interop.sldworks.dll"
Add-Type -Path "$PSScriptRoot\RelocateCad.cs" -ReferencedAssemblies "$SolidWorksDir\SolidWorks.Interop.sldworks.dll"
[RelocateCad]::Run($target)
