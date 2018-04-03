//
// Author  : Oliver Brodhage
// Company : Decentralised Team of Developers
//

using System;
using UnityEngine;

namespace VoxelChunks
{
    /// <summary>
    /// class for creating Box primitive
    /// </summary>
    public class VoxelChunkMesh
    {
        //private static bool dbg;

        /// <summary>
        /// generate mesh geometry for box
        /// </summary>
        /// <param name="mesh">mesh to be generated</param>
        /// <param name="width">width of cube</param>
        /// <param name="height">height of cube</param>
        /// <param name="depth">depth of cube</param>
        /// <param name="widthSegments">number of triangle segments in width direction</param>
        /// <param name="heightSegments">number of triangle segments in height direction</param>
        /// <param name="depthSegments">number of triangle segments in depth direction</param>
        /// <param name="cubeMap">enable 6-sides cube map uv mapping</param>
        /// <param name="edgeOffsets">offsets on edges for creating a ramp</param>
        /// <param name="flipUV">flag to flip uv mapping</param>
        /// <param name="pivot">position of the model pivot</param>
		public static void create(Mesh mesh, float width, float height, float depth, int widthSegments, int heightSegments, int depthSegments, bool cubeMap) //, float[] edgeOffsets, bool flipUV)
        {
            width = Mathf.Clamp(width, 0, 100);
            height = Mathf.Clamp(height, 0, 100);
            depth = Mathf.Clamp(depth, 0, 100);
            heightSegments = Mathf.Clamp(heightSegments, 1, 100);
            widthSegments = Mathf.Clamp(widthSegments, 1, 100);
            depthSegments = Mathf.Clamp(depthSegments, 1, 100);

			//Debug.Log ("createcube mesh - w, h, d, segW, segH, segD: "+width+", "+height+", "+depth+", "+widthSegments+", "+heightSegments+", "+depthSegments);

            mesh.Clear();

            int numTriangles = widthSegments*depthSegments*6 +
                               widthSegments*heightSegments*6 +
                               depthSegments*heightSegments*6;

            int numVertices = (widthSegments + 1)*(depthSegments + 1) +
                              (widthSegments + 1)*(heightSegments + 1) +
                              (depthSegments + 1)*(heightSegments + 1);

            numTriangles *= 2;
            numVertices *= 2;

            var pivotOffset = Vector3.zero;
            /*switch (pivot)
            {
                case PivotPosition.Top: pivotOffset = new Vector3(0.0f, -height/2, 0.0f);
                    break;
                case PivotPosition.Botttom: pivotOffset = new Vector3(0.0f, height/2, 0.0f);
                    break;
            }*/

            if (numVertices > 60000)
            {
                UnityEngine.Debug.LogError("Too much vertices!");
				return;// 0.0f;
            }

            var vertices = new Vector3[numVertices];
            var uvs = new Vector2[numVertices];
            var triangles = new int[numTriangles];

            int vertIndex = 0;
            int triIndex = 0;

            var a0 = new Vector3(-width / 2, pivotOffset.y - height / 2, -depth / 2);
            var b0 = new Vector3(-width / 2, pivotOffset.y - height / 2, depth / 2);
            var c0 = new Vector3(width / 2, pivotOffset.y - height / 2, depth / 2);
            var d0 = new Vector3(width / 2, pivotOffset.y - height / 2, -depth / 2);

            var a1 = new Vector3(-width / 2, height / 2 + pivotOffset.y, -depth / 2);
            var b1 = new Vector3(-width / 2, height / 2 + pivotOffset.y, depth / 2);
            var c1 = new Vector3(width / 2, height / 2 + pivotOffset.y, depth / 2);
            var d1 = new Vector3(width / 2, height / 2 + pivotOffset.y, -depth / 2);

            /*if (edgeOffsets != null && edgeOffsets.Length > 3)
            {
                b1.x += edgeOffsets[0];
                a1.x += edgeOffsets[0];
                b0.x += edgeOffsets[1];
                a0.x += edgeOffsets[1];

                c0.x += edgeOffsets[3];
                c1.x += edgeOffsets[2];
                d0.x += edgeOffsets[3];
                d1.x += edgeOffsets[2];
            }*/

			CreatePlane(0, a0, b0, c0, d0, widthSegments, depthSegments, cubeMap, ref vertices, ref uvs, ref triangles, ref vertIndex, ref triIndex, width, depth);
			CreatePlane(1, b1, a1, d1, c1, widthSegments, depthSegments, cubeMap, ref vertices, ref uvs, ref triangles, ref vertIndex, ref triIndex, width, depth);

			CreatePlane(2, b0, b1, c1, c0, widthSegments, heightSegments, cubeMap, ref vertices, ref uvs, ref triangles, ref vertIndex, ref triIndex, width, height);
			CreatePlane(3, d0, d1, a1, a0, widthSegments, heightSegments, cubeMap, ref vertices, ref uvs, ref triangles, ref vertIndex, ref triIndex, width, height);

			CreatePlane(4, a0, a1, b1, b0, depthSegments, heightSegments, cubeMap, ref vertices, ref uvs, ref triangles, ref vertIndex, ref triIndex, depth, height);
			CreatePlane(5, c0, c1, d1, d0, depthSegments, heightSegments, cubeMap, ref vertices, ref uvs, ref triangles, ref vertIndex, ref triIndex, depth, height);

            /*if (flipUV)
            {
                for (var i = 0; i < uvs.Length; i++)
                {
                    uvs[i].x = 1.0f - uvs[i].x;
                }
            }*/

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;

			// Editor only :( -> MeshUtility.Optimize(mesh);

            mesh.RecalculateNormals();
            //CalculateTangents(mesh);
            mesh.RecalculateBounds();
        }

