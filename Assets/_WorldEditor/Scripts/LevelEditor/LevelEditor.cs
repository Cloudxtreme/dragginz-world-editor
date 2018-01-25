//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DragginzWorldEditor
{
	/// <summary>
	/// ...
	/// </summary>
	public class LevelEditor : MonoSingleton<LevelEditor>
	{
		public Camera mainCam;
		public GameObject goWorld;
		public GameObject goPlayer;
		public GameObject goPlayerEdit;

		public GameObject cubePrefab;

		public List<Material> materialsWalls;

		public Camera lineCamera;
		public LineRenderer laserPointer;
		public GameObject laserSphere;

		private Ray _ray;
		private RaycastHit _hit;
		private GameObject _goHit;

		private Dictionary<int, Dictionary<int, Dictionary<int, int>>> _levelCubeFlags;
		private int _iMinLevelCoord;
		private int _iMaxLevelCoord;

		private GameObject _goLastShaderChange;
		private List<GameObject> _aGoShaderChanged;

		private bool _mouseIsDown;

		#region SystemMethods

		/// <summary>
		/// System methods
		/// </summary>
		void Awake()
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			_goLastShaderChange = null;
			_aGoShaderChanged = new List<GameObject> ();

			_mouseIsDown = false;

			/*if (GetComponent<Light>() != null) {
				GetComponent<Light>().enabled = false; // turn off lights!
			}*/

			// Instantiate app controller singleton
			if (GameObject.Find(Globals.appContainerName) == null) {
				GameObject goAppController = new GameObject(Globals.appContainerName);
				DontDestroyOnLoad(goAppController);
				goAppController.AddComponent<AppController>();
			}

			_iMinLevelCoord = -50;
			_iMaxLevelCoord = 50;

			_goHit = null;

			_levelCubeFlags = new Dictionary<int, Dictionary<int, Dictionary<int, int>>> ();

			for (int x = _iMinLevelCoord; x <= _iMaxLevelCoord; ++x) {
				_levelCubeFlags.Add(x, new Dictionary<int, Dictionary<int, int>> ());
				for (int y = _iMinLevelCoord; y <= _iMaxLevelCoord; ++y) {
					_levelCubeFlags [x].Add(y, new Dictionary<int, int> ());
					for (int z = _iMinLevelCoord; z <= _iMaxLevelCoord; ++z) {
						_levelCubeFlags [x] [y].Add(z, 0);
					}
				}
			}
		}

		//
		void Start() {

			setMode (AppState.Null, true);

			createWorld ();

			setLaserSphereSize ();

			AppController.Instance.showPopup(
				PopupMode.Notification,
				"Controls",
				"Normal movement: AWSD\nUp and down: QE - rotate: ZC\nSlow walk: Mouse wheel\nAdjust movement speed: -/+\n\nPress ESC to reset speed and position.",
				startUpPopupCallback
			);
		}

		#endregion

		#region PublicMethods

		//
		public void customUpdateCheckControls() {

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
		public void customUpdateDig() {

			doRayCast ();
			if (_mouseIsDown && _goHit != null) {
				if (Screen.height - Input.mousePosition.y > 90) {
					digIt (_hit.point);
					_mouseIsDown = false;
				}
			}
		}

		public void customUpdatePaint() {
			
			doRayCast ();
			if (_mouseIsDown && _goHit != null) {
				if (Screen.height - Input.mousePosition.y > 90) {
					paintIt (_goHit);
				}
			}
		}

		//
		public void startUpPopupCallback(int buttonId) {

			AppController.Instance.hidePopup ();
			setMode (AppState.Dig);
		}

		public void setMode(AppState mode, bool forceMode = false) {

			if (forceMode || (mode != AppController.Instance.appState))
			{
				AppController.Instance.setAppState (mode);
				MainMenu.Instance.setModeButtons (mode);
				resetFlyCam ();

				if (mode == AppState.Dig) {
					MainMenu.Instance.showDigButtons (true);
					//MainMenu.Instance.setDigSizeButtons (_iCurDigSizeIndex);
					MainMenu.Instance.showMaterialBox (false);
				}
				else if (mode == AppState.Paint) {
					MainMenu.Instance.showDigButtons (false);
					MainMenu.Instance.showMaterialBox (true);
				}
				else {
					MainMenu.Instance.showDigButtons (false);
					MainMenu.Instance.showMaterialBox (false);
				}

				if (goPlayer != null && goPlayerEdit != null) {
					goPlayer.SetActive ((mode == AppState.Play));
					goPlayerEdit.SetActive (!goPlayer.activeSelf);
				}
			}
		}

		/*public void setDigSize(int size) {
			if (size != _iCurDigSizeIndex) {
				_iCurDigSizeIndex = size;
				MainMenu.Instance.setDigSizeButtons (_iCurDigSizeIndex);
				setLaserSphereSize ();
			}
		}*/

		public void updateDigSettings(Vector3 v3DigSettings) {
			
		}

		public void resetFlyCam()
		{
			mainCam.gameObject.GetComponent<FlyCam> ().reset ();
		}

		public void toggleFlyCamOffset()
		{
			mainCam.gameObject.GetComponent<FlyCam> ().toggleOffset ();
		}

		#endregion

		#region PrivateMethods

		private void doRayCast()
		{
			_ray = mainCam.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (_ray, out _hit, 500f)) {

				laserSphere.transform.position = _hit.point;
				_goHit = _hit.collider.gameObject;
				changeShader (_goHit, "Legacy Shaders/Transparent/Diffuse");
			}
			else {
				changeShader (_goLastShaderChange);
				laserSphere.transform.position = new Vector3(9999,9999,9999);
				_goHit = null;
			}
		}

		//
		private void changeShader(GameObject go, string shader = "Standard")
		{
			if (go == null) {
				return;
			}

			Renderer renderer;

			// reset current shaders
			if (_goLastShaderChange != null && go != _goLastShaderChange) {
				setShaders ("Standard");
				/*renderer = _goLastShaderChange.GetComponent<Renderer> ();
				if (renderer != null) {
					if (renderer.material.shader.name != "Standard") {
						renderer.material.shader = Shader.Find ("Standard");
					}
				}*/
				_goLastShaderChange = null;
				_aGoShaderChanged.Clear ();
			}

			// set new shaders
			renderer = go.GetComponent<Renderer> ();
			if (renderer != null) {
				if (renderer.material.shader.name != shader) {
					renderer.material.shader = Shader.Find (shader);
				}
				_goLastShaderChange = go;
				_aGoShaderChanged.Add (go);
			}
		}

		private void setShaders(string sShader)
		{
			Shader shader = Shader.Find (sShader);
			Renderer renderer;
			int i, len = _aGoShaderChanged.Count;
			for (i = 0; i < len; ++i) {
				if (_aGoShaderChanged [i] != null) {
					renderer = _aGoShaderChanged [i].GetComponent<Renderer> ();
					if (renderer != null && renderer.material.shader.name != sShader) {
						renderer.material.shader = shader;
					}
				}
			}
		}

		//
		private void setLaserSphereSize() {
			float fSphereSize = 0.05f;
			laserSphere.transform.localScale = new Vector3 (fSphereSize, fSphereSize, fSphereSize);
		}

		//
		private void resetWorld() {

			foreach (Transform child in goWorld.transform) {
				Destroy (child.gameObject);
			}

			goPlayer.transform.position = new Vector3(0, 0.6f, -0.75f);

			for (int x = _iMinLevelCoord; x <= _iMaxLevelCoord; ++x) {
				for (int y = _iMinLevelCoord; y <= _iMaxLevelCoord; ++y) {
					for (int z = _iMinLevelCoord; z <= _iMaxLevelCoord; ++z) {
						_levelCubeFlags [x] [y] [z] = 0;
					}
				}
			}
		}

		//
		private void createWorld() {

			resetWorld ();

			// create hollow cube of cubes :)
			int size = 2; // actual size will be size*2+1
			int height = 3;
			Vector3 pos = Vector3.zero;
			for (int x = -size; x <= size; ++x) {
				for (int y = -1; y <= height; ++y) {
					for (int z = -size; z <= size; ++z) {

						if (Mathf.Abs (x) == size || y == -1 || y == height || Mathf.Abs (z) == size) {
							createRockCube (new Vector3 (x, y, z));
						} else {
							_levelCubeFlags [x] [y] [z] = 1;
						}
					}
				}
			}
		}

		// - //
		/*private void swapShapes() {

			//GameObject go;
			string name;

			foreach (Transform cube in goWorld.transform) {

				List<Transform> rocks = new List<Transform>();
				foreach (Transform rock in cube) {
					rocks.Add(rock);
				}

				int count = 0;
				int len = rocks.Count;
				for (int i = 0; i < len; ++i) {
					if (rocks[i].name.IndexOf (Globals.rockGameObjectPrepend) == 0) {

						name = Globals.rockGameObjectPrepend + _iCurShapeType.ToString () + "-" + count.ToString ();
						createRock(rocks[i].localPosition, rocks[i].parent.gameObject, name);
						count++;

						Destroy (rocks[i].gameObject);
					}
				}
			}
		}*/

		//
		private void createRockCube (Vector3 v3CubePos)
		{
			// cube already created at that position
			if (_levelCubeFlags [(int)v3CubePos.x] [(int)v3CubePos.y] [(int)v3CubePos.z] == 1) {
				return;
			}

			GameObject cubeParent = new GameObject(Globals.containerGameObjectPrepend+v3CubePos.x.ToString()+"_"+v3CubePos.y.ToString()+"_"+v3CubePos.z.ToString());
			cubeParent.transform.SetParent(goWorld.transform);
			cubeParent.transform.localPosition = v3CubePos;

			_levelCubeFlags [(int)v3CubePos.x] [(int)v3CubePos.y] [(int)v3CubePos.z] = 1;
			//Debug.Log ("create cube " + cubeParent.name);

			Vector3 pos = Vector3.zero;
			int count = 0;

			float fRockSize = 0.25f; //prefabsSizes [_iCurShapeType][_iCurShapeSizeIndex];
			int len = Mathf.RoundToInt(1f / fRockSize);

			pos.x = -0.5f + (fRockSize * 0.5f);
			for (int x = 0; x < len; ++x) {
				pos.y = 0.5f - (fRockSize * 0.5f);
				for (int y = 0; y < len; ++y) {
					pos.z = -0.5f + (fRockSize * 0.5f);
					for (int z = 0; z < len; ++z) {
						createRock(pos, cubeParent, Globals.rockGameObjectPrepend + count.ToString()); //_iCurShapeType.ToString() + "-" +
						count++;
						pos.z += fRockSize;
					}
					pos.y -= fRockSize;
				}
				pos.x += fRockSize;
			}
		}

		/// <summary>
		/// ...
		/// </summary>
		private GameObject createRock(Vector3 pos, GameObject parent, string name) {

			GameObject go = null;
			GameObject prefab = null;
			Vector3 rotation = Vector3.zero;

			/*if (_iCurShapeType == 0) {
				prefab = cubePrefabs [_iCurShapeSizeIndex];
				//rotation = new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f));
			}
			else if (_iCurShapeType == 1) {
				prefab = spherePrefabs [_iCurShapeSizeIndex];
			}
			else if (_iCurShapeType == 2) {
				prefab = rockPrefabs [_iCurShapeSizeIndex];
				rotation = new Vector3(Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
			}*/

			prefab = cubePrefab;
			if (prefab) {
				go = GameObject.Instantiate(prefab);
				go.name = name;
				go.transform.SetParent(parent.transform);
				go.transform.localPosition = pos;

				go.transform.localRotation = Quaternion.Euler (rotation);
				go.GetComponent<MeshRenderer> ().material = materialsWalls[Random.Range (0, materialsWalls.Count)];
			}

			return go;
		}

		/// <summary>
		/// Destroy game objects within a 0.5f radius of the hit point
		/// </summary>
		private void digIt (Vector3 v3Pos)
		{
			int i, len;

			// keep track of parent objects that had children removed
			List<Transform> listcubeTransforms = new List<Transform> ();

			if (_goHit != null)
			{
				if (_goHit.tag == "DigAndDestroy") {
					listcubeTransforms.Add (_goHit.transform.parent);
					Destroy (_goHit);
					_goHit = null;
				}
			}
			else
			{
				/*float digSize = (float)(_iCurDigSizeIndex + 1) * prefabsSizes [_iCurShapeType] [_iCurShapeSizeIndex];
				Collider[] hitColliders = Physics.OverlapSphere (v3Pos, digSize * 0.5f);

				len = hitColliders.Length;
				for (i = 0; i < len; ++i) {
					if (hitColliders [i].tag == "DigAndDestroy") {

						if (!listcubeTransforms.Contains (hitColliders [i].transform.parent)) {
							listcubeTransforms.Add (hitColliders [i].transform.parent);
						}

						Destroy (hitColliders [i].gameObject);
					}
				}*/
			}

			// extend level if necessary
			len = listcubeTransforms.Count;
			for (i = 0; i < len; ++i) {

				List<Vector3> adjacentCubes = getAdjacentCubes (listcubeTransforms [i].position);

				int j, len2 = adjacentCubes.Count;
				for (j = 0; j < len2; ++j) {
					createRockCube (adjacentCubes [j]);
				}
			}
		}

		private List<Vector3> getAdjacentCubes(Vector3 v3CubePos) {

			List<Vector3> adjacentCubes = new List<Vector3> ();

			int len = 1;
			for (int x = -len; x <= len; ++x) {
				for (int y = -len; y <= len; ++y) {
					for (int z = -len; z <= len; ++z) {
						adjacentCubes.Add (new Vector3(v3CubePos.x+x, v3CubePos.y+y, v3CubePos.z+z));
					}
				}
			}

			return adjacentCubes;
		}

		/// <summary>
		/// Paints it.
		/// </summary>
		/// <param name="go">Go.</param>
		private void paintIt (GameObject go)
		{
			if (go == null) {
				return;
			}

			MeshRenderer renderer = go.GetComponent<MeshRenderer> ();
			if (renderer != null) {
				renderer.material = Resources.Load<Material> ("Materials/" + Globals.materials [MainMenu.Instance.iSelectedMaterial]);
			}
		}

		#endregion
	}
}