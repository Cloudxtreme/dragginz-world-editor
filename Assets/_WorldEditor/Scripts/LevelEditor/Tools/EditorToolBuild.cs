//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragginzWorldEditor
{
	public class EditorToolBuild : EditorTool {

		public EditorToolBuild() : base(Globals.EDITOR_TOOL_BUILD)
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
				_trfmAimTool.position = _goHit.transform.position + (_hit.normal * LevelEditor.Instance.fRockSize);

				if (_mouseIsDown) {
					LevelEditor.Instance.buildIt (_trfmAimTool.position);
					_mouseIsDown = false;
				}
			}
			else {
				LevelEditor.Instance.resetAim ();
			}
		}
	}
}