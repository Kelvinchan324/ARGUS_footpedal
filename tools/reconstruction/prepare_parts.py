from pathlib import Path
import json
import numpy as np
import trimesh
import argparse
parser=argparse.ArgumentParser(description='Group TriPilot source shells into assembly parts.')
parser.add_argument('--root',type=Path,default=Path(__file__).resolve().parents[2]/'cad'/'tripilot-reference')
root=parser.parse_args().root.resolve()
records=json.loads((root/'components.json').read_text())
out=root/'exchange_parts';out.mkdir(exist_ok=True)
names={1:'01_Housing_motor_support',2:'02_Base_plate',3:'03_Inner_footplate',4:'04_Outer_footplate',8:'05_Central_yaw_coupler',9:'06_X_motor_crank',10:'07_Y_motor_crank',17:'08_Y_vertical_link',18:'09_X_vertical_link',32:'10_Plate_joint_mount_A',33:'11_Plate_joint_mount_B',34:'12_Plate_joint_mount_C'}
groups={name:[i] for i,name in names.items()}
motor_ids={5,6,7,11,12,13,14,15,16,19,20,21,*range(50,68)}
for label, ids in [('13_DM4310_yaw',[5,11,16,20]),('14_DM4310_X',[6,12,15,21]),('15_DM4310_Y',[7,13,14,19])]:groups[label]=ids
# Group motor fasteners by closest motor body center.
centers={key:np.mean(records[ids[0]-1]['bounds_mm'],axis=0) for key,ids in groups.items() if key.startswith(('13','14','15'))}
for i in range(50,68):
    c=np.mean(records[i-1]['bounds_mm'],axis=0)
    nearest=min(centers,key=lambda k:np.linalg.norm(c-centers[k]));groups[nearest].append(i)
groups['16_Universal_joints_and_bearings']=[i for i in range(1,len(records)+1) if i not in names and i not in motor_ids]
manifest=[]
summary=[]
for name,ids in groups.items():
    meshes=[trimesh.load(root/'mesh_parts'/f'component_{i:03d}.stl',process=False) for i in ids]
    m=trimesh.util.concatenate(meshes)
    m.apply_translation([-126.361019, -0.080000058,126.323197])
    m.export(out/(name+'.stl'))
    mode='solid' if len(ids)==1 and ids[0] in names else 'graphics'
    manifest.append(name+'|'+mode)
    summary.append({'name':name,'source_components':ids,'import':mode,'triangles':len(m.faces),'bounds_mm':m.bounds.tolist()})
(root/'parts_manifest.txt').write_text('\n'.join(manifest))
(root/'grouped_parts.json').write_text(json.dumps(summary,indent=2))
print('Prepared',len(groups),'parts; every source component assigned once:',sorted(sum(groups.values(),[]))==list(range(1,len(records)+1)))
