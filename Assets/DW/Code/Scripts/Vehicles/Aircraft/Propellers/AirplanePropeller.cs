using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    public class AirplanePropeller : MonoBehaviour
    {
        #region Variables

        #endregion Default Methods


        #region Built In methods

        #endregion

        #region Custom Methods
        public void HandlePropeller(float currentRPM)
        {
            float dps = (currentRPM * 360f)/ 60f * Time.deltaTime;
            transform.Rotate(Vector3.up, dps);
        }
        #endregion

    }
}