//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using AssetsShared;

namespace DragginzVoxelWorldEditor
{
	public class VoxelsLevelChunk : MonoSingleton<VoxelsLevelChunk>
	{
		private Transform _trfmVoxelChunkContainer;

		private Transform _trfmPlaceholder;
		private Transform _trfmVoxels;
		private Transform _trfmProps;

		private ConvertLevelToMesh _ConvertLevelToMesh;

		private int _iVoxelCount;

		private List<VoxelUtils.VoxelChunk> _aVoxelChunks;
		private List<VoxelUtils.VoxelChunk> _aVoxelChunksUndo;

		private Vector3 _lastChunkPos;

		private Dictionary<GameObject, worldProp> _worldProps;

		private Vector3 _chunkPos;
		//private Bounds _chunkBounds;

		private Vector3 _startPos;
		private Vector3 _startRotation;

		// Experimental
		private bool _isExperimentalChunk;
		private List<Globals.RailgunShape> _aRailgunShapes;
		private LevelMap _levelMap;

		#region Getters

		public Transform trfmVoxels {
			get { return _trfmVoxels; }
		}

		public Transform trfmProps {
			get { return _trfmProps; }
		}

		public Transform trfmPlaceholder {
			get { return _trfmPlaceholder; }
		}

		public Vector3 chunkPos {
			get { return _chunkPos; }
		}

		/*public Bounds chunkBounds {
			get { return _chunkBounds; }
		}*/

		public List<VoxelUtils.VoxelChunk> aVoxelChunks {
			get { return _aVoxelChunks; }
		}

		public Dictionary<GameObject, worldProp> worldProps {
			get { return _worldProps; }
		}

		// Experimental
		public List<Globals.RailgunShape> aRailgunShapes {
			get { return _aRailgunShapes; }
		}

		#endregion

		// ---------------------------------------------------------------------------------------------
		// Init shit
		// ---------------------------------------------------------------------------------------------
		public void init (GameObject goParent, bool isExperimental = false)
		{
			_trfmVoxelChunkContainer = goParent.transform;

			_chunkPos = _trfmVoxelChunkContainer.position;

			_trfmVoxels = _trfmVoxelChunkContainer.Find ("voxelsContainer");
			_trfmProps = _trfmVoxelChunkContainer.Find ("propsContainer");
			_trfmPlaceholder = _trfmVoxelChunkContainer.Find ("placeholderContainer");

			_aVoxelChunks = new List<VoxelUtils.VoxelChunk> ();
			_aVoxelChunksUndo = new List<VoxelUtils.VoxelChunk> ();

			_worldProps = new Dictionary<GameObject, worldProp> ();


			// Experimental

			_isExperimentalChunk = isExperimental;
			_levelMap = LevelMap.Instance;

			_aRailgunShapes = new List<Globals.RailgunShape> ();

			//
			// cross
			//
			Globals.RailgunShape shape = new Globals.RailgunShape (4, 4, 8, new List<Vector3>(), new List<Vector3>());
			shape.pos.Add (new Vector3 (0, 1, 0));
			shape.size.Add (new Vector3 (1, 2, 8));
			shape.pos.Add (new Vector3 (1, 0, 0));
			shape.size.Add (new Vector3 (2, 4, 8));
			shape.pos.Add (new Vector3 (3, 1, 0));
			shape.size.Add (new Vector3 (1, 2, 8));

			_aRailgunShapes.Add(shape);

			//
			// steps
			//
			shape = new Globals.RailgunShape (4, 4, 4, new List<Vector3>(), new List<Vector3>());
			shape.pos.Add (new Vector3 (0, 0, 0));
			shape.size.Add (new Vector3 (4, 4, 1));
			shape.pos.Add (new Vector3 (0, 1, 1));
			shape.size.Add (new Vector3 (4, 3, 1));
			shape.pos.Add (new Vector3 (0, 2, 2));
			shape.size.Add (new Vector3 (4, 2, 1));
			shape.pos.Add (new Vector3 (0, 3, 3));
			shape.size.Add (new Vector3 (4, 1, 1));

			_aRailgunShapes.Add(shape);

			//
			// corner right
			//
			shape = new Globals.RailgunShape (6, 4, 6, new List<Vector3>(), new List<Vector3>());
			shape.pos.Add (new Vector3 (0, 0, 0));
			shape.size.Add (new Vector3 (3, 4, 6));
			shape.pos.Add (new Vector3 (0, 0, 3));
			shape.size.Add (new Vector3 (6, 4, 3));

			_aRailgunShapes.Add(shape);

			//
			// corner left
			//
			shape = new Globals.RailgunShape (6, 4, 6, new List<Vector3>(), new List<Vector3>());
			shape.pos.Add (new Vector3 (3, 0, 0));
			shape.size.Add (new Vector3 (3, 4, 6));
			shape.pos.Add (new Vector3 (0, 0, 3));
			shape.size.Add (new Vector3 (6, 4, 3));

			_aRailgunShapes.Add(shape);

			// Experimental


			newLevel ();
		}

