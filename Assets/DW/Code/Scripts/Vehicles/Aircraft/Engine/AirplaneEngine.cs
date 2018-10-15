using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace dw
{
    public class AirplaneEngine : MonoBehaviour
    {
        #region Variables
        [Tooltip("Engine output in kN")]
        public float maxForce = 200f;
        public float MaxRPM = 2550f;
        [Range(0,1)]
        public float throttle = 0f;
        public AnimationCurve powerCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public AirplanePropeller propeller;
        #endregion

        #region Built in Methods

        #endregion

        #region Custom Methods
        public Vector3 CalculateForce(float newThrottle)
        {
            throttle = newThrottle;
            float finalThrottle = Mathf.Clamp01(throttle);
            finalThrottle = powerCurve.Evaluate(finalThrottle);

            float currentRPM = finalThrottle * MaxRPM;
            if (propeller) {
                propeller.HandlePropeller(currentRPM);
            }


            float finalPower = finalThrottle * maxForce * 1000f;
            Vector3 finalForce = transform.forward * finalPower;
            return finalForce;
        }
        #endregion

    }
}