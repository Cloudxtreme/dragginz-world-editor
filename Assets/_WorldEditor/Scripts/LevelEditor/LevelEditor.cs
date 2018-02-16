//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using RTEditor;

namespace DragginzWorldEditor
{
	struct undoItem {
		public GameObject go;
		public Vector3 position;
		public Quaternion rotation;
	};

	struct undoAction {
		public AppState action;
		public GameObject go;
		public string name;
		public Vector3 position;
		public Transform parent;
		public Material material;
		public List<undoItem> items;
	};

	public class LevelEditor : MonoSingleton<LevelEditor>
	{
		public Camera editCam;
		public Camera itemCam;
		public RTEditorCam itemCamScript;

		public GameObject goWorld;
		public GameObject goProps;
		public GameObject goPlayer;
		public GameObject goPlayerEdit;

		public GameObject cubePrefab;
		public GameObject cubePrefabEdge;
		public GameObject cubePrefabCenter;

		public GameObject laserAim;
		public GameObject laserAimCenterCube;
		public Material laserAimMaterial;

		public GameObject itemAim;

		//public List<GameObject> itemPrefabs;
        public List<Material> materialsWalls;

		private World _World;

		private List<EditorTool> _aEditorTools;
		private EditorTool _curEditorTool;

		private List<Material> _aMaterials;
		private Dictionary<string, Material> _aDictMaterials;

		private List<undoAction> _undoActions;

		//private List<propDef> _levelPropDefs;
		//private int _iSelectedItem = 0;

		private GameObject _goCurItem;
		private List<GameObject> _selectedObjects;

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

		public Dictionary<string, Material> aDictMaterials {
			get { return _aDictMaterials; }
		}

		/*public int iSelectedItem {
			get { return _iSelectedItem; }
		}

		public List<propDef> levelPropDefs {
			get { return _levelPropDefs; }
		}*/

		public GameObject goCurItem {
			get { return _goCurItem; }
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
			_aDictMaterials = new Dictionary<string, Material> ();
			int i, len = Globals.materials.Length;
			for (i = 0; i < len; ++i) {
				_aMaterials.Add(Resources.Load<Material> ("Materials/" + Globals.materials [i]));
				_aDictMaterials.Add(Globals.materials [i], _aMaterials[_aMaterials.Count-1]);
			}

			_undoActions = new List<undoAction> ();

			//_levelPropDefs = new List<propDef> ();

			_goCurItem = null;
			_selectedObjects = new List<GameObject> ();

			_fRockSize = 0.5f;
			_cubesPerQuadrant = 2;
			_fQuadrantSize = (float)_cubesPerQuadrant * _fRockSize;

			PropsManager.Instance.init ();

			// Instantiate app controller singleton
			if (GameObject.Find(Globals.appContainerName) == null) {
				GameObject goAppController = new GameObject(Globals.appContainerName);
				DontDestroyOnLoad(goAppController);
				goAppController.AddComponent<AppController>();
			}
		}

		//
		void Start()
		{
			// init props

			/*PropsList propList = Resources.Load<PropsList> ("Data/" + Globals.propListName);
			int i, len = propList.props.Count;
			for (i = 0; i < len; ++i) {

				PropDefinition propDef = propList.props [i];
				if (propDef.prefab != null) {
					
					propDef p   = new propDef ();
					p.id          = propDef.id;
					p.name        = propDef.propName;
					p.prefab      = propDef.prefab;
					p.useCollider = propDef.isUsingCollider;
					p.useGravity  = propDef.isUsingGravity;

					_levelPropDefs.Add (p);
				}
			}*/

			// other stuff

			setMode (AppState.Null, true);

			MainMenu.Instance.setUndoButton (false);

			_World.init ();

			goPlayer.transform.position = new Vector3(0, 0.6f, -0.75f);

			_aEditorTools = new List<EditorTool> (Globals.NUM_EDITOR_TOOLS);
			_aEditorTools.Add(new EditorToolLook());
			_aEditorTools.Add(new EditorToolDig());
			_aEditorTools.Add(new EditorToolPaint());
			_aEditorTools.Add(new EditorToolBuild());
			_aEditorTools.Add(new EditorToolItem());

			_curEditorTool = null;

			//updateDigSettings (new Vector3(1,1,1));

			showHelpPopup ();
		}

        #endregion

        // ----------------------------------------------------------------------------------------

        #region PublicMethods

