using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    public class ShapeGenerator
    {

        ShapeData shapeData;
        INoiseFilter[] noiseFilters;
        public MinMax elevationMinMax;
        private float radius;

        public void UpdateSettings(ShapeData data, float radius)
        {
            this.radius = radius;
            this.shapeData = data;
            noiseFilters = new INoiseFilter[data.noiseLayers.Length];
            for (int i = 0; i < noiseFilters.Length; i++) {
                noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(data.noiseLayers[i].noiseSettings);
            }
            elevationMinMax = new MinMax();
        }


        public float CalculateHeight(Vector3 pointOnUnitSphere)
        {
            float firstLayerValue = 0;
            float elevation = 0;

            if (noiseFilters.Length > 0) {
                firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
                if (shapeData.noiseLayers[0].enabled) {
                    elevation = firstLayerValue;
                }
            }

            for (int i = 0; i < noiseFilters.Length; i++) {
                if (shapeData.noiseLayers[i].enabled) {
                    float mask = (shapeData.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
                    elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
                }
            }

            elevation = radius * (1 + elevation);
            elevationMinMax.AddValue(elevation);
            return elevation;
        }

        public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
        {
            float elevation = CalculateHeight(pointOnUnitSphere);
            return pointOnUnitSphere * elevation;
        }
    }
}