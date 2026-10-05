# Reconstruct the website model

The checked-in CAD is ready to inspect; rerunning these tools is optional and can overwrite generated reference files. Work on a branch or a copy. `convert_reference.py` and `prepare_parts.py` preserve the specific pinned source mesh grouping, not arbitrary replacement meshes.

## Mesh conversion

Use a Python environment compatible with the pinned packages in `requirements.txt`. The original conversion used Python 3.14 on Windows.

```sh
python -m venv .venv
# Activate the environment using the command for your shell.
python -m pip install -r tools/reconstruction/requirements.txt
python tools/reconstruction/convert_reference.py
python tools/reconstruction/prepare_parts.py
```

Both scripts default to `cad/tripilot-reference`; use `--root PATH` for a separate output/reference folder containing `references/pedal.glb`. Intermediate `mesh_parts` and the combined STL are regenerable and ignored by Git.

## SolidWorks conversion

Windows and a licensed SolidWorks 2024 installation are required. The bridge uses local SolidWorks interop libraries and the installed assembly template; adjust their paths in the scripts for other installations. These are CAD tools, not motor-control code.

```powershell
powershell -ExecutionPolicy Bypass -File tools/reconstruction/cad_bridge.ps1 -Mode build
powershell -ExecutionPolicy Bypass -File tools/reconstruction/cad_bridge.ps1 -Mode verify
```

`build` imports the 16 grouped STL files, expecting one solid per structural component, then creates a fixed assembly. It reuses existing part documents if present, so do not assume rerunning it refreshes previously imported geometry. `verify` refuses to run when matching open reference documents have unsaved edits. Otherwise it closes those matching documents, reopens the generated assembly and saves its shaded preview. It checks reference resolution and body counts, not geometry correctness or manufacturability. The original conversion was verified in SolidWorks 2024 SP0.1.

Run `python tools/validate_repository.py` from the repository root for offline geometry-file integrity and local-link checks. This does not open SolidWorks or actuate hardware.
