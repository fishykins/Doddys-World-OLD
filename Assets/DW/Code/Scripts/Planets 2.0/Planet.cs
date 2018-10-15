using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dw;

/// <summary>
/// This is the base planet component. 
/// </summary>

namespace dw.planet {
    [RequireComponent(typeof(PlanetGenerator))]
	public class Planet : GravitationalBody
    {
        #region Variables
        //Public
        [Header("Data Settings")]
        public ShapeSettings shapeSettings;
        public GraphicalSettings graphicalSettings;
        public EnviromentalSettings enviromentalSettings;

        [Header("Render Settings")]
        [Range(1,9)]
        public int faceResolution = 5;
        public LODInfo[] detailLevels;
        public Transform player;
        public static float maxViewDst;
        public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
        public FaceRenderMask faceRenderMask;
        public bool autoUpdate = true;

        //Private
        private PlanetGenerator planetGenerator;

        private bool initialized = false;
        private bool renderFaces = true;

        const float viewerMoveThresholdForChunkUpdate = 25f;
        const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
        private Vector3 playerPos;
        private Vector3 playerPosOld = Vector3.zero;

        private Dictionary<IUV, Chunk> chunkDictionary = new Dictionary<IUV, Chunk>();
        private List<Chunk> chunksVisibleLastUpdate = new List<Chunk>();
        private List<Chunk> chunks = new List<Chunk>();

        [SerializeField, HideInInspector]
        Cube cube;
        [SerializeField, HideInInspector]
        MeshFilter[] faceMeshFilters;
        [SerializeField, HideInInspector]
        Face[] faces;


        [HideInInspector]
        public bool shapeSettingsFoldout;
        [HideInInspector]
        public bool graphicalSettingsFoldout;
        [HideInInspector]
        public bool enviromentalSettingsFoldout;

        #endregion;

        #region Properties
        public PlanetGenerator PlanetGenerator { get { return planetGenerator; } }
        public Dictionary<IUV, Chunk> ChunkDictionary { get { return chunkDictionary; } }
        #endregion;

        #region Unity Methods
        protected override void Start () {
            base.Start();
            CreateGamePlanet();
        }

        private void Update () {
            playerPos = player.transform.position;
            if ((playerPosOld - playerPos).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
                playerPosOld = playerPos;
                UpdateVissibleChunks();
            }
            
        }
        private void OnValidate()
        {
            CreateDebugPlanet();
        }
        #endregion;

        #region Custom Methods
        public void CreateGamePlanet()
        {
            InitializePlanet();
            InitializeChunks();
            InitializeFaces(false);
        }

        public void CreateDebugPlanet()
        {
            InitializePlanet();
            InitializeFaces(true);
        }

        public void InitializePlanet()
        {
            planetGenerator = GetComponent<PlanetGenerator>();
            planetGenerator.planet = this;
            cube = planetGenerator.cubeGenerator.GenerateCube(this);
            faces = cube.faces;
            PlanetGenerator.CreateGenerators(false); //True will force a new generator to be created, false will only init if none already made
            PlanetGenerator.UpdateGenerators();
        }

        //Generates the absolute base level chunks with minimal data. Only works in play mode
        private void InitializeChunks()
        {
            if (Application.isPlaying) {
                foreach (Face face in faces) {
                    for (int u = 0; u < faceResolution; u++) {
                        for (int v = 0; v < faceResolution; v++) {
                            Chunk newChunk = new Chunk(this, face, u, v);
                            chunks.Add(newChunk);
                            chunkDictionary.Add(newChunk.faceCoord, newChunk);

                            //For debug only- auto genereate planet
                            //newChunk.UpdateChunk();
                        }
                    }
                }
            }

            //Once all the chunks have initialized, we can hook them up with mapping coordinates
            foreach (Chunk chunk in chunks) {
                chunk.FindConnected();
            }

            Debug.Log("------- CHUNKS CREATED: " + chunks.Count + " ---------");
        }

        //Generates meshes for faces if needs be
        private void InitializeFaces(bool generateMeshes)
        {
            if (cube != null) {

                if (faceMeshFilters == null || faceMeshFilters.Length == 0) {
                    faceMeshFilters = new MeshFilter[6];
                }

                for (int i = 0; i < faces.Length; i++) {
                    Face face = faces[i];

                    faceMeshFilters[i] = face.InitializeMesh(faceMeshFilters[i], generateMeshes);

                    if (generateMeshes) {
                        face.ConstructMesh(faceMeshFilters[i].sharedMesh, 128, PlanetGenerator.shapeGenerator);
                        face.SetVisible(true);
                    }
                    else {
                        face.SetVisible(false);
                    }
                }
            } else {
                Debug.LogError("No cube has been created- cannot initialize faces. ");
            }
        }


        //Will run if player position has changed by a set amount
        private void UpdateVissibleChunks()
        {
            //Hide all currently vissible chunks
            for (int i = 0; i < chunksVisibleLastUpdate.Count; i++) {
                chunksVisibleLastUpdate[i].SetVisible(false);
            }
            chunksVisibleLastUpdate.Clear();

            Chunk nearestChunk = FindNearestChunk(player);

            if (nearestChunk != null) {
                nearestChunk.SetVisible(true);
                nearestChunk.UpdateChunk();
                chunksVisibleLastUpdate.Add(nearestChunk);

                foreach (Chunk chunk in nearestChunk.ConnectedChunks) {
                    chunk.SetVisible(true);
                    chunk.UpdateChunk();
                    chunksVisibleLastUpdate.Add(chunk);
                }
            }
        }

        //Public method for getting the nearest chunk to selected point
        public Chunk FindNearestChunk(Transform target)
        {
            Chunk nearestChunk = null;

            if (target != null && chunks.Count > 0) {
                float nearestDist = Mathf.Infinity;
                
                LatLon position = GeoCoord.GetLatLon(this, target.position);

                for (int i = 0; i < chunks.Count; i++) {
                    float dist = GeoCoord.GetDistance(position, chunks[i].LatLon);
                    if (dist < nearestDist) {
                        nearestDist = dist;
                        nearestChunk = chunks[i];
                    }
                }
            } else {
                Debug.LogWarning("No transform provided!");
            }
            return nearestChunk;
        }

        #endregion

        #region Editor Update Methods 
        public void OnShapeSettingsUpdated()
        {
            if (autoUpdate) {
                CreateDebugPlanet();
            }
        }
        public void OnGraphicalSettingsUpdated()
        {

        }
        public void OnEnviromentalSettingsUpdated()
        {

        }
        #endregion
    }

    //LOD settings
    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public int chunkDistance;
    }


    //Chunk coord system
    public struct IUV
    {
        public int i;
        public int u;
        public int v;

        public IUV(int i, int u, int v)
        {
            this.i = i;
            this.u = u;
            this.v = v;
        }
    }
}