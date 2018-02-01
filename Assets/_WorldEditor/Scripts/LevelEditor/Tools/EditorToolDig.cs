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
				LevelEditor.Instance.changeShaders (Globals.highlightShaderName);

				if (_mouseIsDown) {
					LevelEditor.Instance.digIt (_trfmAimTool.position);
					_mouseIsDown = false;
				}
			}
			else {
				LevelEditor.Instance.resetAim ();
			}
		}
	}
}