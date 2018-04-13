//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;
using System.Collections.Generic;

namespace GridEditor
{
	public class ConvertLevelToMesh : MonoBehaviour
    {
        public Material m_material;

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

            Marching marching = new MarchingCubes ();

            marching.Surface = 0.0f;

            List<Vector3> verts = new List<Vector3>();
            List<int> indices = new List<int>();

			marching.Generate(voxels, width, height, depth, verts, indices);

            int maxVertsPerMesh = 30000; //must be divisible by 3, ie 3 verts == 1 triangle
            int numMeshes = verts.Count / maxVertsPerMesh + 1;

			List<Vector3> splitVerts = new List<Vector3>();
			List<int> splitIndices = new List<int>();

			Renderer renderer;
			MeshFilter filter;

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
                    }
                }

                if (splitVerts.Count == 0) continue;

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
				go.AddComponent<MeshCollider> (); //collider = 

				go.transform.localPosition = pos;

                meshes.Add(go);

				splitVerts.Clear ();
				splitIndices.Clear ();
           }
        }
    }
}