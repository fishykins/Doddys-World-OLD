using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw.planet
{
    [CreateAssetMenu]
    public class EnviromentalSettings : ScriptableObject
    {
        public bool hasAtmposhpere;
        [Tooltip("Air Density in kg/m^3 at sea level (at average temperature)")]
        public float airDensityASL = 1.225f; //measured in kg/m^3
        [Tooltip("Average temperature in Celcius at sea level")]
        public float temperatureASL = 15f; //Degrees Celcius

        public AnimationCurve temperatureHeightCurve;
    }
}