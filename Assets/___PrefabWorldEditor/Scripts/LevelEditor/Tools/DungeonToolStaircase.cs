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
	public class DungeonToolStaircase : DungeonTool
    {
		private PrefabLevelEditor.Part partFloor;
		private PrefabLevelEditor.Part partWall;
		private PrefabLevelEditor.Part partCorner;
		private PrefabLevelEditor.Part partWallNF;
		private PrefabLevelEditor.Part partCornerNF;
		private PrefabLevelEditor.Part partStairsLower;
		private PrefabLevelEditor.Part partStairsUpper;

		public DungeonToolStaircase(GameObject container) : base(container)
		{
			partFloor    = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Floor];
			partWall     = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Wall_L];
			partCorner   = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Corner];
			partWallNF   = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Wall_L_NF];
			partCornerNF = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Corner_NF];
			partStairsLower = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Stairs_1];
			partStairsUpper = PrefabLevelEditor.Instance.parts [PrefabLevelEditor.PartList.Dungeon_Stairs_2];
		}

		// ------------------------------------------------------------------------
		public override void createObjects()
		{
			GameObject go;
			PrefabLevelEditor.PartList partId;

			float distance = _cubeSize;
			bool isWall = false;

			int xStart = _width / 2 * -1;
			int xEnd = xStart + _width;
			int zStart = _depth / 2 * -1;
			int zEnd = zStart + _depth;

			bool isLowerStairs = true;
			Vector3Int v3NextStairs = new Vector3Int (xStart, 0, zStart);
			int stairsRotation = 0;
			bool isStairs = false;
				
			int x, z, y;
			for (x = xStart; x < xEnd; ++x) {
				for (z = zStart; z < zEnd; ++z) {
					for (y = 0; y <= _height; ++y) {
						
						Vector3 pos = new Vector3 (x * distance, y * distance, z * distance);

						isWall = false;
						isStairs = false;

						if (y == _height)
						{
							if (!_ceiling) {
								continue;
							} else {
								partId = partFloor.id;
							}
						}
						else
						{
							if (x == v3NextStairs.x && y == v3NextStairs.y && z == v3NextStairs.z)
							{
								partId = isLowerStairs ? partStairsLower.id : partStairsUpper.id;
								isLowerStairs = !isLowerStairs;
								if (isLowerStairs) {
									v3NextStairs.y++;
									v3NextStairs.z = zStart;
								} else {
									v3NextStairs.z++;
								}
								isStairs = true;
							}
							else {
								if (x > xStart && z > zStart && x < (xEnd - 1) && z < (zEnd - 1)) {
									if (y > 0) {
										continue; // no floors on upper floors
									}
									partId = partFloor.id;
								} else {
									if ((x == xStart && z == zStart) || (x == (xEnd - 1) && z == (zEnd - 1)) || (x == xStart && z == (zEnd - 1)) || (x == (xEnd - 1) && z == zStart)) {
										if (y > 0) {
											partId = partCornerNF.id;
										} else {
											partId = partCorner.id;
										}
									} else {
										if (y > 0) {
											partId = partWallNF.id;
										} else {
											partId = partWall.id;
										}
									}
									isWall = true;
								}
							}
						}

						go = PrefabLevelEditor.Instance.createPartAt (partId, 0, 0, 0);

						if (go != null) {
							go.name = "temp_part_" + _container.transform.childCount.ToString ();
							go.transform.SetParent (_container.transform);
							go.transform.localPosition = pos;

							if (isStairs) {
								go.transform.rotation = Quaternion.Euler (new Vector3 (0, stairsRotation, 0));
								if (isLowerStairs) {
									stairsRotation += 90;
								}
							}
							else if (isWall) {
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