using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using dw;

namespace dw.planet {
    [RequireComponent(typeof(Planet))]
	public class PlanetGenerator : MonoBehaviour
    {
        #region Variables
        //Public
        [HideInInspector]
        public Planet planet;
        [HideInInspector]
        public CubeGenerator cubeGenerator = new CubeGenerator();
        [HideInInspector]
        public ChunkGenerator chunkGenerator;
        [HideInInspector]
        public MeshGenerator meshGenerator;
        [HideInInspector]
        public ShapeGenerator shapeGenerator = new ShapeGenerator();
        [HideInInspector]
        public GraphicalGenerator graphicalGenerator = new GraphicalGenerator();

        //private
        private Queue<GenThreadInfo<ChunkData>> chunkDataThreadInfoQueue = new Queue<GenThreadInfo<ChunkData>>();
        private Queue<GenThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<GenThreadInfo<MeshData>>();
        #endregion;

        #region Constants
        //Important value, dont mess with me!
        const int resolution = 241;
        #endregion

        #region Properties

        #endregion;


        #region Unity Methods
        private void Update()
        {
            //return data back to chunks
            if (chunkDataThreadInfoQueue.Count > 0) {
                for (int i = 0; i < chunkDataThreadInfoQueue.Count; i++) {
                    GenThreadInfo<ChunkData> threadInfo = chunkDataThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
            if (meshDataThreadInfoQueue.Count > 0) {
                for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
                    GenThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }
        #endregion;

        #region Custom Methods
        public void CreateGenerators(bool forceReset = false)
        {
            if (chunkGenerator == null || forceReset) {
                chunkGenerator = new ChunkGenerator(this, resolution);
            }
            if (meshGenerator == null || forceReset) {
                meshGenerator = new MeshGenerator(this, resolution);
            }
        }

        public void UpdateGenerators()
        {
            shapeGenerator.UpdateSettings(planet.shapeSettings, planet.radius);
            graphicalGenerator.UpdateSettings(planet.graphicalSettings);
            graphicalGenerator.UpdateColours();
        }

        //Chunk requests data using this method
        public void RequestChunkData(Chunk chunk, Action<ChunkData> callback)
        {
            ThreadStart threadStart = delegate {
                ChunkDataThread(chunk, callback);
            };

            new Thread(threadStart).Start();
        }

        //Chunk requests are handled in this thread
        void ChunkDataThread(Chunk chunk, Action<ChunkData> callback)
        {
            ChunkData chunkData = chunkGenerator.GenerateChunkData(chunk);
            lock (chunkDataThreadInfoQueue) {
                chunkDataThreadInfoQueue.Enqueue(new GenThreadInfo<ChunkData>(callback, chunkData));
            }
        }

        //Mesh requests data using this method
        public void RequestmeshData(Chunk chunk, int lod, Action<MeshData> callback)
        {
            ThreadStart threadStart = delegate {
                MeshDataThread(chunk, lod, callback);
            };

            new Thread(threadStart).Start();
        }

        //Mesh requests are handled in this thread
        void MeshDataThread(Chunk chunk, int lod, Action<MeshData> callback)
        {
            Debug.Log("Chunk [" + chunk.faceCoord.i + "," +  chunk.faceCoord.u + "," + chunk.faceCoord.v + "] has requested lod " + lod + "- Thread starting");
            MeshData meshData = meshGenerator.GenerateMeshData(chunk, lod);

            lock (meshDataThreadInfoQueue) {
                meshDataThreadInfoQueue.Enqueue(new GenThreadInfo<MeshData>(callback, meshData));
            }
        }
        #endregion

        #region Structs
        //Somewhere for our threads to hold data...
        struct GenThreadInfo<T>
        {
            public readonly Action<T> callback;
            public readonly T parameter;

            public GenThreadInfo(Action<T> callback, T parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }
        }
        #endregion  
    }
}