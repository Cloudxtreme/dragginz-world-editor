//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DragginzWorldEditor
{
	public class EditorToolItem : EditorTool {

		private Collider _collider;

		public EditorToolItem() : base(Globals.EDITOR_TOOL_ITEMS)
		{
			//
		}

		public override void customUpdate(float time, float timeDelta)
		{
			if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
				if (time > _lastMouseWheelUpdate) {
					_lastMouseWheelUpdate = time + 0.2f;
					_levelEditor.toggleItem (Input.GetAxis ("Mouse ScrollWheel"));
				}
			}

			doRayCast ();

			if (_goHit != null && _levelEditor.goCurItem != null)
			{
				if (_hit.normal.y != 0) {
					_levelEditor.goCurItem.transform.forward = Vector3.forward;
				} else {
					_levelEditor.goCurItem.transform.forward = _hit.normal;
				}

				if (_levelEditor.goCurItem.GetComponent<Collider> () != null) {
					_collider = _levelEditor.goCurItem.GetComponent<Collider> ();
					_bounds = _collider.bounds;
				}
				/*else if (_levelEditor.goCurItem.GetComponent<MeshCollider> () != null) {
					_collider = _levelEditor.goCurItem.GetComponent<MeshCollider> ();
					_bounds = _collider.bounds;
				}
				else {
					_bounds = new Bounds ();
				}*/

				_v3Pos = _goHit.transform.position;
				_v3Pos +=  (_hit.normal * (_levelEditor.fRockSize + 0.05f));

				_levelEditor.goCurItem.transform.position = _v3Pos;

				if (_mouseIsDown) {
					placeIt (_v3Pos);
					_mouseIsDown = false;
				}

			}
			else {
				resetItem ();
			}
		}

		public void placeIt(Vector3 v3Pos)
		{
			string sName = _levelEditor.goCurItem.name+"_"+_levelEditor.goProps.transform.childCount;

			propDef prop = _levelEditor.levelPropDefs [_levelEditor.iSelectedItem];

			GameObject goNew = World.Instance.createProp (prop, v3Pos, sName, _levelEditor.goProps.transform, prop.useCollider, prop.useGravity);
			goNew.transform.forward = _levelEditor.goCurItem.transform.forward;

			PropsManager.Instance.addWorldProp (prop.id, goNew);

			_levelEditor.resetUndoActions ();
			_levelEditor.addUndoAction (AppState.Props, goNew);
		}
	}
}