		static void CreatePlane(int id, Vector3 a, Vector3 b, Vector3 c, Vector3 d, int segX, int segY, bool cubeMap, ref Vector3[] vertices, ref Vector2[] uvs, ref int[] triangles, ref int vertIndex, ref int triIndex, float width, float height)
        {
			var uvFactorX = 1.0f / segX;
			var uvFactorY = 1.0f / segY;

			//float fUVX = 0.01388889f;//(float)segX / 72.0f / (float)segX;
			//float fUVY = 0.01388889f;//(float)segY / 72.0f / (float)segY;
			//Debug.Log ("w, h, segX, segY, fUVX, fUVY: "+width+", "+height+", "+segX+", "+segY+", "+fUVX+", "+fUVY);

            var vDown = d - a;
            var vUp = c - b;

            var vertOffset = vertIndex;

            for (var y = 0.0f; y < segY+1; y++)
            {
                for (var x = 0.0f; x < segX+1; x++)
                {
                    var pDown = a + vDown*y*uvFactorY;
                    var pUp = b + vUp*y*uvFactorY;

                    var v = pDown + (pUp - pDown)*x*uvFactorX;

                    vertices[vertIndex] = v;
					//var uvFactor = new Vector2 (x * uvFactorX, y * uvFactorY);
					uvs[vertIndex] = new Vector2 (x * uvFactorX, y * uvFactorY);;//new Vector2 (x * fUVX, y * fUVY);
                    /*if (cubeMap)
                    {
                        uvs[vertIndex] = GetCube6UV(id/2, id%2, uvFactor);
                    }
                    else
                    {
                        uvs[vertIndex] = uvFactor;
                    }*/

                    vertIndex++;
                }
            }

            var hCount2 = segX + 1;

            for (int y = 0; y < segY; y++)
            {
                for (int x = 0; x < segX; x++)
                {
                    triangles[triIndex + 0] = vertOffset + (y * hCount2) + x;
                    triangles[triIndex + 1] = vertOffset + ((y + 1) * hCount2) + x;
                    triangles[triIndex + 2] = vertOffset + (y * hCount2) + x + 1;

                    triangles[triIndex + 3] = vertOffset + ((y + 1) * hCount2) + x;
                    triangles[triIndex + 4] = vertOffset + ((y + 1) * hCount2) + x + 1;
                    triangles[triIndex + 5] = vertOffset + (y * hCount2) + x + 1;
                    triIndex += 6;
                }
            }
        }

