//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DragginzVoxelWorldEditor
{
	public class EditorToolRailgun : EditorTool {

		private Vector3 _v3AimSize;
		private Material matRailgunCross = Resources.Load<Material>("Materials/Tools/vwe_railgun_cross");

		public EditorToolRailgun() : base((int)Globals.TOOL.RAILGUN)
		{
			_v3AimSize = new Vector3 (4, 4, 0.05f);
		}

		//
		public override void deactivate()
		{
			MainMenu.Instance.resetDigSettings (Vector3.one);
		}

		//
		public override void activate()
		{
			MainMenu.Instance.resetDigSettings (_v3AimSize);
		}

		//
		public override void customUpdate(float time, float timeDelta)
		{
			/*if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
				MainMenu.Instance.toggleDigSize (Input.GetAxis ("Mouse ScrollWheel"));
			}*/

			doRayCast ();

			if (_goHit != null)
			{
				setRailgunAim ();

				if (_rendererAimTool.material != matRailgunCross) {
					_rendererAimTool.material = matRailgunCross;
				}

				if (_mouseIsDown) {
					railIt (_trfmAimTool.position);
					resetAim ();
					_mouseIsDown = false;
				}
			}
			else {
				resetAim ();
			}
		}

		private void railIt (Vector3 v3Pos)
		{
			_levelEditor.curVoxelsLevelChunk.railgunDig (_hit, new Vector3(_v3AimSize.x, _v3AimSize.y, 8));
		}
	}
}