﻿//
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

		protected static Dictionary<string, Shader> _aUsedShaders;
		protected static GameObject _goLastShaderChange;
		protected static List<GameObject> _aGoShaderChanged;

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

				_aUsedShaders = new Dictionary<string, Shader> ();
				_goLastShaderChange = null;
				_aGoShaderChanged = new List<GameObject> ();

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

		public void resetAll() {
			resetAim ();
			_goLastShaderChange = null;
			resetMaterial ();
		}

		public void resetAim()
		{
			setSingleShader (_goLastShaderChange, Globals.defaultShaderName);
			changeShaders ();
			_trfmAimTool.position = new Vector3(9999,9999,9999);
		}

		public void resetMaterial()
		{
			setSingleMaterial (_goLastMaterialChanged, _tempMaterial);
			_goLastMaterialChanged = null;
			_tempMaterial = null;
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
			_aGoShaderChanged = LevelEditor.Instance.getOverlappingObjects(_trfmAimTool.position);
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
			if (MainMenu.Instance.popup.isVisible ()) {
				return;
			}

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