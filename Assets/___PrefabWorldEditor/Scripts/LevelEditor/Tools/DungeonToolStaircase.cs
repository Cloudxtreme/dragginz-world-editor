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

		private struct stairStep
		{
			public Vector3Int pos;
			public float rot;
			public PrefabLevelEditor.PartList partId;

			public stairStep(Vector3Int p, float r, PrefabLevelEditor.PartList id) {
				pos = p;
				rot = r;
				partId = id;
			}
		};

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

			int stairsStep = 0;
			float stairsRotation = 0;
			bool isStairs = false;
				
			int x, z, y;

			// create stair step positions first
			List<stairStep> stairSteps = new List<stairStep> ();
			stairStep step;
			for (y = 0; y < _height; ++y)
			{
				// first stairs
				if (stairsStep == 0)
				{	// z+
					step = new stairStep(new Vector3Int (xStart, y, zEnd-2), stairsRotation, partStairsLower.id);
					stairSteps.Add (step);
					step = new stairStep(new Vector3Int (xStart, y, zEnd-1), stairsRotation, partStairsUpper.id);
					stairSteps.Add (step);
				}
				else if (stairsStep == 1)
				{	// x+
					step = new stairStep(new Vector3Int (xEnd-2, y, zEnd-1), stairsRotation, partStairsLower.id);
					stairSteps.Add (step);
					step = new stairStep(new Vector3Int (xEnd-1, y, zEnd-1), stairsRotation, partStairsUpper.id);
					stairSteps.Add (step);
				}
				else if (stairsStep == 2)
				{	// z-
					step = new stairStep(new Vector3Int (xEnd-1, y, zStart+1), stairsRotation, partStairsLower.id);
					stairSteps.Add (step);
					step = new stairStep(new Vector3Int (xEnd-1, y, zStart), stairsRotation, partStairsUpper.id);
					stairSteps.Add (step);
				}
				else if (stairsStep == 3)
				{	// x-
					step = new stairStep(new Vector3Int (xStart+1, y, zStart), stairsRotation, partStairsLower.id);
					stairSteps.Add (step);
					step = new stairStep(new Vector3Int (xStart, y, zStart), stairsRotation, partStairsUpper.id);
					stairSteps.Add (step);
				}

				stairsStep = (stairsStep < 3 ? stairsStep + 1 : 0);
				stairsRotation = (stairsRotation < 270 ? stairsRotation + 90 : 0);
			}

			for (x = xStart; x < xEnd; ++x) {
				for (z = zStart; z < zEnd; ++z) {
					for (y = 0; y <= _height; ++y) {
						
						Vector3 pos = new Vector3 (x * distance, y * distance, z * distance);

						isWall = false;
						isStairs = false;

						partId = partFloor.id;

						if (y == _height) {
							if (!_ceiling) {
								continue;
							} else {
								partId = partFloor.id;
							}
						}
						else if (x == xStart && y == 0 && z == zStart) {
							partId = partWall.id;
						}
						else
						{
							int s, numStairSteps = stairSteps.Count;
							for (s = 0; s < numStairSteps; ++s) {
								if (stairSteps[s].pos.x == x && stairSteps[s].pos.y == y && stairSteps[s].pos.z == z) {
									step = stairSteps [s];
									partId = step.partId;
									stairsRotation = step.rot;
									isStairs = true;
									break;
								}
							}

							if (!isStairs) {
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

							LevelController.Instance.setMeshCollider (go, false);
							LevelController.Instance.setRigidBody (go, false);

							LevelController.LevelElement element = new LevelController.LevelElement ();
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