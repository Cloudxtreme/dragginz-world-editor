//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using UnityEngine;

namespace VoxelChunks
{
	public class VoxelCubeMesh
	{
		private int xSize, ySize, zSize;
		private float triSize;

		private Mesh mesh;
		private Vector3[] vertices;
		private Vector2[] uv;

		public VoxelCubeMesh (int x, int y, int z, float tS)
		{
			xSize = x;
			ySize = y;
			zSize = z;

			triSize = tS;
		}

		public Mesh create ()
		{
			mesh = new Mesh();
			mesh.name = "VoxelChunk";

			Debug.Log ("create VoxelChunkMesh "+xSize+", "+ySize+", "+zSize+" - "+triSize);

			CreateVertices();
			CreateTriangles();

			mesh.RecalculateNormals ();
			//mesh.RecalculateTangents ();
			mesh.RecalculateBounds ();

			return mesh;
		}

		private void CreateVertices ()
		{
			int cornerVertices = 8;
			int edgeVertices = (xSize + ySize + zSize - 3) * 4;
			int faceVertices = (
				(xSize - 1) * (ySize - 1) +
				(xSize - 1) * (zSize - 1) +
				(ySize - 1) * (zSize - 1)) * 2;
			
			vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

			uv = new Vector2[vertices.Length];

			int v = 0;
			for (int y = 0; y <= ySize; y++) {
				for (int x = 0; x <= xSize; x++) {
					vertices[v++] = new Vector3(x*triSize, y*triSize, 0);
					uv[v-1] = new Vector2(vertices[v-1].x, vertices[v-1].y);
				}
				for (int z = 1; z <= zSize; z++) {
					vertices[v++] = new Vector3(xSize*triSize, y*triSize, z*triSize);
					uv[v-1] = new Vector2(vertices[v-1].y, vertices[v-1].z);
				}
				for (int x = xSize - 1; x >= 0; x--) {
					vertices[v++] = new Vector3(x*triSize, y*triSize, zSize*triSize);
					uv[v-1] = new Vector2(vertices[v-1].x, vertices[v-1].y);
				}
				for (int z = zSize - 1; z > 0; z--) {
					vertices[v++] = new Vector3(0, y*triSize, z*triSize);
					uv[v-1] = new Vector2(vertices[v-1].y, vertices[v-1].z);
				}
			}
			for (int z = 1; z < zSize; z++) {
				for (int x = 1; x < xSize; x++) {
					vertices[v++] = new Vector3(x*triSize, ySize*triSize, z*triSize);
					uv[v-1] = new Vector2(vertices[v-1].x, vertices[v-1].z);
				}
			}
			for (int z = 1; z < zSize; z++) {
				for (int x = 1; x < xSize; x++) {
					vertices[v++] = new Vector3(x*triSize, 0, z*triSize);
					uv[v-1] = new Vector2(vertices[v-1].x, vertices[v-1].z);
				}
			}

			/*int len = uv.Length;
			for (int i = 0; i < len; i++) {
				uv[i] = new Vector2(vertices[i].x, vertices[i].z);
			}*/

			mesh.vertices = vertices;
			//mesh.uv = uv;
		}

		private void CreateTriangles () {
			int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
			int[] triangles = new int[quads * 6];
			int ring = (xSize + zSize) * 2;
			int t = 0, v = 0;

			for (int y = 0; y < ySize; y++, v++) {
				for (int q = 0; q < ring - 1; q++, v++) {
					t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
				}
				t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
			}

			t = CreateTopFace(triangles, t, ring);
			t = CreateBottomFace(triangles, t, ring);
			mesh.triangles = triangles;
		}

		private int CreateTopFace (int[] triangles, int t, int ring) {
			int v = ring * ySize;
			for (int x = 0; x < xSize - 1; x++, v++) {
				t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
			}
			t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

			int vMin = ring * (ySize + 1) - 1;
			int vMid = vMin + 1;
			int vMax = v + 2;

			for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++) {
				t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
				for (int x = 1; x < xSize - 1; x++, vMid++) {
					t = SetQuad(
						triangles, t,
						vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
				}
				t = SetQuad(triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
			}

			int vTop = vMin - 2;
			t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
			for (int x = 1; x < xSize - 1; x++, vTop--, vMid++) {
				t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
			}
			t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

			return t;
		}

		private int CreateBottomFace (int[] triangles, int t, int ring) {
			int v = 1;
			int vMid = vertices.Length - (xSize - 1) * (zSize - 1);
			t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
			for (int x = 1; x < xSize - 1; x++, v++, vMid++) {
				t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
			}
			t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

			int vMin = ring - 2;
			vMid -= xSize - 2;
			int vMax = v + 2;

			for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++) {
				t = SetQuad(triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
				for (int x = 1; x < xSize - 1; x++, vMid++) {
					t = SetQuad(
						triangles, t,
						vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
				}
				t = SetQuad(triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
			}

			int vTop = vMin - 1;
			t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
			for (int x = 1; x < xSize - 1; x++, vTop--, vMid++) {
				t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
			}
			t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

			return t;
		}

		private static int
		SetQuad (int[] triangles, int i, int v00, int v10, int v01, int v11) {
			triangles[i] = v00;
			triangles[i + 1] = triangles[i + 4] = v01;
			triangles[i + 2] = triangles[i + 3] = v10;
			triangles[i + 5] = v11;
			return i + 6;
		}
	}
}