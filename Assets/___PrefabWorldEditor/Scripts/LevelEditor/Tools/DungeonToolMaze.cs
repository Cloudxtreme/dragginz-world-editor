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
	public class DungeonToolMaze : DungeonTool
    {
		private PrefabLevelEditor.Part partFloor;
		private PrefabLevelEditor.Part partWall;
		private PrefabLevelEditor.Part partCorner;
		private PrefabLevelEditor.Part partTurn;

		public DungeonToolMaze(GameObject container) : base(container)
		{
			partWall   = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Wall_L];
			partCorner = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Corner];
			partTurn   = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Turn];
		}

		// ------------------------------------------------------------------------
		public override void createObjects()
		{
			GameObject go;
			PrefabLevelEditor.PartList partId;

			float distance = 2.0f;
			bool isWall = false;

			int xStart = _width / 2 * -1;
			int xEnd = xStart + _width;
			int zStart = _depth / 2 * -1;
			int zEnd = zStart + _depth;

			int x, z;
			for (x = xStart; x < xEnd; ++x) {
				for (z = zStart; z < zEnd; ++z) {

					Vector3 pos = new Vector3 (x * distance, 0, z * distance);

					isWall = false;
					if (x > xStart && z > zStart && x < (xEnd - 1) && z < (zEnd - 1)) {
						partId = partTurn.id;
					} else {
						if ((x == xStart && z == zStart) || (x == (xEnd - 1) && z == (zEnd - 1)) || (x == xStart && z == (zEnd - 1)) || (x == (xEnd - 1) && z == zStart)) {
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
							if (x == xStart && z == (zEnd - 1)) {
								go.transform.rotation = Quaternion.Euler (new Vector3 (0, 90, 0));
							} else if (x == (xEnd - 1) && z == zStart) {
								go.transform.rotation = Quaternion.Euler (new Vector3 (0, 270, 0));
							} else if (x == xStart) {
								go.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
							} else if (x == (xEnd - 1)) {
								go.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
							} else if (z == zStart) {
								go.transform.rotation = Quaternion.Euler (new Vector3 (0, 270, 0));
							} else if (z == (zEnd - 1)) {
								go.transform.rotation = Quaternion.Euler (new Vector3 (0, 90, 0));
							}
						} else {
							go.transform.rotation = Quaternion.Euler (new Vector3 (0, Random.Range (0, 4) * 90, 0));
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