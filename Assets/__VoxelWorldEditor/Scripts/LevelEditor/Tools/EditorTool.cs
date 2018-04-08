//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DragginzVoxelWorldEditor
{
	public class EditorTool {

		protected int _type;

		protected static LevelEditor _levelEditor;
		protected static Camera _curCam;
		protected static FlyCam _flycam;

		protected static Transform _trfmAimTool;
		protected static Transform _trfmAimCenterCube;
		protected static Transform _trfmAimProp;

		protected static Renderer _rendererAimTool;
		protected static Renderer _rendererAimCenterCube;
		protected static Material _materialAimTool;

		protected static Ray _ray;
		protected static RaycastHit _hit;
		protected static GameObject _goHit;

		protected static GameObject _goHitLast;
		protected static bool _raycastedObjectHasChanged;

		protected static GameObject _goCurProp;
		protected static Collider _collider;
		protected static Bounds _bounds;

		protected static float _yOffset;
		protected static float _zOffset;
		protected static Vector3 _v3Pos;

		protected static Dictionary<string, Shader> _aUsedShaders;
		protected static GameObject _goLastShaderChange;
		protected static List<GameObject> _aGoShaderChanged;

		//protected static GameObject _goLastMaterialChanged;
		//protected static Material _tempMaterial;

		protected static float _lastMouseWheelUpdate;

		protected static bool _mouseIsDown;

		private static float _uiHeight  = (float)Screen.height * (90.0f / 1080.0f);
		private static float _maxClickY = (float)Screen.height - _uiHeight;

		private static Vector3 _v3AimPos;
		private static float _chunkSizeHalved;
		private static float _xAimScale;
		private static float _yAimScale;
		private static float _zAimScale;

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

				_levelEditor = LevelEditor.Instance;
				_curCam = _levelEditor.editCam;
				_flycam = FlyCam.Instance;

				_trfmAimTool = _levelEditor.laserAim.transform;
				_trfmAimCenterCube = _levelEditor.laserAimCenterCube.transform;
				_trfmAimProp = _levelEditor.propAim.transform;

				_rendererAimTool = _trfmAimTool.GetComponent<Renderer> ();
				_rendererAimCenterCube = _trfmAimCenterCube.GetComponent<Renderer> ();
				_materialAimTool = _rendererAimTool.material;

				_aUsedShaders = new Dictionary<string, Shader> ();
				_goLastShaderChange = null;
				_aGoShaderChanged = new List<GameObject> ();

				_goHitLast = null;
				_raycastedObjectHasChanged = false;

				_lastMouseWheelUpdate = 0;

				_mouseIsDown = false;

				_v3AimPos = new Vector3 ();
				_chunkSizeHalved = VoxelUtils.CHUNK_SIZE * 0.5f;
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
		public virtual void activate()
		{
			//
		}

		//
		public virtual void deactivate()
		{
			//
		}

		// LateUpdate call
		public virtual void customUpdate(float time, float timeDelta)
		{
			//
		}

		//
		// PUBLIC METHODS
		//

		public void resetAll()
		{
			resetAim ();
			_goLastShaderChange = null;
			resetMaterial ();
			resetProp ();
		}

		public void resetAim()
		{
			setSingleShader (_goLastShaderChange, Globals.defaultShaderName);
			changeShaders ();
			_trfmAimTool.position = new Vector3(9999,9999,9999);
			_trfmAimTool.forward = Vector3.forward;
			_rendererAimTool.material = _materialAimTool;
			_rendererAimCenterCube.material = _materialAimTool;
			_goHitLast = null;
		}

		public void setCurAimMaterial() {
			_rendererAimTool.sharedMaterial = _levelEditor.aToolMaterials [MainMenu.Instance.iSelectedMaterial];
		}

		public void setCurAimCenterCubeMaterial() {
			_rendererAimTool.material = _materialAimTool;
			_rendererAimCenterCube.sharedMaterial = _levelEditor.aMaterials [MainMenu.Instance.iSelectedMaterial];
		}

		public void resetMaterial()
		{
			//setSingleMaterial (_goLastMaterialChanged, _tempMaterial);
			//_goLastMaterialChanged = null;
			//_tempMaterial = null;
		}

		public void resetProp()
		{
			_trfmAimProp.forward = Vector3.forward;
			_trfmAimProp.position = new Vector3(0,0,0);

			if (_levelEditor.goCurItem != null)
			{
				_goCurProp = _levelEditor.goCurItem;
				if (_goCurProp.GetComponent<Collider> () != null)
				{
					_goCurProp.transform.position = new Vector3(0,0,0);
					_collider = _goCurProp.GetComponent<Collider> ();
					_collider.enabled = true;
					_bounds = _collider.bounds;
					_yOffset = _bounds.extents.y - _bounds.center.y;
					_zOffset = _bounds.extents.z - _bounds.center.z;
					_collider.enabled = false;
				}
			}
			else {
				_goCurProp = null;
			}

			_trfmAimProp.position = new Vector3(9999,9999,9999);
		}

		private void changeSingleShader(GameObject go, string shaderName = Globals.defaultShaderName)
		{
			if (go == null) {
				return;
			}

			// reset current shader
			if (_goLastShaderChange != null && go != _goLastShaderChange) {
				setSingleShader (_goLastShaderChange, Globals.defaultShaderName);
				_goLastShaderChange = null;
			}

			_goLastShaderChange = go;
			setSingleShader (_goLastShaderChange, shaderName);
		}

		private void setSingleShader(GameObject go, string shaderName)
		{
			if (go != null) {
				Shader shader = getShader (shaderName);
				Renderer renderer = go.GetComponent<Renderer> ();
				if (renderer != null && renderer.material.shader.name != shaderName) {
					renderer.material.shader = shader;
					//Debug.Log ("changing shader for game object " + go.name + " to " + shader.name);
				}
			}
		}

		//
		public void changeShaders(string shaderName = Globals.defaultShaderName)
		{
			// reset current shaders
			setShaders (Globals.defaultShaderName);
			_aGoShaderChanged.Clear ();

			// set new shaders
			_aGoShaderChanged = _levelEditor.getOverlappingObjects(_trfmAimTool.position, _rendererAimTool.bounds.extents);
			setShaders (shaderName);
		}

		private void setShaders(string shaderName)
		{
			Shader shader = getShader(shaderName);

			Renderer renderer;
			int i, len = _aGoShaderChanged.Count;
			for (i = 0; i < len; ++i) {
				if (_aGoShaderChanged [i] != null) {
					renderer = _aGoShaderChanged [i].GetComponent<Renderer> ();
					if (renderer != null && renderer.material.shader.name != shaderName) {
						renderer.material.shader = shader;
					}
				}
			}
		}

		private Shader getShader(string shaderName)
		{
			if (!_aUsedShaders.ContainsKey(shaderName)) {
				_aUsedShaders.Add(shaderName, Shader.Find (shaderName));
				//Debug.Log ("added shader " + shaderName);
			}

			return _aUsedShaders[shaderName];
		}

		//
		/*public void changeSingleMaterial(GameObject go, int materialIndex)
		{
			if (go == null) {
				return;
			}

			// reset current material
			if (_goLastMaterialChanged != null && go != _goLastMaterialChanged) {
				resetMaterial ();
			}

			_goLastMaterialChanged = go;
			setSingleMaterial (_goLastMaterialChanged, _levelEditor.aMaterials[materialIndex]);
		}*/

		//
		public void setSingleMaterial(GameObject go, Material material, bool setTempMaterial = true)
		{
			if (go != null && material != null) {
				Renderer renderer = go.GetComponent<Renderer> ();
				if (renderer != null && renderer.sharedMaterial.name != material.name) {
					//if (setTempMaterial) {
					//	_tempMaterial = renderer.sharedMaterial;
					//}
					renderer.sharedMaterial = material;
				}
			}
		}

		//
		// PRIVATE METHODS
		//
		protected void doRayCast()
		{
			_goHit = null;
			_raycastedObjectHasChanged = false;

			if (MainMenu.Instance.popup.isVisible ()) {
				return;
			}

			// no raycasting if mouse cursor is over top menu
			if (Input.mousePosition.y >= _maxClickY) {
				return;
			}

			_ray = _curCam.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (_ray, out _hit, Globals.RAYCAST_DISTANCE_EDIT)) {
				_goHit = _hit.collider.gameObject;
				if (_goHit != _goHitLast) {
					_goHitLast = _goHit;
					_raycastedObjectHasChanged = true;
				}
			}
		}

		//
		protected void setAimTool ()
		{
			//_trfmAimTool.forward = _hit.normal;

			_v3AimPos.x = (int)((_hit.point.x + (_hit.normal.x * -1 * _chunkSizeHalved)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;
			_v3AimPos.y = (int)((_hit.point.y + (_hit.normal.y * -1 * _chunkSizeHalved)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;
			_v3AimPos.z = (int)((_hit.point.z + (_hit.normal.z * -1 * _chunkSizeHalved)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;

			_xAimScale = MainMenu.Instance.v3DigSettings.x * VoxelUtils.CHUNK_SIZE;
			_yAimScale = MainMenu.Instance.v3DigSettings.y * VoxelUtils.CHUNK_SIZE;
			_zAimScale = MainMenu.Instance.v3DigSettings.z * VoxelUtils.CHUNK_SIZE;
			// painting doesn't use depth setting
			//_zAimScale = (AppController.Instance.appState == AppState.Paint ? 0.5f : MainMenu.Instance.v3DigSettings.z * VoxelUtils.CHUNK_SIZE);

			if (_hit.normal.x != 0.0f)
			{
				_v3AimPos.y += (_yAimScale * 0.5f);//(_yAimScale * _chunkSizeHalved);
				_v3AimPos.z += (_zAimScale * 0.5f);//(_zAimScale * _chunkSizeHalved);

				if (_hit.normal.x > 0) {
					_v3AimPos.x += (VoxelUtils.CHUNK_SIZE - (_xAimScale * 0.5f));
					//_v3AimPos.x -= -_chunkSizeHalved + ((_xAimScale - 1) * _chunkSizeHalved);
				} else {
					_v3AimPos.x += (_xAimScale * 0.5f);//(_xAimScale * _chunkSizeHalved);
				}
			}
			else if (_hit.normal.y != 0.0f)
			{
				_v3AimPos.x += (_xAimScale * 0.5f);//(_xAimScale * _chunkSizeHalved);
				_v3AimPos.z += (_zAimScale * 0.5f);//(_zAimScale * _chunkSizeHalved);

				if (_hit.normal.y > 0) {
					_v3AimPos.y += (VoxelUtils.CHUNK_SIZE - (_yAimScale * 0.5f));
					//_v3AimPos.y -= -_chunkSizeHalved + ((_yAimScale - 1) * _chunkSizeHalved);
				} else {
					_v3AimPos.y += (_yAimScale * 0.5f);//(_yAimScale * _chunkSizeHalved);
				}
			}
			else if (_hit.normal.z != 0.0f)
			{
				_v3AimPos.x += (_xAimScale * 0.5f);//(_xAimScale * _chunkSizeHalved);
				_v3AimPos.y += (_yAimScale * 0.5f);//(_yAimScale * _chunkSizeHalved);

				if (_hit.normal.z > 0) {
					_v3AimPos.z += (VoxelUtils.CHUNK_SIZE - (_zAimScale * 0.5f));
					//_v3AimPos.z -= -_chunkSizeHalved + ((_zAimScale - 1) * _chunkSizeHalved);
				} else {
					_v3AimPos.z += (_zAimScale * 0.5f);
				}
			}

			_trfmAimTool.position = _v3AimPos;
		}
	}
}