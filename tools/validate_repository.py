"""Offline integrity checks. Does not connect to or actuate hardware."""
from pathlib import Path
import hashlib
import json
import re
import struct
import sys

ROOT = Path(__file__).resolve().parents[1]
CAD = ROOT / 'cad' / 'tripilot-reference'


def require(condition, message):
    if not condition:
        raise ValueError(message)


def main():
    groups = json.loads((CAD / 'grouped_parts.json').read_text())
    shells = json.loads((CAD / 'components.json').read_text())
    manifest = dict(line.split('|') for line in (CAD / 'parts_manifest.txt').read_text().splitlines())
    require(len(groups) == len(manifest) == 16, 'Expected 16 grouped components')
    require(len(shells) == 93, 'Expected 93 source shells')
    require(sorted(i for g in groups for i in g['source_components']) == list(range(1, 94)), 'Shells duplicated or missing')
    require(sum(g['import'] == 'solid' for g in groups) == 12, 'Expected 12 structural solids')
    for group in groups:
        name = group['name']
        require(manifest[name] == group['import'], f'Manifest mismatch: {name}')
        part = CAD / 'SolidWorks_parts' / (name + '.SLDPRT')
        require(part.exists(), f'Missing native part: {name}')
        with part.open('rb') as stream:
            header = stream.read(100)
            require(part.stat().st_size > 1024 and not header.startswith(b'version https://git-lfs.github.com/spec/v1'), f'Native part missing or still an LFS pointer: {name}')
        stl = CAD / 'exchange_parts' / (name + '.stl')
        with stl.open('rb') as stream:
            header = stream.read(84)
        require(len(header) == 84, f'Invalid STL header: {name}')
        triangles = struct.unpack('<I', header[80:84])[0]
        require(triangles == group['triangles'], f'Triangle count mismatch: {name}')
        require(stl.stat().st_size == 84 + 50 * triangles, f'STL length mismatch: {name}')
    glb = CAD / 'references' / 'pedal.glb'
    digest = hashlib.sha1()
    digest.update(f'blob {glb.stat().st_size}\0'.encode())
    with glb.open('rb') as stream:
        while chunk := stream.read(1024 * 1024):
            digest.update(chunk)
    require(digest.hexdigest() == 'ff84085f8ea8f9bd825bb90935fc224e2907ed76', 'Source GLB differs from the recorded upstream blob')
    for document in ROOT.rglob('*.md'):
        if 'upstream' in document.parts or '.git' in document.parts:
            continue
        for link in re.findall(r'\]\(([^)]+)\)', document.read_text(encoding='utf-8')):
            if '://' in link or link.startswith('#'):
                continue
            require((document.parent / link.split('#')[0]).exists(), f'Broken link in {document.relative_to(ROOT)}: {link}')
    upstream = ROOT / 'software' / 'upstream' / 'tripilot'
    require((upstream / 'common/dm_drivers/dm_controllers/config/dm_motor.yaml').exists(), 'Initialize the upstream Git submodule')
    print('PASS: 16 CAD groups, 12 structural entries, 93 shells represented exactly once.')
    print('PASS: native payload presence, STL sizes/counts, source GLB hash and local documentation links.')
    print('This is a file-integrity check, not a SolidWorks rebuild or hardware validation.')


if __name__ == '__main__':
    try:
        main()
    except (ValueError, OSError, KeyError) as error:
        print(f'FAIL: {error}', file=sys.stderr)
        sys.exit(1)
