using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw.planet {
	public class LodData {

        #region Variables
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        public int lod;
        System.Action updateCallback;
        PlanetGenerator planetGenerator;
        Chunk chunk;
        #endregion;

        #region constructor
        //called in Chunk init
        public LodData(int lod, System.Action updateCallback, PlanetGenerator planetGenerator)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
            this.planetGenerator = planetGenerator;
        }
        #endregion

        #region Custom Methods
        void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;

            updateCallback();
        }

        public void RequestMesh(Chunk chunk)
        {
            this.chunk = chunk;
            hasRequestedMesh = true;
            planetGenerator.RequestmeshData(chunk, lod, OnMeshDataReceived);
        }
        #endregion;
    }
}