using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    public class GraphicalGenerator
    {

        GraphicalData data;
        Texture2D texture;
        const int textureResolution = 50;

        public void UpdateSettings(GraphicalData data)
        {
            this.data = data;
            if (texture == null) {
                texture = new Texture2D(textureResolution, 1);
            }
        }

        public void UpdateElevation(MinMax elevationMinMax)
        {
            data.planetMaterial.SetVector("_elavationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
        }

        public void UpdateColours()
        {
            Color[] colours = new Color[textureResolution];
            for (int i = 0; i < textureResolution; i++) {
                colours[i] = data.gradient.Evaluate(i / (textureResolution - 1f));
            }
            texture.SetPixels(colours);
            texture.Apply();
            data.planetMaterial.SetTexture("_texture", texture);
        }
    }
}