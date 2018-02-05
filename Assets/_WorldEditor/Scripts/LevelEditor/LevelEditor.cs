﻿//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DragginzWorldEditor
{
	struct undoAction {
		public AppState action;
		public GameObject go;
		public string name;
		public Vector3 position;
		public Transform parent;
		public Material material;
	};

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

		private List<Material> _aMaterials;

		private List<undoAction> _undoActions;

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

		/*public List<undoAction> undoActions {
			get { return _undoActions; }
		}*/

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

			_undoActions = new List<undoAction> ();

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

			MainMenu.Instance.setUndoButton (false);

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
					_curEditorTool.resetAim ();
				}

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
				undo.material = go.GetComponent<Renderer> ().sharedMaterial;
			}

			_undoActions.Add(undo);
			MainMenu.Instance.setUndoButton (true);
		}

		//
		public void resetUndoActions()
		{
			undoAction undo;
			int i, len = _undoActions.Count;
			//Debug.Log ("resetUndoActions " + len);
			for (i = 0; i < len; ++i) {
				undo = _undoActions [i];
				undo.go = null;
				undo.parent = null;
				undo.material = null;
				_undoActions [i] = undo;
			}

			_undoActions.Clear ();
		}

		//
		public void undoLastActions()
		{
			int effectedCubes = 0;

			int i, len = _undoActions.Count;
			//Debug.Log ("undoLastActions " + len);
			for (i = 0; i < len; ++i)
			{
				undoAction undo = _undoActions [i];

				// DIG
				if (undo.action == AppState.Dig) {
					if (undo.parent != null) {
						GameObject go = World.Instance.createRock (undo.position, undo.parent.gameObject, undo.name);
						undo.material.shader = Shader.Find (Globals.defaultShaderName);
						go.GetComponent<MeshRenderer> ().material = undo.material;
						effectedCubes++;
					}
				}
				// PAINT
				else if (undo.action == AppState.Build) {
					if (undo.go != null) {
						Destroy (undo.go);
						effectedCubes--;
					}
				}

				undo.go = null;
				undo.parent = null;
				undo.material = null;
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

		//
		public void resetFlyCam()
		{
			FlyCam.Instance.reset ();
			PlayerEditCollision.Instance.isColliding = false;
		}

		//
		public void toggleFlyCamOffset()
		{
			FlyCam.Instance.toggleOffset ();
		}

		//
		public List<GameObject> getOverlappingObjects(Vector3 v3Pos)
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