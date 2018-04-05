//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

//using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace VoxelChunks
{
	public class ConvertLevelToMesh : MonoBehaviour
    {
        public Material m_material;

		public VoxelUtils.MARCHING_MODE mode = VoxelUtils.MARCHING_MODE.CUBES;

        public int seed = 0;

        List<GameObject> meshes = new List<GameObject>();

		public void resetAll()
		{
			int i, len = meshes.Count;
			for (i = 0; i < len; ++i) {
				Destroy(meshes[i]);
				meshes[i] = null;
			}
			meshes.Clear();
		}

		public void create(int width, int height, int depth, float[] voxels, Vector3 pos)
        {
			resetAll ();

			seed = (int)(Time.time * 10f);

			//Debug.Log ("seed: " + seed);
			//Debug.Log ("w, h, d: " + width + ", " + height + ", " + depth);
			//Debug.Log ("voxels: " + voxels.Length + " - pos: " + pos);

            INoise perlin = new PerlinNoise(seed, 2.0f);
            FractalNoise fractal = new FractalNoise(perlin, 3, 1.0f);

            //Set the mode used to create the mesh.
            //Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface.
            Marching marching = null;
			if (mode == VoxelUtils.MARCHING_MODE.TETRAHEDRON) {
				marching = new MarchingTertrahedron ();
			} else {
				marching = new MarchingCubes ();
			}

            //Surface is the value that represents the surface of mesh
            //For example the perlin noise has a range of -1 to 1 so the mid point is where we want the surface to cut through.
            //The target value does not have to be the mid point it can be any value with in the range.
            marching.Surface = 0.0f;

			/*
            //The size of voxel array.
            int width = 32;
            int height = 32;
			int depth = 32;

			float[] voxels = new float[width * height * depth];

            //Fill voxels with values. Im using perlin noise but any method to create voxels will work.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
					for (int z = 0; z < depth; z++)
                    {
                        float fx = x / (width - 1.0f);
                        float fy = y / (height - 1.0f);
						float fz = z / (depth - 1.0f);

                        int idx = x + y * width + z * width * height;

                        voxels[idx] = fractal.Sample3D(fx, fy, fz);
                    }
                }
            }
			*/

            List<Vector3> verts = new List<Vector3>();
            List<int> indices = new List<int>();

            //The mesh produced is not optimal. There is one vert for each index.
            //Would need to weld vertices for better quality mesh.
			marching.Generate(voxels, width, height, depth, verts, indices);

            //A mesh in unity can only be made up of 65000 verts.
            //Need to split the verts between multiple meshes.

            int maxVertsPerMesh = 30000; //must be divisible by 3, ie 3 verts == 1 triangle
            int numMeshes = verts.Count / maxVertsPerMesh + 1;

			List<Vector3> splitVerts = new List<Vector3>();
			List<int> splitIndices = new List<int>();
			//List<Vector2> splitUVs = new List<Vector2> ();

			Renderer renderer;
			MeshFilter filter;
			MeshCollider collider;

			int i, j, idx;
            for (i = 0; i < numMeshes; i++)
            {
                for (j = 0; j < maxVertsPerMesh; j++)
                {
                    idx = i * maxVertsPerMesh + j;

                    if (idx < verts.Count)
                    {
                        splitVerts.Add(verts[idx]);
                        splitIndices.Add(j);
						//splitUVs.Add (new Vector2 (0f, 1f));
                    }
                }

                if (splitVerts.Count == 0) continue;

				//Debug.Log ("verts: " + splitVerts.Count);
				//Debug.Log ("indices: " + splitIndices.Count);

                Mesh mesh = new Mesh();
                mesh.SetVertices(splitVerts);
                mesh.SetTriangles(splitIndices, 0);
				//mesh.SetUVs (0, splitUVs);
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                GameObject go = new GameObject("Mesh");
                go.transform.parent = transform;
                filter = go.AddComponent<MeshFilter>();
                renderer = go.AddComponent<MeshRenderer>();
                renderer.material = m_material;
                filter.mesh = mesh;
				collider = go.AddComponent<MeshCollider> ();

				go.transform.localPosition = pos; //new Vector3(-width / 2, -height / 2, -depth / 2);

                meshes.Add(go);

				splitVerts.Clear ();
				splitIndices.Clear ();
				//splitUVs.Clear ();
            }
        }

        /*void Update()
        {
            transform.Rotate(Vector3.up, 10.0f * Time.deltaTime);
        }*/
    }
}