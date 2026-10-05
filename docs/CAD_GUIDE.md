# Footpedal CAD guide

## Two independent baselines

`cad/argus-original` preserves the supplied ARGUS SolidWorks parts, print exports and ergonomic reference files. Start with `foot_pedal_assem.SLDASM` to inspect that design. Do not mix its STS3215 motor parts with the DM4310-based TriPilot BOM.

`cad/tripilot-reference` contains the reconstruction of the model on the TriPilot website. Open `TriPilot_pedal_reconstructed.SLDASM` with SolidWorks 2024 or a compatible newer version. Keep the `SolidWorks_parts` folder beside it.

Close other assemblies with identically named parts before opening a relocated copy. SolidWorks may otherwise reuse already-open parts from another folder. If asked for missing references, select the matching part inside this repository, not an unrelated copy with the same name. See the validation report for known source-reference gaps.

## TriPilot geometry

The source GLB was separated into 93 connected mesh shells, then grouped into 16 assembly components. The 12 structural components are closed faceted solid bodies. Three motor groups and one joint/bearing group remain graphics meshes to avoid unnecessarily heavy solid geometry.

All source shells are represented in the grouped parts. `components.json`, `grouped_parts.json`, and `parts_manifest.txt` document the mapping. The original website model is retained as `references/pedal.glb`.

The envelope is approximately 250 by 249.679 mm horizontally and 155.776 mm high. Y is vertical. The source numerical coordinates were interpreted as millimetres, cross-checked against 57 mm motor housings; this is not dimensional certification. All components retain the source pose and are fixed. There are no kinematic mates or motion validation.

## Editing and printing

The TriPilot parts do not contain the original authors' sketches or design history. Use their surfaces and measurements as references for new native sketches and features. Triangle-derived hole geometry must be checked against actual hardware and supplier drawings.

The grouped STLs retain assembly coordinates and are not individually oriented for printing. Structural-part labels identify geometry roles, not approved filament or process choices. The original `3dp_to_print` folder is retained as supplied; that name does not establish fabrication validation.

Create a branch and a new revision folder for mechanical changes, leaving the baselines intact. Rebuild the highest-impact structural parts parametrically, then add mates, limits and interference checks. Record every changed interface and verified dimension in the BOM and a revision note.
