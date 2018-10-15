using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the main component required to turn an object into a planet!
/// </summary>

namespace dw
{
    public class Planet : GravitationalBody
    {

        #region Variables
        //Public Data types
        [Header("Planet Settings")]
        public GraphicalData graphicalData;
        public ShapeData shapeData;
        public EnviromentalData enviromentalData;
        public bool generateChunksOnStart = false;

        [Header("Debug Settings")]
        public bool debug = false;
        public bool autoUpdate = true;
        public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
        public FaceRenderMask faceRenderMask;

        //Private Variables
        [HideInInspector]
        public bool shapeSettingsFoldout;
        [HideInInspector]
        public bool graphicalSettingsFoldout;
        [HideInInspector]
        public bool enviromentalSettingsFoldout;

        private Vector3 centre;

        ShapeGenerator shapeGenerator = new ShapeGenerator();
        GraphicalGenerator graphicalGenerator = new GraphicalGenerator();
        ChunkCaching chunkCacher;

        [SerializeField, HideInInspector]
        MeshFilter[] faceMeshFilters;

        public Face[] terrainFaces;
        Chunk[] chunks;
        int chunkIndex = 0;
        bool renderFaces = false;

        [HideInInspector]
        public int[] connectedFaceNorth = new int[6];
        [HideInInspector]
        public int[] connectedFaceEast = new int[6];
        [HideInInspector]
        public int[] connectedFaceSouth = new int[6];
        [HideInInspector]
        public int[] connectedFaceWest = new int [6];
        [HideInInspector]
        public int[,] adjacentFaceCorrection = new int[6, 6];
        #endregion

        #region Properties 
        public ShapeGenerator ShapeGenerator { get { return shapeGenerator; } }
        public GraphicalGenerator GraphicalGenerator { get { return graphicalGenerator; } }
        public Chunk[] Chunks { get { return chunks; } }
        public Chunk[,,] chunkMap;
        #endregion

        #region Unity Methods
        protected override void Start()
        {
            //debugBody = debug;
            base.Start();
            CreatePlanet();
        }

        private void OnValidate()
        {
            CreateDebugPlanet();
        }

        private void Update()
        {
            if (chunkCacher) {
                chunkCacher.CacheChunks(chunks);
            }
        }
        #endregion

        #region Custom Methods
        public void CreateDebugPlanet()
        {
            renderFaces = true;
            InitializePlanet();
            GenerateFaces();
            GenerateGraphics();
        }

        public void CreatePlanet()
        {
            renderFaces = false;
            InitializePlanet();
            if (generateChunksOnStart) {
                GenerateChunks();
            }
            GenerateGraphics();
        }

        private void InitializePlanet()
        {
            //Update settings of our generators
            shapeGenerator.UpdateSettings(shapeData, radius);
            graphicalGenerator.UpdateSettings(graphicalData);

            //See if we have a cache component
            chunkCacher = GetComponent<ChunkCaching>();
            if (!chunkCacher) {
                generateChunksOnStart = true;
            }

            InitializeFaces();
            InitializeChunks();
        }

        private void InitializeFaces()
        {
            //Create meshfilters for our cube faces
            if (faceMeshFilters == null || faceMeshFilters.Length == 0) {
                faceMeshFilters = new MeshFilter[6];
            }
            terrainFaces = new Face[6];

            //Setup hard-coded values for our 6 faces
            Vector3[] directionsA = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.right, Vector3.right };
            Vector3[] directionsB = { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward, Vector3.down, Vector3.up };
            string[] names = { "Top", "Down", "Left", "Right", "Forward", "Back" };

            //Generate faces and add components
            for (int i = 0; i < 6; i++) {

                if (faceMeshFilters[i] == null) {
                    GameObject meshObj = new GameObject(names[i]);
                    meshObj.transform.parent = transform;

                    meshObj.AddComponent<MeshRenderer>();
                    faceMeshFilters[i] = meshObj.AddComponent<MeshFilter>();
                    faceMeshFilters[i].sharedMesh = new Mesh();
                }

                //Add the mesh renderer to array for later use!
                faceMeshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = graphicalData.planetMaterial;

                //Create a face class to store our data in
                terrainFaces[i] = new Face(shapeGenerator, faceMeshFilters[i].sharedMesh, shapeData.faceResolution, directionsA[i], directionsB[i], names[i]);
                bool renderFace = (faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i) && renderFaces;
                faceMeshFilters[i].gameObject.SetActive(renderFace);
            }
        }

