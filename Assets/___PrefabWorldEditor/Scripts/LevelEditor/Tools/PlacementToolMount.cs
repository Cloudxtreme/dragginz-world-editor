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
	public class PlacementToolMount : PlacementTool
    {
		public PlacementToolMount(GameObject container) : base(container)
		{
			//
		}

		// ------------------------------------------------------------------------
		public override void createObjects() //int step)
		{
			GameObject go;
			int i, len = (5 + _interval) * _density;
			for (i = 0; i < _interval; ++i)
			{
				float radius = (float)_radius * (float)(i);
				float height = (float)((_interval-1) - i) * .5f;
				if (_inverse) {
					height = (float)i * .5f;
				}

				// first element is center element
				if (i == 0) {
					createElement (new Vector3 (0, height, 0));
					continue;
				}

				int j, steps = (3 + i * 2) * _density;
				for (j = 0; j < steps; ++j)
				{
					float angle = (float)j * Mathf.PI * 2f / (float)steps;
					Vector3 pos = new Vector3 (Mathf.Cos (angle), height, Mathf.Sin (angle)) * radius;

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