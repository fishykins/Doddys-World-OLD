using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Primary physics component of an aircraft.
/// </summary>

namespace dw {
	public class Airframebody : Physicsbody {

        #region Variables
        //Public
        [Header("Vehicle Composition")]
        public List<AirplaneEngine> engines = new List<AirplaneEngine>();
        public List<AirplaneWheel> wheels = new List<AirplaneWheel>();

        [Header("Characteristics")]
        public bool ridbodyAssist = true;
        [Range(0f,1f)]
        public float rotationAssist = 0.5f;
        [Range(0f, 0.2f)]
        public float velocityAssist = 0.01f;
        public float minAssistSpeed = 40f;
        public float maxAssistSpeed = 80f;



        //Private

        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods

        #endregion;

        #region Custom Methods
        public override void HandlePhysics()
        {
            base.HandlePhysics();
            HandlePitch();
            HandleRoll();
            HandleEngines();

            if (ridbodyAssist) {
                HandleRigidbodyAssist();
            }
        }


        private void HandleEngines()
        {
            if (engines != null) {
                if (engines.Count > 0) {
                    foreach (AirplaneEngine engine in engines) {
                        rb.AddForce(engine.CalculateForce(vehicle.Throttle));
                    }
                }
            }
        }

        private void HandlePitch()
        {
            Vector3 pitchTorque = vehicle.Depth * transform.right;
            rb.AddTorque(pitchTorque);
        }

        private void HandleRoll()
        {
            Vector3 rollTorque = vehicle.Width * transform.forward;
            rb.AddTorque(rollTorque);
        }

        private void HandleRigidbodyAssist()
        {
            float airspeed = rb.velocity.magnitude;
            float assistLerp = Mathf.InverseLerp(minAssistSpeed, maxAssistSpeed, airspeed);
            float velocityAssistFinal = velocityAssist * assistLerp;
            float rotationAssistFinal = rotationAssist * assistLerp;

            Vector3 updatedVelocity = Vector3.Lerp(rb.velocity, transform.forward * rb.velocity.magnitude, rb.velocity.magnitude * Time.deltaTime * velocityAssistFinal);
            rb.velocity = updatedVelocity;

            // "Aerodynamic" calculations. This is a very simple approximation of the effect that a plane
            // will naturally try to align itself in the direction that it's facing when moving at speed.
            // Without this, the plane would behave a bit like the asteroids spaceship!
            if (rb.velocity.magnitude > 0) {
                // compare the direction we're pointing with the direction we're moving:
                float aeroFactor = Vector3.Dot(transform.forward, rb.velocity.normalized);
                // multipled by itself results in a desirable rolloff curve of the effect
                aeroFactor *= aeroFactor;
                // Finally we calculate a new velocity by bending the current velocity direction towards
                // the the direction the plane is facing, by an amount based on this aeroFactor
                var newVelocity = Vector3.Lerp(rb.velocity, transform.forward * ForwardSpeed,
                                               aeroFactor * ForwardSpeed * rotationAssistFinal * Time.deltaTime);
                rb.velocity = newVelocity;

                // also rotate the plane towards the direction of movement - this should be a very small effect, but means the plane ends up
                // pointing downwards in a stall
                rb.rotation = Quaternion.Slerp(rb.rotation,
                                                      Quaternion.LookRotation(rb.velocity, transform.up),
                                                      rotationAssist * Time.deltaTime);
            }
        }

        #endregion
    }
}