from pathlib import Path
import json
import numpy as np
import trimesh
import argparse

parser=argparse.ArgumentParser(description='Split the pinned TriPilot web model into source shells.')
parser.add_argument('--root',type=Path,default=Path(__file__).resolve().parents[2]/'cad'/'tripilot-reference')
root=parser.parse_args().root.resolve()
scene=trimesh.load(root/'references/pedal.glb',process=False)
mesh=scene.to_mesh()
print('Raw',len(mesh.vertices),len(mesh.faces),mesh.bounds,flush=True)
# Web asset stores millimetre-valued coordinates despite the glTF metre convention.
mesh.merge_vertices(digits_vertex=5)
pieces=mesh.split(only_watertight=False,repair=False)
pieces=sorted(pieces,key=lambda m:abs(m.volume),reverse=True)
out=root/'mesh_parts';out.mkdir(exist_ok=True)
records=[]
for i,m in enumerate(pieces):
    name=f'component_{i+1:03d}'
    rec={'name':name,'triangles':len(m.faces),'watertight':bool(m.is_watertight),'volume_mm3':float(m.volume),'bounds_mm':m.bounds.tolist(),'extents_mm':m.extents.tolist()}
    records.append(rec)
    m.export(out/(name+'.stl'))
(root/'components.json').write_text(json.dumps(records,indent=2))
mesh.export(root/'references/pedal_complete_mm.stl')
print(json.dumps(records,indent=2),flush=True)
