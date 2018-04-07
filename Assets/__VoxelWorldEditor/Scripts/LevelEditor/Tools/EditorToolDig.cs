//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DragginzVoxelWorldEditor
{
	public class EditorToolDig : EditorTool {

		//private Vector3 _v3AimSize;

		public EditorToolDig() : base(Globals.EDITOR_TOOL_DIG)
		{
			//_v3AimSize = new Vector3 (1, 1, 1);
		}

		//
		public override void deactivate()
		{
			//_v3AimSize = MainMenu.Instance.v3DigSettings;
		}

		//
		public override void activate()
		{
			//MainMenu.Instance.resetDigSettings (_v3AimSize);
		}

		//
		public override void customUpdate(float time, float timeDelta)
		{
			if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
				MainMenu.Instance.toggleDigSize (Input.GetAxis ("Mouse ScrollWheel"));
			}

			doRayCast ();

			if (_goHit != null)
			{
				setAimTool ();

				//if (_rendererAimTool.material != _materialAimTool) {
				//	_rendererAimTool.material = _materialAimTool;
				//}

				//changeShaders (Globals.highlightShaderName);

				if (_mouseIsDown) {
					digIt (_trfmAimTool.position);
					resetAim ();
					_mouseIsDown = false;
				}
			}
			else {
				resetAim ();
			}
		}

		private void digIt (Vector3 v3Pos)
		{
			_levelEditor.curVoxelsLevelChunk.dig (_hit, MainMenu.Instance.v3DigSettings);

			/*
			int i, len;
			int destroyedCubes = 0;

			//World world = World.Instance;

			_levelEditor.resetUndoActions ();

			// keep track of parent objects that had children removed
			List<Transform> listcubeTransforms = new List<Transform>();

			List<GameObject> listCollidingObjects = _levelEditor.getOverlappingObjects(v3Pos, _rendererAimTool.bounds.extents);
			len = listCollidingObjects.Count;
			for (i = 0; i < len; ++i) {
				if (!listcubeTransforms.Contains (listCollidingObjects [i].transform.parent)) {
					listcubeTransforms.Add (listCollidingObjects [i].transform.parent);
				}

				if (listCollidingObjects [i].activeSelf) {
					_levelEditor.addUndoAction (AppState.Dig, listCollidingObjects [i]);
					//GameObject.Destroy (listCollidingObjects [i]);
					listCollidingObjects [i].SetActive (false);
					destroyedCubes++;
				}
			}
			listCollidingObjects.Clear ();
			listCollidingObjects = null;

			if (destroyedCubes > 0) {
				MainMenu.Instance.setUndoButton (true);
			}

			_levelEditor.curLevelChunk.numCubes -= destroyedCubes;
			MainMenu.Instance.setCubeCountText (_levelEditor.curLevelChunk.numCubes);

			// extend level if necessary
			len = listcubeTransforms.Count;
			for (i = 0; i < len; ++i) {

				List<Vector3> adjacentCubes = _levelEditor.getAdjacentCubes (listcubeTransforms [i].position);

				int j, len2 = adjacentCubes.Count;
				for (j = 0; j < len2; ++j) {
					_levelEditor.curLevelChunk.createRockCube (adjacentCubes [j]);
				}
			}
			listcubeTransforms.Clear ();
			listcubeTransforms = null;
			*/
		}
	}
}