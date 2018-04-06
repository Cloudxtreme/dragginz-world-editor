//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace VoxelChunks
{
	public class VoxelChunkDemo4 : MonoBehaviour
	{
		public GameObject prefabCube;
		public GameObject prefabCut;

		public Transform aimHelper;

		public Text txtHelp;
		public Text txtCount;
		public Text txtError;

		public Transform _voxelChunkContainer;
		public Transform _voxelChunkMeshesContainer;

		public ConvertLevelToMesh _ConvertLevelToMesh;

		public GameObject playerEdit;
		public GameObject playerPlay;

		public Light _light;

		//
		private int _curStep;

		private int _iCount;

		private List<VoxelUtils.VoxelChunk> _aVoxelChunks;

		private Vector3 _curDigSize;

		private List<Vector3> _cutHolesPos;
		private List<Vector3> _cutHolesSize;
		private int _cutHolesIndex;

		// ---------------------------------------------------------------------------------------------
		// Init shit
		// ---------------------------------------------------------------------------------------------
		void Awake () {

			_curStep = 0;
			_curDigSize = new Vector3 (4, 6, 4);

			_cutHolesPos   = new List<Vector3> ();
			_cutHolesSize  = new List<Vector3> ();
			_cutHolesIndex = 0;

			createProceduralLevelData ();
		}

		void Start () {

			playerEdit.SetActive (true);
			playerPlay.SetActive (false);

			_light.enabled = true;
			aimHelper.gameObject.SetActive(false);

			txtError.text = "";
			txtHelp.text  = "Click to create procedural level";

			_iCount = 0;

			_aVoxelChunks = new List<VoxelUtils.VoxelChunk> ();

			// create the full chunk voxel
			VoxelUtils.VoxelVector3Int pos = VoxelUtils.convertVector3ToVoxelVector3Int(Vector3.zero);
			VoxelUtils.VoxelChunk vs = createVoxelChunk(pos, VoxelUtils.MAX_CHUNK_UNITS, VoxelUtils.MAX_CHUNK_UNITS, VoxelUtils.MAX_CHUNK_UNITS);
			_aVoxelChunks.Add (vs);
			setVoxelChunkMesh (_aVoxelChunks [0]);

			txtCount.text = _aVoxelChunks.Count.ToString() + " Voxel Chunk" + (_aVoxelChunks.Count > 1 ? "s" : "");

			aimHelper.localScale = new Vector3(_curDigSize.x * VoxelUtils.CHUNK_SIZE * 1.01f, _curDigSize.y * VoxelUtils.CHUNK_SIZE * 1.01f, _curDigSize.z * VoxelUtils.CHUNK_SIZE * 1.01f);
		}
		
		// ---------------------------------------------------------------------------------------------
		// Update shit
		// ---------------------------------------------------------------------------------------------
		void Update () {

			if (Input.GetMouseButtonDown (0)) {

				if (!EventSystem.current.IsPointerOverGameObject ()) {

					if (_curStep == 0) {
						_curStep = 1;
					}
					else if (_curStep == 2) {
						
						_curStep = 3;
					}
				}
			}

			if (_curStep == 1) {
				
				if (!createProceduralLevel ()) {

					_curStep = 2;
					txtHelp.text = "Click to polygonize level";
				}
			}
			else if (_curStep == 3) {

				txtHelp.text  = "Running...";
				txtCount.text = "Hold tight!";
				_curStep = 4;
			}
			else if (_curStep == 4) {
				
				_curStep = 5;

				_voxelChunkContainer.gameObject.SetActive (false);
				_voxelChunkMeshesContainer.gameObject.SetActive (true);

				float timer = Time.realtimeSinceStartup;
				float[] voxels = convertChunksToVoxels ();
				_ConvertLevelToMesh.create (VoxelUtils.MAX_CHUNK_UNITS, VoxelUtils.MAX_CHUNK_UNITS, VoxelUtils.MAX_CHUNK_UNITS, voxels, Vector3.zero);

				txtHelp.text  = "Use mouse and arrow keys to move!";
				txtCount.text = "Processing time: " + (Time.realtimeSinceStartup - timer).ToString ("F2") + "sec.";

				playerEdit.SetActive (false);
				playerPlay.SetActive (true);
				playerPlay.transform.position = new Vector3 (34, 33, 1);
			}
		}

		// ---------------------------------------------------------------------------------------------
		private bool createProceduralLevel()
		{
			subtractChunk (_cutHolesPos[_cutHolesIndex], _cutHolesSize[_cutHolesIndex]);

			return (++_cutHolesIndex < _cutHolesPos.Count);
		}

		// ---------------------------------------------------------------------------------------------
		private float[] convertChunksToVoxels()
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
				txtError.text = "Looks like we got ourselves an endless loop here!";
				success = false;
			}
			else
			{
				int i, len = _aVoxelChunks.Count;
				for (i = 0; i < len; ++i) {
					setVoxelChunkMesh (_aVoxelChunks [i]);
				}
			}

			Debug.Log ("Time to create chunk(s): " + (Time.realtimeSinceStartup - timer).ToString ());

			txtCount.text = _aVoxelChunks.Count.ToString() + " Voxel Chunk" + (_aVoxelChunks.Count > 1 ? "s" : "");

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

			GameObject cube = Instantiate(prefabCube);
			cube.transform.SetParent (_voxelChunkContainer);
			_iCount++;
			cube.name = "cube_" + _iCount.ToString ();

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
			vs.bounds = b;//coll.bounds;
			vs.corners = VoxelUtils.createVoxelCorners (p, w, h, d);

			return vs;
		}

		// ---------------------------------------------------------------------------------------------
		// 
		// ---------------------------------------------------------------------------------------------
		private void setVoxelChunkMesh(VoxelUtils.VoxelChunk vc)
		{
			vc.go.transform.localPosition = vc.goPos;
			Mesh mesh = vc.go.GetComponent<MeshFilter> ().mesh;
			VoxelChunkMesh.create (mesh, vc.size.x * VoxelUtils.CHUNK_SIZE, vc.size.y * VoxelUtils.CHUNK_SIZE, vc.size.z * VoxelUtils.CHUNK_SIZE, vc.size.x, vc.size.x, vc.size.x, false);

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
			vs.bounds = b; //coll.bounds;
			vs.corners = VoxelUtils.createVoxelCorners (p, w, h, d);

			return vs;
		}

		// ---------------------------------------------------------------------------------------------
		private void createProceduralLevelData ()
		{
			Vector3 digSize  = new Vector3 (4, 6, 4);
			Vector3 startPos = new Vector3 (VoxelUtils.MAX_CHUNK_UNITS - digSize.x - 1, VoxelUtils.MAX_CHUNK_UNITS - digSize.y - 1, 1);

			// go around to the right

			while ((startPos.z+digSize.z) < (VoxelUtils.MAX_CHUNK_UNITS - 1))
			{
				_cutHolesSize.Add (digSize);
				_cutHolesPos.Add (startPos);

				startPos.y -= 1;
				startPos.z += digSize.z;
			}

			startPos.x -= digSize.x;
			startPos.y -= 1;
			startPos.z -= digSize.z;
			while (startPos.x > 0)
			{
				_cutHolesSize.Add (digSize);
				_cutHolesPos.Add (startPos);

				startPos.y -= 1;
				startPos.x -= digSize.x;
			}

			startPos.x += digSize.x;
			startPos.y -= 1;
			startPos.z -= digSize.z;
			while (startPos.z > 0)
			{
				_cutHolesSize.Add (digSize);
				_cutHolesPos.Add (startPos);

				startPos.y -= 1;
				startPos.z -= digSize.z;
			}

			startPos.x += digSize.x;
			startPos.y -= 1;
			startPos.z += digSize.z;
			while (((startPos.x+digSize.x) < (VoxelUtils.MAX_CHUNK_UNITS - 1)) && (startPos.y > 0))
			{
				_cutHolesSize.Add (digSize);
				_cutHolesPos.Add (startPos);

				startPos.y -= 1;
				startPos.x += digSize.x;
			}

			// reached ground - create hall

			startPos.x -= digSize.x;
			startPos.y += 1;
			startPos.z += digSize.z;
			_cutHolesSize.Add (digSize);
			_cutHolesPos.Add (startPos);

			startPos.x = 1;
			startPos.z += digSize.z;
			_cutHolesSize.Add (new Vector3(VoxelUtils.MAX_CHUNK_UNITS - 2, 6, 48));
			_cutHolesPos.Add (startPos);

			// go around to the left

			startPos = new Vector3 (VoxelUtils.MAX_CHUNK_UNITS - digSize.x - 1, VoxelUtils.MAX_CHUNK_UNITS - digSize.y - 1, 1);
			while (startPos.x > 0)
			{
				_cutHolesSize.Add (digSize);
				_cutHolesPos.Add (startPos);

				startPos.y -= 1;
				startPos.x -= digSize.x;
			}

			startPos.x += digSize.x;
			startPos.y -= 1;
			startPos.z += digSize.z;
			while ((startPos.z+digSize.z) < (VoxelUtils.MAX_CHUNK_UNITS - 1))
			{
				_cutHolesSize.Add (digSize);
				_cutHolesPos.Add (startPos);

				startPos.y -= 1;
				startPos.z += digSize.z;
			}
		}

		// ---------------------------------------------------------------------------------------------
		/*private void createDummyGameObject(Vector3 p, int w, int h, int d) {

			GameObject cube = Instantiate(prefabCut);
			cube.transform.SetParent (_voxelChunkContainer);
			cube.name = "cube_cut";

			float width  = w * VoxelUtils.CHUNK_SIZE;
			float height = h * VoxelUtils.CHUNK_SIZE;
			float depth  = d * VoxelUtils.CHUNK_SIZE;
			cube.transform.localScale = new Vector3(width, height, depth);

			Vector3 pos = new Vector3 ((p.x * VoxelUtils.CHUNK_SIZE) + (width / 2f), (p.y * VoxelUtils.CHUNK_SIZE) + (height / 2f), (p.z * VoxelUtils.CHUNK_SIZE) + (depth / 2f));
			cube.transform.localPosition = pos;
		}*/
	}
}