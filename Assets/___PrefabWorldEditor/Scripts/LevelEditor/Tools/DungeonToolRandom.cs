//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using AssetsShared;

namespace PrefabWorldEditor
{
	public class DungeonToolRandom : DungeonTool
    {
		private PrefabLevelEditor.Part[] parts;

		public DungeonToolRandom(GameObject container) : base(container)
		{
			parts = new PrefabLevelEditor.Part[7];
			parts[0] = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Floor];
			parts[1] = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Wall_L];
			parts[2] = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Wall_LR];
			parts[3] = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Corner];
			parts[4] = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_DeadEnd];
			parts[5] = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Turn];
			parts[6] = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Floor];
		}

		// ------------------------------------------------------------------------
		public override void createObjects()
		{
			GameObject go;
			PrefabLevelEditor.PartList partId;

			float distance = 2.0f;

			int step;
			for (step = 1; step < _size; ++step)
			{
				int x, z, len = step;
				for (x = -len; x <= len; ++x) {
					for (z = -len; z <= len; ++z) {
						
						if (x > -len && x < len && z > -len && z < len) {
							continue;
						}

						Vector3 pos = new Vector3 (x * distance, 0, z * distance);

						partId = parts [Random.Range (0, parts.Length)].id;
						go = PrefabLevelEditor.Instance.createPartAt (partId, 0, 0, 0);

						if (go != null) {
							go.name = "temp_part_" + _container.transform.childCount.ToString ();
							go.transform.SetParent (_container.transform);
							go.transform.localPosition = pos;
							go.transform.rotation = Quaternion.Euler (new Vector3 (0, Random.Range (0, 4) * 90, 0));

							PrefabLevelEditor.Instance.setMeshCollider (go, false);
							PrefabLevelEditor.Instance.setRigidBody (go, false);

							PrefabLevelEditor.LevelElement element = new PrefabLevelEditor.LevelElement ();
							element.go = go;
							element.part = partId;

							_dungeonElements.Add (element);
						}
					}
				}
			}
		}
	}
}