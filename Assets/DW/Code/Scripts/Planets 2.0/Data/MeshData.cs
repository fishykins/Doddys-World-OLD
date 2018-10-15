using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw.planet {
	public class MeshData {

        #region Variables
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        int triangleIndex;
        #endregion;

        #region Generator
        public MeshData(int meshSize)
        {
            vertices = new Vector3[meshSize * meshSize];
            uvs = new Vector2[meshSize * meshSize];
            triangles = new int[(meshSize - 1) * (meshSize - 1) * 6];
        }
        #endregion

        #region Custom Methods
        public void AddTriangle(int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        Vector3[] CalculateNormals()
        {
            Vector3[] vertexNormals = new Vector3[vertices.Length];
            int triangleCount = triangles.Length / 3;
            for (int i = 0; i < triangleCount; i++) {
                int normalTriangleIndex = i * 3;
                int vertexIndexA = triangles[normalTriangleIndex];
                int vertexIndexB = triangles[normalTriangleIndex + 1];
                int vertexIndexC = triangles[normalTriangleIndex + 2];

                Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
                vertexNormals[vertexIndexA] += triangleNormal;
                vertexNormals[vertexIndexB] += triangleNormal;
                vertexNormals[vertexIndexC] += triangleNormal;
            }

            for (int i = 0; i < vertexNormals.Length; i++) {
                vertexNormals[i].Normalize();
            }

            return vertexNormals;
        }

        Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
        {
            Vector3 pointA = vertices[indexA];
            Vector3 pointB = vertices[indexB];
            Vector3 pointC = vertices[indexC];

            Vector3 sideAB = pointB - pointA;
            Vector3 sideAC = pointC - pointA;
            return Vector3.Cross(sideAB, sideAC).normalized;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.normals = CalculateNormals();

            Debug.Log("Creating mesh with " + mesh.vertices.Length + " verts and " + mesh.triangles.Length + " triangles");
            return mesh;
        }
        #endregion
    }
}