        private void InitializeChunks()
        {
            #region Face data
            //(0)
            adjacentFaceCorrection[0, 4] = 0;
            adjacentFaceCorrection[0, 3] = 0;
            adjacentFaceCorrection[0, 5] = 0;
            adjacentFaceCorrection[0, 2] = 0;
            connectedFaceNorth[0] = 4;
            connectedFaceEast[0] = 3;
            connectedFaceSouth[0] = 5;
            connectedFaceWest[0] = 2;

            //(1)
            adjacentFaceCorrection[1, 4] = 2;
            adjacentFaceCorrection[1, 2] = 0;
            adjacentFaceCorrection[1, 5] = 2;
            adjacentFaceCorrection[1, 3] = 0;
            connectedFaceNorth[1] = 4;
            connectedFaceEast[1] =2;
            connectedFaceSouth[1] = 5;
            connectedFaceWest[1] = 3;

            //(2)
            adjacentFaceCorrection[2, 4] = 1;
            adjacentFaceCorrection[2, 0] = 0;
            adjacentFaceCorrection[2, 5] = 3;
            adjacentFaceCorrection[2, 1] = 0;
            connectedFaceNorth[2] = 4;
            connectedFaceEast[2] = 0;
            connectedFaceSouth[2] = 5;
            connectedFaceWest[2] = 1;

            //(3)
            adjacentFaceCorrection[3, 4] = 3;
            adjacentFaceCorrection[3, 1] = 0;
            adjacentFaceCorrection[3, 5] = 1;
            adjacentFaceCorrection[3, 0] = 0;
            connectedFaceNorth[3] = 4;
            connectedFaceEast[3] = 1;
            connectedFaceSouth[3] = 5;
            connectedFaceWest[3] = 0;

            //(4)
            adjacentFaceCorrection[4, 1] = 2;
            adjacentFaceCorrection[4, 3] = 1;
            adjacentFaceCorrection[4, 0] = 0;
            adjacentFaceCorrection[4, 2] = 3;
            connectedFaceNorth[4] = 1;
            connectedFaceEast[4] = 3;
            connectedFaceSouth[4] = 0;
            connectedFaceWest[4] = 2;

            //(5)
            adjacentFaceCorrection[5, 0] = 0;
            adjacentFaceCorrection[5, 3] = 3;
            adjacentFaceCorrection[5, 1] = 2;
            adjacentFaceCorrection[5, 2] = 1;
            connectedFaceNorth[5] = 0;
            connectedFaceEast[5] = 3;
            connectedFaceSouth[5] = 1;
            connectedFaceWest[5] = 2;
            #endregion  

            chunkIndex = 0;
            centre = transform.position;

            chunks = new Chunk[6 * shapeData.chunksPerFace * shapeData.chunksPerFace];
            chunkMap = new Chunk[6, shapeData.chunksPerFace, shapeData.chunksPerFace];

            //Build our chunk bases. Will not init a mesh so we can knock them all into existance if we so desire.
            for (int i = 0; i < 6; i++) {
                for (int u = 0; u < shapeData.chunksPerFace; u++) {
                    for (int v = 0; v < shapeData.chunksPerFace; v++) {
                        Chunk newChunk = new Chunk(this, i, u, v, shapeGenerator);
                        chunks[chunkIndex] = newChunk;
                        chunkMap[i, u, v] = newChunk;
                        chunkIndex++;
                    }
                }
            }

            //Allocate neighbors! 
            foreach (Chunk chunk in chunks) {
                chunk.FindConnected();
            }

            Debug.Log("Planet initialized with " + chunks.Length + " chunks.");
        }

        private void GenerateFaces()
        {
            for (int i = 0; i < 6; i++) {
                if (faceMeshFilters[i].gameObject.activeSelf) {
                    terrainFaces[i].ConstructMesh();
                }
            }
            graphicalGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
        }

        private void GenerateChunks()
        {
            foreach (Chunk chunk in chunks) {
                chunk.ConstructMesh();
            }
            graphicalGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
        }

        private void GenerateGraphics()
        {
            graphicalGenerator.UpdateColours();
        }

        void UpdateElevation()
        {
            graphicalGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
            //Override default value for our minimum elevation!
            gravityMinRadius = radius + shapeGenerator.elevationMinMax.Min;
        }

        #endregion

        #region Editor Update Methods 
        public void OnShapeSettingsUpdated() {
            if (autoUpdate) {
                CreateDebugPlanet();
            }
        }
        public void OnGraphicalSettingsUpdated() {
            if (autoUpdate) {
                debug = true;
                InitializePlanet();
                GenerateGraphics();
            }
        }
        public void OnEnviromentalSettingsUpdated() { }
        #endregion

    }
}