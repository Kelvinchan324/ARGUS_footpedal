# Source attribution and license status

This repository contains materials with different origins. A public GitHub repository is not, by itself, a license grant. No single license is asserted over the entire collection.

## TriPilot website geometry and figures

- Authors: Zihao Li, Yanan Zhou, Ranpeng Qiu, Hangyu Wu, Guoqiang Ren, Weiming Zhi.
- Project: https://jeong-zju.github.io/RSS-TriPilot-FF-Page/
- Website repository: https://github.com/Jeong-zju/RSS-TriPilot-FF-Page
- Website revision checked: `0c8079959350a44dec033bdc27d6277a810d10fa`.
- Original model: https://jeong-zju.github.io/RSS-TriPilot-FF-Page/static/models/pedal.glb
- Git blob ID of the matching downloaded model: `ff84085f8ea8f9bd825bb90935fc224e2907ed76`.
- Checked: 2026-10-05.

The website footer states **Creative Commons Attribution-ShareAlike 4.0 International**, linked at https://creativecommons.org/licenses/by-sa/4.0/. This notice is retained for the website-derived material and its geometry conversions. No asset-specific license file was found in the inspected website metadata; confirm model-specific rights with the authors if needed. This website notice does not establish the license of their separate control-code repository.

Changes made for this repository: converted the website mesh to grouped STL files and SolidWorks part/assembly documents, interpreted source coordinates as millimetres, centred the assembly, named component groups, applied display colours, and captured a SolidWorks preview. These are adaptations, not original engineering CAD released by the TriPilot authors. They are not endorsed by the authors. The website-derived adaptations retain the website's stated CC BY-SA 4.0 terms to the extent applicable; no broader rights are claimed.

## Official TriPilot software

Upstream: https://github.com/Jeong-zju/zeno-wholebody-teleop

Pinned commit: `6e130b2ab50e7d6adddc6a446cff6698ddc10008`.

The repository is included as a Git submodule, not as ARGUS-owned source. No root license was returned by GitHub or found in the inspected root. Relevant `common/dm_drivers` packages and `teleop_side/teleop_setup` declare `TODO` licenses. Other subdirectories have their own notices; preserve them. The authors describe their system as open source, but that description does not resolve these incomplete package declarations. Obtain clarification before publishing modified copies or applying a new license to their code.

## Original ARGUS files and ergonomic references

The original CAD and print exports came from the project owner's local `ARGUS/footpedal_joystick` folder. Boot, sole, motor and horn models may include third-party geometry; their origin and license were not established by local filenames. Their inclusion is an archive of supplied project assets, not an assertion of exclusive authorship or a new permission grant. Record the original sources before broader redistribution or reuse claims.

## ARGUS work

The repository owner has not selected a license for original ARGUS contributions. New documentation and reconstruction tooling do not remove restrictions on underlying third-party materials. Decide licensing separately for original software, original hardware designs and documentation once the source boundaries are resolved.
