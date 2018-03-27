//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelChunks
{
	public class VoxelChunkDemo : MonoBehaviour
	{
		public GameObject prefabCube;
		public GameObject prefabCut;

		private int _iCount;

		private List<VoxelUtils.VoxelChunk> _aVoxelChunks;


		// ---------------------------------------------------------------------------------------------
		// Init shit
		// ---------------------------------------------------------------------------------------------
		void Start () {

			_iCount = 0;

			_aVoxelChunks = new List<VoxelUtils.VoxelChunk> ();

			// create the full chunk voxel
			VoxelUtils.VoxelVector3Int pos = VoxelUtils.convertVector3ToVoxelVector3Int(Vector3.zero);
			VoxelUtils.VoxelChunk vs = createVoxelChunk(pos, VoxelUtils.MAX_CHUNK_UNITS, VoxelUtils.MAX_CHUNK_UNITS, VoxelUtils.MAX_CHUNK_UNITS);
			_aVoxelChunks.Add (vs);

			// cut out center section and separate voxels
			pos = VoxelUtils.convertVector3ToVoxelVector3Int(new Vector3(33,33,33));
			subtractChunk(pos, 6, 6, 6);
		}
		
		// ---------------------------------------------------------------------------------------------
		// Update shit
		// ---------------------------------------------------------------------------------------------
		void Update () {

		}

		// ---------------------------------------------------------------------------------------------
		// cut a hole!
		// ---------------------------------------------------------------------------------------------
		private void subtractChunk(VoxelUtils.VoxelVector3Int pos, int w, int h, int d)
		{
			VoxelUtils.VoxelChunk vsCut = createCutGameObject(pos, w, h, d);

			// does the new voxel intersect with any existing voxels?
			bool splittage = splitVoxels (vsCut);
			int loops = 0;
			while (splittage && loops < 1000) {
				splittage = splitVoxels (vsCut);
				loops++;
			}

			if (loops >= 1000) Debug.LogWarning("looks like we got ourselves an endless loop here!");
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

					Debug.Log ("bounds intersect detected: "+_aVoxelChunks[i].go.name);

					// check for identical size and position
					if (_aVoxelChunks [i].Identical (vsCut)) {

						Debug.LogWarning ("    ->IDENTICAL!");
						Destroy (_aVoxelChunks [i].go);
						_aVoxelChunks.RemoveAt (i);
						intersectDetected = true;
					}
					// check for collision along left X axis
					else if (_aVoxelChunks [i].IntersectsX( vsCut.corners.bot_left_front )) {

						separateVoxelChunkAlongXAxis (i, vsCut.corners.bot_left_front.x);
						intersectDetected = true;
					}
					// check for collision along right X axis
					else if (_aVoxelChunks [i].IntersectsX( vsCut.corners.bot_right_front )) {

						separateVoxelChunkAlongXAxis (i, vsCut.corners.bot_right_front.x + 1);
						intersectDetected = true;
					}
					// check for collision along bottom Y axis
					else if (_aVoxelChunks [i].IntersectsY( vsCut.corners.bot_left_front )) {

						separateVoxelChunkAlongYAxis (i, vsCut.corners.bot_left_front.y);
						intersectDetected = true;
					}
					// check for collision along top Y axis
					else if (_aVoxelChunks [i].IntersectsY( vsCut.corners.top_left_front )) {

						separateVoxelChunkAlongYAxis (i, vsCut.corners.top_left_front.y + 1);
						intersectDetected = true;
					}
					// check for collision along front Z axis
					else if (_aVoxelChunks [i].IntersectsZ( vsCut.corners.bot_left_front )) {

						separateVoxelChunkAlongZAxis (i, vsCut.corners.bot_left_front.z);
						intersectDetected = true;
					}
					// check for collision along back Z axis
					else if (_aVoxelChunks [i].IntersectsZ( vsCut.corners.bot_left_back )) {

						separateVoxelChunkAlongZAxis (i, vsCut.corners.bot_left_back.z + 1);
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
		private void separateVoxelChunkAlongXAxis(int index, int pointCutX) {

			Debug.Log ("    ->separateVoxelChunkAlongXAxis "+pointCutX.ToString());

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

			Debug.Log ("    ->separateVoxelChunkAlongYAxis "+pointCutY.ToString());

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

			Debug.Log ("    ->separateVoxelChunkAlongZAxis "+pointCutZ.ToString());

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
			_iCount++;
			cube.name = "cube_" + _iCount.ToString ();

			float width  = w * VoxelUtils.CHUNK_SIZE;
			float height = h * VoxelUtils.CHUNK_SIZE;
			float depth  = d * VoxelUtils.CHUNK_SIZE;
			cube.transform.localScale = new Vector3(width, height, depth);

			Vector3 pos = new Vector3 ((p.x * VoxelUtils.CHUNK_SIZE) + (width / 2f), (p.y * VoxelUtils.CHUNK_SIZE) + (height / 2f), (p.z * VoxelUtils.CHUNK_SIZE) + (depth / 2f));
			cube.transform.position = pos;

			BoxCollider coll = cube.GetComponent<BoxCollider> ();

			VoxelUtils.VoxelChunk vs = new VoxelUtils.VoxelChunk ();
			vs.go      = cube;
			vs.pos     = p;
			vs.size    = new VoxelUtils.VoxelVector3Int(w, h, d);
			vs.bounds  = coll.bounds;
			vs.corners = VoxelUtils.createVoxelCorners (p, w, h, d);

			//Debug.Log ("pos: "+vs.pos.ToString());
			//Debug.Log ("size: "+vs.size.ToString());
			//Debug.Log ("new chunk corners: "+vs.corners.ToString());
			//Debug.Log (vs.bounds);

			return vs;
		}

		// ---------------------------------------------------------------------------------------------
		// 
		// ---------------------------------------------------------------------------------------------
		private VoxelUtils.VoxelChunk createCutGameObject(VoxelUtils.VoxelVector3Int p, int w, int h, int d) {

			GameObject cube = Instantiate(prefabCut);
			cube.name = "cube_cut";

			float width  = w * VoxelUtils.CHUNK_SIZE;
			float height = h * VoxelUtils.CHUNK_SIZE;
			float depth  = d * VoxelUtils.CHUNK_SIZE;
			cube.transform.localScale = new Vector3(width, height, depth);

			Vector3 pos = new Vector3 ((p.x * VoxelUtils.CHUNK_SIZE) + (width / 2f), (p.y * VoxelUtils.CHUNK_SIZE) + (height / 2f), (p.z * VoxelUtils.CHUNK_SIZE) + (depth / 2f));
			cube.transform.position = pos;

			BoxCollider coll = cube.GetComponent<BoxCollider> ();

			VoxelUtils.VoxelChunk vs = new VoxelUtils.VoxelChunk ();
			vs.go      = cube;
			vs.pos     = p;
			vs.size    = new VoxelUtils.VoxelVector3Int(w, h, d);
			vs.bounds  = coll.bounds;
			vs.corners = VoxelUtils.createVoxelCorners (p, w, h, d);

			//Debug.Log ("cut chunk corners: "+vs.corners.ToString());

			return vs;
		}
	}
}