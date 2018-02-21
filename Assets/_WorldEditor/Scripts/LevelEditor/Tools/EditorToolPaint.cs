//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragginzWorldEditor
{
	public class EditorToolPaint : EditorTool {

		private Vector3 _v3AimSize;

		public EditorToolPaint() : base(Globals.EDITOR_TOOL_PAINT)
		{
			_v3AimSize = new Vector3 (1, 1, 1);
		}

		//
		public override void deactivate()
		{
			_v3AimSize = MainMenu.Instance.v3DigSettings;
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
				//MainMenu.Instance.toggleMaterial (Input.GetAxis ("Mouse ScrollWheel"));
				//setCurAimMaterial();
			}

			doRayCast ();

			if (_goHit != null)
			{
				setAimTool ();

				if (_rendererAimTool.material == _materialAimTool) {
					setCurAimMaterial();
				}

				changeShaders (Globals.highlightShaderName);
				//changeSingleMaterial (_goHit, MainMenu.Instance.iSelectedMaterial);

				if (_mouseIsDown) {
					paintIt (_trfmAimTool.position, _levelEditor.aMaterials[MainMenu.Instance.iSelectedMaterial]);
					//setSingleMaterial (_goHit, _levelEditor.aMaterials[MainMenu.Instance.iSelectedMaterial]);
					//_goLastMaterialChanged = null;
					//_tempMaterial = null;
					_mouseIsDown = false;
				}
			}
			else {
				resetAim ();
				//resetMaterial ();
			}
		}

		private void paintIt (Vector3 v3Pos, Material material)
		{
			int i, len;

			_levelEditor.resetUndoActions ();

			List<GameObject> listCollidingObjects = _levelEditor.getOverlappingObjects(v3Pos, _rendererAimTool.bounds.extents);
			len = listCollidingObjects.Count;
			Renderer renderer;
			for (i = 0; i < len; ++i) {
				_levelEditor.addUndoAction (AppState.Paint, listCollidingObjects [i]);
				renderer = listCollidingObjects [i].GetComponent<Renderer> ();
				if (renderer != null) {
					renderer.sharedMaterial = material;
				}
			}
			listCollidingObjects.Clear ();
			listCollidingObjects = null;

			MainMenu.Instance.setUndoButton (true);
		}
	}
}