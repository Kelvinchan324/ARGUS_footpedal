# Footpedal software

`upstream/tripilot` is a pinned submodule of the authors' full TriPilot-FF repository, not a modified ARGUS controller. Run `git submodule update --init --recursive` from the repository root to populate it. The pinned commit is `6e130b2ab50e7d6adddc6a446cff6698ddc10008`.

No dedicated ARGUS footpedal firmware was found in the supplied pedal project. Existing reconstruction code is in [tools/reconstruction](../tools/reconstruction). Generic servo examples elsewhere in the robotics workspace are not represented here as a tested pedal controller.

## Useful upstream entry points

All paths below are inside `upstream/tripilot`.

| Path | Purpose |
| --- | --- |
| `common/dm_drivers/dm_hw` | Damiao hardware interface and protocol implementation |
| `common/dm_drivers/dm_controllers` | Pedal motor controllers |
| `common/dm_drivers/dm_controllers/config/dm_motor.yaml` | Three DM4310 entries and connection settings |
| `teleop_side/teleop_setup/config/paddle_haptic.yaml` | Axis-specific haptic parameters |
| `teleop_side/teleop_setup/scripts/paddle_haptic_client.py` | Lidar-driven pedal feedback |
| `bridge/scripts/ranger_teleop_to_robot_paddle.py` | Pedal state to mobile-base commands |
| `bridge/config/paddle.yaml` | Axis ranges and command mapping |
| `README.md` | Full system dependencies and launch instructions |

This software targets the authors' ROS/catkin whole-body system, not a plug-and-play Windows USB joystick. Its launch files can enable real motors, arms and a mobile base. No launch files were executed during repository preparation; no hardware behavior or full ROS build has been verified.

## Development boundary

Keep the pinned upstream reference unchanged while evaluating it. Place new ARGUS-specific implementations outside the submodule. Do not edit files inside the submodule and assume an ordinary parent-repository commit will publish those edits: a submodule only records an upstream commit pointer.

There is no repository-wide upstream license file, and relevant pedal ROS packages contain `TODO` license declarations at the pinned commit. Clarify the authors' intended code license before publishing modified copies or relicensing their implementation. The submodule preserves provenance without copying that source into the ARGUS Git history. See [third-party notices](../THIRD_PARTY_NOTICES.md).

The proposed first software milestone is a read-only pedal-state logger, followed by calibrated command mapping in simulation. Motor actuation should follow a separate reviewed safety and commissioning plan; see [development notes](../docs/DEVELOPMENT.md).
