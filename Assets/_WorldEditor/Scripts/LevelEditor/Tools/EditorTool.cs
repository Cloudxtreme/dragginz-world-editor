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

		protected static Camera _curCam;
		protected static FlyCam _flycam;

		protected static Transform _trfmAimTool;

		protected static Ray _ray;
		protected static RaycastHit _hit;
		protected static GameObject _goHit;

		protected static GameObject _goLastMaterialChanged;
		protected static Material _tempMaterial;

		protected static bool _mouseIsDown;

		private static bool _initialised = false;

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

			if (!_initialised) {

				_initialised = true;

				_curCam = LevelEditor.Instance.mainCam;
				_flycam = FlyCam.Instance;

				_trfmAimTool = LevelEditor.Instance.laserAim.transform;

				_goLastMaterialChanged = null;
				_tempMaterial = null;

				_mouseIsDown = false;
			}
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
		// PUBLIC METHODS
		//

		public void resetMaterial()
		{
			setSingleMaterial (_goLastMaterialChanged, _tempMaterial);
			_goLastMaterialChanged = null;
			_tempMaterial = null;
		}

		//
		public void changeSingleMaterial(GameObject go, int materialIndex)
		{
			if (go == null) {
				return;
			}

			// reset current material
			if (_goLastMaterialChanged != null && go != _goLastMaterialChanged) {
				resetMaterial ();
			}

			_goLastMaterialChanged = go;
			setSingleMaterial (_goLastMaterialChanged, LevelEditor.Instance.aMaterials[materialIndex]);
		}

		//
		public void setSingleMaterial(GameObject go, Material material, bool setTempMaterial = true)
		{
			if (go != null && material != null) {
				Renderer renderer = go.GetComponent<Renderer> ();
				if (renderer != null && renderer.sharedMaterial.name != material.name) {
					if (setTempMaterial) {
						_tempMaterial = renderer.sharedMaterial;
					}
					renderer.sharedMaterial = material;
				}
			}
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