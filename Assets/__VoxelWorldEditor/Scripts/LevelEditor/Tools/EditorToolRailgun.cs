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

		public EditorToolRailgun() : base((int)Globals.TOOL.RAILGUN)
		{
			_v3AimSize = new Vector3 ();
		}

		//
		public override void deactivate()
		{
			MainMenu.Instance.resetDigSettings (Vector3.one);
			_levelEditor.updateDigSettings(_v3AimSize);
		}

		//
		public override void activate()
		{
			Globals.RailgunShape shape = LevelEditor.Instance.curVoxelsLevelChunk.aRailgunShapes[_iSelectedRailgunShape];
			_v3AimSize.x = shape.width;
			_v3AimSize.y = shape.height;
			_v3AimSize.z = shape.depth;
			MainMenu.Instance.resetDigSettings (_v3AimSize);
			_levelEditor.updateDigSettings(_v3AimSize);
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

				if (_rendererAimTool.material != _matRailgunShape && _matRailgunShape != null) {
					_rendererAimTool.material = _matRailgunShape;
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
			_levelEditor.curVoxelsLevelChunk.railgunDig (_hit);
		}
	}
}