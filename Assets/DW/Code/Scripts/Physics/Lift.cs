using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw {
	public class Lift : PhysicsLimb {
        #region Variables

        #endregion;
        [Header("Lift")]
        public float wingAngleOfAttack = 5f;
        public float wingArea = 800f;
        public AnimationCurve liftCoefficientCurve = AnimationCurve.EaseInOut(0f, 0.2f, 20f, 1.6f);
        public float maxLift = 20000f; //Protect us from crazy results

        //Private
        private float liftForce;
        private float angleOfAttack;
        private float liftCoefficient;

        #region Properties
        public float LiftForce { get { return liftForce; } }
        public float AngleOfAttack { get { return angleOfAttack; } }
        public float LiftCoefficient { get { return liftCoefficient; } }
        #endregion;

        #region Unity Methods

        #endregion;
        #region Custom Methods
        protected override void HandlePhysics()
        {
            base.HandlePhysics();
            CalculateLiftCoefficient();
            CalculateAirDensity();
            CalculateSqrVelocity(); //Base class
            CalculateLift();

            GenerateLift();
        }

        private void CalculateLiftCoefficient()
        {
            angleOfAttack = wingAngleOfAttack + Vector3.Angle(transform.forward, rb.velocity.normalized);
            liftCoefficient = Mathf.Max(0f, liftCoefficientCurve.Evaluate(angleOfAttack));
        }

        private void CalculateLift()
        {
            liftForce = Mathf.Min(maxLift, .5f * airDensity * sqrVelocity * wingArea * liftCoefficient);
            liftForce *= lbsToN; //Convert to Newtons
        }

        private void GenerateLift()
        {
            Vector3 liftDir = transform.up;
            Vector3 finalLiftForce = liftDir * liftForce;
            rb.AddForce(finalLiftForce);
        }
        #endregion  
    }
}