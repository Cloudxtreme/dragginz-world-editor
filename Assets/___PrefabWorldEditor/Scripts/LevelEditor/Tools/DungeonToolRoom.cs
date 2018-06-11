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
		private PrefabLevelEditor.Part partFloor;
		private PrefabLevelEditor.Part partWall;
		private PrefabLevelEditor.Part partCorner;

		public DungeonToolRoom(GameObject container) : base(container)
		{
			partFloor  = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Floor];
			partWall   = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Wall_L];
			partCorner = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Corner];
		}

		// ------------------------------------------------------------------------
		public override void createObjects()
		{
			GameObject go;
			PrefabLevelEditor.PartList partId;

			float distance = 2.0f;
			bool isWall = false;

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

						isWall = false;
						if (step < (_size - 1)) {
							partId = partFloor.id;
						} else {
							if ((x == -len && z == -len) || (x == len && z == len) || (x == -len && z == len) || (x == len && z == -len)) {
								partId = partCorner.id;
							} else {
								partId = partWall.id;
							}
							isWall = true;
						}

						go = PrefabLevelEditor.Instance.createPartAt (partId, 0, 0, 0);

						if (go != null) {
							go.name = "temp_part_" + _container.transform.childCount.ToString ();
							go.transform.SetParent (_container.transform);
							go.transform.localPosition = pos;

							if (isWall) {
								if (x == -len && z == len) {
									go.transform.rotation = Quaternion.Euler (new Vector3 (0, 90, 0));
								} else if (x == len && z == -len) {
									go.transform.rotation = Quaternion.Euler (new Vector3 (0, 270, 0));
								} else if (x == -len) {
									go.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
								} else if (x == len) {
									go.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
								} else if (z == -len) {
									go.transform.rotation = Quaternion.Euler (new Vector3 (0, 270, 0));
								} else if (z == len) {
									go.transform.rotation = Quaternion.Euler (new Vector3 (0, 90, 0));
								}
							}

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