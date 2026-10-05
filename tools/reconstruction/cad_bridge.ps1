param(
    [ValidateSet('build','verify')][string]$Mode='verify',
    [string]$CadRoot=(Join-Path $PSScriptRoot '..\..\cad\tripilot-reference')
)
$ErrorActionPreference='Stop'
$lib='C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS'
Add-Type -Path "$lib\SolidWorks.Interop.sldworks.dll"
Add-Type -Path "$lib\SolidWorks.Interop.swconst.dll"
Add-Type -Path "$PSScriptRoot\CadBridge.cs" -ReferencedAssemblies "$lib\SolidWorks.Interop.sldworks.dll","$lib\SolidWorks.Interop.swconst.dll"
[CadBridge]::Run((Resolve-Path -LiteralPath $CadRoot).Path,$Mode)
