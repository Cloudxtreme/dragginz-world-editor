//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelChunks
{
	public class VoxelTest : MonoBehaviour
	{
		public GameObject prefabCube;
		public GameObject prefabCut;

		//
		struct voxelStruct
		{
			public Transform transform;
			public Bounds bounds;
			public Vector3[] corners;

			public voxelStruct(Transform t, Bounds b, Vector3[] c)
			{
				this.transform = t;
				this.bounds    = b;
				this.corners   = c;
			}
		};

		private int _iCount;

		private List<voxelStruct> _aVoxels;


		// ---------------------------------------------------------------------------------------------
		// Init shit
		// ---------------------------------------------------------------------------------------------
		void Start () {

			_iCount = 0;

			_aVoxels = new List<voxelStruct> ();

			// create the full chunk voxel
			_aVoxels.Add( createVoxel(Vector2.zero, new Vector3(VoxelUtils.CHUNK_SIZE, VoxelUtils.CHUNK_SIZE, VoxelUtils.CHUNK_SIZE)) );

			// cut out center section and separate voxels
			//subtract(Vector2.zero, new Vector3(5.0f, 5.0f, 5.0f));
		}
		
		// ---------------------------------------------------------------------------------------------
		// Update shit
		// ---------------------------------------------------------------------------------------------
		void Update () {

		}

		// ---------------------------------------------------------------------------------------------
		// cut a hole!
		// ---------------------------------------------------------------------------------------------
		private void subtract(Vector3 pos, Vector3 scale) {

			Bounds b = new Bounds ();
			b.center = pos;
			b.size   = scale;

			createCutGameObject (pos, scale);

			// does the new voxel intersect with any existing voxels?
			bool splittage = splitVoxels (b);
			int loops = 0;
			while (splittage && loops < 1) {
				splitVoxels (b);
				loops++;
			}

			if (loops >= 1000) Debug.LogWarning("looks like we got ourselves an endless loop here!");
		}

		// ---------------------------------------------------------------------------------------------
		// split them voxels one at a time
		// ---------------------------------------------------------------------------------------------
		private bool splitVoxels(Bounds b) {

			bool intersectDetected = false;

			Vector3 point = Vector3.zero;
			int i, len = _aVoxels.Count;
			for (i = 0; i < len; ++i) {

				// intersecting?
				if (b.Intersects(_aVoxels[i].bounds)) {

					Debug.Log ("intersect detected: "+i);

					// check all 8 corners for collision

					// y0 z0 x0
					point = new Vector3 (b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z);
					if (_aVoxels [i].bounds.Contains (point)) {

						separateVoxelAlongX (i, point.x);
					}

					intersectDetected = true;
					break;
				}
			}

			return intersectDetected;
		}

		// ---------------------------------------------------------------------------------------------
		// create single voxel
		// ---------------------------------------------------------------------------------------------
		private void separateVoxelAlongX(int index, float xCut) {

			Debug.Log ("    ->separateVoxelAlongX");

			voxelStruct vs = _aVoxels [index];
			Bounds bOrig = vs.bounds;

			Destroy (vs.transform.gameObject);
			_aVoxels.RemoveAt (index);

			float leftX  = (bOrig.center.x - bOrig.extents.x);
			float width1 = (xCut - leftX);
			float width2 = bOrig.size.x - width1;

			// create left side
			Vector3 size1 = bOrig.size;
			size1.x = width1;

			Vector3 center1 = bOrig.center;
			center1.x = leftX + (width1 / 2f);
			_aVoxels.Add( createVoxel (center1, size1) );

			// create right side
			Vector3 size2 = bOrig.size;
			size2.x = width2;

			Vector3 center2 = bOrig.center;
			center2.x = leftX + bOrig.size.x - (width2 / 2f);
			_aVoxels.Add( createVoxel (center2, size2) );

			//Debug.Log ("new voxels intersecting: " + vs1.bounds.Intersects (vs2.bounds));
		}
			
		// ---------------------------------------------------------------------------------------------
		// create single voxel
		// ---------------------------------------------------------------------------------------------
		private voxelStruct createVoxel(Vector3 pos, Vector3 scale) {

			GameObject cube = Instantiate(prefabCube);
			_iCount++;
			cube.name = "cube_" + _iCount.ToString ();
			cube.transform.position   = pos;
			cube.transform.localScale = scale;

			BoxCollider coll = cube.GetComponent<BoxCollider> ();
			Bounds b = coll.bounds;

			Vector3[] corners = new Vector3 [8];
			corners[VoxelUtils.LEFT_TOP_FRONT]  = new Vector3 (b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z);
			corners[VoxelUtils.LEFT_TOP_BACK]   = new Vector3 (b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z);
			corners[VoxelUtils.RIGHT_TOP_FRONT] = new Vector3 (b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z);
			corners[VoxelUtils.RIGHT_TOP_BACK]  = new Vector3 (b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z);
			corners[VoxelUtils.LEFT_BOT_FRONT]  = new Vector3 (b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z);
			corners[VoxelUtils.LEFT_BOT_BACK]   = new Vector3 (b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z);
			corners[VoxelUtils.RIGHT_BOT_FRONT] = new Vector3 (b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z);
			corners[VoxelUtils.RIGHT_BOT_BACK]  = new Vector3 (b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z);

			voxelStruct vs = new voxelStruct (cube.transform, b, corners);

			return vs;
		}

		// ---------------------------------------------------------------------------------------------
		// DEBUG
		// ---------------------------------------------------------------------------------------------
		private void createCutGameObject(Vector3 pos, Vector3 scale) {

			GameObject cube = Instantiate(prefabCut);
			cube.name = "cube_cut";
			cube.transform.position = pos;
			cube.transform.localScale = scale;
		}
	}
}