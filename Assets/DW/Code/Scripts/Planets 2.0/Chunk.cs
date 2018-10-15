using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dw;

namespace dw.planet {
	public class Chunk : PlanetRenderObject
    {
        #region Variables
        //Public
        public readonly Face face;
        public readonly IUV faceCoord;
        public readonly LatLon geoCoord;

        //Private
        private Planet planet;
        private Vector3 centre;
        private LatLon latLon;

        private PlanetGenerator planetGenerator;

        private Mesh mesh;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;

        private ChunkData chunkData;
        private Chunk[] connectedChunks = new Chunk[8];

        private bool hasMesh = false;
        private bool hasData = false;
        private bool hasGenerated = false;

        private int previousLodIndex = -1;
        private int lodIndex = 0;
        private LodData[] lodMeshes;
        #endregion;

        #region Constants
        //Important value, dont mess with me!
        const int resolution = 241;
        #endregion

        #region Properties
        public ChunkData ChunkData { get { return chunkData; } }
        public Chunk[] ConnectedChunks { get { return connectedChunks; } }
        public Vector3 Centre { get { return centre; } }
        public LatLon LatLon { get { return latLon; } }
        public string Name { get { return meshObject.name; } }
        #endregion

        #region Constructor
        public Chunk(Planet planet, Face face, int u, int v)
        {
            this.planet = planet;
            this.face = face;
            this.faceCoord = new IUV(face.index, u, v);
            this.planetGenerator = planet.PlanetGenerator;

            CalculateCentre(planet.faceResolution, planet.radius);

            //Add lod levels based on planet presets
            lodMeshes = new LodData[planet.detailLevels.Length];
            for (int i = 0; i < planet.detailLevels.Length; i++) {
                lodMeshes[i] = new LodData(planet.detailLevels[i].lod, UpdateChunk, planetGenerator);
            }

            //Debug.Log("Chunk [" + iuv.x + "," + iuv.y + "," + iuv.z + "] has been constructed");
        }
        #endregion

        #region Custom Methods
        //Generate Chunk data (usually when triggered by near player)
        public void GenerateChunk ()
        {
            hasGenerated = true;

            meshObject = new GameObject("chunk [" + faceCoord.i + "," + faceCoord.u + "," + faceCoord.v + "]");

            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshCollider = meshObject.AddComponent<MeshCollider>();

            mesh = meshFilter.sharedMesh = new Mesh();
            mesh.name = "ChunkMesh";
            meshCollider.sharedMesh = mesh;
            meshObject.transform.parent = planet.transform;
            meshObject.transform.localEulerAngles = Vector3.zero;
            meshObject.transform.localPosition = Vector3.zero;
            meshObject.layer = LayerMask.NameToLayer("Terrain");
            meshObject.tag = "Terrain";

            planetGenerator.RequestChunkData(this, OnChunkDataReceived);

            Debug.Log(meshObject.name + " has been Generated- sending data request...");

            UpdateChunk();
        }

        private void OnChunkDataReceived(ChunkData data)
        {
            this.chunkData = data;
            hasData = true;

            Debug.Log(meshObject.name + " received Chunk Data!");

            UpdateChunk();
        }

        //Update the current chunk
        public void UpdateChunk()
        {
            if (!hasGenerated) {
                GenerateChunk();
            }

            if (hasData) {

                //GET LOD INDEX HERE!

                //Fetch the mesh or request it if not found
                if (lodIndex != previousLodIndex) {
                    LodData lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh) {
                        previousLodIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                        meshCollider.sharedMesh = lodMesh.mesh;
                        meshRenderer.sharedMaterial = planet.graphicalSettings.planetMaterial;
                    }
                    else if (!lodMesh.hasRequestedMesh) {
                        lodMesh.RequestMesh(this);
                    }
                }
            }
        }

        //Gets a vector for the centre of the chunk. 
        public void CalculateCentre(int chunkResolution, float radius)
        {
            float UVstep = 1f / chunkResolution;
            float step = UVstep / (resolution -1);
            Vector2 offset = new Vector3((-0.5f + faceCoord.u * UVstep), (-0.5f + faceCoord.v * UVstep));

            int x = resolution / 2;

            Vector2 p = offset + new Vector2(x * step, x * step);
            Vector3 pointOnUnitCube = face.axisB * p.x + face.axisA * p.y + face.axisC * -0.5f;
            Vector3 pointOnUnitSphere = SphereMaths.UnitCubeToUnitSphere(pointOnUnitCube);
            this.centre = pointOnUnitSphere * radius;
            this.latLon = GeoCoord.GetLatLon(planet, centre);
        }

        public void FindConnected()
        {
            int edgeLimit = planet.faceResolution;
            int chunkIndex = 0;
            

            //Debug.Log("---------" + chunkID + "---------");

            for (int u = faceCoord.u - 1; u <= faceCoord.u + 1; u++) {
                for (int v = faceCoord.v - 1; v <= faceCoord.v + 1; v++) {

                    int edgeCount = 0;
                    int rotation = 0;
                    IUV newFaceCoord = new IUV(0, 0, 0);

                    if (u >= edgeLimit) {
                        edgeCount++;
                        //Find North edge
                        newFaceCoord = new IUV(face.north, 0, v);
                        rotation = face.correctionNorth;
                    }

                    if (u < 0) {
                        edgeCount++;
                        //Find South edge
                        newFaceCoord = new IUV(face.south, edgeLimit - 1, v);
                        rotation = face.correctionSouth;
                    }

                    if (v >= edgeLimit) {
                        edgeCount++;
                        //Find East edge
                        newFaceCoord = new IUV(face.east, u, 0);
                        rotation = face.correctionEast;
                    }

                    if (v < 0) {
                        edgeCount++;
                        //Find West edge
                        newFaceCoord = new IUV(face.west, u, edgeLimit - 1);
                        rotation = face.correctionWest;
                    }

                    //If we have more than 2 edges, this is a virtual space and can be ignored
                    if (edgeCount < 2) {
                        if (edgeCount == 1) {
                            //One edge found- this chunk is on a different side of the cube! 
                            if (rotation != 0) {
                                //Rotate the face to get the correct lineup
                                newFaceCoord = RotateUV(newFaceCoord, edgeLimit, rotation);
                            }
                        }
                        else {
                            //No edges found so its on this side of the cube
                            newFaceCoord = new IUV(faceCoord.i, u, v);
                        }

                        //Add this chunk!
                        if (planet.ChunkDictionary.ContainsKey(newFaceCoord)) {
                            Chunk selectedChunk = planet.ChunkDictionary[newFaceCoord];
                            if (selectedChunk != this) {
                                connectedChunks[chunkIndex] = selectedChunk;

                                //Debug.Log(chunkIndex + ": " + connectedChunks[chunkIndex].chunkID + " (edge count " + edgeCount + ")");

                                chunkIndex++;
                            }
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

        //Rotates a given IUV coordinate counterclockwise n amount around the face. 
        private IUV RotateUV(IUV coord, int width, int amount)
        {
            IUV temp;
            for (int i = 0; i < amount; i++) {
                temp = new IUV(coord.i, (width - 1 - coord.v), coord.u);
                coord = temp;
            }
            return coord;
        }
        #endregion
    }
}