
//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DragginzVoxelWorldEditor
{
	public class GridEditor : MonoBehaviour
	{
		//public Camera camMain;

		public GameObject prefabCube;
		public GameObject prefabPlayer;
		public GameObject prefabVoxelChunk;

		public Text txtHelp;
		public Text txtCount;
		public Text txtError;

		public Button butResetRotation;
		public Button butCreateRandom;
		public Button butResetGrid;
		public Button butSetPath;
		public Button butErasePath;
		public Button butExit;
		public Button butSaveAndExit;

		public GameObject gridContainer;
		public VoxelsLevelChunk voxelsLevelChunk;
		public GameObject voxelChunkContainer;

		public Material matStart;
		public Material matOpaque;
		public Material matTransparent;
		public Material matPath;

		private int gridDimension = 36;
		private float cubeScale   =  2f;

		//

		private GameObject _goPlayer;

		struct GridCube {
			public GameObject go;
			public int x;
			public int y;
			public int z;
			public bool isSet;
			public bool isCorner;
			public bool isPath;
			public Vector3Int prevCell;
		};

		private int _numCubesPerAxis;

		private GridCube[,,] _aGridCubes;

		private List<Vector3Int> _aPath;

		private Vector3 _v3StartPos;
		private Vector3 _v3CurPos;

		//private Vector3 _v3MouseDown;

		//private bool _gridIsOn;
		private bool _randomGridHasBeenCreated;
		private bool _gridHasBeenUpdated;

		private bool _threadRunning;
		private bool _threadComplete;

		// ---------------------------------------------------------------------------------------------
		// Init shit
		// ---------------------------------------------------------------------------------------------
		void Awake ()
		{
			_numCubesPerAxis = Mathf.CeilToInt((float)gridDimension / cubeScale);

			voxelsLevelChunk.init (voxelChunkContainer, true); // true = _isExperimentalChunk
			voxelChunkContainer.SetActive (false);

			_aPath = new List<Vector3Int> ();

			_threadRunning = false;
			_threadComplete = true;

			gameObject.SetActive (false);
		}

		// ---------------------------------------------------------------------------------------------
		public void activate(bool state)
		{
			if (!state) {
				gameObject.SetActive (false);
				return;
			}

			_v3CurPos   = new Vector3 (_numCubesPerAxis / 2, _numCubesPerAxis / 2, _numCubesPerAxis / 2);
			_v3StartPos = new Vector3 (_v3CurPos.x, _v3CurPos.y, _v3CurPos.z);

			resetAll ();
			gameObject.SetActive (true);

			txtHelp.text  = "left mouse = rotate grid   -   A, D, W, S, Q and E = create tunnel path";
			txtCount.text = "";
			txtError.text = "";

			//_v3MouseDown = Vector3.zero;

			//_gridIsOn = true;
			_randomGridHasBeenCreated = false;
			_gridHasBeenUpdated = false;

			_goPlayer = Instantiate (prefabPlayer);
			_goPlayer.name = "Player";
			_goPlayer.transform.SetParent (gridContainer.transform);
			_goPlayer.transform.localScale = new Vector3 (cubeScale, cubeScale, cubeScale) * 1.01f;
			_goPlayer.transform.localRotation = Quaternion.identity;

			setPlayerPos ();
			setPathElement ((int)_v3CurPos.x, (int)_v3CurPos.y, (int)_v3CurPos.z);

			createCubeGrid ();

			setContainersAndUI ();
		}

		// ---------------------------------------------------------------------------------------------
		public Vector3 getTranslatedPlayerPos()
		{
			Vector3 pos = Vector3.zero;

			pos.x = _v3StartPos.x * cubeScale + (cubeScale * 0.5f);
			pos.y = _v3StartPos.y * cubeScale + (cubeScale * 0.5f);
			pos.z = _v3StartPos.z * cubeScale + (cubeScale * 0.5f);

			return pos;
		}

		// ---------------------------------------------------------------------------------------------
		public List<VoxelUtils.VoxelChunk> getVoxelsData()
		{
			return voxelsLevelChunk.aVoxelChunks;
		}

		// ---------------------------------------------------------------------------------------------
		private void resetAll()
		{
			float multiply = cubeScale / VoxelUtils.CHUNK_SIZE;
			voxelsLevelChunk.newLevel ();
			voxelsLevelChunk.subtractChunk (new Vector3 ((int)(_v3StartPos.x * multiply), (int)(_v3StartPos.y * multiply), (int)(_v3StartPos.z * multiply)), new Vector3 (multiply, multiply, multiply));

			_aGridCubes = new GridCube[_numCubesPerAxis, _numCubesPerAxis, _numCubesPerAxis];

			_aPath.Clear ();

			foreach (Transform child in gridContainer.transform) {
				Destroy (child.gameObject);
			}
		}

		// ---------------------------------------------------------------------------------------------
		// Update shit
		// ---------------------------------------------------------------------------------------------
		void Update () {

			if (_threadRunning)
			{
				if (_threadComplete){
					_threadRunning = false;
					AppController.Instance.hidePopup ();
					setContainersAndUI ();
				}

				return;
			}

			/*if (Input.GetMouseButtonDown (0))
			{
				if (!EventSystem.current.IsPointerOverGameObject ()) {
					_v3MouseDown = Input.mousePosition;
				}
			}*/

			if (Input.GetMouseButton (0)) {

				if (!EventSystem.current.IsPointerOverGameObject ())
				{
					float rotX = Input.GetAxis("Mouse X") * 50f * Mathf.Deg2Rad;
					float rotY = Input.GetAxis("Mouse Y") * 50f * Mathf.Deg2Rad;

					gridContainer.transform.Rotate(Vector3.up, -rotX);
					gridContainer.transform.Rotate(Vector3.right, rotY);

					/*float xRot = _v3MouseDown.y - Input.mousePosition.y;
					float yRot = _v3MouseDown.x - Input.mousePosition.x;
					Vector3 newRot = gridContainer.transform.eulerAngles;
					newRot.y += yRot * (360f / (float)Screen.width);
					newRot.x += xRot * (360f / (float)Screen.height);
					gridContainer.transform.eulerAngles = newRot;

					_v3MouseDown.x = Input.mousePosition.x;*/
				}
			}

			if (Input.anyKeyDown)
			{
				if (Input.GetKeyDown (KeyCode.A)) {
					movePos (-1, 0, 0);
				}
				else if (Input.GetKeyDown (KeyCode.D)) {
					movePos (1, 0, 0);
				}
				else if (Input.GetKeyDown (KeyCode.W)) {
					movePos (0, 0, 1);
				}
				else if (Input.GetKeyDown (KeyCode.S)) {
					movePos (0, 0, -1);
				}
				else if (Input.GetKeyDown (KeyCode.Q)) {
					movePos (0, 1, 0);
				}
				else if (Input.GetKeyDown (KeyCode.E)) {
					movePos (0, -1, 0);
				}
			}
		}

		// ---------------------------------------------------------------------------------------------
		private void setContainersAndUI ()
		{
			voxelsLevelChunk.trfmVoxels.gameObject.SetActive (true);

			if (_threadRunning) {
				butResetRotation.gameObject.SetActive (false);
				butCreateRandom.gameObject.SetActive (false);
				butResetGrid.gameObject.SetActive (false);
				butSetPath.gameObject.SetActive (false);
				butErasePath.gameObject.SetActive (false);
				butExit.gameObject.SetActive (false);
				butSaveAndExit.gameObject.SetActive (false);
			} else {
				butResetRotation.gameObject.SetActive (true);
				butCreateRandom.gameObject.SetActive (true);
				butResetGrid.gameObject.SetActive (_randomGridHasBeenCreated || _gridHasBeenUpdated || _aPath.Count > 0);
				butSetPath.gameObject.SetActive (_aPath.Count > 0);
				butErasePath.gameObject.SetActive (_aPath.Count > 0);
				butExit.gameObject.SetActive (true);
				butSaveAndExit.gameObject.SetActive (true);
			}
		}

		// ---------------------------------------------------------------------------------------------
		private void movePos (int x, int y, int z)
		{
			Vector3Int v3LastPos = new Vector3Int ((int)_v3CurPos.x, (int)_v3CurPos.y, (int)_v3CurPos.z);
			GridCube gcLast = _aGridCubes [v3LastPos.x, v3LastPos.y, v3LastPos.z];

			bool moved = false;
			int newPos;

			if (x != 0)
			{
				newPos = (int)_v3CurPos.x + x;
				if (newPos > 0 && newPos < (_numCubesPerAxis - 1)) {
					_v3CurPos.x = newPos;
					moved = true;
				}
			}
			else if (y != 0)
			{
				newPos = (int)_v3CurPos.y + y;
				if (newPos > 0 && newPos < (_numCubesPerAxis - 1)) {
					_v3CurPos.y = newPos;
					moved = true;
				}
			}
			else if (z != 0)
			{
				newPos = (int)_v3CurPos.z + z;
				if (newPos > 0 && newPos < (_numCubesPerAxis - 1)) {
					_v3CurPos.z = newPos;
					moved = true;
				}
			}

			if (moved)
			{
				setPlayerPos ();

				// did we turn around?
				Vector3Int v3NewPos = new Vector3Int ((int)_v3CurPos.x, (int)_v3CurPos.y, (int)_v3CurPos.z);
				GridCube gcNew = _aGridCubes [v3NewPos.x, v3NewPos.y, v3NewPos.z];

				if (gcLast.prevCell.x == v3NewPos.x && gcLast.prevCell.y == v3NewPos.y && gcLast.prevCell.z == v3NewPos.z)
				{
					gcLast.prevCell = new Vector3Int (-1, -1, -1);
					_aGridCubes [v3LastPos.x, v3LastPos.y, v3LastPos.z] = gcLast;

					//Debug.Log ("remove");
					removePathElement (v3LastPos.x, v3LastPos.y, v3LastPos.z);
				}
				else if (gcNew.prevCell.x == -1 && gcNew.prevCell.y == -1 && gcNew.prevCell.z == -1)
				{
					if (_v3StartPos.x != _v3CurPos.x || _v3StartPos.y != _v3CurPos.y || _v3StartPos.z != _v3CurPos.z)
					{
						gcNew.prevCell = v3LastPos;
						_aGridCubes [v3NewPos.x, v3NewPos.y, v3NewPos.z] = gcNew;

						//Debug.Log ("add");
						setPathElement (v3NewPos.x, v3NewPos.y, v3NewPos.z);
					}
				}

				setContainersAndUI ();
			}
		}

		//
		private void setPlayerPos()
		{
			float half = (float)gridDimension * 0.5f;
			_goPlayer.transform.localPosition = new Vector3 (-half + _v3CurPos.x * cubeScale, -half + _v3CurPos.y * cubeScale, -half + _v3CurPos.z * cubeScale);
		}

		// ---------------------------------------------------------------------------------------------
		private void removePathElement(int x, int y, int z)
		{
			int i, len = _aPath.Count;
			for (i = 0; i < len; ++i) {

				// remove element if it already exists
				if (_aPath [i].x == x && _aPath [i].y == y && _aPath [i].z == z) {

					_aPath.RemoveAt (i);

					GridCube gc = _aGridCubes [x, y, z];
					gc.isPath = false;
					if (gc.go != null) {
						Destroy (gc.go);
						gc.go = null;
					}
					_aGridCubes [x, y, z] = gc;

					len--;
					break;
				}
			}
		}

		//
		private void setPathElement(int x, int y, int z)
		{
			if (x == (int)_v3StartPos.x && y == (int)_v3StartPos.y && z == (int)_v3StartPos.z) {
				return;
			}

			_aPath.Add (new Vector3Int (x, y, z));

			GridCube gc = _aGridCubes [x, y, z];
			gc.isPath = true;
			_aGridCubes [x, y, z] = gc;
			createCubeGameObject (x, y, z);
			setMaterial (_aGridCubes [x, y, z]);
		}

		// ---------------------------------------------------------------------------------------------
		private void createCubeGrid()
		{
			//float half = (float)gridDimension * 0.5f;

			// create grid of cubes
			int endCorner = _numCubesPerAxis - 1;
			int x, y, z;
			for (x = 0; x < _numCubesPerAxis; ++x) {
				for (y = 0; y < _numCubesPerAxis; ++y) {
					for (z = 0; z < _numCubesPerAxis; ++z) {

						GridCube gc = new GridCube ();

						gc.x = x;
						gc.y = y;
						gc.z = z;

						gc.prevCell = new Vector3Int (-1, -1, -1);

						if (x == (int)_v3CurPos.x && y == (int)_v3CurPos.y && z == (int)_v3CurPos.z) {
							gc.isSet = true;
						} else {
							gc.isSet = false;
						}

						gc.isPath = false;
						gc.isCorner = false;

						if (x == 0 || x == endCorner)
						{
							if (y == 0 && z == 0) { // bottom left front
								gc.isCorner = true;
							}
							else if (y == endCorner && z == 0) { // top left front
								gc.isCorner = true;
							}
							else if (y == 0 && z == endCorner) { // bottom left back
								gc.isCorner = true;
							}
							else if (y == endCorner && z == endCorner) { // top left back
								gc.isCorner = true;
							}
						}

						_aGridCubes [x, y, z] = gc;

						if (gc.isCorner || gc.isSet) {
							createCubeGameObject (x, y, z);
							setMaterial (_aGridCubes [x, y, z]);
						}
					}
				}
			}
		}

		// ---------------------------------------------------------------------------------------------
		private void createCubeGameObject(int x, int y, int z)
		{
			if (_aGridCubes [x, y, z].go != null) {
				return;
			}

			float half = (float)gridDimension * 0.5f;

			GridCube gc = _aGridCubes [x, y, z];

			gc.go = Instantiate (prefabCube);
			gc.go.name = "x" + x.ToString () + "-" + "y" + y.ToString () + "-" + "z" + z.ToString ();
			gc.go.transform.SetParent (gridContainer.transform);
			gc.go.transform.localScale    = new Vector3 (cubeScale, cubeScale, cubeScale);
			gc.go.transform.localPosition = new Vector3 (-half + x * cubeScale, -half + y * cubeScale, -half + z * cubeScale);
			gc.go.transform.localRotation = Quaternion.identity;

			_aGridCubes [x, y, z] = gc;
		}

		// ---------------------------------------------------------------------------------------------
		private void setMaterial(GridCube gc)
		{
			if (gc.go != null) {
				Renderer r = gc.go.GetComponent<Renderer> ();
				if (r != null) {
					r.sharedMaterial = (gc.isSet ? matOpaque : (gc.isPath ? matPath : matTransparent));
				}
			}
		}

		// ---------------------------------------------------------------------------------------------
		public float[] convertChunksToVoxels(int numChunks, int newSize)
		{
			//Debug.Log ("numChunks: " + numChunks + ", newSize: " + newSize);

			float[] voxels = new float[newSize * newSize * newSize];
			int i, numVoxels = voxels.Length;
			for (i = 0; i < numVoxels; ++i) {
				voxels [i] = 0;
			}

			GridCube gc;
			int x, y, z;
			int a, b, c;
			int index;
			int size2 = newSize * newSize;

			for (x = 0; x < _numCubesPerAxis; ++x) {
				for (y = 0; y < _numCubesPerAxis; ++y) {
					for (z = 0; z < _numCubesPerAxis; ++z) {

						gc = _aGridCubes [x, y, z];

						if (!gc.isSet) {
							continue;
						}

						for (a = 0; a < numChunks; ++a) {
							for (b = 0; b < numChunks; ++b) {
								for (c = 0; c < numChunks; ++c) {

									index = (x*numChunks+a) + (y*numChunks+b)*newSize + (z*numChunks+c) * size2;

									if ((a > 0 && a < (numChunks-1))
									&&	(b > 0 && b < (numChunks-1))
									&&	(c > 0 && c < (numChunks-1)))
									{
										voxels [index] = -1;
									}
									else
									{
										voxels [index] = 1 - Random.value;
									}
								}
							}
						}

					}
				}
			}

			return voxels;
		}

		// ---------------------------------------------------------------------------------------------
		// misc methods
		// ---------------------------------------------------------------------------------------------
		/*private void toggleGrid()
		{
			GridCube gc;
			int x, y, z;
			//int endCorner = _numCubesPerAxis - 1;

			for (x = 0; x < _numCubesPerAxis; ++x) {
				for (y = 0; y < _numCubesPerAxis; ++y) {
					for (z = 0; z < _numCubesPerAxis; ++z) {

						gc = _aGridCubes [x, y, z];
						if (gc.go != null) {
							gc.go.SetActive (gc.isCorner || gc.isSet || _gridIsOn);
						}
					}
				}
			}
		}*/

		// ---------------------------------------------------------------------------------------------
		//
		// ---------------------------------------------------------------------------------------------
		private void setCurrentPath ()
		{
			_gridHasBeenUpdated = true;

			float multiply = cubeScale / VoxelUtils.CHUNK_SIZE;

			GridCube gc;
			int i, len = _aPath.Count;
			for (i = 0; i < len; ++i) {

				gc = _aGridCubes [_aPath [i].x, _aPath [i].y, _aPath [i].z];
				gc.isPath = false;

				if (!gc.isSet)
				{
					voxelsLevelChunk.subtractChunk (new Vector3 (_aPath [i].x * multiply, _aPath [i].y * multiply, _aPath [i].z * multiply), new Vector3 (multiply, multiply, multiply));

					gc.isSet = true;
					setMaterial (gc);
				}

				_aGridCubes [_aPath [i].x, _aPath [i].y, _aPath [i].z] = gc;
			}

			_aPath.Clear ();

			resetAllPrevPathData ();

			_v3CurPos = _v3StartPos;
			setPlayerPos ();
		}

		//
		private void eraseCurrentPath()
		{
			GridCube gc;
			int i, len = _aPath.Count;
			for (i = 0; i < len; ++i) {

				gc = _aGridCubes [_aPath [i].x, _aPath [i].y, _aPath [i].z];
				gc.isPath = false;

				if (gc.go != null)
				{
					Destroy (gc.go);
					gc.go = null;
				}

				_aGridCubes [_aPath [i].x, _aPath [i].y, _aPath [i].z] = gc;
			}

			_aPath.Clear ();

			resetAllPrevPathData ();

			_v3CurPos = _v3StartPos;
			setPlayerPos ();
		}

		// ---------------------------------------------------------------------------------------------
		private void resetAllPrevPathData()
		{
			GridCube gc;
			int x, y, z;
			for (x = 0; x < _numCubesPerAxis; ++x) {
				for (y = 0; y < _numCubesPerAxis; ++y) {
					for (z = 0; z < _numCubesPerAxis; ++z) {
						gc = _aGridCubes [x, y, z];
						gc.prevCell = new Vector3Int (-1, -1, -1);
						_aGridCubes [x, y, z] = gc;
					}
				}
			}
		}

		// ---------------------------------------------------------------------------------------------
		//
		// ---------------------------------------------------------------------------------------------
		private void createRandomLevel()
		{
			resetGrid ();

			_randomGridHasBeenCreated = true;

			Random.InitState ((int)(Time.time * 10f));

			_threadRunning = true;
			_threadComplete = false;

			setContainersAndUI ();

			StartCoroutine ("workerCreateRandomLevel");
		}

		// ---------------------------------------------------------------------------------------------
		IEnumerator workerCreateRandomLevel()
		{
			yield return new WaitForEndOfFrame ();

			int minCells = _numCubesPerAxis * 4;
			int maxCells = Random.Range(minCells, (minCells * 8));
			int numCells = 0;
			int len = 0, dir = -1;

			_v3StartPos = new Vector3 (_v3CurPos.x, _v3CurPos.y, _v3CurPos.z);

			Vector3 v3Pos = _v3CurPos;
			Vector3 v3Dir = Vector3.zero;

			float multiply = cubeScale / VoxelUtils.CHUNK_SIZE;

			voxelsLevelChunk.subtractChunk (new Vector3 ((int)(v3Pos.x * multiply), (int)(v3Pos.y * multiply), (int)(v3Pos.z * multiply)), new Vector3 (multiply, multiply, multiply));

			int[] aDirs = new int[]{0,1,2,3,2,3,2,3,4,5};
			int lenDirs = aDirs.Length;

			bool lastMoveWasY = false;
			GridCube gc;

			int threadLoopCounter = 0;
			while (numCells < maxCells) {

				// change direction?
				v3Dir = Vector3.zero;
				dir = aDirs[Random.Range(0, lenDirs)];

				if (dir == 0) {
					v3Dir.x = -1;
				}
				else if (dir == 1) {
					v3Dir.x = 1;
				}
				else if (dir == 2) {
					v3Dir.y = -1;
				}
				else if (dir == 3) {
					v3Dir.y = 1;
				}
				else if (dir == 4) {
					v3Dir.z = -1;
				}
				else {
					v3Dir.z = 1;
				}

				// avoid more than 1 cell in the y dir
				if (lastMoveWasY) {
					if (v3Dir.y != 0) {
						continue;
					} else {
						lastMoveWasY = false;
					}
				}

				if (v3Dir.y != 0) {
					len = Random.Range (1, 2);
					lastMoveWasY = true;
				} else {
					len = Random.Range (4, (int)(_numCubesPerAxis / 2));
				}

				bool moved;
				int i, newPos;
				for (i = 0; i < len; ++i) {

					moved = false;
					if (v3Dir.x != 0)
					{
						newPos = (int)(v3Pos.x + v3Dir.x);
						if (newPos > 0 && newPos < (_numCubesPerAxis - 1)) {
							v3Pos.x = newPos;
							moved = true;
						}
					}
					else if (v3Dir.y != 0)
					{
						newPos = (int)(v3Pos.y + v3Dir.y);
						if (newPos > 0 && newPos < (_numCubesPerAxis - 1)) {
							v3Pos.y = newPos;
							moved = true;
						}
					}
					else if (v3Dir.z != 0)
					{
						newPos = (int)(v3Pos.z + v3Dir.z);
						if (newPos > 0 && newPos < (_numCubesPerAxis - 1)) {
							v3Pos.z = newPos;
							moved = true;
						}
					}

					if (moved) {

						createCubeGameObject ((int)v3Pos.x, (int)v3Pos.y, (int)v3Pos.z);

						gc = _aGridCubes [(int)v3Pos.x, (int)v3Pos.y, (int)v3Pos.z];
						if (!gc.isSet && gc.go != null) {
							
							gc.isSet = true;
							gc.go.SetActive(true);
							_aGridCubes [(int)v3Pos.x, (int)v3Pos.y, (int)v3Pos.z] = gc;
							setMaterial (gc);

							numCells++;

							voxelsLevelChunk.subtractChunk (new Vector3 ((int)(v3Pos.x * multiply), (int)(v3Pos.y * multiply), (int)(v3Pos.z * multiply)), new Vector3 (multiply, multiply, multiply));

							MainMenu.Instance.popup.setOverlayText ("Creating random level\n"+((float)numCells / (float)maxCells * 100).ToString("F0")+"%");

							if (++threadLoopCounter >= 10) {
								threadLoopCounter = 0;
								yield return new WaitForEndOfFrame ();
							}
						}
					}
				}
			}

			_threadComplete = true;
		}

		// ---------------------------------------------------------------------------------------------
		/*private void setAllCubes()
		{
			int x, y, z;

			for (x = 0; x < _numCubesPerAxis; ++x) {
				for (y = 0; y < _numCubesPerAxis; ++y) {
					for (z = 0; z < _numCubesPerAxis; ++z) {

						if (_aGridCubes [x, y, z].isSet) {
							createCubeGameObject (x, y, z);
							setMaterial (_aGridCubes [x, y, z]);
						}
					}
				}
			}
		}*/

		// ---------------------------------------------------------------------------------------------
		//
		// ---------------------------------------------------------------------------------------------
		private void resetGrid()
		{
			_randomGridHasBeenCreated = false;
			_gridHasBeenUpdated = false;

			_v3StartPos = new Vector3 (_v3CurPos.x, _v3CurPos.y, _v3CurPos.z);

			float multiply = cubeScale / VoxelUtils.CHUNK_SIZE;
			voxelsLevelChunk.newLevel ();
			voxelsLevelChunk.subtractChunk (new Vector3 ((int)(_v3StartPos.x * multiply), (int)(_v3StartPos.y * multiply), (int)(_v3StartPos.z * multiply)), new Vector3 (multiply, multiply, multiply));

			GridCube gc;
			int x, y, z;
			for (x = 0; x < _numCubesPerAxis; ++x) {
				for (y = 0; y < _numCubesPerAxis; ++y) {
					for (z = 0; z < _numCubesPerAxis; ++z) {

						gc = _aGridCubes [x, y, z];

						gc.prevCell = new Vector3Int (-1, -1, -1);

						if (x == (int)_v3CurPos.x && y == (int)_v3CurPos.y && z == (int)_v3CurPos.z) {
							gc.isSet = true;
						} else {
							gc.isSet = false;
						}

						gc.isPath = false;

						if (gc.go != null) {
							if (!gc.isCorner && !gc.isSet) {
								Destroy (gc.go);
								gc.go = null;
							}
						}

						_aGridCubes [x, y, z] = gc;
					}
				}
			}

			_aPath.Clear();

			setPlayerPos ();
			setPathElement ((int)_v3CurPos.x, (int)_v3CurPos.y, (int)_v3CurPos.z);
		}

		// ---------------------------------------------------------------------------------------------
		// handlers
		// ---------------------------------------------------------------------------------------------
		public void onResetRotationClick()
		{
			gridContainer.transform.rotation = Quaternion.identity;

			//_gridIsOn = !_gridIsOn;
			//toggleGrid ();
		}

		// ---------------------------------------------------------------------------------------------
		public void onCreateRandomClick()
		{
			AppController.Instance.showPopup (PopupMode.Overlay, null, "Creating random level");

			createRandomLevel ();
		}

		// ---------------------------------------------------------------------------------------------
		public void onResetGridClick()
		{
			AppController.Instance.showPopup(PopupMode.Confirmation, "Reset Grid", "Are you sure?", onResetGridConfirmed);
		}

		private void onResetGridConfirmed(int buttonId)
		{
			AppController.Instance.hidePopup ();

			if (buttonId == 1)
			{
				resetGrid ();
				setContainersAndUI ();
			}
		}

		// ---------------------------------------------------------------------------------------------
		public void onExitClick()
		{
			LevelEditor.Instance.activateExperimentalGridEditor (false, false);
		}

		// ---------------------------------------------------------------------------------------------
		public void onExitAndSaveClick()
		{
			LevelEditor.Instance.activateExperimentalGridEditor (false, true);
		}

		// ---------------------------------------------------------------------------------------------
		public void onSetPathClick()
		{
			setCurrentPath ();
			setContainersAndUI ();
		}

		// ---------------------------------------------------------------------------------------------
		public void onErasePathClick()
		{
			eraseCurrentPath ();
			setContainersAndUI ();
		}
	}
}