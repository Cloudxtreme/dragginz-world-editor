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
	public class DungeonToolRoom : DungeonTool
    {
		public DungeonToolRoom(GameObject container) : base(container)
		{
			//
		}

		// ------------------------------------------------------------------------
		public override void createObjects(int step)
		{
			float radius = (float)_radius * (float)(step);

			GameObject go;
			int i, len = (5 + step) * _density;
			for (i = 0; i < len; ++i)
			{
				float angle = (float)i * Mathf.PI * 2f / (float)len;
				Vector3 pos = new Vector3 (Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

				go = PrefabLevelEditor.Instance.createPartAt (_curPart.id, 0, 0, 0);
				if (go != null)
				{
					go.name = "temp_part_" + _container.transform.childCount.ToString();
					go.transform.SetParent(_container.transform);
					go.transform.localPosition = pos;

					PrefabLevelEditor.Instance.setMeshCollider (go, false);
					PrefabLevelEditor.Instance.setRigidBody (go, false);

					_gameObjects [step].Add (go);
				}
			}
		}
	}
}