		private void showHelpPopup() {
			AppController.Instance.showPopup(
				PopupMode.Notification,
				"Controls",
				"Normal movement: AWSD\nUp and down: QE\nLook around: Right mouse button\nWireframe mode: Shift\nToggle tools: Mouse wheel\n\nPress ESC to reset speed and position.",
				startUpPopupCallback
			);
		}

		public void startUpPopupCallback(int buttonId) {

			AppController.Instance.hidePopup ();
			setMode (AppState.Look);
		}

		//
		public void resetAll() {

			resetUndoActions ();

			if (_curEditorTool != null) {
				_curEditorTool.resetAll ();
			}

			World.Instance.resetAll ();
		}

		//
		public void customUpdateCheckControls(float time, float timeDelta)
		{
			if (Input.GetKeyDown (KeyCode.LeftShift)) {
				FlyCam.Instance.drawWireframe = true;
				itemCamScript.drawWireframe = true;
			}
			else if (Input.GetKeyUp (KeyCode.LeftShift)) {
				FlyCam.Instance.drawWireframe = false;
				itemCamScript.drawWireframe = false;
			}

			if (MainMenu.Instance.popup.isVisible ())
			{
				if (Input.GetKeyDown (KeyCode.Escape)) {
					AppController.Instance.hidePopup ();
				}
			}
			else {
				
				if (Input.GetKeyDown (KeyCode.F1)) {
					showHelpPopup ();
				}
				else if (Input.GetKeyDown (KeyCode.Escape)) {
					if (AppController.Instance.appState != AppState.Look) {
						setMode (AppState.Look);
					}
				}
				else if (Input.GetKeyDown(KeyCode.H)) {
					resetFlyCam ();
				}
				else if (Input.GetKeyDown(KeyCode.Alpha1)) {
					setMode (AppState.Look);
				}
				else if (Input.GetKeyDown(KeyCode.Alpha2)) {
					setMode (AppState.Dig);
				}
				else if (Input.GetKeyDown(KeyCode.Alpha3)) {
					setMode (AppState.Build);
				}
				else if (Input.GetKeyDown(KeyCode.Alpha4)) {
					setMode (AppState.Paint);
				}
				else if (Input.GetKeyDown(KeyCode.Alpha5)) {
					setMode(AppState.Props);
				}
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
		public void setMode(AppState mode, bool forceMode = false)
		{
			if (mode == AppController.Instance.appState && !forceMode) {
				return;
			}

			AppController.Instance.setAppState (mode);
			MainMenu.Instance.setModeButtons (mode);

			if (_curEditorTool != null) {
				_curEditorTool.setSingleMaterial (laserAim, laserAimMaterial, false);
				_curEditorTool.resetMaterial ();
				_curEditorTool.resetAim ();
				_curEditorTool.resetItem ();
				_curEditorTool.deactivate ();
			}
			_curEditorTool = null;

			MainMenu.Instance.showTransformBox (false);
			MainMenu.Instance.showDigButtons (false);
			MainMenu.Instance.showMaterialBox (false);
			MainMenu.Instance.showItemsBox (false);

			laserAim.SetActive (false);
			itemAim.SetActive (false);

			editCam.enabled = true;
			itemCam.enabled = false;

			EditorObjectSelection.Instance.ClearSelection(false);

			setSelectedObjects ();

			SceneGizmo.Instance.editorCameraTransform = EditorCamera.Instance.transform;

			if (mode == AppState.Look)
			{
				MainMenu.Instance.showTransformBox (true);
				_curEditorTool = _aEditorTools [Globals.EDITOR_TOOL_LOOK];
				itemCam.enabled = true;
				itemCam.transform.position = editCam.transform.position;
				editCam.enabled = false;
			}
			else if (mode == AppState.Dig)
			{
				MainMenu.Instance.showDigButtons (true);
				laserAim.SetActive (true);
				_curEditorTool = _aEditorTools [Globals.EDITOR_TOOL_DIG];
			}
			else if (mode == AppState.Paint)
			{
				MainMenu.Instance.showMaterialBox (true);
				MainMenu.Instance.showDigButtons (true);
				laserAim.SetActive (true);
				_curEditorTool = _aEditorTools [Globals.EDITOR_TOOL_PAINT];
				_curEditorTool.setCurAimMaterial ();
			}
			else if (mode == AppState.Build)
			{
				MainMenu.Instance.showMaterialBox (true);
				MainMenu.Instance.showDigButtons (true);
				laserAim.SetActive (true);
				_curEditorTool = _aEditorTools [Globals.EDITOR_TOOL_BUILD];
				//_curEditorTool.setSingleMaterial (laserAim, _aMaterials[MainMenu.Instance.iSelectedMaterial], false);
				_curEditorTool.setCurAimMaterial ();
			}
			else if (mode == AppState.Props)
			{
				//MainMenu.Instance.showItemsBox (true);
				_curEditorTool = _aEditorTools [Globals.EDITOR_TOOL_ITEMS];
				laserAim.SetActive (true);
				itemAim.SetActive (true);
				if (_goCurItem == null) {
					newItemSelected (PropsManager.Instance.iSelectedItem);
				}
			}

			if (goPlayer != null && goPlayerEdit != null) {
				goPlayer.SetActive ((mode == AppState.Play));
				goPlayerEdit.SetActive (!goPlayer.activeSelf);
			}

			SceneGizmo.Instance.editorCameraTransform = (itemCam.enabled ? EditorCamera.Instance.transform : FlyCam.Instance.player);

			MainMenu.Instance.resetDigSettings (new Vector3 (1, 1, 1));
			updateDigSettings (MainMenu.Instance.v3DigSettings);

			if (_curEditorTool != null) {
				_curEditorTool.activate ();
			}
		}

		//
		public void addUndoAction (AppState action, GameObject go)
		{
			//Debug.Log ("addUndoAction " + action);
			undoAction undo = new undoAction();
			undo.action = action;
			if (go != null) {
				undo.go = go;
				undo.name = go.name;
				undo.position = go.transform.localPosition;
				undo.parent = go.transform.parent;
				if (go.GetComponent<Renderer> () != null) {
					undo.material = go.GetComponent<Renderer> ().sharedMaterial;
				}
			}

			// items
			undo.items = new List<undoItem> ();
			foreach (Transform child in goProps.transform) {
				undoItem item = new undoItem ();
				item.go = child.gameObject;
				item.position = child.position;
				item.rotation = child.rotation;
				undo.items.Add (item);
			}

			// save undo step
			_undoActions.Add(undo);
			MainMenu.Instance.setUndoButton (true);
		}

		//
		public void resetUndoActions()
		{
			undoAction undo;
			undoItem item;

			int i, len = _undoActions.Count;
			for (i = 0; i < len; ++i) {
				undo = _undoActions [i];
				undo.go = null;
				undo.parent = null;
				undo.material = null;

				// items
				int j, len2 = undo.items.Count;
				for (j = 0; j < len2; ++j) {
					item = undo.items[j];
					item.go = null;
					undo.items[j] = item;
				}
				undo.items.Clear ();

				_undoActions [i] = undo;
			}

			_undoActions.Clear ();
			MainMenu.Instance.setUndoButton (false);
		}

		//
		public void undoLastActions()
		{
			EditorObjectSelection.Instance.ClearSelection(false);

			int effectedCubes = 0;

			Shader shader = Shader.Find (Globals.defaultShaderName);
			Renderer renderer;

			undoAction undo;
			undoItem item;

			int i, len = _undoActions.Count;
			//Debug.Log ("undoLastActions " + len);
			for (i = 0; i < len; ++i)
			{
				undo = _undoActions [i];

				// DIG
				if (undo.action == AppState.Dig) {
					if (undo.parent != null) {
						World.Instance.createRock (undo.position, undo.parent.gameObject, undo.name, undo.material);
						undo.material.shader = shader;
						effectedCubes++;
					}
				}
				// BUILD
				else if (undo.action == AppState.Build) {
					if (undo.go != null) {
						Destroy (undo.go);
						effectedCubes--;
					}
				}
				// PAINT
				else if (undo.action == AppState.Paint) {
					if (undo.go != null) {
						undo.material.shader = shader;
						renderer = undo.go.GetComponent<Renderer> ();
						if (renderer != null) {
							renderer.sharedMaterial = undo.material;
						}
					}
				}
				// ITEM
				else if (undo.action == AppState.Props) {
					if (undo.go != null) {
						PropsManager.Instance.removeWorldProp (undo.go);
						Destroy (undo.go);
					}
				}

				undo.go = null;
				undo.parent = null;
				undo.material = null;

				// items
				int j, len2 = undo.items.Count;
				for (j = 0; j < len2; ++j) {
					item = undo.items[j];
					if (item.go != null) {
						item.go.transform.position = item.position;
						item.go.transform.rotation = item.rotation;
					}
					item.go = null;
				}
				undo.items.Clear ();

				_undoActions [i] = undo;
			}

			_undoActions.Clear ();
			MainMenu.Instance.setUndoButton (false);

			if (effectedCubes != 0) {
				World.Instance.numCubes += effectedCubes;
				MainMenu.Instance.setCubeCountText (World.Instance.numCubes);
			}
		}

		//
		public void setSelectedObjects(List<GameObject> selectedObjects = null) {

			int i, len = _selectedObjects.Count;
			for (i = 0; i < len; ++i) {
				if (_selectedObjects [i] != null) {
					if (_selectedObjects [i].GetComponent<Rigidbody> () != null) {
						_selectedObjects [i].GetComponent<Rigidbody> ().useGravity = true;
					}
					if (_selectedObjects [i].GetComponent<Collider> () != null) {
						_selectedObjects [i].GetComponent<Collider> ().enabled = true;
					}
				}
			}

			_selectedObjects.Clear ();

			if (selectedObjects != null) {
				
				_selectedObjects = selectedObjects;

				len = _selectedObjects.Count;
				for (i = 0; i < len; ++i) {
					if (_selectedObjects [i] != null) {
						if (_selectedObjects [i].GetComponent<Rigidbody> () != null) {
							_selectedObjects [i].GetComponent<Rigidbody> ().useGravity = false;
						}
						if (_selectedObjects [i].GetComponent<Collider> () != null) {
							_selectedObjects [i].GetComponent<Collider> ().enabled = false;
						}
					}
				}
			}
		}

		//
		public void updateDigSettings(Vector3 v3DigSettings)
		{
			float fScale = _fRockSize;
			if (AppController.Instance.appState == AppState.Build) {
				laserAim.transform.localScale = v3DigSettings * fScale;
				laserAimCenterCube.SetActive (true);
				fScale -= 0.01f;
				laserAimCenterCube.transform.localScale = new Vector3(fScale / laserAim.transform.localScale.x, fScale / laserAim.transform.localScale.y, fScale / laserAim.transform.localScale.z);
			} else {
				fScale *= 0.75f;
				laserAim.transform.localScale = v3DigSettings * fScale;
				laserAimCenterCube.SetActive (false);
			}
		}

		//
		public void newMaterialSelected (int iSelectedMaterial)
		{
			if (AppController.Instance.appState == AppState.Paint) {
				_curEditorTool.resetMaterial();
			}
			//else if (AppController.Instance.appState == AppState.Build) {
			//	_curEditorTool.setSingleMaterial (laserAim, _aMaterials [MainMenu.Instance.iSelectedMaterial], false);
			//}
		}

		//
		public void newItemSelected (int iSelectedItem)
		{
			if (AppController.Instance.appState == AppState.Null) {
				return;
			}
				
			if (itemAim != null) {
				if (_goCurItem != null) {
					Destroy (_goCurItem);
					_goCurItem = null;
				}
				propDef prop = PropsManager.Instance.getSelectedPropDef ();
				_goCurItem = _World.createProp (prop, Vector3.zero, prop.name , itemAim.transform, false, false);
				_goCurItem.transform.localPosition = Vector3.zero;
			}

			if (_curEditorTool != null) {
				_curEditorTool.resetItem ();
			}
		}

		//
		public void resetFlyCam()
		{
			FlyCam.Instance.reset ();
			//PlayerEditCollision.Instance.isColliding = false;
		}

		//
		public void toggleFlyCamOffset()
		{
			FlyCam.Instance.toggleOffset ();
		}

		//
		public List<GameObject> getOverlappingObjects(Vector3 v3Pos, Vector3 extents)
		{
			List<GameObject> listCollidingObjects = new List<GameObject>();

			int i, len;

			//Vector3 pos = MainMenu.Instance.v3DigSettings * (_fRockSize * .75f) * .5f;
			Collider[] hitColliders = Physics.OverlapBox (v3Pos, extents);

			len = hitColliders.Length;
			for (i = 0; i < len; ++i) {
				if (hitColliders [i].tag == "DigAndDestroy") {
					listCollidingObjects.Add (hitColliders [i].gameObject);
				}
			}

			return listCollidingObjects;
		}

		//
		public List<Vector3> getAdjacentCubes(Vector3 v3CubePos) {

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

		#endregion
	}
}