//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DragginzVoxelWorldEditor
{
	public class ConvertLevelToMesh : MonoBehaviour
    {
        public Material m_material;

		public VoxelUtils.MARCHING_MODE mode = VoxelUtils.MARCHING_MODE.CUBES;

		private int width, height, depth;
		private float[] voxels;
		private Vector3 pos;

		private Marching marching;

		private List<Vector3> verts;
		private List<int> indices;

        private List<GameObject> meshes = new List<GameObject>();

		//
		public void resetAll()
		{
			int i, len = meshes.Count;
			for (i = 0; i < len; ++i) {
				Destroy(meshes[i]);
				meshes[i] = null;
			}
			meshes.Clear();
		}

		//
		public void setData(int w, int h, int d, float[] v, Vector3 p)
		{
			resetAll ();

			//seed = (int)(Time.time * 10f);

			width = w;
			height = h;
			depth = d;
			voxels = v;
			pos = p;

			// Set the mode used to create the mesh.
			// Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface.
			marching = null;
			if (mode == VoxelUtils.MARCHING_MODE.TETRAHEDRON) {
				marching = new MarchingTertrahedron ();
			} else {
				marching = new MarchingCubes ();
			}

			// Surface is the value that represents the surface of mesh
			// For example the perlin noise has a range of -1 to 1 so the mid point is where we want the surface to cut through.
			// The target value does not have to be the mid point it can be any value with in the range.
			marching.Surface = 0.0f;

			verts = new List<Vector3>();
			indices = new List<int>();
		}

		//
		public void march()
		{
			// The mesh produced is not optimal. There is one vert for each index.
			// Would need to weld vertices for better quality mesh.
			float timer = Time.realtimeSinceStartup;
			marching.Generate (voxels, width, height, depth, verts, indices);
			Debug.LogWarning ("Time to run marching cubes: " + (Time.realtimeSinceStartup - timer).ToString ());
		}

		public void split()
		{
			int numVerts = verts.Count;

            // A mesh in unity can only be made up of 65000 verts.
            // Need to split the verts between multiple meshes.
            int maxVertsPerMesh = 30000; //must be divisible by 3, ie 3 verts == 1 triangle
			int numMeshes = numVerts / maxVertsPerMesh + 1;

			List<Vector3> splitVerts = new List<Vector3>();
			List<int> splitIndices = new List<int>();
			//List<Vector2> splitUVs = new List<Vector2> ();

			Renderer renderer;
			MeshFilter filter;
			//MeshCollider collider;

			float timer = Time.realtimeSinceStartup;
			int i, j, idx;
            for (i = 0; i < numMeshes; i++)
            {
                for (j = 0; j < maxVertsPerMesh; j++)
                {
                    idx = i * maxVertsPerMesh + j;

					if (idx < numVerts)
                    {
                        splitVerts.Add(verts[idx]);
                        splitIndices.Add(j);
						//splitUVs.Add (new Vector2 (0f, 1f));
                    }
                }

                if (splitVerts.Count == 0) continue;

                Mesh mesh = new Mesh();
                mesh.SetVertices(splitVerts);
                mesh.SetTriangles(splitIndices, 0);
				//mesh.SetUVs (0, splitUVs);

				//Debug.Log ("verts: " + splitVerts.Count);
				AutoWeld (mesh, 0.001f, 1f);
				//Debug.Log ("mesh.vertices: " + mesh.vertices.Length);

                mesh.RecalculateBounds();
                //mesh.RecalculateNormals();

                GameObject go = new GameObject("Mesh");
                go.transform.parent = transform;
                filter = go.AddComponent<MeshFilter>();
                renderer = go.AddComponent<MeshRenderer>();
				renderer.material = LevelEditor.Instance.aToolMaterials [0];
                filter.mesh = mesh;
				go.AddComponent<MeshCollider> (); //collider = 

				go.transform.localPosition = pos;

                meshes.Add(go);

				splitVerts.Clear ();
				splitIndices.Clear ();
				//splitUVs.Clear ();
            }
			Debug.LogWarning ("Time to split mesh data: " + (Time.realtimeSinceStartup - timer).ToString ());
        }

		//
		private void AutoWeld (Mesh mesh, float threshold, float bucketStep)
		{
			int i, len;

			Vector3[] oldVertices = mesh.vertices;
			Vector3[] newVertices = new Vector3[oldVertices.Length];
			int[] old2new = new int[oldVertices.Length];
			int newSize = 0;

			// Find AABB
			Vector3 min = new Vector3 (float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 max = new Vector3 (float.MinValue, float.MinValue, float.MinValue);
			len = oldVertices.Length;
			for (i = 0; i < len; i++) {
				if (oldVertices[i].x < min.x) min.x = oldVertices[i].x;
				if (oldVertices[i].y < min.y) min.y = oldVertices[i].y;
				if (oldVertices[i].z < min.z) min.z = oldVertices[i].z;
				if (oldVertices[i].x > max.x) max.x = oldVertices[i].x;
				if (oldVertices[i].y > max.y) max.y = oldVertices[i].y;
				if (oldVertices[i].z > max.z) max.z = oldVertices[i].z;
			}

			// Make cubic buckets, each with dimensions "bucketStep"
			int bucketSizeX = Mathf.FloorToInt ((max.x - min.x) / bucketStep) + 1;
			int bucketSizeY = Mathf.FloorToInt ((max.y - min.y) / bucketStep) + 1;
			int bucketSizeZ = Mathf.FloorToInt ((max.z - min.z) / bucketStep) + 1;
			List<int>[,,] buckets = new List<int>[bucketSizeX, bucketSizeY, bucketSizeZ];

			// Make new vertices
			len = oldVertices.Length;
			for (i = 0; i < len; i++) {
				// Determine which bucket it belongs to
				int x = Mathf.FloorToInt ((oldVertices[i].x - min.x) / bucketStep);
				int y = Mathf.FloorToInt ((oldVertices[i].y - min.y) / bucketStep);
				int z = Mathf.FloorToInt ((oldVertices[i].z - min.z) / bucketStep);

				// Check to see if it's already been added
				if (buckets[x, y, z] == null) {
					buckets[x, y, z] = new List<int> (); // Make buckets lazily
				}

				int j, len2 = buckets [x, y, z].Count;
				for (j = 0; j < len2; j++) {
					Vector3 to = newVertices[buckets[x, y, z][j]] - oldVertices[i];
					if (Vector3.SqrMagnitude (to) < threshold) {
						old2new[i] = buckets[x, y, z][j];
						goto skip; // Skip to next old vertex if this one is already there
					}
				}

				// Add new vertex
				newVertices[newSize] = oldVertices[i];
				buckets[x, y, z].Add (newSize);
				old2new[i] = newSize;
				newSize++;

				skip:;
			}

			// Make new triangles
			int[] oldTris = mesh.triangles;
			int[] newTris = new int[oldTris.Length];
			len = oldTris.Length;
			for (i = 0; i < len; i++) {
				newTris[i] = old2new[oldTris[i]];
			}

			Vector3[] finalVertices = new Vector3[newSize];
			for (i = 0; i < newSize; i++) {
				finalVertices [i] = newVertices [i];
			}

			mesh.Clear();
			mesh.vertices = finalVertices;
			mesh.triangles = newTris;

			mesh.RecalculateNormals();

			Vector3[] normals = mesh.normals;
			Vector2[] finalUV = new Vector2[newSize];

			Vector3 v1, v2, v3;
			Vector3 side1, side2, perp;

			len = newTris.Length;
			for (i = 0; i < len; i += 3)
			{
				v1 = finalVertices[newTris[i]];
				v2 = finalVertices[newTris[i + 1]];
				v3 = finalVertices[newTris[i + 2]];

				side1 = v2 - v1;
				side2 = v3 - v1;

				perp = Vector3.Cross(side1, side2);
				perp.Normalize ();

				finalUV [newTris [i]]     = new Vector2 (v1.x, v1.z);
				finalUV [newTris [i + 1]] = new Vector2 (v2.x, v2.z);
				finalUV [newTris [i + 2]] = new Vector2 (v3.x, v3.z);

				if (perp.x > 0.5f || perp.x < -0.5f) {
					finalUV [newTris [i]]     = new Vector2 (v1.y, v1.z);
					finalUV [newTris [i + 1]] = new Vector2 (v2.y, v2.z);
					finalUV [newTris [i + 2]] = new Vector2 (v3.y, v3.z);
				}
				else if (perp.z > 0.5f || perp.z < -0.5f) {
					finalUV [newTris [i]]     = new Vector2 (v1.x, v1.y);
					finalUV [newTris [i + 1]] = new Vector2 (v2.x, v2.y);
					finalUV [newTris [i + 2]] = new Vector2 (v3.x, v3.y);
				}
			}

			mesh.uv = finalUV;
		}
	}
}