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
		}

		//
		// OVERRIDE METHODS
		//
		public virtual void customUpdateControls(float time, float timeDelta) {}
		public virtual void customUpdate(float time, float timeDelta) {}

		//
		// PRIVATE METHODS
		//
		protected void doRayCast() {

			_goHit = null;
			_ray = _curCam.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (_ray, out _hit, Globals.RAYCAST_DISTANCE_EDIT)) {
				_goHit = _hit.collider.gameObject;
			}
		}
	}
}