        /// <summary>
        /// generate uv coordinates for a texture with 6 sides of the box
        /// </summary>
        static Vector2 GetCube6UV(int sideID, int paralel, Vector2 factor)
        {
            factor.x = factor.x*0.3f;
            factor.y = factor.y*0.5f;

            switch (sideID)
            {
                case 0:
                    if (paralel == 0)
                    {
                        factor.y += 0.5f;
                        return factor;
                    }
                    else
                    {
                        factor.y += 0.5f;
                        factor.x += 2.0f / 3;
                        return factor;
                    }
                case 1:
                    if (paralel == 0)
                    {
                        factor.x += 1.0f / 3;
                        return factor;
                    }
                    else
                    {
                        factor.x += 2.0f / 3;
                        return factor;
                    }
                case 2:
                    if (paralel == 0)
                    {
                        factor.y += 0.5f;
                        factor.x += 1.0f / 3;
                        return factor;
                    }
                    else
                    {
                        return factor;
                    }
            }

            return Vector2.zero;
        }

		static void CalculateTangents(Mesh mesh)
		{
			var vertexCount = mesh.vertexCount;
			var vertices = mesh.vertices;
			var normals = mesh.normals;
			var texcoords = mesh.uv;
			var triangles = mesh.triangles;
			var triangleCount = triangles.Length/3;
			var tangents = new Vector4[vertexCount];
			var tan1 = new Vector3[vertexCount];
			var tan2 = new Vector3[vertexCount];
			var tri = 0;

			for (var i = 0; i < triangleCount; i++)
			{
				var i1 = triangles[tri];
				var i2 = triangles[tri + 1];
				var i3 = triangles[tri + 2];

				var v1 = vertices[i1];
				var v2 = vertices[i2];
				var v3 = vertices[i3];

				var w1 = texcoords[i1];
				var w2 = texcoords[i2];
				var w3 = texcoords[i3];

				var x1 = v2.x - v1.x;
				var x2 = v3.x - v1.x;
				var y1 = v2.y - v1.y;
				var y2 = v3.y - v1.y;
				var z1 = v2.z - v1.z;
				var z2 = v3.z - v1.z;

				var s1 = w2.x - w1.x;
				var s2 = w3.x - w1.x;
				var t1 = w2.y - w1.y;
				var t2 = w3.y - w1.y;

				float div = s1*t2 - s2*t1;
				float r = Math.Abs(div - 0.0f) < 0.0001f ? 0.0f : 1.0f/div;
				var sdir = new Vector3((t2*x1 - t1*x2)*r, (t2*y1 - t1*y2)*r, (t2*z1 - t1*z2)*r);
				var tdir = new Vector3((s1*x2 - s2*x1)*r, (s1*y2 - s2*y1)*r, (s1*z2 - s2*z1)*r);

				tan1[i1] += sdir;
				tan1[i2] += sdir;
				tan1[i3] += sdir;

				tan2[i1] += tdir;
				tan2[i2] += tdir;
				tan2[i3] += tdir;

				tri += 3;
			}

			for (var i = 0; i < (vertexCount); i++)
			{
				var n = normals[i];
				var t = tan1[i];

				// Gram-Schmidt orthogonalize
				Vector3.OrthoNormalize(ref n, ref t);

				tangents[i].x = t.x;
				tangents[i].y = t.y;
				tangents[i].z = t.z;

				// Calculate handedness
				tangents[i].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0) ? -1.0f : 1.0f;
			}

			mesh.tangents = tangents;
		}
	}
}