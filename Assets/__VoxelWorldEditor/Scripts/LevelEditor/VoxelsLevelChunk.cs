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
		public ConvertLevelToMesh _ConvertLevelToMesh;

		private Transform _trfmVoxelChunkContainer;

		//
		private int _editMode;

		private int _iCount;

		private List<VoxelUtils.VoxelChunk> _aVoxelChunks;

		//private Ray _ray;
		//private RaycastHit _hit;
		//private GameObject _goHit;

		//private Vector3 _curDigSize;

		// ---------------------------------------------------------------------------------------------
		// Init shit
		// ---------------------------------------------------------------------------------------------
		void Awake () {

			_editMode = 0;
			//_curDigSize = new Vector3 (4, 6, 4);
		}

		public void init (GameObject goParent) {

			_trfmVoxelChunkContainer = goParent.transform;

			_iCount = 0;

			_aVoxelChunks = new List<VoxelUtils.VoxelChunk> ();

			// create the full chunk voxel
			VoxelUtils.VoxelVector3Int pos = VoxelUtils.convertVector3ToVoxelVector3Int(Vector3.zero);
			VoxelUtils.VoxelChunk vs = createVoxelChunk(pos, VoxelUtils.MAX_CHUNK_UNITS, VoxelUtils.MAX_CHUNK_UNITS, VoxelUtils.MAX_CHUNK_UNITS);
			_aVoxelChunks.Add (vs);

			subtractChunk (new Vector3 (33, 34, 33), new Vector3 (6, 4, 6));
		}
		
		// ---------------------------------------------------------------------------------------------
		// Update shit
		// ---------------------------------------------------------------------------------------------
		/*void Update () {

			if (Input.GetMouseButtonDown (0)) {

				if (!EventSystem.current.IsPointerOverGameObject ()) {
					onMouseClick ();
				}
			}
			else if (Input.GetKeyDown(KeyCode.P))
			{
				toggleEditMode ();
			}
			else {

				doRayCast ();
			}
		}*/

		// ---------------------------------------------------------------------------------------------
		/*private void toggleEditMode()
		{
			_editMode = (_editMode == 0 ? 1 : 0);

			if (_editMode == 1)
			{
				float timer = Time.realtimeSinceStartup;

				//float fCenter  = (float)VoxelUtils.MAX_CHUNK_UNITS * VoxelUtils.CHUNK_SIZE * .5f;
				float[] voxels = convertChunksToVoxels ();

				_ConvertLevelToMesh.create (VoxelUtils.MAX_CHUNK_UNITS, VoxelUtils.MAX_CHUNK_UNITS, VoxelUtils.MAX_CHUNK_UNITS, voxels, Vector3.zero); //new Vector3(fCenter, fCenter, fCenter));

				Debug.LogWarning ("Time run marching cubes: " + (Time.realtimeSinceStartup - timer).ToString ());
			}
			else {
				_ConvertLevelToMesh.resetAll ();
			}
		}*/

		// ---------------------------------------------------------------------------------------------
		/*private void doRayCast()
		{
			_goHit = null;

			_ray = _curCam.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (_ray, out _hit, 10))
			{
				if (_hit.collider.gameObject.tag == "DigAndDestroy")
				{
					_goHit = _hit.collider.gameObject;

					float vcsHalf = VoxelUtils.CHUNK_SIZE * 0.5f;

					float xChunk = (int)((_hit.point.x + (_hit.normal.x * -1 * vcsHalf)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;
					float yChunk = (int)((_hit.point.y + (_hit.normal.y * -1 * vcsHalf)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;
					float zChunk = (int)((_hit.point.z + (_hit.normal.z * -1 * vcsHalf)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;

					aimHelper.forward = _hit.normal;

					Vector3 aimPos = new Vector3 (xChunk + vcsHalf, yChunk + vcsHalf, zChunk + vcsHalf);
					//aimPos += (_curDigSize * 0.5f);
					aimHelper.localPosition = aimPos;
				}
			}
		}*/

		// ---------------------------------------------------------------------------------------------
		public void dig(RaycastHit hit, Vector3 digSize)
		{
			float vcsHalf = VoxelUtils.CHUNK_SIZE * 0.5f;

			float xChunk = (int)((hit.point.x + (hit.normal.x * -1 * vcsHalf)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;
			float yChunk = (int)((hit.point.y + (hit.normal.y * -1 * vcsHalf)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;
			float zChunk = (int)((hit.point.z + (hit.normal.z * -1 * vcsHalf)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;

			subtractChunk (new Vector3 ((int)(xChunk / VoxelUtils.CHUNK_SIZE), (int)(yChunk / VoxelUtils.CHUNK_SIZE), (int)(zChunk / VoxelUtils.CHUNK_SIZE)), digSize);
		}

		// ---------------------------------------------------------------------------------------------
		public void paint(RaycastHit hit, Vector3 digSize, Material material)
		{
			float vcsHalf = VoxelUtils.CHUNK_SIZE * 0.5f;

			float xChunk = (int)((hit.point.x + (hit.normal.x * -1 * vcsHalf)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;
			float yChunk = (int)((hit.point.y + (hit.normal.y * -1 * vcsHalf)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;
			float zChunk = (int)((hit.point.z + (hit.normal.z * -1 * vcsHalf)) / VoxelUtils.CHUNK_SIZE) * VoxelUtils.CHUNK_SIZE;

			subtractChunk (new Vector3 ((int)(xChunk / VoxelUtils.CHUNK_SIZE), (int)(yChunk / VoxelUtils.CHUNK_SIZE), (int)(zChunk / VoxelUtils.CHUNK_SIZE)), digSize);
		}

		// ---------------------------------------------------------------------------------------------
		private float[] convertChunksToVoxels()
		{
			int seed = (int)(Time.time * 10f);
			//INoise perlin = new PerlinNoise(seed, 2.0f);
			//FractalNoise fractal = new FractalNoise(perlin, 3, 1.0f);
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

							index = x + ySize + z * size2; //size * size;

							if ((x > vc.corners.bot_left_front.x && x < maxX)
							&&	(y > vc.corners.bot_left_front.y && y < maxY)
							&&	(z > vc.corners.bot_left_front.z && z < maxZ))
							{
								voxels [index] = 0;//1 - Random.value;
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
		private bool subtractChunk(Vector3 v3Pos, Vector3 v3Size)
		{
			bool success = true;

			float timer = Time.realtimeSinceStartup;

			VoxelUtils.VoxelVector3Int pos = VoxelUtils.convertVector3ToVoxelVector3Int (v3Pos);
			VoxelUtils.VoxelChunk vsCut = createCutVoxelChunk(pos, (int)v3Size.x, (int)v3Size.y, (int)v3Size.z);

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
				//Debug.Log ("num voxels: " + len + " - loops: " + loops + " - meshes created: " + count);
			}

			Debug.Log ("Time to create chunk(s): " + (Time.realtimeSinceStartup - timer).ToString ());
			MainMenu.Instance.setCubeCountText ("Voxel Chunks: "+_aVoxelChunks.Count.ToString());

			return success;
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

			VoxelUtils.VoxelChunk vs = _aVoxelChunks [index];
			Destroy (vs.go);
			_aVoxelChunks.RemoveAt (index);

			int leftX  = vs.corners.bot_left_front.x;
			int width1 = Mathf.Abs(pointCutX - leftX);

			// create left part
			VoxelUtils.VoxelChunk vsLeft = createVoxelChunk(vs.pos, width1, vs.size.y, vs.size.z);
			_aVoxelChunks.Add (vsLeft);

			// create right part
			vs.pos.x += width1;
			int width2 = vs.size.x - width1;
			VoxelUtils.VoxelChunk vsRight = createVoxelChunk(vs.pos, width2, vs.size.y, vs.size.z);
			_aVoxelChunks.Add (vsRight);
		}
			
		// ---------------------------------------------------------------------------------------------
		private void separateVoxelChunkAlongYAxis(int index, int pointCutY) {

			//Debug.Log ("    ->separateVoxelChunkAlongYAxis "+pointCutY.ToString());

			VoxelUtils.VoxelChunk vs = _aVoxelChunks [index];
			Destroy (vs.go);
			_aVoxelChunks.RemoveAt (index);

			int botY    = vs.corners.bot_left_front.y;
			int height1 = Mathf.Abs(pointCutY - botY);

			// create bottom part
			VoxelUtils.VoxelChunk vsBottom = createVoxelChunk(vs.pos, vs.size.x, height1, vs.size.z);
			_aVoxelChunks.Add (vsBottom);

			// create top part
			vs.pos.y += height1;
			int height2 = vs.size.y - height1;
			VoxelUtils.VoxelChunk vsTop = createVoxelChunk(vs.pos, vs.size.x, height2, vs.size.z);
			_aVoxelChunks.Add (vsTop);
		}

		// ---------------------------------------------------------------------------------------------
		private void separateVoxelChunkAlongZAxis(int index, int pointCutZ) {

			//Debug.Log ("    ->separateVoxelChunkAlongZAxis "+pointCutZ.ToString());

			VoxelUtils.VoxelChunk vs = _aVoxelChunks [index];
			Destroy (vs.go);
			_aVoxelChunks.RemoveAt (index);

			int frontZ = vs.corners.bot_left_front.z;
			int depth1 = Mathf.Abs(pointCutZ - frontZ);

			// create front part
			VoxelUtils.VoxelChunk vsFront = createVoxelChunk(vs.pos, vs.size.x, vs.size.y, depth1);
			_aVoxelChunks.Add (vsFront);

			// create back part
			vs.pos.z += depth1;
			int depth2 = vs.size.z - depth1;
			VoxelUtils.VoxelChunk vsBack = createVoxelChunk(vs.pos, vs.size.x, vs.size.y, depth2);
			_aVoxelChunks.Add (vsBack);
		}

		// ---------------------------------------------------------------------------------------------
		// create single voxel
		// ---------------------------------------------------------------------------------------------
		private VoxelUtils.VoxelChunk createVoxelChunk(VoxelUtils.VoxelVector3Int p, int w, int h, int d) {

			GameObject cube = AssetFactory.Instance.createVoxelChunkClone();
			cube.transform.SetParent (_trfmVoxelChunkContainer);
			_iCount++;
			cube.name = "voxchunk_" + _iCount.ToString ();

			float width  = w * VoxelUtils.CHUNK_SIZE;
			float height = h * VoxelUtils.CHUNK_SIZE;
			float depth  = d * VoxelUtils.CHUNK_SIZE;

			Vector3 pos = new Vector3 ((p.x * VoxelUtils.CHUNK_SIZE) + (width / 2f), (p.y * VoxelUtils.CHUNK_SIZE) + (height / 2f), (p.z * VoxelUtils.CHUNK_SIZE) + (depth / 2f));

			Bounds b = new Bounds ();
			b.size = new Vector3 (width, height, depth);
			b.center = pos;

			VoxelUtils.VoxelChunk vs = new VoxelUtils.VoxelChunk ();
			vs.go      = cube;
			vs.goPos   = pos;
			vs.pos     = p;
			vs.size    = new VoxelUtils.VoxelVector3Int(w, h, d);
			vs.bounds  = b;
			vs.corners = VoxelUtils.createVoxelCorners (p, w, h, d);
			vs.meshCreated = false;

			return vs;
		}

		// ---------------------------------------------------------------------------------------------
		// 
		// ---------------------------------------------------------------------------------------------
		private void setVoxelChunkMesh(VoxelUtils.VoxelChunk vc)
		{
			vc.go.transform.localPosition = vc.goPos;
			Mesh mesh = vc.go.GetComponent<MeshFilter> ().mesh;
			VoxelChunkMesh.create (mesh, vc.size.x * VoxelUtils.CHUNK_SIZE, vc.size.y * VoxelUtils.CHUNK_SIZE, vc.size.z * VoxelUtils.CHUNK_SIZE, vc.size.x, vc.size.y, vc.size.z, false);

			BoxCollider coll = vc.go.GetComponent<BoxCollider> ();
			coll.size = new Vector3 (vc.size.x * VoxelUtils.CHUNK_SIZE, vc.size.y * VoxelUtils.CHUNK_SIZE, vc.size.z * VoxelUtils.CHUNK_SIZE);
		}

		// ---------------------------------------------------------------------------------------------
		// 
		// ---------------------------------------------------------------------------------------------
		private VoxelUtils.VoxelChunk createCutVoxelChunk(VoxelUtils.VoxelVector3Int p, int w, int h, int d) {

			float width  = w * VoxelUtils.CHUNK_SIZE;
			float height = h * VoxelUtils.CHUNK_SIZE;
			float depth  = d * VoxelUtils.CHUNK_SIZE;

			Vector3 pos = new Vector3 ((p.x * VoxelUtils.CHUNK_SIZE) + (width / 2f), (p.y * VoxelUtils.CHUNK_SIZE) + (height / 2f), (p.z * VoxelUtils.CHUNK_SIZE) + (depth / 2f));

			Bounds b = new Bounds (); //coll.bounds;
			b.size = new Vector3 (width - VoxelUtils.CHUNK_SIZE, height - VoxelUtils.CHUNK_SIZE, depth - VoxelUtils.CHUNK_SIZE);
			b.center = pos;

			VoxelUtils.VoxelChunk vs = new VoxelUtils.VoxelChunk ();
			vs.pos     = p;
			vs.size    = new VoxelUtils.VoxelVector3Int(w, h, d);
			vs.bounds  = b;
			vs.corners = VoxelUtils.createVoxelCorners (p, w, h, d);
			vs.meshCreated = false;

			return vs;
		}
	}
}