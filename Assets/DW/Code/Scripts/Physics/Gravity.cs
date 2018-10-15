using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    public static class Gravity
    {
        #region Constants
        public const float gravitationalConstant = 6.674e-11f;
        #endregion

        #region Custom Methods
        public static float GetForce(float massA, float massB, Vector3 posA, Vector3 posB, float gc = gravitationalConstant)
        {
            return (gc * massA * massB) / (posA - posB).sqrMagnitude;
        }
        #endregion
    }
}