using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw.planet {
    public class Face : PlanetRenderObject
    {
        #region Variables
        //Public
        public readonly string name;
        public readonly int index;
        public readonly Vector3 axisA;
        public readonly Vector3 axisB;
        public readonly Vector3 axisC;

        public int north;
        public int south;
        public int west;
        public int east;
        public int correctionNorth;
        public int correctionSouth;
        public int correctionWest;
        public int correctionEast;

        public Planet planet;
        #endregion;

        #region Constructor
        public Face(string name, int index, Vector3 axisA, Vector3 axisB, int north, int east, int south, int west, int correctionNorth, int correctionEast, int correctionSouth, int correctionWest, Planet planet)
        {
            this.name = name;
            this.index = index;
            this.planet = planet;
            this.north = north;
            this.south = south;
            this.west = west;
            this.east = east;
            this.correctionNorth = correctionNorth;
            this.correctionSouth = correctionSouth;
            this.correctionWest = correctionWest;
            this.correctionEast = correctionEast;

            this.axisA = axisA;
            this.axisB = axisB;
            this.axisC = Vector3.Cross(axisA, axisB);

            //Debug.Log("Face " + index + " (" + name + ") has been created");
        }
        #endregion

        #region Custom Methods
        public MeshFilter InitializeMesh(MeshFilter meshFilter, bool render)
        {
            if (meshFilter == null) {
                GameObject meshObject = new GameObject(name);
                meshObject.transform.parent = planet.transform;

                meshObject.AddComponent<MeshRenderer>();
                meshFilter = meshObject.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = new Mesh();
            }

            meshFilter.GetComponent<MeshRenderer>().sharedMaterial = planet.graphicalSettings.planetMaterial;
            meshFilter.gameObject.SetActive(render);

            this.meshObject = meshFilter.gameObject;

            return meshFilter;
        }


        public void ConstructMesh(Mesh mesh, int resolution, ShapeGenerator shapeGenerator)
        {
            if (mesh != null) {
                Vector3[] vertices = new Vector3[resolution * resolution];
                int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
                int triIndex = 0;

                for (int y = 0; y < resolution; y++) {
                    for (int x = 0; x < resolution; x++) {
                        int i = x + y * resolution;
                        Vector2 percent = new Vector2(x, y) / (resolution - 1);
                        Vector3 pointOnUnitCube = axisC + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
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

        #endregion;
    }
}