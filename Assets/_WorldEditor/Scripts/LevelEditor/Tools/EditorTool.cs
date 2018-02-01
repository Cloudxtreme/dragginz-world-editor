//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragginzWorldEditor
{
	public class EditorTool {

		protected int _type;

		protected Camera _curCam;
		protected FlyCam _flycam;

		protected static Transform _trfmAimTool;

		protected Ray _ray;
		protected RaycastHit _hit;
		protected GameObject _goHit;

		protected bool _mouseIsDown;

		//
		// GETTERS
		//
		public int type {
			get { return _type; }
		}

		//
		// CONSTRUCTOR
		//
		public EditorTool(int type)
		{
			_type = type;

			_curCam = LevelEditor.Instance.mainCam;
			_flycam = FlyCam.Instance;

			_trfmAimTool = LevelEditor.Instance.laserAim.transform;

			_mouseIsDown = false;
		}

		//
		// OVERRIDE METHODS
		//
		public virtual void customUpdateControls(float time, float timeDelta)
		{
			if (!_mouseIsDown) {
				if (Input.GetButtonDown ("Fire1")) {
					_mouseIsDown = true;
				}
			} else {
				if (Input.GetButtonUp ("Fire1")) {
					_mouseIsDown = false;
				}
			}
		}

		//
		public virtual void customUpdate(float time, float timeDelta)
		{
			//
		}

		//
		// PRIVATE METHODS
		//
		protected void doRayCast() {

			_goHit = null;

			// no raycasting if mouse cursor is over top menu
			if (Screen.height - Input.mousePosition.y < 90) {
				return;
			}

			_ray = _curCam.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (_ray, out _hit, Globals.RAYCAST_DISTANCE_EDIT)) {
				_goHit = _hit.collider.gameObject;
			}
		}
	}
}