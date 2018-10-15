using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    [CreateAssetMenu]
    public class ShapeData : ScriptableObject
    {
        [Range(1, 16)]
        public int chunksPerFace = 10;

        [Range(4, 256)]
        public int faceResolution = 128;

        [Range(0, 6)]
        public int levelOfDetail;

        public float dayLength = 15f;  //Time in seconds for planet to do a full rotation

        public NoiseLayer[] noiseLayers;

        [System.Serializable]
        public class NoiseLayer
        {
            public bool enabled = true;
            public bool useFirstLayerAsMask;
            public NoiseSettings noiseSettings;
        }
    }
}