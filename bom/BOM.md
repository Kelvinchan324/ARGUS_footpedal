# Footpedal bill of materials

Working inventory as of 2026-10-05, **not a purchase-ready BOM**. Quantities refer to one pedal. Keep the two designs separate: the original ARGUS assembly uses STS3215-related parts, while the TriPilot reference uses three DM4310-family motors. These motors are not interchangeable without mechanical and electrical redesign.

## TriPilot purchased parts

| Item | Quantity | Identification and evidence | Still to confirm |
| --- | --- | --- | --- |
| Pedal motors | 3 | Paper v2 Appendix A Table 8 calls them **dmBots DM-J4310**. Upstream `dm_motor.yaml` configures three **DM4310** motors. Three motor groups appear in the reconstructed CAD. | Exact orderable revision, voltage variant, connector, firmware, output interface and included accessories. Do not infer V1.1 or a voltage from the CAD filenames. |
| Universal joints | 3 | Paper Table 8; source model contains joint geometry. | Supplier, bore sizes, pin sizes, ratings and whether supplied with bearings. The CAD hardware group is not a vendor specification. |
| Damiao USB-to-CAN interface | 1 proposed for a standalone pedal | Upstream README specifies a Damiao USB2CAN module; the motor configuration uses one serial port for all three motors. | Exact compatible interface and protocol revision; included USB/CAN cables. Quantity is a setup inference, not a published purchasing BOM. |
| Motor power supply | To specify | Required for powered motors; not selected in this repository. | Voltage and continuous/peak current for the actual motor variant, protection, regeneration handling and mains isolation. |
| CAN and power harness | 1 set proposed | Required system integration, not quantified by the source model. | Cable gauges, connector part numbers, pinout, routing, termination and strain relief. |
| Fasteners, washers, nuts or threaded inserts | To count | Small hardware exists in the mesh, but shell counts are not fastener counts. | Thread sizes, lengths, grades, quantities and interference clearances from verified drawings. |
| Bearings, pins and spacers not included in joints | To count | Combined with other hardware in CAD component 16. | Determine which are purchased separately; avoid double-counting joint internals. |
| Rubber or other non-slip foot surface | Proposed | ARGUS improvement, not confirmed as an original TriPilot purchase. | Material, thickness, attachment and wear resistance. |
| Emergency power cutoff, fuse and enclosure | To design | ARGUS safety integration requirement, not an extracted source BOM. | Ratings, circuit architecture, accessible placement and reset behavior. |
| Host computer | 1 setup dependency | Official software is a ROS/catkin stack. | Supported environment and a pedal-only software build. |

For reproducing obstacle-derived haptics, the paper lists a **SLAMTEC C1 lidar on the robot**. It is not required for a passive CAD prototype or a standalone pedal with simulated feedback. The mobile base, arms and cameras are outside this pedal BOM. [Source: paper v2 Appendix A](https://arxiv.org/html/2602.09888v2#A1).

Motor and interface configuration: [pinned upstream motor configuration](../software/upstream/tripilot/common/dm_drivers/dm_controllers/config/dm_motor.yaml), [upstream setup README](../software/upstream/tripilot/README.md).

## TriPilot structural part inventory

Each row is one **mesh-derived closed solid**, not a verified print instruction. Quantities come from the reconstructed assembly. All files are in [SolidWorks_parts](../cad/tripilot-reference/SolidWorks_parts), with matching STLs in [exchange_parts](../cad/tripilot-reference/exchange_parts). Filament, infill, wall count, layer orientation, inserts and tolerances remain unselected.

| Part stem | Quantity | Role |
| --- | --- | --- |
| 01_Housing_motor_support | 1 | Main motor support housing |
| 02_Base_plate | 1 | Lower plate |
| 03_Inner_footplate | 1 | Inner plate |
| 04_Outer_footplate | 1 | Outer footplate |
| 05_Central_yaw_coupler | 1 | Central linkage |
| 06_X_motor_crank | 1 | X linkage crank |
| 07_Y_motor_crank | 1 | Y linkage crank |
| 08_Y_vertical_link | 1 | Y vertical link |
| 09_X_vertical_link | 1 | X vertical link |
| 10_Plate_joint_mount_A | 1 | Plate joint mount A |
| 11_Plate_joint_mount_B | 1 | Plate joint mount B |
| 12_Plate_joint_mount_C | 1 | Plate joint mount C |

Components 13-15 are motor reference groups. Component 16 groups joints and bearings; its assembly quantity of one is **not** a purchasing quantity of one joint.

## Existing ARGUS assembly inventory

The following instance counts were inspected in the existing `foot_pedal_assem.SLDASM` active configuration. They exclude suppressed components and are not proof that the assembly is complete.

| Existing component | Active instances | Identification limits |
| --- | --- | --- |
| servoFeetechSTS3215_footpedal.SLDPRT | 1 | Feetech STS3215 family inferred from filename; verify actual motor variant. |
| STS3215_metal_horn_footpedal.SLDPRT | 2 | Verify spline and fasteners against actual hardware. |
| 20_50_spring.SLDPRT | 2 | Filename alone does not establish dimensions, spring rate or preload. |
| foot_pedal.SLDPRT | 1 | Native structural part. |
| foot_pedal_roll_mount.SLDPRT | 1 | Native structural part. |
| pitch_motor_cover.SLDPRT | 1 | Native structural part. |
| pitch_yaw_connector.SLDPRT | 1 | Native structural part. |

The saved assembly also references `boot.stp.SLDPRT`, which was not found as a separate file in the source folder and is not active in the inspected configuration. Available boot STEP/IGES/Rhino source geometry is retained under `cad/argus-original/boot`. Sole and boot assets are ergonomic references, not additional required manufactured pedal components.

## Before ordering

- Resolve the exact DM-J4310/DM4310 variant and obtain its supplier drawing.
- Measure joint and motor interfaces; reconcile them with CAD rather than purchasing from mesh dimensions alone.
- Add supplier URLs, quantities, unit prices and checked specifications as they are verified.
- Complete fastener and electrical BOMs after the mechanical revision is chosen.
- Record material and print settings only after fit and load testing.
