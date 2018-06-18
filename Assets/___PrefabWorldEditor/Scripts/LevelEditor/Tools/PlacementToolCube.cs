//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using AssetsShared;

namespace PrefabWorldEditor
{
	public class PlacementToolCube : PlacementTool
    {
		public PlacementToolCube(GameObject container) : base(container)
		{
			//
		}

		// ------------------------------------------------------------------------
		public override void createObjects()
		{
			int i;
			for (i = 0; i < _interval; ++i) {

				// first element is center element
				if (i == 0) {
					createElement (Vector3.zero);
					continue;
				}

				// create rings around center
				int steps = (2 + i) * _density;

				float size = (float)i * (float)_radius;
				float start = size / 2 * -1;
				float distance = size / (float)(steps-1);

				int x, z, y;
				for (x = 0; x < steps; ++x) {
					for (z = 0; z < steps; ++z) {
						for (y = 0; y < steps; ++y) {

							if (x > 0 && x < (steps - 1) && z > 0 && z < (steps - 1) && y > 0 && y < (steps - 1)) {
								continue;
							}

							float xPos = start + (float)x * distance;
							float zPos = start + (float)z * distance;
							float yPos = (float)y * distance;

							Vector3 pos = new Vector3 (xPos, yPos, zPos);

							createElement (pos);
						}
					}
				}
			}
		}

		//
		private void createElement(Vector3 pos) {

			GameObject go = PrefabLevelEditor.Instance.createPartAt (_curPart.id, 0, 0, 0);
			if (go != null)
			{
				go.name = "temp_part_" + _container.transform.childCount.ToString ();
				go.transform.SetParent (_container.transform);
				go.transform.localPosition = pos;

				PrefabLevelEditor.Instance.setMeshCollider (go, false);
				PrefabLevelEditor.Instance.setRigidBody (go, false);

				PrefabLevelEditor.LevelElement element = new PrefabLevelEditor.LevelElement ();
				element.go = go;
				element.part = _curPart.id;

				_elements.Add (element);
			}
		}
	}
}