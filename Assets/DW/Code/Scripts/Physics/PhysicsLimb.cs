using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
	public class PhysicsLimb : MonoBehaviour {
        #region Variables
        //Private
        protected Rigidbody rb;
        protected Physicsbody pb;

        protected float sqrVelocity;
        protected float airDensity;
        #endregion;

        #region Constants
        protected const float metersToFeet = 3.28084f;
        protected const float sqrMetersToFeet = 10.7639f;
        protected const float lbsToN = 4.44822f;
        #endregion

        #region Properties
        public float AirDensity { get { return airDensity; } }
        public float SqrVelocity { get { return sqrVelocity; } }
        #endregion;

        #region Unity Methods
        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody>();
            pb = GetComponent<Physicsbody>();
        }

        protected virtual void FixedUpdate () {
            if (rb && pb) {
                HandlePhysics();
            }
        }
        #endregion;

        #region Custom Methods
        protected virtual void HandlePhysics() { }

        protected virtual void CalculateSqrVelocity()
        {
            //Gets feet per second squared
            sqrVelocity = rb.velocity.sqrMagnitude * sqrMetersToFeet;
        }

        protected virtual void CalculateAirDensity()
        {
            airDensity = .002377f;
        }
        #endregion
    }
}