//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DragginzVoxelWorldEditor
{
	public class EditorToolPaint : EditorTool {

		//private Vector3 _v3AimSize;

		public EditorToolPaint() : base((int)Globals.TOOL.PAINT)
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
				//MainMenu.Instance.toggleMaterial (Input.GetAxis ("Mouse ScrollWheel"));
				//setCurAimMaterial();
			}

			doRayCast ();

			if (_goHit != null)
			{
				setAimTool ();

				if (_rendererAimTool.sharedMaterial == _materialAimTool) {
					setCurAimMaterial();
				}

				//changeShaders (Globals.highlightShaderName);

				if (_mouseIsDown)
				{
					_levelEditor.curVoxelsLevelChunk.paint (_hit, MainMenu.Instance.v3DigSettings, MainMenu.Instance.iSelectedMaterial);
					_mouseIsDown = false;
				}
			}
			else {
				resetAim ();
			}
		}
	}
}