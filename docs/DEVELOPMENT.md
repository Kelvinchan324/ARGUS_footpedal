# Footpedal development plan

These are proposed improvement directions, not implemented or measured improvements.

## Mechanical baseline

- Confirm actual motor variants and joint dimensions before changing geometry.
- Rebuild the plate, housing and linkage interfaces as native parametric parts.
- Add mates and travel limits; check clearance through the full intended motion range.
- Evaluate adjustable foot positioning, a replaceable non-slip surface, mounting stability, pinch-point guards and accessible wiring.
- Check bearing support, backlash, fastener retention and service access. Select print orientation and material only after loads and interfaces are defined.

## Control development

- First record pedal positions without enabling actuation.
- Establish axis signs, neutral calibration, usable range, deadzones and command limits in simulation.
- Add explicit arming, a hardware emergency power cutoff, stale-data handling, a communications watchdog and bounded output ramps before powered tests.
- Commission one unloaded axis at a time with conservative limits; establish a safe shutdown behavior for every fault path.
- Introduce centering and damping only after measuring the mechanism. Add obstacle feedback later, rather than using the upstream gains as validated ARGUS settings.

## Evidence for improvement

Compare revisions using measured neutral repeatability, backlash, usable range, actuation force, temperature, response latency, assembly time and operator comfort. Keep test conditions and results with each revision. Do not claim a new design is safer or better solely from its appearance.

## Collaboration

Keep original ARGUS CAD, TriPilot reference geometry and new revisions separate. Make small commits with a clear purpose. Include changed CAD, exported comparison images and BOM updates together. Before publishing derivatives of upstream code or third-party CAD, resolve the relevant license status in the notices.
