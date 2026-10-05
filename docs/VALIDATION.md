# Repository preparation checks

Checked on 2026-10-05.

## Original reconstruction

Before repository packaging, the reconstructed TriPilot assembly was saved and reopened in SolidWorks 2024 SP0.1 with zero load errors and zero load warnings. It rebuilt successfully with 16 resolved fixed components and 12 solid bodies. This is a geometry import check, not a load, collision, motion or manufacturing validation.

## Repository package

The copied assembly references were submitted to the SolidWorks reference-replacement API, targeting parts under this repository. The API returned success for the available part references; the detailed report is [cad/reference_audit.txt](../cad/reference_audit.txt). This does not replace a fresh reopen test on a second machine.

A fresh native reopen of the repository copy was not completed because identically named reference documents are open with unsaved changes. The verification tool refused to close them. Save or close those documents yourself, then run `tools/reconstruction/cad_bridge.ps1 -Mode verify` to check the repository assembly. Do not discard your work to run a verification script.

The original ARGUS assembly has an unresolved saved reference to `boot.stp.SLDPRT`; that component was not active in the inspected configuration. The supplied boot exchange geometry is retained. The seven distinct active part files and their nine active instances were found. Fasteners and missing/suppressed components were not inferred as a complete BOM.

The offline validation checks 16 component groups, all 93 source shells assigned once, the 12 structural entries, native payload presence, binary STL triangle counts and file sizes, the source GLB blob hash, and local documentation links. It does not validate SolidWorks feature geometry.

## Not tested

No upstream ROS build, motor communication, firmware update, powered movement or force-feedback test was performed. No complete procurement list, print process, load rating, tolerance stack, material selection, kinematic motion envelope or licensing clearance is claimed.
