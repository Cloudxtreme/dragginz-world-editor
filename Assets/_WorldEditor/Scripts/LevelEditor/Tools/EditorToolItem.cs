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

		public EditorToolItem() : base(Globals.EDITOR_TOOL_ITEMS)
		{
			//
		}

		public override void customUpdate(float time, float timeDelta)
		{
			if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
				if (Input.GetKey (KeyCode.LeftShift)) {
					MainMenu.Instance.toggleItem (Input.GetAxis ("Mouse ScrollWheel"));
				}
			}

			doRayCast ();

			if (_goHit != null && _levelEditor.goCurItem != null)
			{
				_bounds = _levelEditor.goCurItem.GetComponent<Renderer> ().bounds;

				//_trfmAimItem.forward = _hit.normal;
				//_trfmAimTool.forward = _hit.normal;

				//Vector3 v3Bounds = _levelEditor.goCurItem.GetComponent<Renderer> ().bounds.extents;
				_v3Pos = _goHit.transform.position + _hit.normal;
				//v3Pos.x += (_hit.normal.x * v3Bounds.x);
				//v3Pos.y += (_hit.normal.y * v3Bounds.y);
				//v3Pos.z += (_hit.normal.z * v3Bounds.z);

				_v3Pos.y -= (_bounds.extents.y * _goHit.transform.localScale.y);
				_trfmAimItem.position = _v3Pos;

				_trfmAimTool.position = _bounds.center;
				_trfmAimTool.localScale = _levelEditor.goCurItem.GetComponent<Renderer> ().bounds.extents * 2.0f;

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
			string sName = _levelEditor.goCurItem.name+"_"+_levelEditor.goItems.transform.childCount;

			GameObject goNew = GameObject.Instantiate (_levelEditor.itemPrefabs [MainMenu.Instance.iSelectedItem]);
			goNew.transform.SetParent (_levelEditor.goItems.transform);
			goNew.transform.position = v3Pos;
			goNew.name = sName;
			// turn on gravity and collider
			goNew.GetComponent<BoxCollider>().enabled = true;
			goNew.GetComponent<Rigidbody>().useGravity = true;

			_levelEditor.resetUndoActions ();
			_levelEditor.addUndoAction (AppState.Items, goNew);
		}
	}
}