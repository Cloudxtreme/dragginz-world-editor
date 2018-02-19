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
		private GameObject _goCurItem;

		public EditorToolItem() : base(Globals.EDITOR_TOOL_ITEMS)
		{
			//
		}

		public override void customUpdate(float time, float timeDelta)
		{
			if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
				if (time > _lastMouseWheelUpdate) {
					_lastMouseWheelUpdate = time + 0.2f;
					PropsManager.Instance.toggleSelectedProp (Input.GetAxis ("Mouse ScrollWheel"));
				}
			}

			doRayCast ();

			if (_goHit != null && _levelEditor.goCurItem != null)
			{
				_goCurItem = _levelEditor.goCurItem;

				if (_hit.normal.y != 0) {
					_goCurItem.transform.forward = Vector3.forward;
				} else {
					_goCurItem.transform.forward = _hit.normal;
				}

				if (_goCurItem.GetComponent<BoxCollider> () != null) {
					_collider = _goCurItem.GetComponent<BoxCollider> ();
					_bounds = _collider.bounds;
					Debug.Log (_goCurItem.name+": "+_bounds.size.ToString("F4")+", "+_bounds.extents.ToString("F4"));
				}
				else if (_goCurItem.GetComponent<Collider> () != null) {
					_collider = _goCurItem.GetComponent<Collider> ();
					_bounds = _collider.bounds;
				}

				_v3Pos = _hit.point;// _goHit.transform.position;
				_v3Pos += _bounds.size;

				_goCurItem.transform.position = _v3Pos;

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
			string sName = _goCurItem.name+"_"+_levelEditor.goProps.transform.childCount;

			propDef prop = PropsManager.Instance.getSelectedPropDef ();

			GameObject goNew = World.Instance.createProp (prop, v3Pos, sName, _levelEditor.goProps.transform, prop.useCollider, prop.useGravity);
			goNew.transform.forward = _goCurItem.transform.forward;

			PropsManager.Instance.addWorldProp (prop.id, goNew);

			_levelEditor.resetUndoActions ();
			_levelEditor.addUndoAction (AppState.Props, goNew);
		}
	}
}