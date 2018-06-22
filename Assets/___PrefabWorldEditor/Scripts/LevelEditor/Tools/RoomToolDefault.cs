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
	public class RoomToolDefault : RoomTool
    {
		public RoomToolDefault(GameObject container) : base(container)
		{
			//
		}

		// ------------------------------------------------------------------------
		public override void createObjects()
		{
			if (_curPart.type == PrefabLevelEditor.AssetType.Floor) {
				createFloor ();
			} else {
				if (_curPart.extra == "Z") {
					createWallZ ();
				} else {
					createWallX ();
				}
			}
		}

		// ------------------------------------------------------------------------
		private void createFloor()
		{
			int xStart = _width / 2 * -1;
			int xEnd = xStart + _width;
			int zStart = _height / 2 * -1;
			int zEnd = zStart + _height;

			int x, z;
			for (x = xStart; x < xEnd; ++x) {
				for (z = zStart; z < zEnd; ++z) {
					createElement (new Vector3 (x * _floorSize, 0, z * _floorSize));
				}
			}
		}

		// ------------------------------------------------------------------------
		private void createWallZ()
		{
			int xStart = _width / 2 * -1;
			int xEnd = xStart + _width;
			int yStart = _height / 2 * -1;
			int yEnd = yStart + _height;

			int x, y;
			for (x = xStart; x < xEnd; ++x) {
				for (y = yStart; y < yEnd; ++y) {
					createElement (new Vector3 (x * _wallSize, y * _wallSize, 0));
				}
			}
		}

		// ------------------------------------------------------------------------
		private void createWallX()
		{
			int zStart = _width / 2 * -1;
			int zEnd = zStart + _width;
			int yStart = _height / 2 * -1;
			int yEnd = yStart + _height;

			int z, y;
			for (z = zStart; z < zEnd; ++z) {
				for (y = yStart; y < yEnd; ++y) {
					createElement (new Vector3 (0, y * _wallSize, z * _wallSize));
				}
			}
		}

		// ------------------------------------------------------------------------
		private void createElement(Vector3 pos) {

			GameObject go = PrefabLevelEditor.Instance.createPartAt (_curPart.id, 0, 0, 0);
			if (go != null)
			{
				go.name = "temp_part_" + _container.transform.childCount.ToString ();
				go.transform.SetParent (_container.transform);
				go.transform.localPosition = pos;

				LevelController.Instance.setMeshCollider (go, false);
				LevelController.Instance.setRigidBody (go, false);

				LevelController.LevelElement element = new LevelController.LevelElement ();
				element.go = go;
				element.part = _curPart.id;

				_roomElements.Add (element);
			}
		}
	}
}