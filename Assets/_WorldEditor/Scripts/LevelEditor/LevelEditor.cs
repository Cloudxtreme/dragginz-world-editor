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
		public Material laserAimMaterial;

		private World _World;

		private List<EditorTool> _aEditorTools;
		private EditorTool _curEditorTool;

		private Dictionary<string, Shader> _aUsedShaders;
		private List<Material> _aMaterials;

		private GameObject _goLastShaderChange;
		private List<GameObject> _aGoShaderChanged;

		private float _fRockSize;
        private int _cubesPerQuadrant;
        private float _fQuadrantSize;

        private int _numCubes;

		#region Getters

		public float fRockSize {
			get { return _fRockSize; }
		}

		public int cubesPerQuadrant {
			get { return _cubesPerQuadrant; }
		}

		public float fQuadrantSize {
			get { return _fQuadrantSize; }
		}

		public List<Material> aMaterials {
			get { return _aMaterials; }
		}

		#endregion

		#region SystemMethods

		void Awake()
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			gameObject.AddComponent<World> ();
			_World = World.Instance;

			_aMaterials = new List<Material> ();
			int i, len = Globals.materials.Length;
			for (i = 0; i < len; ++i) {
				_aMaterials.Add(Resources.Load<Material> ("Materials/" + Globals.materials [i]));
			}

			_aUsedShaders = new Dictionary<string, Shader> ();
			_goLastShaderChange = null;
			_aGoShaderChanged = new List<GameObject> ();

			_fRockSize = 0.5f;
			_cubesPerQuadrant = 2;
			_fQuadrantSize = (float)_cubesPerQuadrant * _fRockSize;

			// Instantiate app controller singleton
			if (GameObject.Find(Globals.appContainerName) == null) {
				GameObject goAppController = new GameObject(Globals.appContainerName);
				DontDestroyOnLoad(goAppController);
				goAppController.AddComponent<AppController>();
			}
		}

		//
		void Start() {

			setMode (AppState.Null, true);

			_World.init ();

			goPlayer.transform.position = new Vector3(0, 0.6f, -0.75f);

			_aEditorTools = new List<EditorTool> (Globals.NUM_EDITOR_TOOLS);
			_aEditorTools.Add(new EditorToolLook());
			_aEditorTools.Add(new EditorToolDig());
			_aEditorTools.Add(new EditorToolPaint());
			_aEditorTools.Add(new EditorToolBuild());

			_curEditorTool = null;

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

		public void customUpdateCheckControls(float time, float timeDelta)
		{
			if (!MainMenu.Instance.popup.isVisible ())
			{
				if (Input.GetKeyDown(KeyCode.Escape)) {
					if (AppController.Instance.appState == AppState.Dig || AppController.Instance.appState == AppState.Paint || AppController.Instance.appState == AppState.Build) {
						resetFlyCam();
					}
				}
				else if (Input.GetKeyDown(KeyCode.Alpha1)) {
					setMode (AppState.Look);
				}
				else if (Input.GetKeyDown(KeyCode.Alpha2)) {
					setMode (AppState.Dig);
				}
				else if (Input.GetKeyDown(KeyCode.Alpha3)) {
					setMode (AppState.Paint);
				}
				else if (Input.GetKeyDown(KeyCode.Alpha4)) {
					setMode (AppState.Build);
				}
				//else if (Input.GetKeyDown(KeyCode.Alpha5)) {
				//	setMode(AppState.Play);
				//}
			}

			if (_curEditorTool != null) {
				_curEditorTool.customUpdateControls (time, timeDelta);
			}

			_World.customUpdate ();
		}

		//
		public void customUpdate(float time, float timeDelta)
		{
			if (_curEditorTool != null) {
				_curEditorTool.customUpdate (time, timeDelta);
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

				if (_curEditorTool != null) {
					_curEditorTool.setSingleMaterial (laserAim, laserAimMaterial, false);
					_curEditorTool.resetMaterial ();
				}
					
				resetAim ();

				_curEditorTool = null;

				if (mode == AppState.Dig)
				{
					MainMenu.Instance.showDigButtons (true);
					MainMenu.Instance.showMaterialBox (false);
					laserAim.SetActive (true);
					updateDigSettings (MainMenu.Instance.v3DigSettings);
					_curEditorTool = _aEditorTools [Globals.EDITOR_TOOL_DIG];
				}
				else if (mode == AppState.Paint)
				{
					MainMenu.Instance.showDigButtons (false);
					MainMenu.Instance.showMaterialBox (true);
					laserAim.SetActive (false);
					_curEditorTool = _aEditorTools [Globals.EDITOR_TOOL_PAINT];
				}
				else if (mode == AppState.Build)
				{
					MainMenu.Instance.showDigButtons (false);
					MainMenu.Instance.showMaterialBox (true);
					laserAim.SetActive (true);
					laserAim.transform.localScale = new Vector3(_fRockSize, _fRockSize, _fRockSize);
					_curEditorTool = _aEditorTools [Globals.EDITOR_TOOL_BUILD];
					_curEditorTool.setSingleMaterial (laserAim, _aMaterials[MainMenu.Instance.iSelectedMaterial], false);
				}
				else
				{
					MainMenu.Instance.showDigButtons (false);
					MainMenu.Instance.showMaterialBox (false);
					laserAim.SetActive (false);

					if (mode == AppState.Look) {
						_curEditorTool = _aEditorTools [Globals.EDITOR_TOOL_LOOK];
					}
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

		public void newMaterialSelected (int iSelectedMaterial)
		{
			if (AppController.Instance.appState == AppState.Paint) {
				_curEditorTool.resetMaterial();
			}
			else if (AppController.Instance.appState == AppState.Build) {
				_curEditorTool.setSingleMaterial (laserAim, _aMaterials [MainMenu.Instance.iSelectedMaterial], false);
			}
		}

		public void resetFlyCam()
		{
			FlyCam.Instance.reset ();
			PlayerEditCollision.Instance.isColliding = false;
		}

		public void toggleFlyCamOffset()
		{
			FlyCam.Instance.toggleOffset ();
		}

		//
		public void resetAim()
		{
			setSingleShader (_goLastShaderChange, Globals.defaultShaderName);
			changeShaders ();
			laserAim.transform.position = new Vector3(9999,9999,9999);
		}

		#endregion

		// ----------------------------------------------------------------------------------------

		#region PrivateMethods

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
		public void digIt (Vector3 v3Pos)
		{
			int i, len;

			// keep track of parent objects that had children removed
			List<Transform> listcubeTransforms = new List<Transform>();

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
					_World.createRockCube (adjacentCubes [j]);
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
		/*public void paintIt (GameObject go)
		{
			MeshRenderer renderer = go.GetComponent<MeshRenderer> ();
			if (renderer != null) {
				renderer.sharedMaterial = _aMaterials[MainMenu.Instance.iSelectedMaterial];
			}

			_goLastMaterialChanged = null;
			_tempMaterial = null;
		}*/

		//
		public void buildIt(Vector3 v3Pos)
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
				GameObject goNew = _World.createRock (v3LocalBlockPos, container.gameObject, sName);
				_curEditorTool.setSingleMaterial (goNew, _aMaterials[MainMenu.Instance.iSelectedMaterial], false);
			}
		}

		#endregion
	}
}