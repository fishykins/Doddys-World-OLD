﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw.planet
{
    public class GraphicalGenerator
    {

        GraphicalSettings settings;
        Texture2D texture;
        const int textureResolution = 50;

        public void UpdateSettings(GraphicalSettings settings)
        {
            this.settings = settings;
            if (texture == null) {
                texture = new Texture2D(textureResolution, 1);
            }
        }

        public void UpdateElevation(MinMax elevationMinMax)
        {
            settings.planetMaterial.SetVector("_elavationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
        }

        public void UpdateColours()
        {
            Color[] colours = new Color[textureResolution];
            for (int i = 0; i < textureResolution; i++) {
                colours[i] = settings.gradient.Evaluate(i / (textureResolution - 1f));
            }
            texture.SetPixels(colours);
            texture.Apply();
            settings.planetMaterial.SetTexture("_texture", texture);
        }
    }
}