using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Todo: Intergrate a border so we can calculate the edge of the mesh a little better
/// </summary>

namespace dw.planet {
	public class MeshGenerator {
        #region Variables
        PlanetGenerator planetGenerator;
        int borderedSize;
        int faceResolution;
        #endregion;

        #region Constructor
        public MeshGenerator(PlanetGenerator planetGenerator, int resolution)
        {
            this.planetGenerator = planetGenerator;
            this.borderedSize = resolution;
            this.faceResolution = planetGenerator.planet.faceResolution;
        }
        #endregion;

        #region Custom Methods
        public MeshData GenerateMeshData(Chunk chunk, int levelOfDetail)
        {
            int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
            int verticesPerLine = (borderedSize - 1) / meshSimplificationIncrement + 1;

            Debug.Log("Generating meshData (lod " + levelOfDetail + ") for Chunk [" + chunk.faceCoord.i + "," + chunk.faceCoord.u + "," + chunk.faceCoord.v + "]");

            MeshData meshData = new MeshData(verticesPerLine);
            ChunkData chunkData = chunk.ChunkData;
            Face face = chunk.face;

            Vector3[,] heightMap = chunkData.heightMap;

            float UVstep = 1f / faceResolution;
            float step = UVstep / (borderedSize -1);

            Vector2 offset = new Vector3((-0.5f + chunk.faceCoord.u * UVstep), (-0.5f + chunk.faceCoord.v * UVstep));

            //Generate points on mesh
            for (int y = 0; y < borderedSize; y += meshSimplificationIncrement) {
                for (int x = 0; x < borderedSize; x += meshSimplificationIncrement) {

                    int i = (x / meshSimplificationIncrement) + (y / meshSimplificationIncrement) * verticesPerLine;

                    meshData.vertices[i] = heightMap[x, y];
                    meshData.uvs[i] = new Vector2(x / (float)borderedSize, y / (float)borderedSize);
                    
                    if (x < borderedSize - 1 && y < borderedSize - 1) {
                        meshData.AddTriangle(i, i + verticesPerLine + 1, i + verticesPerLine);
                        meshData.AddTriangle(i, i + 1, i + verticesPerLine + 1);
                    }

                }
            }
            return meshData;
        }
        #endregion;
    }
}