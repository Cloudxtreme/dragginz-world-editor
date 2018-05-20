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
	public class PlacementToolQuad : PlacementTool
    {
		public PlacementToolQuad(GameObject container) : base(container)
		{
			//
		}

		// ------------------------------------------------------------------------
		// Override Methods
		// ------------------------------------------------------------------------
		public override void createObjects()
		{
			GameObject go;
			float distance = (_curPart.w + _curPart.d) / 2.5f;
			int x, z, len = _step + 1;
			for (x = -len; x <= len; ++x)
			{
				for (z = -len; z <= len; ++z)
				{
					if (x > -len && x < len && z > -len && z < len) {
						continue;
					}

					Vector3 pos = new Vector3 (x * distance, 0, z * distance);

					go = PrefabLevelEditor.Instance.createPartAt (_curPart.id, 0, 0, 0);
					if (go != null) {
						go.name = "temp_part_" + _container.transform.childCount.ToString ();
						go.transform.SetParent (_container.transform);
						go.transform.localPosition = pos;

						PrefabLevelEditor.Instance.setMeshCollider (go, false);
						PrefabLevelEditor.Instance.setRigidBody (go, false);

						_gameObjects [_step].Add (go);
					}
				}
			}
		}
	}
}