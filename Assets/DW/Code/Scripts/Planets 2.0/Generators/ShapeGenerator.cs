using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw.planet
{
    public class ShapeGenerator
    {

        ShapeSettings shapeSettings;
        INoiseFilter[] noiseFilters;
        public MinMax elevationMinMax;
        private float radius;

        public void UpdateSettings(ShapeSettings settings, float radius)
        {
            this.radius = radius;
            this.shapeSettings = settings;
            noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
            for (int i = 0; i < noiseFilters.Length; i++) {
                noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
            }
            elevationMinMax = new MinMax();
        }


        public float CalculateHeight(Vector3 pointOnUnitSphere)
        {
            float firstLayerValue = 0;
            float elevation = 0;

            if (noiseFilters.Length > 0) {
                firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
                if (shapeSettings.noiseLayers[0].enabled) {
                    elevation = firstLayerValue;
                }
            }

            for (int i = 0; i < noiseFilters.Length; i++) {
                if (shapeSettings.noiseLayers[i].enabled) {
                    float mask = (shapeSettings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
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