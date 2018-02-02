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

		public EditorToolPaint() : base(Globals.EDITOR_TOOL_PAINT)
		{
			//
		}

		public override void customUpdate(float time, float timeDelta)
		{
			if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
				MainMenu.Instance.toggleMaterial (Input.GetAxis ("Mouse ScrollWheel"));
			}

			doRayCast ();

			if (_goHit != null)
			{
				changeSingleMaterial (_goHit, MainMenu.Instance.iSelectedMaterial);

				if (_mouseIsDown) {
					//LevelEditor.Instance.paintIt (_goHit);
					setSingleMaterial (_goHit, LevelEditor.Instance.aMaterials[MainMenu.Instance.iSelectedMaterial]);
					_goLastMaterialChanged = null;
					_tempMaterial = null;
				}
			}
			else {
				resetMaterial ();
			}
		}
	}
}