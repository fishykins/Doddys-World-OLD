using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using dw;

namespace dw.planet {
	public class ChunkGenerator  {
        #region Variables
        PlanetGenerator planetGenerator;
        int resolution;
        #endregion;

        #region Constructor
        public ChunkGenerator (PlanetGenerator planetGenerator, int resolution)
        {
            this.planetGenerator = planetGenerator;
            this.resolution = resolution;
        }
        #endregion

        #region Custom Methods
        //Method for generating chunk data (Height map)
        public ChunkData GenerateChunkData (Chunk chunk)
        {
            string chunkName = "chunk [" + chunk.faceCoord.i + ", " + chunk.faceCoord.u + ", " + chunk.faceCoord.v + "]";
            Face face = chunk.face;

            float UVstep = 1f / planetGenerator.planet.faceResolution;
            float step = UVstep / (resolution -1); 
            Vector2 offset = new Vector3((-0.5f + chunk.faceCoord.u * UVstep), (-0.5f + chunk.faceCoord.v * UVstep));

            Vector3[,] heightMap = new Vector3[resolution, resolution];

            for (int y = 0; y < resolution; y ++) {
                for (int x = 0; x < resolution; x ++) {

                    Vector2 p = offset + new Vector2(x * step, y * step);
                    Vector3 pointOnUnitCube = face.axisB * p.x + face.axisA * p.y + face.axisC * -0.5f;
                    Vector3 pointOnUnitSphere = pointOnUnitCube.normalized; 
                    Vector3 pointWithNoise = planetGenerator.shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);
                    heightMap[x, y] = pointWithNoise;

                    if (x % 100 == 0 && y % 100 == 0) {
                        //Debug.Log(chunkName + ": heightMap [" + x + "," + y + "] = [" + heightMap[x, y].x + "," + heightMap[x, y].y + "," + heightMap[x, y].z + "]");
                    }
                }
            }
            return new ChunkData(heightMap);
        }
        #endregion
    }
}