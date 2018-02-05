//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragginzWorldEditor
{
	public class EditorToolDig : EditorTool {

		public EditorToolDig() : base(Globals.EDITOR_TOOL_DIG)
		{
			//
		}

		public override void customUpdate(float time, float timeDelta)
		{
			if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
				MainMenu.Instance.toggleDigSize (Input.GetAxis ("Mouse ScrollWheel"));
			}

			doRayCast ();

			if (_goHit != null)
			{
				_trfmAimTool.position = _hit.point;
				_trfmAimTool.forward = _hit.normal;
				changeShaders (Globals.highlightShaderName);

				if (_mouseIsDown) {
					digIt (_trfmAimTool.position);
					_mouseIsDown = false;
				}
			}
			else {
				resetAim ();
			}
		}

		private void digIt (Vector3 v3Pos)
		{
			int i, len;
			int destroyedCubes = 0;

			World world = World.Instance;
			LevelEditor levelEditor = LevelEditor.Instance;

			// keep track of parent objects that had children removed
			List<Transform> listcubeTransforms = new List<Transform>();

			List<GameObject> listCollidingObjects = levelEditor.getOverlappingObjects(v3Pos);
			len = listCollidingObjects.Count;
			for (i = 0; i < len; ++i) {
				if (!listcubeTransforms.Contains (listCollidingObjects [i].transform.parent)) {
					listcubeTransforms.Add (listCollidingObjects [i].transform.parent);
				}

				levelEditor.addUndoAction (AppState.Dig, listCollidingObjects [i]);

				GameObject.Destroy (listCollidingObjects [i]);
				destroyedCubes++;
			}
			listCollidingObjects.Clear ();
			listCollidingObjects = null;

			if (destroyedCubes > 0) {
				MainMenu.Instance.setUndoButton (true);
			}

			world.numCubes -= destroyedCubes;
			MainMenu.Instance.setCubeCountText (world.numCubes);

			// extend level if necessary
			len = listcubeTransforms.Count;
			for (i = 0; i < len; ++i) {

				List<Vector3> adjacentCubes = levelEditor.getAdjacentCubes (listcubeTransforms [i].position);

				int j, len2 = adjacentCubes.Count;
				for (j = 0; j < len2; ++j) {
					world.createRockCube (adjacentCubes [j]);
				}
			}
			listcubeTransforms.Clear ();
			listcubeTransforms = null;
		}
	}
}