		// ---------------------------------------------------------------------------------------------
		public void newLevel()
		{
			reset ();
			createDefaultLevel ();
		}

		// ---------------------------------------------------------------------------------------------
		public void reset()
		{
			_levelMap.reset ();

			foreach (Transform child in _trfmVoxels) {
				Destroy (child.gameObject);
			}
			_aVoxelChunks.Clear ();
			_iVoxelCount = 0;

			foreach (Transform child in _trfmProps) {
				Destroy (child.gameObject);
			}
			_worldProps.Clear ();
		}

		//
		public void setStartPos(Vector3 pos, Vector3 rot)
		{
			_startPos = pos;
			_startRotation = rot;
		}

		public Vector3 getStartPos()
		{
			return _chunkPos + _startPos;
		}

		public Vector3 getStartRotation()
		{
			return _startRotation;
		}

		// ---------------------------------------------------------------------------------------------
		public void setVoxelsData(List<VoxelUtils.VoxelChunk> aVoxelsData)
		{
			reset ();

			VoxelUtils.VoxelChunk vc;
			int i, len = aVoxelsData.Count;
			for (i = 0; i < len; ++i)
			{
				vc = createVoxelChunk(aVoxelsData[i].pos, aVoxelsData[i].size.x, aVoxelsData[i].size.y, aVoxelsData[i].size.z);
				vc.meshCreated = true;
				_aVoxelChunks.Add (vc);
				setVoxelChunkMesh (vc);
			}
		}

		// ---------------------------------------------------------------------------------------------
		private void createDefaultLevel()
		{
			// create the full chunk voxel
			VoxelUtils.VoxelVector3Int pos = VoxelUtils.convertVector3ToVoxelVector3Int(Vector3.zero);
			VoxelUtils.VoxelChunk vc = createVoxelChunk(pos, VoxelUtils.MAX_CHUNK_UNITS, VoxelUtils.MAX_CHUNK_UNITS, VoxelUtils.MAX_CHUNK_UNITS);
			_aVoxelChunks.Add (vc);

			if (_isExperimentalChunk) {
				setVoxelChunkMesh (vc);
			} else {
				// create room in center of level
				subtractChunk (new Vector3 (32, 34, 32), new Vector3 (8, 4, 8));
			}
		}

