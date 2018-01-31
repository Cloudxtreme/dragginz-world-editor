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
	public class LevelEditor : MonoSingleton<LevelEditor>
	{
		public Camera mainCam;
		public GameObject goWorld;
		public GameObject goPlayer;
		public GameObject goPlayerEdit;

		public GameObject cubePrefab;
        public GameObject cubePrefab2;
		public GameObject cubePrefabCenter;

        public List<Material> materialsWalls;

		public GameObject laserAim;

		private Ray _ray;
		private RaycastHit _hit;
		private GameObject _goHit;

		private Dictionary<int, Dictionary<int, Dictionary<int, int>>> _quadrantFlags;
		private int _iMinLevelCoord;
		private int _iMaxLevelCoord;

		private Dictionary<string, Shader> _aUsedShaders;
		private List<Material> _aMaterials;

		private GameObject _goLastMaterialChanged;
		private Material _tempMaterial;

		private GameObject _goLastShaderChange;
		private List<GameObject> _aGoShaderChanged;

		private Dictionary<GameObject, bool> _visibleQuadrants;
		private List<GameObject> _aQuadrantChangedVisibility;
		private bool _coroutineIsRunning;

		private float _fRockSize;
        private int _cubesPerQuadrant;
        private float _fQuadrantSize;
        private int _cubeIndex;

        private int _numCubes;

		private bool _mouseIsDown;

		#region SystemMethods

		/// <summary>
		/// System methods
		/// </summary>
		void Awake()
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			_aMaterials = new List<Material> ();
			int i, len = Globals.materials.Length;
			for (i = 0; i < len; ++i) {
				_aMaterials.Add(Resources.Load<Material> ("Materials/" + Globals.materials [i]));
			}

			_goLastMaterialChanged = null;
			_tempMaterial = null;

			_aUsedShaders = new Dictionary<string, Shader> ();
			_goLastShaderChange = null;
			_aGoShaderChanged = new List<GameObject> ();

			_visibleQuadrants = new Dictionary<GameObject, bool> ();
			_aQuadrantChangedVisibility = new List<GameObject> ();
			_coroutineIsRunning = false;

            _cubeIndex = 1;
            toggleCubeSizes();

            _numCubes = 0;

			_mouseIsDown = false;

			// Instantiate app controller singleton
			if (GameObject.Find(Globals.appContainerName) == null) {
				GameObject goAppController = new GameObject(Globals.appContainerName);
				DontDestroyOnLoad(goAppController);
				goAppController.AddComponent<AppController>();
			}

			_iMinLevelCoord = -50;
			_iMaxLevelCoord = 50;

			_goHit = null;

			_quadrantFlags = new Dictionary<int, Dictionary<int, Dictionary<int, int>>> ();

			for (int x = _iMinLevelCoord; x <= _iMaxLevelCoord; ++x) {
				_quadrantFlags.Add(x, new Dictionary<int, Dictionary<int, int>> ());
				for (int y = _iMinLevelCoord; y <= _iMaxLevelCoord; ++y) {
					_quadrantFlags [x].Add(y, new Dictionary<int, int> ());
					for (int z = _iMinLevelCoord; z <= _iMaxLevelCoord; ++z) {
						_quadrantFlags [x] [y].Add(z, 0);
					}
				}
			}
		}

		//
		void Start() {

			setMode (AppState.Null, true);

			createWorld ();

			updateDigSettings (new Vector3(1,1,1));

			AppController.Instance.showPopup(
				PopupMode.Notification,
				"Controls",
				"Normal movement: AWSD\nUp and down: QE\nLook around: Right mouse button\nToggle tools: Mouse wheel\nAdjust movement speed: -/+\n\nPress ESC to reset speed and position.",
				startUpPopupCallback
			);
		}

        #endregion

        // ----------------------------------------------------------------------------------------

        #region PublicMethods

        public void toggleCubes() {

            //toggleCubeSizes();
            //createWorld();
        }

		//
		public void setQuadrantVisibilityFlag(GameObject quadrant, bool visible)
		{
			//Debug.Log (quadrant.name+".visible = "+visible);

			_visibleQuadrants [quadrant] = visible;

			if (_aQuadrantChangedVisibility.Contains (quadrant)) {
				_aQuadrantChangedVisibility.Remove (quadrant);
			}

			_aQuadrantChangedVisibility.Add (quadrant);
		}

        //
        public void customUpdateCheckControls()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1)) {
				if (!MainMenu.Instance.popup.isVisible ()) {
					setMode (AppState.Look);
				}
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2)) {
				if (!MainMenu.Instance.popup.isVisible ()) {
					setMode (AppState.Dig);
				}
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3)) {
				if (!MainMenu.Instance.popup.isVisible ()) {
					setMode (AppState.Paint);
				}
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4)) {
				if (!MainMenu.Instance.popup.isVisible ()) {
					setMode (AppState.Build);
				}
			}
			//else if (Input.GetKeyDown(KeyCode.Alpha5)) {
			//	setMode(AppState.Play);
			//}

			if (!_mouseIsDown) {
				if (Input.GetButtonDown ("Fire1")) {
					_mouseIsDown = true;
				}
			} else {
				if (Input.GetButtonUp ("Fire1")) {
					_mouseIsDown = false;
				}
			}

			if (!_coroutineIsRunning && _visibleQuadrants.Count > 0) {
				StartCoroutine("updateQuadrantVisibility");
			}
		}

		//
		public void customUpdateDig() {

			doRayCastDig ();
			if (_mouseIsDown && _goHit != null) {
				if (Screen.height - Input.mousePosition.y > 90) {
					digIt (_goHit.transform.position);//_hit.point);
					_mouseIsDown = false;
				}
			}
		}

		public void customUpdatePaint() {
			
			if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
				MainMenu.Instance.toggleMaterial (Input.GetAxis ("Mouse ScrollWheel"));
			}

			doRayCastPaint ();
			if (_mouseIsDown && _goHit != null) {
				if (Screen.height - Input.mousePosition.y > 90) {
					paintIt (_goHit);
				}
			}
		}

		public void customUpdateBuild() {

			doRayCastBuild ();
			if (_mouseIsDown && _goHit != null) {
				if (Screen.height - Input.mousePosition.y > 90) {
					buildIt (laserAim.transform.position);
					_mouseIsDown = false;
				}
			}
		}

		//
		public void startUpPopupCallback(int buttonId) {

			AppController.Instance.hidePopup ();
			setMode (AppState.Dig);
		}

		//
		public void setMode(AppState mode, bool forceMode = false) {

			if (forceMode || (mode != AppController.Instance.appState))
			{
				AppController.Instance.setAppState (mode);
				MainMenu.Instance.setModeButtons (mode);
				resetAim ();
				resetMaterial ();

				if (mode == AppState.Dig)
				{
					MainMenu.Instance.showDigButtons (true);
					MainMenu.Instance.showMaterialBox (false);
					laserAim.SetActive (true);
					updateDigSettings (MainMenu.Instance.v3DigSettings);
				}
				else if (mode == AppState.Paint)
				{
					MainMenu.Instance.showDigButtons (false);
					MainMenu.Instance.showMaterialBox (true);
					laserAim.SetActive (false);
				}
				else if (mode == AppState.Build) {
					MainMenu.Instance.showDigButtons (false);
					MainMenu.Instance.showMaterialBox (false);
					laserAim.SetActive (true);
					laserAim.transform.localScale = new Vector3(_fRockSize, _fRockSize, _fRockSize);
				}
				else
				{
					MainMenu.Instance.showDigButtons (false);
					MainMenu.Instance.showMaterialBox (false);
					laserAim.SetActive (false);

				}

				if (goPlayer != null && goPlayerEdit != null) {
					goPlayer.SetActive ((mode == AppState.Play));
					goPlayerEdit.SetActive (!goPlayer.activeSelf);
				}
			}
		}

		public void updateDigSettings(Vector3 v3DigSettings)
		{
			float _fScale = _fRockSize * .75f;
			laserAim.transform.localScale = v3DigSettings * _fScale;
		}

		public void resetFlyCam()
		{
			mainCam.gameObject.GetComponent<FlyCam> ().reset ();
			PlayerEditCollision.Instance.isColliding = false;
		}

		public void toggleFlyCamOffset()
		{
			mainCam.gameObject.GetComponent<FlyCam> ().toggleOffset ();
		}

		#endregion

        // ----------------------------------------------------------------------------------------

		#region PrivateMethods

		private IEnumerator updateQuadrantVisibility()
		{
			int len = _aQuadrantChangedVisibility.Count;
			while (len > 0) {

				_coroutineIsRunning = true;

				int i;
				for (i = 0; i < 5; ++i) {
					
					GameObject go = _aQuadrantChangedVisibility [0];
					bool visible = _visibleQuadrants [go];
					_aQuadrantChangedVisibility.RemoveAt (0);
					go.transform.Find ("container").gameObject.SetActive (visible);

					len = _aQuadrantChangedVisibility.Count;
					if (len <= 0) {
						break;
					}
				}

				yield return new WaitForEndOfFrame();
			}

			_coroutineIsRunning = false;
		}

        private void toggleCubeSizes() {

            _cubeIndex = (_cubeIndex == 1 ? 0 : 1);

            if (_cubeIndex == 0) {
                _fRockSize = 0.5f;
                _cubesPerQuadrant = 2;
            }
            else {
                _fRockSize = 1.0f;
                _cubesPerQuadrant = 1;
            }

            _fQuadrantSize = (float)_cubesPerQuadrant * _fRockSize;
        }

        private void doRayCastDig()
		{
			_ray = mainCam.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (_ray, out _hit, 20f)) {

				_goHit = _hit.collider.gameObject;
				laserAim.transform.position = _hit.point;
				laserAim.transform.forward  = _hit.normal;
				changeShaders (Globals.highlightShaderName);
			}
			else {
				resetAim ();
			}
		}

		private void doRayCastPaint()
		{
			_ray = mainCam.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (_ray, out _hit, 20f)) {

				_goHit = _hit.collider.gameObject;
				//changeSingleShader (_goHit, Globals.highlightShaderName);
				changeSingleMaterial (_goHit, MainMenu.Instance.iSelectedMaterial);
			} else {
				resetMaterial ();
			}
		}

		private void doRayCastBuild()
		{
			_ray = mainCam.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (_ray, out _hit, 20f)) {

				_goHit = _hit.collider.gameObject;
				laserAim.transform.position = _goHit.transform.position + (_hit.normal * _fRockSize);
			}
			else {
				resetAim ();
			}
		}

		//
		private void resetAim()
		{
			setSingleShader (_goLastShaderChange, Globals.defaultShaderName);
			changeShaders ();
			laserAim.transform.position = new Vector3(9999,9999,9999);
			_goHit = null;
		}

		//
		private void resetMaterial()
		{
			setSingleMaterial (_goLastMaterialChanged, _tempMaterial);
			_goLastMaterialChanged = null;
			_tempMaterial = null;
		}

		//
		private void changeSingleMaterial(GameObject go, int materialIndex)
		{
			if (go == null) {
				return;
			}

			// reset current material
			if (_goLastMaterialChanged != null && go != _goLastMaterialChanged) {
				setSingleMaterial (_goLastMaterialChanged, _tempMaterial);
				_goLastMaterialChanged = null;
				_tempMaterial = null;
			}

			_goLastMaterialChanged = go;
			setSingleMaterial (_goLastMaterialChanged, _aMaterials[materialIndex]);
		}

		//
		private void setSingleMaterial(GameObject go, Material material)
		{
			if (go != null && material != null) {
				Renderer renderer = go.GetComponent<Renderer> ();
				if (renderer != null && renderer.sharedMaterial.name != material.name) {
					_tempMaterial = renderer.sharedMaterial;
					renderer.sharedMaterial = material;
					Debug.Log ("changing material for game object " + go.name + " to " + material.name);
				}
			}
		}
		//
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
		private void changeShaders(string shaderName = Globals.defaultShaderName)
		{
			// reset current shaders
			setShaders (Globals.defaultShaderName);
			_aGoShaderChanged.Clear ();

			// set new shaders
			_aGoShaderChanged = getOverlappingObjects(laserAim.transform.position);
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
		private void resetWorld()
		{
			_numCubes = 0;

			foreach (Transform child in goWorld.transform) {
				Destroy (child.gameObject);
			}

			_visibleQuadrants.Clear ();
			_aQuadrantChangedVisibility.Clear ();

			goPlayer.transform.position = new Vector3(0, 0.6f, -0.75f);

			for (int x = _iMinLevelCoord; x <= _iMaxLevelCoord; ++x) {
				for (int y = _iMinLevelCoord; y <= _iMaxLevelCoord; ++y) {
					for (int z = _iMinLevelCoord; z <= _iMaxLevelCoord; ++z) {
						_quadrantFlags [x] [y] [z] = 0;
					}
				}
			}
		}

		//
		private void createWorld() {

			resetWorld ();

			int count = 0;
			
			// create hollow cube of cubes :)
			int size = 2; // actual size will be size*2+1
			int height = 3;
			Vector3 pos = Vector3.zero;
			for (int x = -size; x <= size; ++x) {
				for (int y = -1; y <= height; ++y) {
					for (int z = -size; z <= size; ++z) {

						pos = new Vector3 (x * _fQuadrantSize, y * _fQuadrantSize, z * _fQuadrantSize);

						if (Mathf.Abs (x) == size || y == -1 || y == height || Mathf.Abs (z) == size) {
							createRockCube (pos);
						} else {
							createRockCube (pos, false);
						}

						count++;
						/*if (count == 1) {
							x = size+1;
							y = height+1;
							z = size+1;
						}*/
					}
				}
			}
			Debug.Log ("quadrants: "+count.ToString());

			MainMenu.Instance.setCubeCountText (_numCubes);
		}

		//
		private void createRockCube (Vector3 v3CubePos, bool fillQuadrant = true)
		{
			// cube already created at that position
			if (_quadrantFlags [(int)v3CubePos.x] [(int)v3CubePos.y] [(int)v3CubePos.z] == 1) {
				return;
			}

			GameObject cubeParent = createQuadrant (v3CubePos);
			GameObject container = new GameObject ();
			container.name = "container";
			container.transform.SetParent (cubeParent.transform);
			container.transform.localPosition = Vector3.zero;

			if (!fillQuadrant) {
				return;
			}

			Vector3 pos = Vector3.zero;
			int count = 0;

			int len = _cubesPerQuadrant;
			float startPos = 0;//(int)len * _fRockSize * .5f;
			string sName = "";

			pos.x = startPos;// + (_fRockSize * 0.5f);
			for (int x = 0; x < len; ++x) {
				pos.y = startPos;// - (_fRockSize * 0.5f);
				for (int y = 0; y < len; ++y) {
					pos.z = startPos;// + (_fRockSize * 0.5f);
					for (int z = 0; z < len; ++z) {
						sName = "r-" + x.ToString () + "-" + y.ToString () + "-" + z.ToString (); // Globals.rockGameObjectPrepend + count.ToString ();
						createRock (pos, container, sName);
						//count++;
						pos.z += _fRockSize;
					}
					pos.y += _fRockSize;
				}
				pos.x += _fRockSize;
			}
		}

		//
		private GameObject createQuadrant(Vector3 v3CubePos)
		{
			string sPos = v3CubePos.x.ToString () + "_" + v3CubePos.y.ToString () + "_" + v3CubePos.z.ToString ();

			GameObject quadrant = new GameObject(Globals.containerGameObjectPrepend + sPos);
			quadrant.transform.SetParent(goWorld.transform);
			quadrant.transform.localPosition = v3CubePos;

			if (cubePrefabCenter != null) {
				GameObject go = GameObject.Instantiate(cubePrefabCenter);
				go.name = "center_" + sPos;
				go.transform.SetParent(quadrant.transform);
				go.transform.localPosition = new Vector3(0.25f, 0.25f, 0.25f);
				Block blockScript = go.AddComponent<Block> ();
				blockScript.init ();
			}

			_quadrantFlags [(int)v3CubePos.x] [(int)v3CubePos.y] [(int)v3CubePos.z] = 1;

			_visibleQuadrants.Add (quadrant, true);

			return quadrant;
		}

		/// <summary>
		/// ...
		/// </summary>
		private GameObject createRock(Vector3 pos, GameObject parent, string name) {

			GameObject go = null;
			GameObject prefab = null;
			Vector3 rotation = Vector3.zero;

			prefab = (_cubeIndex == 0 ? cubePrefab : cubePrefab2);
			if (prefab) {
				go = GameObject.Instantiate(prefab);
				go.name = name;
				go.transform.SetParent(parent.transform);
				go.transform.localPosition = pos;
				go.transform.localRotation = Quaternion.Euler (rotation);
				go.GetComponent<MeshRenderer> ().material = materialsWalls[UnityEngine.Random.Range (0, materialsWalls.Count)];
				_numCubes++;
			}

			return go;
		}

		/// <summary>
		/// Destroy game objects within a 0.5f radius of the hit point
		/// </summary>
		private void digIt (Vector3 v3Pos)
		{
			//Debug.LogWarning ("DIG:");
			//Debug.Log (_goHit.name);

			int i, len;

			// keep track of parent objects that had children removed
			List<Transform> listcubeTransforms = new List<Transform>();

			if (laserAim != null) {
				v3Pos = laserAim.transform.position;
			}
			List<GameObject> listCollidingObjects = getOverlappingObjects(v3Pos);
			len = listCollidingObjects.Count;
			for (i = 0; i < len; ++i) {
				if (!listcubeTransforms.Contains (listCollidingObjects [i].transform.parent)) {
					listcubeTransforms.Add (listCollidingObjects [i].transform.parent);
				}
				Destroy (listCollidingObjects [i]);
				_numCubes--;
			}
			listCollidingObjects.Clear ();
			listCollidingObjects = null;

			MainMenu.Instance.setCubeCountText (_numCubes);

			// extend level if necessary
			len = listcubeTransforms.Count;
			for (i = 0; i < len; ++i) {

				List<Vector3> adjacentCubes = getAdjacentCubes (listcubeTransforms [i].position);

				int j, len2 = adjacentCubes.Count;
				for (j = 0; j < len2; ++j) {
					createRockCube (adjacentCubes [j]);
				}
			}
			listcubeTransforms.Clear ();
			listcubeTransforms = null;
		}

		//
		private List<GameObject> getOverlappingObjects(Vector3 v3Pos)
		{
			List<GameObject> listCollidingObjects = new List<GameObject>();

			int i, len;

			Vector3 pos = MainMenu.Instance.v3DigSettings * (_fRockSize * .75f) * .5f;
			Collider[] hitColliders = Physics.OverlapBox (v3Pos, pos);

			len = hitColliders.Length;
			for (i = 0; i < len; ++i) {
				if (hitColliders [i].tag == "DigAndDestroy") {
					listCollidingObjects.Add (hitColliders [i].gameObject);
				}
			}

			return listCollidingObjects;
		}

		//
		private List<Vector3> getAdjacentCubes(Vector3 v3CubePos) {

			List<Vector3> adjacentCubes = new List<Vector3> ();

			int len = 1;
			for (int x = -len; x <= len; ++x) {
				for (int y = -len; y <= len; ++y) {
					for (int z = -len; z <= len; ++z) {
						adjacentCubes.Add (new Vector3(v3CubePos.x+(x*_fQuadrantSize), v3CubePos.y+(y*_fQuadrantSize), v3CubePos.z+(z*_fQuadrantSize)));
					}
				}
			}

			return adjacentCubes;
		}

		//
		private void paintIt (GameObject go)
		{
			if (go == null) {
				return;
			}

			MeshRenderer renderer = go.GetComponent<MeshRenderer> ();
			if (renderer != null) {
				renderer.sharedMaterial = _aMaterials[MainMenu.Instance.iSelectedMaterial];
			}

			_goLastMaterialChanged = null;
			_tempMaterial = null;
		}

		//
		private void buildIt(Vector3 v3Pos)
		{
			int x = (int)(v3Pos.x < 0 ? Math.Round(v3Pos.x, MidpointRounding.AwayFromZero) : v3Pos.x);
			int y = (int)(v3Pos.y < 0 ? Math.Round(v3Pos.y, MidpointRounding.AwayFromZero) : v3Pos.y);
			int z = (int)(v3Pos.z < 0 ? Math.Round(v3Pos.z, MidpointRounding.AwayFromZero) : v3Pos.z);

			//Debug.LogWarning ("BUILD:");

			// get quadrant

			Vector3 v3QuadrantPos = new Vector3 ((float)x / 1f, (float)y / 1f, (float)z / 1f);
			string sPos = v3QuadrantPos.x.ToString () + "_" + v3QuadrantPos.y.ToString () + "_" + v3QuadrantPos.z.ToString ();
			string sQuadrantName = Globals.containerGameObjectPrepend + sPos;
			Transform trfmQuadrant = goWorld.transform.Find (sQuadrantName);

			//Debug.Log ("quadrant: "+trfmQuadrant+" - "+trfmQuadrant.name);

			// get cild
			Vector3 v3LocalBlockPos = new Vector3 (
					Mathf.Abs(v3QuadrantPos.x-v3Pos.x),
					Mathf.Abs(v3QuadrantPos.y-v3Pos.y),
					Mathf.Abs(v3QuadrantPos.z-v3Pos.z)
				);

			string sName = "r";
			sName += "-" + ((int)(v3LocalBlockPos.x / _fRockSize)).ToString ();
			sName += "-" + ((int)(v3LocalBlockPos.y / _fRockSize)).ToString ();
			sName += "-" + ((int)(v3LocalBlockPos.z / _fRockSize)).ToString ();

			Transform container = trfmQuadrant.Find ("container");
			Transform trfmChild = container.Find (sName);
			if (trfmChild != null) {
				Debug.LogError ("child "+sName+" exists!");
			} else {
				createRock (v3LocalBlockPos, container.gameObject, sName);
			}
		}

		#endregion
	}
}