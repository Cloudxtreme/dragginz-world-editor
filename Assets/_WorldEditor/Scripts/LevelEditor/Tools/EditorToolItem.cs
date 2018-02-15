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
				//if (Input.GetKey (KeyCode.LeftShift)) {
					//MainMenu.Instance.toggleItem (Input.GetAxis ("Mouse ScrollWheel"));
				//}
			}

			doRayCast ();

			if (_goHit != null && _levelEditor.goCurItem != null)
			{
				if (_levelEditor.goCurItem.GetComponent<BoxCollider> () != null) {
					_collider = _levelEditor.goCurItem.GetComponent<BoxCollider> ();
					_bounds = _collider.bounds;
				} else if (_levelEditor.goCurItem.GetComponent<MeshCollider> () != null) {
					_collider = _levelEditor.goCurItem.GetComponent<MeshCollider> ();
					_bounds = _collider.bounds;
				} else {
					_bounds = new Bounds ();
				}

				_v3Pos = _goHit.transform.position;
				_v3Pos +=  (_hit.normal * (_levelEditor.fRockSize + 0.05f));

				//_trfmAimTool.position   = _bounds.center;
				//_trfmAimTool.forward    = _trfmAimItem.forward;
				//_trfmAimTool.localScale = _bounds.extents * 2.0f;

				_v3Pos.y -= (_bounds.extents.y * _goHit.transform.localScale.y);
				_trfmAimItem.position = _v3Pos;

				if (_hit.normal.y != 0) {
					_levelEditor.goCurItem.transform.forward = -Vector3.forward;
				} else {
					_levelEditor.goCurItem.transform.forward = _hit.normal;
				}

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

			GameObject goNew = World.Instance.createProp (MainMenu.Instance.iSelectedItem, v3Pos, sName, _levelEditor.goProps.transform);
			//goNew.transform.forward = _trfmAimItem.forward;

			_levelEditor.resetUndoActions ();
			_levelEditor.addUndoAction (AppState.Props, goNew);
		}
	}
}