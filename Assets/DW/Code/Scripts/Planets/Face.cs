using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw {
    public class Face
    {

        ShapeGenerator shapeGenerator;
        Mesh mesh;
        public string name;
        public Vector3 axisA;
        public Vector3 axisB;
        public Vector3 height;
        int resolution;

        public Face(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 a, Vector3 b, string name)
        {
            this.name = name;
            this.shapeGenerator = shapeGenerator;
            this.mesh = mesh;
            this.resolution = resolution;

            axisA = a;
            axisB = b;
            height = Vector3.Cross(axisA, axisB);
        }

        public void ConstructMesh()
        {
            Vector3[] vertices = new Vector3[resolution * resolution];
            int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
            int triIndex = 0;

            for (int y = 0; y < resolution; y++) {
                for (int x = 0; x < resolution; x++) {
                    int i = x + y * resolution;
                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    Vector3 pointOnUnitCube = height + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                    Vector3 pointOnUnitSphere = SphereMaths.UnitCubeToUnitSphere(pointOnUnitCube);
                    vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                    if (x != resolution - 1 && y != resolution - 1) {
                        triangles[triIndex] = i;
                        triangles[triIndex + 1] = i + resolution + 1;
                        triangles[triIndex + 2] = i + resolution;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 4] = i + 1;
                        triangles[triIndex + 5] = i + resolution + 1;
                        triIndex += 6;
                    }
                }
            }
            //mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }
    }
}