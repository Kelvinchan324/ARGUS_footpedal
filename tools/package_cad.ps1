param([Parameter(Mandatory=$true)][string]$SourceRoot)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$source = (Resolve-Path -LiteralPath $SourceRoot).Path
$original = Join-Path $repoRoot 'cad\argus-original'
$reference = Join-Path $repoRoot 'cad\tripilot-reference'
if (Test-Path -LiteralPath $original) { throw 'Destination already exists; refuse to overwrite CAD.' }
if (Test-Path -LiteralPath $reference) { throw 'Destination already exists; refuse to overwrite CAD.' }
New-Item -ItemType Directory -Path $original,$reference -Force | Out-Null
Get-ChildItem -LiteralPath $source -File | Where-Object { !$_.Name.StartsWith('~$') } | Copy-Item -Destination $original
foreach ($folder in @('boot','sole','3dp_to_print')) {
    Copy-Item -LiteralPath (Join-Path $source $folder) -Destination $original -Recurse
}
$reconstruction = Join-Path $source 'TriPilot_reconstruction'
foreach ($folder in @('SolidWorks_parts','exchange_parts')) {
    Copy-Item -LiteralPath (Join-Path $reconstruction $folder) -Destination $reference -Recurse
}
foreach ($file in @('TriPilot_pedal_reconstructed.SLDASM','components.json','grouped_parts.json','parts_manifest.txt')) {
    Copy-Item -LiteralPath (Join-Path $reconstruction $file) -Destination $reference
}
New-Item -ItemType Directory -Path (Join-Path $reference 'references') -Force | Out-Null
foreach ($file in @('pedal.glb','appendix_pedal_1.png','appendix_pedal_2.png')) {
    Copy-Item -LiteralPath (Join-Path $reconstruction ('references\' + $file)) -Destination (Join-Path $reference 'references')
}
$images = Join-Path $repoRoot 'docs\images'
New-Item -ItemType Directory -Path $images -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $reconstruction 'assembly_preview.png') -Destination $images
$toolFolder = Join-Path $PSScriptRoot 'reconstruction'
New-Item -ItemType Directory -Path $toolFolder -Force | Out-Null
foreach ($file in @('convert_reference.py','prepare_parts.py','CadBridge.cs','cad_bridge.ps1')) {
    Copy-Item -LiteralPath (Join-Path $reconstruction $file) -Destination $toolFolder
}
Write-Output 'Copied original CAD and reconstructed reference without altering source files.'