		// ---------------------------------------------------------------------------------------------
		public Vector3 getHitChunkPos(RaycastHit hit)
		{
			float vcsHalf = VoxelUtils.CHUNK_SIZE * 0.5f;

			float xChunk = (int)((hit.point.x + (hit.normal.x * -1 * vcsHalf)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;
			float yChunk = (int)((hit.point.y + (hit.normal.y * -1 * vcsHalf)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;
			float zChunk = (int)((hit.point.z + (hit.normal.z * -1 * vcsHalf)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;

			return new Vector3 ((int)(xChunk / VoxelUtils.CHUNK_SIZE), (int)(yChunk / VoxelUtils.CHUNK_SIZE), (int)(zChunk / VoxelUtils.CHUNK_SIZE));
		}

		// ---------------------------------------------------------------------------------------------
		public void dig(RaycastHit hit, Vector3 chunkSize)
		{
			_lastChunkPos = getHitChunkPos (hit);

			if (hit.normal.x > 0) {
				_lastChunkPos.x -= (chunkSize.x - 1);
			}
			else if (hit.normal.y > 0) {
				_lastChunkPos.y -= (chunkSize.y - 1);
			}
			else if (hit.normal.z > 0) {
				_lastChunkPos.z -= (chunkSize.z - 1);
			}

			subtractChunk (_lastChunkPos, chunkSize);
		}

		// ---------------------------------------------------------------------------------------------
		public void railgunDig(RaycastHit hit)
		{
			LevelEditor.Instance.resetUndoActions ();
			saveCurrentVoxelChunks ();
			MainMenu.Instance.setUndoButton (true);

			_lastChunkPos = getHitChunkPos (hit);

			Globals.RailgunShape shape = _aRailgunShapes [MainMenu.Instance.iSelectedRailgunMaterialIndex];

			int i, len = shape.pos.Count;
			Vector3 pos, size;

			if (hit.normal.x != 0)
			{
				if (hit.normal.x < 0) {
					//Debug.Log ("right");
					for (i = 0; i < len; ++i) {
						pos = new Vector3 (_lastChunkPos.x + shape.pos [i].z, _lastChunkPos.y + shape.pos [i].y, _lastChunkPos.z - (shape.size [i].x - 1) - shape.pos [i].x);
						size = new Vector3 (shape.size [i].z, shape.size [i].y, shape.size [i].x);
						subtractChunk (pos, size, false);
					}
				} else {
					//Debug.Log ("left");
					for (i = 0; i < len; ++i) {
						pos  = new Vector3 (_lastChunkPos.x - (shape.size [i].z - 1) - shape.pos [i].z, _lastChunkPos.y + shape.pos [i].y, _lastChunkPos.z + shape.pos [i].x);
						size = new Vector3 (shape.size [i].z, shape.size [i].y, shape.size [i].x);
						subtractChunk (pos, size, false);
					}
				}
			}
			else if (hit.normal.y != 0)
			{
				if (hit.normal.y < 0) {
					//Debug.Log ("ceiling");
					for (i = 0; i < len; ++i) {
						pos  = new Vector3 (_lastChunkPos.x + shape.pos [i].x, _lastChunkPos.y + shape.pos [i].z, _lastChunkPos.z + shape.pos [i].y);
						size = new Vector3 (shape.size [i].x, shape.size [i].z, shape.size [i].y);
						subtractChunk (pos, size, false);
					}
				} else {
					//Debug.Log ("floor");
					for (i = 0; i < len; ++i) {
						pos = new Vector3 (_lastChunkPos.x + shape.pos [i].x, _lastChunkPos.y - (shape.size [i].z - 1) - shape.pos [i].z, _lastChunkPos.z + shape.pos [i].y);
						size = new Vector3 (shape.size [i].x, shape.size [i].z, shape.size [i].y);
						subtractChunk (pos, size, false);
					}
				}
			}
			else if (hit.normal.z != 0)
			{
				// this is the default direction - everything going +
				if (hit.normal.z < 0) {
					//Debug.Log ("front");
					for (i = 0; i < len; ++i) {
						pos  = new Vector3 (_lastChunkPos.x + shape.pos [i].x, _lastChunkPos.y + shape.pos [i].y, _lastChunkPos.z + shape.pos [i].z);
						size = shape.size [i];
						subtractChunk (pos, size, false);
					}
				} else {
					//Debug.Log ("back");
					for (i = 0; i < len; ++i) {
						pos = new Vector3 (_lastChunkPos.x - (shape.size [i].x - 1) - shape.pos [i].x, _lastChunkPos.y + shape.pos [i].y, _lastChunkPos.z - (shape.size [i].z - 1) - shape.pos [i].z);
						size = shape.size [i];
						subtractChunk (pos, size, false);
					}
				}
			}
		}

		// ---------------------------------------------------------------------------------------------
		public void paint(RaycastHit hit, Vector3 chunkSize, int materialIndex)
		{
			//if (AppController.Instance.appState == AppState.Paint) {
				//chunkSize.z = 1;
			//}

			dig (hit, chunkSize);

			VoxelUtils.VoxelChunk vc = createVoxelChunk (VoxelUtils.convertVector3ToVoxelVector3Int (_lastChunkPos), (int)chunkSize.x, (int)chunkSize.y, (int)chunkSize.z);
			vc.materialIndex = materialIndex;

			_aVoxelChunks.Add (vc);
			setVoxelChunkMesh (vc);
		}

		// ---------------------------------------------------------------------------------------------
		public void build(RaycastHit hit, Vector3 chunkSize, int materialIndex)
		{
			_lastChunkPos = getHitChunkPos (hit);

			if (hit.normal.x != 0) {
				if (hit.normal.x > 0) {
					_lastChunkPos.x += 1;
				} else {
					_lastChunkPos.x -= chunkSize.x;
				}
			}
			else if (hit.normal.y != 0) {
				if (hit.normal.y > 0) {
					_lastChunkPos.y += 1;
				} else {
					_lastChunkPos.y -= chunkSize.y;
				}
			}
			else if (hit.normal.z != 0) {
				if (hit.normal.z > 0) {
					_lastChunkPos.z += 1;
				} else {
					_lastChunkPos.z -= chunkSize.z;
				}
			}
					
			subtractChunk (_lastChunkPos, chunkSize);

			VoxelUtils.VoxelChunk vc = createVoxelChunk (VoxelUtils.convertVector3ToVoxelVector3Int (_lastChunkPos), (int)chunkSize.x, (int)chunkSize.y, (int)chunkSize.z);
			vc.materialIndex = materialIndex;

			_aVoxelChunks.Add (vc);
			setVoxelChunkMesh (vc);
		}

		// ---------------------------------------------------------------------------------------------
		public float[] convertChunksToVoxels()
		{
			int seed = (int)(Time.time * 10f);
			Random.InitState (seed);

			int size = VoxelUtils.MAX_CHUNK_UNITS;
			float[] voxels = new float[size * size * size];
			int numVoxels = voxels.Length;

			VoxelUtils.VoxelChunk vc;
			int x, y, z, index;
			int maxX, maxY, maxZ;

			int i;
			for (i = 0; i < numVoxels; ++i) {
				voxels [i] = -1;
			}

			int size2 = size * size;
			int ySize;
			int len = _aVoxelChunks.Count;
			for (i = 0; i < len; ++i) {
	
				vc = _aVoxelChunks [i];

				maxX = (vc.corners.bot_left_front.x + vc.size.x) - 1;
				maxY = (vc.corners.bot_left_front.y + vc.size.y) - 1;
				maxZ = (vc.corners.bot_left_front.z + vc.size.z) - 1;

				for (x = vc.corners.bot_left_front.x; x <= maxX; x++) {
					for (y = vc.corners.bot_left_front.y; y <= maxY; y++) {
						ySize = y * size;
						for (z = vc.corners.bot_left_front.z; z <= maxZ; z++) {

							index = x + ySize + z * size2;

							if ((x > vc.corners.bot_left_front.x && x < maxX)
							&&	(y > vc.corners.bot_left_front.y && y < maxY)
							&&	(z > vc.corners.bot_left_front.z && z < maxZ))
							{
								voxels [index] = 0;
							}
							else
							{
								voxels [index] = 1 - Random.value;//-1 + Random.value;
							}
						}
					}
				}
			}

			/*for (i = 0; i < numVoxels; ++i) {
				if (voxels [i] == 0) {
					voxels [i] = -1.0f + Random.value * 2.0f;
				}
			}*/

			return voxels;
		}

		// ---------------------------------------------------------------------------------------------
		// cut a hole!
		// ---------------------------------------------------------------------------------------------
		public bool subtractChunk(Vector3 v3Pos, Vector3 v3Size, bool allowUndo = true)
		{
			bool success = true;

			float timer = Time.realtimeSinceStartup;

			if (allowUndo && !_isExperimentalChunk)
			{
				LevelEditor.Instance.resetUndoActions ();
				saveCurrentVoxelChunks ();
				MainMenu.Instance.setUndoButton (true);
			}

			VoxelUtils.VoxelVector3Int pos = VoxelUtils.convertVector3ToVoxelVector3Int (v3Pos);
			VoxelUtils.VoxelChunk vsCut = createCutVoxelChunk(pos, (int)v3Size.x, (int)v3Size.y, (int)v3Size.z);

			_levelMap.addCube (pos, new Vector3 ((int)v3Size.x, (int)v3Size.y, (int)v3Size.z));

			// does the new voxel intersect with any existing voxels?
			bool splittage = splitVoxels (vsCut);
			int loops = 0;
			while (splittage && loops < 1000) {
				splittage = splitVoxels (vsCut);
				loops++;
			}

			if (loops >= 1000) {
				Debug.LogWarning ("looks like we got ourselves an endless loop here!");
				success = false;
			}
			else
			{
				VoxelUtils.VoxelChunk vc;
				int i, len = _aVoxelChunks.Count;
				//int count = 0;
				for (i = 0; i < len; ++i) {
					vc = _aVoxelChunks [i];
					if (!vc.meshCreated) {
						setVoxelChunkMesh (vc);
						vc.meshCreated = true;
						//count++;
						_aVoxelChunks [i] = vc;
					}
				}

				//if (_isExperimentalChunk) {
				//	Debug.Log ("num voxels: " + len + " - loops: " + loops + " - meshes created: " + count);
				//}
			}

			//if (_isExperimentalChunk) {
			//	Debug.Log ("Time to create chunk(s): " + (Time.realtimeSinceStartup - timer).ToString ());
			//}

			if (!_isExperimentalChunk) {
				MainMenu.Instance.setCubeCountText ("Voxel Chunks: " + _aVoxelChunks.Count.ToString ());
			}

			return success;
		}

		// ---------------------------------------------------------------------------------------------
		private void saveCurrentVoxelChunks ()
		{
			_aVoxelChunksUndo.Clear ();

			int i, len = _aVoxelChunks.Count;
			for (i = 0; i < len; ++i) {
				_aVoxelChunksUndo.Add (_aVoxelChunks [i]);
			}
		}

		// ---------------------------------------------------------------------------------------------
		public void resetUndo()
		{
			_aVoxelChunksUndo.Clear ();
		}

		// ---------------------------------------------------------------------------------------------
		public void undo()
		{
			int len = _aVoxelChunksUndo.Count;
			if (len <= 0) {
				return;
			}

			VoxelUtils.VoxelChunk vc;

			foreach (Transform child in _trfmVoxels) {
				Destroy (child.gameObject);
			}
			_aVoxelChunks.Clear ();

			_iVoxelCount = 0;

			int i = _aVoxelChunksUndo.Count;
			for (i = 0; i < len; ++i) {
				vc = createVoxelChunk(_aVoxelChunksUndo[i].pos, _aVoxelChunksUndo[i].size.x, _aVoxelChunksUndo[i].size.y, _aVoxelChunksUndo[i].size.z);
				_aVoxelChunks.Add (vc);
			}
			_aVoxelChunksUndo.Clear ();

			len = _aVoxelChunks.Count;
			for (i = 0; i < len; ++i) {
				vc = _aVoxelChunks [i];
				if (!vc.meshCreated) {
					setVoxelChunkMesh (vc);
					vc.meshCreated = true;
					_aVoxelChunks [i] = vc;
				}
			}

			if (!_isExperimentalChunk) {
				MainMenu.Instance.setCubeCountText ("Voxel Chunks: " + _aVoxelChunks.Count.ToString ());
			}
		}

		// ---------------------------------------------------------------------------------------------
		// split them voxels one at a time
		// ---------------------------------------------------------------------------------------------
		private bool splitVoxels(VoxelUtils.VoxelChunk vsCut) {

			bool intersectDetected = false;

			int i, len = _aVoxelChunks.Count;
			for (i = 0; i < len; ++i) {

				// do a bounds intersect check first
				if (vsCut.bounds.Intersects(_aVoxelChunks[i].bounds)) {

					//Debug.Log ("bounds intersect detected: "+_aVoxelChunks[i].go.name);

					// check for identical size and position
					if (_aVoxelChunks [i].Identical (vsCut)) {

						//Debug.LogWarning ("    ->IDENTICAL: "+i+" - "+_aVoxelChunks [i].go.name);
						Destroy (_aVoxelChunks [i].go);
						_aVoxelChunks.RemoveAt (i);
						intersectDetected = true;
					}
					// check for identical size and position
					else if (_aVoxelChunks [i].Encased (vsCut)) {

						//Debug.LogWarning ("    ->ENCASED!");
						Destroy (_aVoxelChunks [i].go);
						_aVoxelChunks.RemoveAt (i);
						intersectDetected = true;
					}
					else if (checkVoxelChunkIntersectX (i, vsCut)) {

						intersectDetected = true;
					}
					else if (checkVoxelChunkIntersectY (i, vsCut)) {

						intersectDetected = true;
					}
					else if (checkVoxelChunkIntersectZ (i, vsCut)) {

						intersectDetected = true;
					}
				}

				if (intersectDetected) {
					break;
				}
			}

			return intersectDetected;
		}

		// ---------------------------------------------------------------------------------------------
		// CHECK ALL 8 CORNERS FOR INTERSECTION ALONG X AXIS
		// ---------------------------------------------------------------------------------------------
		private bool checkVoxelChunkIntersectX(int index, VoxelUtils.VoxelChunk vsCut)
		{
			bool intersectDetected = false;

			VoxelUtils.VoxelChunk vs = _aVoxelChunks [index];

			if (_aVoxelChunks [index].IntersectsRightX (vsCut.corners.bot_left_front)) {

				separateVoxelChunkAlongXAxis (index, vsCut.corners.bot_left_front.x);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsRightX (vsCut.corners.top_left_front)) {

				separateVoxelChunkAlongXAxis (index, vsCut.corners.top_left_front.x);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsRightX (vsCut.corners.top_left_back)) {

				separateVoxelChunkAlongXAxis (index, vsCut.corners.top_left_back.x);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsRightX (vsCut.corners.bot_left_back)) {

				separateVoxelChunkAlongXAxis (index, vsCut.corners.bot_left_back.x);
				intersectDetected = true;
			}
			//
			else if (_aVoxelChunks [index].IntersectsLeftX( vsCut.corners.bot_right_front )) {

				separateVoxelChunkAlongXAxis (index, vsCut.corners.bot_right_front.x + 1);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsLeftX( vsCut.corners.top_right_front )) {

				separateVoxelChunkAlongXAxis (index, vsCut.corners.top_right_front.x + 1);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsLeftX( vsCut.corners.top_right_back )) {

				separateVoxelChunkAlongXAxis (index, vsCut.corners.top_right_back.x + 1);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsLeftX( vsCut.corners.bot_right_back )) {

				separateVoxelChunkAlongXAxis (index, vsCut.corners.bot_right_back.x + 1);
				intersectDetected = true;
			}

			return intersectDetected;
		}

		// ---------------------------------------------------------------------------------------------
		// CHECK ALL 8 CORNERS FOR INTERSECTION ALONG Y AXIS
		// ---------------------------------------------------------------------------------------------
		private bool checkVoxelChunkIntersectY(int index, VoxelUtils.VoxelChunk vsCut)
		{
			bool intersectDetected = false;

			VoxelUtils.VoxelChunk vs = _aVoxelChunks [index];

			if (_aVoxelChunks [index].IntersectsTopY (vsCut.corners.bot_left_front)) {

				separateVoxelChunkAlongYAxis (index, vsCut.corners.bot_left_front.y);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsTopY (vsCut.corners.bot_left_back)) {

				separateVoxelChunkAlongYAxis (index, vsCut.corners.bot_left_back.y);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsTopY (vsCut.corners.bot_right_back)) {

				separateVoxelChunkAlongYAxis (index, vsCut.corners.bot_right_back.y);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsTopY (vsCut.corners.bot_right_front)) {

				separateVoxelChunkAlongYAxis (index, vsCut.corners.bot_right_front.y);
				intersectDetected = true;
			}
			//
			else if (_aVoxelChunks [index].IntersectsBottomY( vsCut.corners.top_left_front )) {

				separateVoxelChunkAlongYAxis (index, vsCut.corners.top_left_front.y + 1);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsBottomY( vsCut.corners.top_left_back )) {

				separateVoxelChunkAlongYAxis (index, vsCut.corners.top_left_back.y + 1);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsBottomY( vsCut.corners.top_right_back )) {

				separateVoxelChunkAlongYAxis (index, vsCut.corners.top_right_back.y + 1);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsBottomY( vsCut.corners.top_right_front )) {

				separateVoxelChunkAlongYAxis (index, vsCut.corners.top_right_front.y + 1);
				intersectDetected = true;
			}

			return intersectDetected;
		}

		// ---------------------------------------------------------------------------------------------
		// CHECK ALL 8 CORNERS FOR INTERSECTION ALONG Z AXIS
		// ---------------------------------------------------------------------------------------------
		private bool checkVoxelChunkIntersectZ(int index, VoxelUtils.VoxelChunk vsCut)
		{
			bool intersectDetected = false;

			VoxelUtils.VoxelChunk vs = _aVoxelChunks [index];

			if (_aVoxelChunks [index].IntersectsBackZ (vsCut.corners.bot_left_front)) {

				separateVoxelChunkAlongZAxis (index, vsCut.corners.bot_left_front.z);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsBackZ (vsCut.corners.top_left_front)) {

				separateVoxelChunkAlongZAxis (index, vsCut.corners.top_left_front.z);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsBackZ (vsCut.corners.top_right_front)) {

				separateVoxelChunkAlongZAxis (index, vsCut.corners.top_right_front.z);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsBackZ (vsCut.corners.bot_right_front)) {

				separateVoxelChunkAlongZAxis (index, vsCut.corners.bot_right_front.z);
				intersectDetected = true;
			}
			//
			else if (_aVoxelChunks [index].IntersectsFrontZ( vsCut.corners.bot_left_back )) {

				separateVoxelChunkAlongZAxis (index, vsCut.corners.bot_left_back.z + 1);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsFrontZ( vsCut.corners.top_left_back )) {

				separateVoxelChunkAlongZAxis (index, vsCut.corners.top_left_back.z + 1);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsFrontZ( vsCut.corners.top_right_back )) {

				separateVoxelChunkAlongZAxis (index, vsCut.corners.top_right_back.z + 1);
				intersectDetected = true;
			}
			else if (_aVoxelChunks [index].IntersectsFrontZ( vsCut.corners.bot_right_back )) {

				separateVoxelChunkAlongZAxis (index, vsCut.corners.bot_right_back.z + 1);
				intersectDetected = true;
			}

			return intersectDetected;
		}

		// ---------------------------------------------------------------------------------------------
		//
		// ---------------------------------------------------------------------------------------------
		private void separateVoxelChunkAlongXAxis(int index, int pointCutX) {

			//Debug.Log ("    ->separateVoxelChunkAlongXAxis "+pointCutX.ToString());

			VoxelUtils.VoxelChunk vc = _aVoxelChunks [index];
			int materialIndex = vc.materialIndex;
			//Debug.Log ("materialIndex: "+materialIndex);

			Destroy (vc.go);
			_aVoxelChunks.RemoveAt (index);

			int leftX  = vc.corners.bot_left_front.x;
			int width1 = Mathf.Abs(pointCutX - leftX);

			// create left part
			VoxelUtils.VoxelChunk vsLeft = createVoxelChunk(vc.pos, width1, vc.size.y, vc.size.z);
			vsLeft.materialIndex = materialIndex;
			_aVoxelChunks.Add (vsLeft);

			// create right part
			vc.pos.x += width1;
			int width2 = vc.size.x - width1;
			VoxelUtils.VoxelChunk vsRight = createVoxelChunk(vc.pos, width2, vc.size.y, vc.size.z);
			vsRight.materialIndex = materialIndex;
			_aVoxelChunks.Add (vsRight);
		}
			
		// ---------------------------------------------------------------------------------------------
		private void separateVoxelChunkAlongYAxis(int index, int pointCutY) {

			//Debug.Log ("    ->separateVoxelChunkAlongYAxis "+pointCutY.ToString());

			VoxelUtils.VoxelChunk vc = _aVoxelChunks [index];
			int materialIndex = vc.materialIndex;
			//Debug.Log ("materialIndex: "+materialIndex);

			Destroy (vc.go);
			_aVoxelChunks.RemoveAt (index);

			int botY    = vc.corners.bot_left_front.y;
			int height1 = Mathf.Abs(pointCutY - botY);

			// create bottom part
			VoxelUtils.VoxelChunk vsBottom = createVoxelChunk(vc.pos, vc.size.x, height1, vc.size.z);
			vsBottom.materialIndex = materialIndex;
			_aVoxelChunks.Add (vsBottom);

			// create top part
			vc.pos.y += height1;
			int height2 = vc.size.y - height1;
			VoxelUtils.VoxelChunk vsTop = createVoxelChunk(vc.pos, vc.size.x, height2, vc.size.z);
			vsTop.materialIndex = materialIndex;
			_aVoxelChunks.Add (vsTop);
		}

		// ---------------------------------------------------------------------------------------------
		private void separateVoxelChunkAlongZAxis(int index, int pointCutZ) {

			//Debug.Log ("    ->separateVoxelChunkAlongZAxis "+pointCutZ.ToString());

			VoxelUtils.VoxelChunk vc = _aVoxelChunks [index];
			int materialIndex = vc.materialIndex;
			//Debug.Log ("materialIndex: "+materialIndex);

			Destroy (vc.go);
			_aVoxelChunks.RemoveAt (index);

			int frontZ = vc.corners.bot_left_front.z;
			int depth1 = Mathf.Abs(pointCutZ - frontZ);

			// create front part
			VoxelUtils.VoxelChunk vsFront = createVoxelChunk(vc.pos, vc.size.x, vc.size.y, depth1);
			vsFront.materialIndex = materialIndex;
			_aVoxelChunks.Add (vsFront);

			// create back part
			vc.pos.z += depth1;
			int depth2 = vc.size.z - depth1;
			VoxelUtils.VoxelChunk vsBack = createVoxelChunk(vc.pos, vc.size.x, vc.size.y, depth2);
			vsBack.materialIndex = materialIndex;
			_aVoxelChunks.Add (vsBack);
		}

		// ---------------------------------------------------------------------------------------------
		// create single voxel
		// ---------------------------------------------------------------------------------------------
		public VoxelUtils.VoxelChunk createVoxelChunk(VoxelUtils.VoxelVector3Int p, int w, int h, int d) {

			//if (_isExperimentalChunk) {
			//	Debug.Log ("createVoxelChunk " + p.ToString () + ", " + w + ", " + h + ", " + d);
			//}

			GameObject cube = AssetFactory.Instance.createVoxelChunkClone();
			cube.transform.SetParent (_trfmVoxels);
			_iVoxelCount++;
			cube.name = "voxchunk_" + _iVoxelCount.ToString ();

			//if (_isExperimentalChunk) {
			//	Debug.Log ("cube: " + cube.name + ", " + _trfmVoxels.name);
			//}

			float width  = w * VoxelUtils.CHUNK_SIZE;
			float height = h * VoxelUtils.CHUNK_SIZE;
			float depth  = d * VoxelUtils.CHUNK_SIZE;

			Vector3 pos = new Vector3 ((p.x * VoxelUtils.CHUNK_SIZE) + (width / 2f), (p.y * VoxelUtils.CHUNK_SIZE) + (height / 2f), (p.z * VoxelUtils.CHUNK_SIZE) + (depth / 2f));

			Bounds b = new Bounds ();
			b.size = new Vector3 (width, height, depth);
			b.center = pos;

			VoxelUtils.VoxelChunk vc = new VoxelUtils.VoxelChunk ();
			vc.go      = cube;
			vc.goPos   = pos;
			vc.pos     = p;
			vc.size    = new VoxelUtils.VoxelVector3Int(w, h, d);
			vc.bounds  = b;
			vc.corners = VoxelUtils.createVoxelCorners (p, w, h, d);
			vc.materialIndex = 0;
			vc.meshCreated = false;

			return vc;
		}

		// ---------------------------------------------------------------------------------------------
		// 
		// ---------------------------------------------------------------------------------------------
		public void setVoxelChunkMesh(VoxelUtils.VoxelChunk vc)
		{
			vc.go.transform.localPosition = vc.goPos;
			Mesh mesh = vc.go.GetComponent<MeshFilter> ().mesh;
			VoxelChunkMesh.create (mesh, vc.size.x * VoxelUtils.CHUNK_SIZE, vc.size.y * VoxelUtils.CHUNK_SIZE, vc.size.z * VoxelUtils.CHUNK_SIZE, vc.size.x, vc.size.y, vc.size.z, false);

			BoxCollider coll = vc.go.GetComponent<BoxCollider> ();
			coll.size = new Vector3 (vc.size.x * VoxelUtils.CHUNK_SIZE, vc.size.y * VoxelUtils.CHUNK_SIZE, vc.size.z * VoxelUtils.CHUNK_SIZE);

			Renderer renderer = vc.go.GetComponent<Renderer> ();
			if (!_isExperimentalChunk) {
				renderer.sharedMaterial = LevelEditor.Instance.aMaterials [vc.materialIndex];
			}
		}

		// ---------------------------------------------------------------------------------------------
		// 
		// ---------------------------------------------------------------------------------------------
		private VoxelUtils.VoxelChunk createCutVoxelChunk(VoxelUtils.VoxelVector3Int p, int w, int h, int d)
		{
			//if (_isExperimentalChunk) {
				//Debug.Log ("createCutVoxelChunk " + p.ToString () + ", " + w + ", " + h + ", " + d);
			//}

			float width  = w * VoxelUtils.CHUNK_SIZE;
			float height = h * VoxelUtils.CHUNK_SIZE;
			float depth  = d * VoxelUtils.CHUNK_SIZE;

			Vector3 pos = new Vector3 ((p.x * VoxelUtils.CHUNK_SIZE) + (width / 2f), (p.y * VoxelUtils.CHUNK_SIZE) + (height / 2f), (p.z * VoxelUtils.CHUNK_SIZE) + (depth / 2f));

			Bounds b = new Bounds (); //coll.bounds;
			b.size = new Vector3 (width - VoxelUtils.CHUNK_SIZE, height - VoxelUtils.CHUNK_SIZE, depth - VoxelUtils.CHUNK_SIZE);
			b.center = pos;

			VoxelUtils.VoxelChunk vc = new VoxelUtils.VoxelChunk ();
			vc.pos     = p;
			vc.size    = new VoxelUtils.VoxelVector3Int(w, h, d);
			vc.bounds  = b;
			vc.corners = VoxelUtils.createVoxelCorners (p, w, h, d);
			vc.materialIndex = 0;
			vc.meshCreated = false;

			return vc;
		}

		#region Props

		// ---------------------------------------------------------------------------------------------
		// 
		// ---------------------------------------------------------------------------------------------
		public void addWorldProp(int id, GameObject go)
		{
			_worldProps.Add (go, new worldProp (id, go.name, go));
		}

		//
		public void removeWorldProp(GameObject go)
		{
			if (_worldProps.ContainsKey (go)) {
				_worldProps.Remove (go);
			}
		}

		public propDef getPropDefForGameObject(GameObject go)
		{
			propDef p = new propDef();
			p.id = -1;

			if (_worldProps.ContainsKey (go)) {
				p = PropsManager.Instance.getPropDefForId(_worldProps [go].id);
			}

			return p;
		}

		#endregion
	}
}