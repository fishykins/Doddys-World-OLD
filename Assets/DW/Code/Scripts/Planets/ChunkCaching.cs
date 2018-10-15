using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    [RequireComponent(typeof(Planet))]
    public class ChunkCaching : MonoBehaviour
    {
        #region Variables
        [Range(0.01f, 1f)]
        public float renderDistance = 0.5f;
        public Transform[] triggerObjects;

        private Planet planet;
        #endregion

        #region Unity Methods
        private void Start()
        {
            planet = GetComponent<Planet>();
        }
        #endregion

        #region Custom Methods
        public void CacheChunks(Chunk[] chunks)
        {
            for (int j = 0; j < chunks.Length; j++) {
                if (chunks[j].hasMesh) {
                    chunks[j].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < triggerObjects.Length; i++) {
                float nearestDist = Mathf.Infinity;
                Chunk nearestChunk = null;
                LatLon position = GeoCoord.GetLatLon(planet, triggerObjects[i].position);

                for (int j = 0; j < chunks.Length; j++) {
                    float dist = GeoCoord.GetDistance(position, chunks[j].centre);
                    if (dist < nearestDist) {
                        nearestDist = dist;
                        nearestChunk = chunks[j];
                    }
                }

                if (nearestChunk != null) {
                    ActivateChunk(nearestChunk);
                    Chunk[] connectedChunks = nearestChunk.ConnectedChunks;
                    for (int j = 0; j < connectedChunks.Length; j++) {
                        ActivateChunk(connectedChunks[j]);
                    }
                }
            }


            
        }

        private void ActivateChunk(Chunk chunk)
        {
            if (chunk.hasMesh) {
                chunk.gameObject.SetActive(true);
            }
            else {
                Debug.Log(chunk.chunkID + " has been triggered by cache: generating...");
                chunk.ConstructMesh();
            }
        }
        #endregion
    }
}