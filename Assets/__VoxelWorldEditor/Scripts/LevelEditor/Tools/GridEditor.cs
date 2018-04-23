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

		public Button butToggleGrid;
		public Button butCreateRandom;

		public Button butResetGrid;
		public Button butBackToGrid;

		public GameObject gridContainer;
		public VoxelsLevelChunk voxelsLevelChunk;
		public GameObject voxelChunkContainer;

		public Material matStart;
		public Material matOpaque;
		public Material matTransparent;

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
		};

		private int _numCubesPerAxis;

		private GridCube[,,] _aGridCubes;

		private Vector3 _v3StartPos;
		private Vector3 _v3CurPos;

		private Vector3 _v3MouseDown;

		private bool _gridIsOn;
		private bool _randomGridHasBeenCreated;

		#region Getters

		/*public Vector3 v3CurPos {
			get { return _v3CurPos; }
		}*/

		#endregion

		// ---------------------------------------------------------------------------------------------
		// Init shit
		// ---------------------------------------------------------------------------------------------
		void Awake ()
		{
			_numCubesPerAxis = Mathf.CeilToInt((float)gridDimension / cubeScale);

			voxelsLevelChunk.init (voxelChunkContainer, true); // true = _isExperimentalChunk
			voxelChunkContainer.SetActive (false);

			gameObject.SetActive (false);
		}

		// ---------------------------------------------------------------------------------------------
		public void activate(bool state)
		{
			resetAll ();

			//camMain.gameObject.SetActive (!state);

			if (!state) {
				gameObject.SetActive (false);
				return;
			}

			gameObject.SetActive (true);

			txtHelp.text  = "Set player starting position with keys a, d, w, s, q, e";
			txtCount.text = "Use left mouse button to rotate grid";
			txtError.text = "";

			_v3CurPos   = new Vector3 (_numCubesPerAxis - 2, _numCubesPerAxis - 2, 1);
			_v3StartPos = new Vector3 (-1, -1, -1);

			_v3MouseDown = Vector3.zero;

			_gridIsOn = true;
			_randomGridHasBeenCreated = false;

			_goPlayer = Instantiate (prefabPlayer);
			_goPlayer.name = "Player";
			_goPlayer.transform.SetParent (gridContainer.transform);
			_goPlayer.transform.localScale = new Vector3 (cubeScale, cubeScale, cubeScale) * 1.01f;

			movePos (0, 0, 0); // force player positioning

			createCubeGrid ();

			setContainersAndUI ();
		}

		// ---------------------------------------------------------------------------------------------
		public Vector3 getTranslatedPlayerPos()
		{
			Vector3 pos = Vector3.zero;

			pos.x = _v3CurPos.x * cubeScale + (cubeScale * 0.5f);
			pos.y = _v3CurPos.y * cubeScale + (cubeScale * 0.5f);
			pos.z = _v3CurPos.z * cubeScale + (cubeScale * 0.5f);

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
			voxelsLevelChunk.newLevel ();

			_aGridCubes = new GridCube[_numCubesPerAxis, _numCubesPerAxis, _numCubesPerAxis];

			foreach (Transform child in gridContainer.transform) {
				Destroy (child.gameObject);
			}
		}

		// ---------------------------------------------------------------------------------------------
		// Update shit
		// ---------------------------------------------------------------------------------------------
		void Update () {

			if (Input.GetMouseButtonDown (0))
			{
				if (!EventSystem.current.IsPointerOverGameObject ()) {
					_v3MouseDown = Input.mousePosition;
				}
			}

			if (Input.GetMouseButton (0)) {

				if (!EventSystem.current.IsPointerOverGameObject ())
				{
					float yRot = _v3MouseDown.x - Input.mousePosition.x;
					Vector3 newRot = gridContainer.transform.eulerAngles;
					newRot.y += yRot * (360f / (float)Screen.width);
					gridContainer.transform.eulerAngles = newRot;

					_v3MouseDown.x = Input.mousePosition.x;
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

			butToggleGrid.gameObject.SetActive (true);
			butCreateRandom.gameObject.SetActive (true);

			butResetGrid.gameObject.SetActive (_randomGridHasBeenCreated);

			butBackToGrid.gameObject.SetActive (true);
		}

		// ---------------------------------------------------------------------------------------------
		private void movePos (int x, int y, int z)
		{
			//Debug.Log ("movePos " + x + ", " + y + ", " + z);

			//bool moved = false;
			int newPos;

			if (x != 0)
			{
				newPos = (int)_v3CurPos.x + x;
				if (newPos > 0 && newPos < (_numCubesPerAxis - 1)) {
					_v3CurPos.x = newPos;
					//moved = true;
				}
			}
			else if (y != 0)
			{
				newPos = (int)_v3CurPos.y + y;
				if (newPos > 0 && newPos < (_numCubesPerAxis - 1)) {
					_v3CurPos.y = newPos;
					//moved = true;
				}
			}
			else if (z != 0)
			{
				newPos = (int)_v3CurPos.z + z;
				if (newPos > 0 && newPos < (_numCubesPerAxis - 1)) {
					_v3CurPos.z = newPos;
					//moved = true;
				}
			}

			float half = (float)gridDimension * 0.5f;
			_goPlayer.transform.localPosition = new Vector3 (-half + _v3CurPos.x * cubeScale, -half + _v3CurPos.y * cubeScale, -half + _v3CurPos.z * cubeScale);

			/*if (moved)
			{
				GridCube gc = _aGridCubes [(int)_v3CurPos.x, (int)_v3CurPos.y, (int)_v3CurPos.z];
				bool lastSet = gc.isSet;
				gc.isSet = (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl));

				if (gc.isSet != lastSet) {
					_aGridCubes [(int)_v3CurPos.x, (int)_v3CurPos.y, (int)_v3CurPos.z] = gc;
					setMaterial (gc);
				}
			}*/
		}

		// ---------------------------------------------------------------------------------------------
		private void createCubeGrid()
		{
			// center gridCubes container
			float half = (float)gridDimension * 0.5f;
			//gridContainer.transform.position = new Vector3 (-half, -half, -half);

			// create grid of cubes
			int endCorner = _numCubesPerAxis - 1;
			int x, y, z;
			for (x = 0; x < _numCubesPerAxis; ++x) {
				for (y = 0; y < _numCubesPerAxis; ++y) {
					for (z = 0; z < _numCubesPerAxis; ++z) {

						GridCube gc = new GridCube ();
						gc.go = Instantiate (prefabCube);
						gc.go.name = "x" + x.ToString () + "-" + "y" + y.ToString () + "-" + "z" + z.ToString ();
						gc.go.transform.SetParent (gridContainer.transform);
						gc.go.transform.localScale    = new Vector3 (cubeScale, cubeScale, cubeScale);
						gc.go.transform.localPosition = new Vector3 (-half + x * cubeScale, -half + y * cubeScale, -half + z * cubeScale);

						gc.x = x;
						gc.y = y;
						gc.z = z;

						if (x == (int)_v3StartPos.x && y == (int)_v3StartPos.y && z == (int)_v3StartPos.z) {
							gc.isSet = true;
						} else {
							gc.isSet = false;
						}

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

						setMaterial (gc);
					}
				}
			}
		}

		// ---------------------------------------------------------------------------------------------
		private void setMaterial(GridCube gc)
		{
			if (gc.go != null) {
				Renderer r = gc.go.GetComponent<Renderer> ();
				if (r != null) {
					r.sharedMaterial = (gc.isSet ? matOpaque : matTransparent);
					/*if (gc.x == (int)_v3StartPos.x && gc.y == (int)_v3StartPos.y && gc.z == (int)_v3StartPos.z) {
						r.sharedMaterial = matStart;
					} else {
						r.sharedMaterial = (gc.isSet ? matOpaque : matTransparent);
					}*/
				}
			}
		}

		// ---------------------------------------------------------------------------------------------
		public float[] convertChunksToVoxels(int numChunks, int newSize)
		{
			Debug.Log ("numChunks: " + numChunks + ", newSize: " + newSize);

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
		private void toggleGrid()
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
		}

		// ---------------------------------------------------------------------------------------------
		private void createRandomLevel()
		{
			resetGrid ();

			_randomGridHasBeenCreated = true;

			Random.InitState ((int)(Time.time * 10f));

			int minCells = _numCubesPerAxis * 4;
			int maxCells = Random.Range(minCells, (minCells * 8));
			int numCells = 0;
			int len = 0, dir = -1;

			_v3StartPos   = _v3CurPos;
			Vector3 v3Pos = _v3CurPos;
			Vector3 v3Dir = Vector3.zero;

			voxelsLevelChunk.subtractChunk (new Vector3 ((int)(v3Pos.x * 4), (int)(v3Pos.y * 4), (int)(v3Pos.z * 4)), new Vector3 (4, 4, 4));

			int[] aDirs = new int[]{0,1,2,2,3,3,4,5};
			int lenDirs = aDirs.Length;

			bool lastMoveWasY = false;
			GridCube gc;
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
					len = Random.Range (1, 3);
					lastMoveWasY = true;
				} else {
					len = Random.Range (1, (int)(_numCubesPerAxis / 2));
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

						gc = _aGridCubes [(int)v3Pos.x, (int)v3Pos.y, (int)v3Pos.z];
						if (!gc.isSet && gc.go != null) {
							
							gc.isSet = true;
							gc.go.SetActive(true);
							_aGridCubes [(int)v3Pos.x, (int)v3Pos.y, (int)v3Pos.z] = gc;
							setMaterial (gc);

							numCells++;

							voxelsLevelChunk.subtractChunk (new Vector3 ((int)(v3Pos.x * 4), (int)(v3Pos.y * 4), (int)(v3Pos.z * 4)), new Vector3 (4, 4, 4));
						}
					}
				}
			}
		}

		// ---------------------------------------------------------------------------------------------
		private void resetGrid()
		{
			_randomGridHasBeenCreated = false;

			_v3StartPos = _v3CurPos;

			GridCube gc;
			int x, y, z;
			for (x = 0; x < _numCubesPerAxis; ++x) {
				for (y = 0; y < _numCubesPerAxis; ++y) {
					for (z = 0; z < _numCubesPerAxis; ++z) {

						gc = _aGridCubes [x, y, z];
						if (x == (int)_v3StartPos.x && y == (int)_v3StartPos.y && z == (int)_v3StartPos.z) {
							gc.isSet = true;
						} else {
							gc.isSet = false;
						}
						_aGridCubes [x, y, z] = gc;

						if (gc.go != null) {
							setMaterial (gc);
							gc.go.SetActive (gc.isCorner || gc.isSet || _gridIsOn);
						}
					}
				}
			}
		}

		// ---------------------------------------------------------------------------------------------
		// handlers
		// ---------------------------------------------------------------------------------------------
		public void onToggleGridClick()
		{
			_gridIsOn = !_gridIsOn;

			toggleGrid ();
		}

		// ---------------------------------------------------------------------------------------------
		public void onCreateRandomClick()
		{
			//float timer = Time.realtimeSinceStartup;
			createRandomLevel ();
			//Debug.Log ("Time to create random level: " + (Time.realtimeSinceStartup - timer).ToString ("F2"));

			setContainersAndUI ();
		}

		// ---------------------------------------------------------------------------------------------
		public void onResetGridClick()
		{
			resetGrid ();
			setContainersAndUI ();
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
	}
}