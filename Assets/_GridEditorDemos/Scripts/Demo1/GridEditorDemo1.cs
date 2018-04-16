//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GridEditor
{
	public class GridEditorDemo1 : MonoBehaviour
	{
		public Camera camMain;

		public GameObject prefabCube;
		public GameObject prefabPlayer;
		public GameObject prefabPlayerPlay;
		public GameObject prefabVoxelChunk;

		public Text txtHelp;
		public Text txtCount;
		public Text txtError;

		public Button butToggleGrid;
		public Button butCreateRandom;

		public Button butResetGrid;
		public Button butCreateMeshes;
		public Button butBackToGrid;

		public Transform gridContainer;
		public Transform meshContainer;
		public Transform voxelChunkContainer;

		public Material matStart;
		public Material matOpaque;
		public Material matTransparent;

		public int gridDimension;
		public float cubeScale;

		//

		private VoxelChunkManager _VoxelChunkManager;

		private int _editMode;

		private GameObject _goPlayer;
		private GameObject _goPlayerPlay;

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

		private ConvertLevelToMesh _ConvertLevelToMesh;


		// ---------------------------------------------------------------------------------------------
		// Init shit
		// ---------------------------------------------------------------------------------------------
		void Awake () {

			_editMode = -1;
		}

		void Start () {

			txtHelp.text  = "Use the a/d, w/s and q/e to move player";
			txtCount.text = "Use left mouse button to rotate grid";
			txtError.text = "";

			_VoxelChunkManager = new VoxelChunkManager ();
			_VoxelChunkManager.init (prefabVoxelChunk, voxelChunkContainer);

			_numCubesPerAxis = Mathf.CeilToInt((float)gridDimension / cubeScale);

			_aGridCubes = new GridCube[_numCubesPerAxis, _numCubesPerAxis, _numCubesPerAxis];

			_v3CurPos   = new Vector3 (_numCubesPerAxis - 2, _numCubesPerAxis - 2, 1);
			_v3StartPos = new Vector3 (-1, -1, -1);

			_v3MouseDown = Vector3.zero;

			_gridIsOn = true;
			_randomGridHasBeenCreated = false;

			_goPlayer = Instantiate (prefabPlayer);
			_goPlayer.name = "Player";
			_goPlayer.transform.SetParent (gridContainer);
			_goPlayer.transform.localScale = new Vector3 (cubeScale, cubeScale, cubeScale) * 1.01f;

			movePos (0, 0, 0); // force player positioning

			_goPlayerPlay = Instantiate (prefabPlayerPlay);
			_goPlayerPlay.name = "PlayerPlay";
			_goPlayerPlay.transform.SetParent (meshContainer);
			_goPlayerPlay.SetActive (false);

			createCubeGrid ();

			setEditMode (0);
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

				/*if (Input.GetKeyDown (KeyCode.Space)) {
					
					GridCube gc = _aGridCubes[(int)_v3CurPos.x, (int)_v3CurPos.y, (int)_v3CurPos.z];
					gc.isSet = (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.LeftShift));
					_aGridCubes [(int)_v3CurPos.x, (int)_v3CurPos.y, (int)_v3CurPos.z] = gc;

					setMaterial (gc);
				}*/
			}
		}

		// ---------------------------------------------------------------------------------------------
		//
		// ---------------------------------------------------------------------------------------------
		private void setEditMode(int mode)
		{
			if (mode == _editMode) {
				return;
			}

			_editMode = mode;

			camMain.gameObject.SetActive (_editMode == 0);
			_goPlayerPlay.SetActive (!camMain.gameObject.activeSelf);
			_goPlayerPlay.transform.localPosition = _goPlayer.transform.localPosition;

			setContainersAndUI ();
		}

		// ---------------------------------------------------------------------------------------------
		private void setContainersAndUI ()
		{
			gridContainer.gameObject.SetActive (_editMode == 0);
			meshContainer.gameObject.SetActive (!gridContainer.gameObject.activeSelf);

			butToggleGrid.gameObject.SetActive (_editMode == 0);
			butCreateRandom.gameObject.SetActive (_editMode == 0);

			butResetGrid.gameObject.SetActive (_editMode == 0 && _randomGridHasBeenCreated);
			butCreateMeshes.gameObject.SetActive (_editMode == 0 && _randomGridHasBeenCreated);

			butBackToGrid.gameObject.SetActive (_editMode == 1);
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
						gc.go.transform.SetParent (gridContainer);
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
		// marching cubes
		// ---------------------------------------------------------------------------------------------
		private void createMeshes()
		{
			_ConvertLevelToMesh = meshContainer.gameObject.GetComponent<ConvertLevelToMesh> ();
			if (_ConvertLevelToMesh == null) {
				_ConvertLevelToMesh = meshContainer.gameObject.AddComponent<ConvertLevelToMesh> ();
			}

			_ConvertLevelToMesh.m_material = Resources.Load<Material> ("Materials/matGridVolcano");

			float timer = Time.realtimeSinceStartup;

			float chunkSize = 0.5f;
			int numChunks = (int)(cubeScale / chunkSize);
			int newSize = numChunks * _numCubesPerAxis;

			float[] voxels = _VoxelChunkManager.convertChunksToVoxels (); // convertChunksToVoxels (numChunks, newSize);

			float half = (float)gridDimension * 0.5f;
			_ConvertLevelToMesh.create (newSize, newSize, newSize, voxels, new Vector3(-half, -half, -half));

			//txtError.text = "Time to run marching cubes: " + (Time.realtimeSinceStartup - timer).ToString ("F2");
			Debug.Log("Time to run marching cubes: " + (Time.realtimeSinceStartup - timer).ToString ("F2"));
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

			_VoxelChunkManager.subtractChunk (new Vector3 ((int)(v3Pos.x * 4), (int)(v3Pos.y * 4), (int)(v3Pos.z * 4)), new Vector3 (4, 4, 4));

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

							_VoxelChunkManager.subtractChunk (new Vector3 ((int)(v3Pos.x * 4), (int)(v3Pos.y * 4), (int)(v3Pos.z * 4)), new Vector3 (4, 4, 4));
						}
					}
				}
			}
		}

		// ---------------------------------------------------------------------------------------------
		private void resetGrid()
		{
			_VoxelChunkManager.reset ();

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
			if (_editMode != 0) {
				return;
			}

			_gridIsOn = !_gridIsOn;

			toggleGrid ();
		}

		// ---------------------------------------------------------------------------------------------
		public void onCreateRandomClick()
		{
			if (_editMode != 0) {
				return;
			}

			float timer = Time.realtimeSinceStartup;
			createRandomLevel ();
			Debug.Log ("Time to create random level: " + (Time.realtimeSinceStartup - timer).ToString ("F2"));

			setContainersAndUI ();
		}

		// ---------------------------------------------------------------------------------------------
		public void onResetGridClick()
		{
			if (_editMode != 0) {
				return;
			}

			resetGrid ();
			setContainersAndUI ();
		}

		// ---------------------------------------------------------------------------------------------
		public void onCreateMeshesClick()
		{
			if (_editMode != 0) {
				return;
			}

			createMeshes ();
			setEditMode (1);
		}

		// ---------------------------------------------------------------------------------------------
		public void onBackToGridClick()
		{
			if (_editMode != 1) {
				return;
			}

			setEditMode (0);
		}
	}
}