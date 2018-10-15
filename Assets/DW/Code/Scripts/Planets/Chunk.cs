using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    public class Chunk
    {
        #region Variables
        //Public
        public string chunkID;
        public GameObject gameObject;
        public Planet planet;
        public Face face;
        public LatLon centre;
        public Vector3 boundsCentre;
        public bool hasMesh = false;

        //Private
        private Mesh mesh;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private ShapeGenerator shapeGenerator;
        private ShapeData shapeData;
        private Chunk[] connectedChunks = new Chunk[8];

        int faceI;
        int faceU;
        int faceV;
        int meshSimplificationIncrement;
        int verticePerLine;
        #endregion

        #region Constants
        //Important value, dont mess with me!
        const int chunkResolution = 241;
        #endregion

        #region properties
        public Chunk[] ConnectedChunks { get { return connectedChunks; } }
        public GameObject GameObject { get { return gameObject; } }
        #endregion


        #region Custom Methods
        public Chunk(Planet planet, int i, int u, int v, ShapeGenerator shapeGenerator)
        {
            this.shapeGenerator = shapeGenerator;
            this.planet = planet;
            this.face = planet.terrainFaces[i];
            this.faceI = i;
            this.faceU = u;
            this.faceV = v;
            this.shapeData = this.planet.shapeData;

            chunkID = "Chunk " + faceI + " (" + faceU + "," + faceV + ")";

            SetLOD();

            //Centre point
            int ResM1 = chunkResolution - 1;

            float UVstep = 1f / shapeData.chunksPerFace;
            float step = UVstep / ResM1;
            Vector2 offset = new Vector3((-0.5f + faceU * UVstep), (-0.5f + faceV * UVstep));

            int x = chunkResolution / 2;

            Vector2 p = offset + new Vector2(x * step, x * step);
            Vector3 pointOnUnitCube = face.axisB * p.x + face.axisA * p.y + face.height * -0.5f;
            Vector3 pointOnUnitSphere = SphereMaths.UnitCubeToUnitSphere(pointOnUnitCube);
            Vector3 point = planet.ShapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);
            this.centre = GeoCoord.GetLatLon(planet, point);
        }

        public void SetLOD()
        {
            meshSimplificationIncrement = (shapeData.levelOfDetail == 0) ? 1 : shapeData.levelOfDetail * 2;
            verticePerLine = (chunkResolution - 1) / meshSimplificationIncrement + 1;
        }

        public void ConstructMesh()
        {

            //Generate chunk object
            gameObject = new GameObject("chunk " + face.name + " (" + faceI + "," + faceU + "," + faceV + ")");

            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshCollider = gameObject.AddComponent<MeshCollider>();
            //meshRenderer.sharedMaterial = planet.material;
            mesh = meshFilter.sharedMesh = new Mesh();
            mesh.name = "ChunkMesh";
            meshCollider.sharedMesh = mesh;
            gameObject.transform.parent = planet.transform;
            gameObject.transform.localEulerAngles = Vector3.zero;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.layer = LayerMask.NameToLayer("Terrain");
            gameObject.tag = "Terrain";


            int ResM1 = chunkResolution - 1;

            Vector3[] vertices = new Vector3[verticePerLine * verticePerLine];
            int[] triangles = new int[verticePerLine * verticePerLine * 6];
            int triIndex = 0;

            Vector2 UVstep = new Vector2(1f / shapeData.chunksPerFace, 1f / shapeData.chunksPerFace);
            Vector2 step = new Vector2(UVstep.x / ResM1, UVstep.y / ResM1);
            Vector2 offset = new Vector3((-0.5f + faceU * UVstep.x), (-0.5f + faceV * UVstep.y));

            //Generate points on mesh
            for (int y = 0; y < chunkResolution; y+= meshSimplificationIncrement) {
                for (int x = 0; x < chunkResolution; x+= meshSimplificationIncrement) {
                    int i = (x/ meshSimplificationIncrement) + (y/ meshSimplificationIncrement) * verticePerLine;
                    //Debug.Log("I = " + i + "(" + x + "," + y + ")");
                    Vector2 p = offset + new Vector2(x * step.x, y * step.y);
                    Vector3 pointOnUnitCube = face.axisB * p.x + face.axisA * p.y + face.height * -0.5f;
                    Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                    vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                    if (x != ResM1 && y != ResM1) {
                        triangles[triIndex] = i;
                        triangles[triIndex + 1] = i + verticePerLine + 1;
                        triangles[triIndex + 2] = i + verticePerLine;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 4] = i + 1;
                        triangles[triIndex + 5] = i + verticePerLine + 1;
                        triIndex += 6;
                    }
                }
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            meshCollider.sharedMesh = mesh;
            meshRenderer.sharedMaterial = planet.graphicalData.planetMaterial;

            hasMesh = true;

            //Debug.Log("Generated " + chunk.name + " mesh");
        }


        public void FindConnected()
        {
            int edgeLimit = planet.shapeData.chunksPerFace;
            int chunkIndex = 0;

            //Debug.Log("---------" + chunkID + "---------");

            for (int u = faceU -1; u <= faceU+1; u ++) {
                for (int v = faceV -1; v <= faceV + 1; v ++) {

                    int edgeCount = 0;

                    int rotation = 0;

                    int newI = 0;
                    int newU = 0;
                    int newV = 0;
                    

                    //Debug.Log(chunkID + ": neightbor " + " = " + faceI + "," + u + "," + v);

                    if (u >= edgeLimit) {
                        edgeCount++;
                        //Find North edge
                        newI = planet.connectedFaceNorth[faceI];
                        newU = 0;
                        newV = v;
                        rotation = planet.adjacentFaceCorrection[faceI, newI];
                    }

                    if (u < 0) {
                        edgeCount++;
                        //Find South edge
                        newI = planet.connectedFaceSouth[faceI];
                        newU = edgeLimit - 1;
                        newV = v;
                        rotation = planet.adjacentFaceCorrection[faceI, newI];
                    }

                    if (v >= edgeLimit) {
                        edgeCount++;
                        //Find East edge
                        newI = planet.connectedFaceEast[faceI];
                        newU = u;
                        newV = 0;
                        rotation = planet.adjacentFaceCorrection[faceI, newI];
                    }

                    if (v < 0) {
                        edgeCount++;
                        //Find West edge
                        newI = planet.connectedFaceWest[faceI];
                        newU = u;
                        newV = edgeLimit - 1;
                        rotation = planet.adjacentFaceCorrection[faceI, newI];
                    }

                    //If we have more than 2 edges, this is a virtual space and can be ignored
                    if (edgeCount < 2) {
                        if (edgeCount == 1) {
                            //One edge found- this chunk is on a different side of the cube! 
                            if (rotation != 0) {
                                //Rotate the face to get the correct lineup
                                int[] result = RotateUV(newU, newV, edgeLimit, rotation);
                                newU = result[0];
                                newV = result[1];
                            }
                        }
                        else {
                            //No edges found so its on this side of the cube
                            newU = u;
                            newV = v;
                            newI = faceI;
                        }

                        //Add this chunk!
                        Chunk selectedChunk = planet.chunkMap[newI, newU, newV];
                        if (selectedChunk != this) {
                            connectedChunks[chunkIndex] = planet.chunkMap[newI, newU, newV];

                            //Debug.Log(chunkIndex + ": " + connectedChunks[chunkIndex].chunkID + " (edge count " + edgeCount + ")");

                            chunkIndex++;
                        }
                    }
                }
            }

            //Clean up the array
            Chunk[] tempArray = new Chunk[chunkIndex];
            for (int i = 0; i < chunkIndex; i++) {
                tempArray[i] = connectedChunks[i];
            }
            connectedChunks = tempArray;
        }

        private int[] RotateUV(int u, int v, int width, int amount)
        {
            int tempU;
            int tempV;
            for (int i = 0; i < amount; i++) {
                tempU = (width - 1 - v);
                tempV = u;
                u = tempU;
                v = tempV;
            }
            int[] ret = {u, v};
            return ret;
        }
        #endregion
    }
}