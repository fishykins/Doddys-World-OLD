using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw.planet {
	public struct ChunkData  {
        #region Variables
        public readonly Vector3[,] heightMap;
        #endregion;

        #region Generator
        public ChunkData(Vector3[,] heightMap)
        {
            this.heightMap = heightMap;
        }
        #endregion
    }
}