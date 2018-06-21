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
	public class PlacementToolCircle : PlacementTool
    {
		public PlacementToolCircle(GameObject container) : base(container)
		{
			//
		}

		// ------------------------------------------------------------------------
		public override void createObjects() //int step)
		{
			int i;
			for (i = 0; i < _interval; ++i)
			{
				// first element is center element
				if (i == 0) {
					createElement (Vector3.zero);
					continue;
				}

				float radius = (float)_radius * (float)(i);

				int j, steps = (3 + i * 2) * _density;
				for (j = 0; j < steps; ++j)
				{
					float angle = (float)j * Mathf.PI * 2f / (float)steps;
					Vector3 pos = new Vector3 (Mathf.Cos (angle), 0, Mathf.Sin (angle)) * radius;

					createElement (pos);
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

				PrefabLevelEditor.Instance.levelController.setMeshCollider (go, false);
				PrefabLevelEditor.Instance.levelController.setRigidBody (go, false);

				LevelController.LevelElement element = new LevelController.LevelElement ();
				element.go = go;
				element.part = _curPart.id;

				_elements.Add (element);
			}
		}
	}
}