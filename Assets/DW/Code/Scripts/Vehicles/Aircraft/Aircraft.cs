using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw {
	public class Aircraft : VehicleBase {
        #region Variables

        #endregion;
        //Public
        [Header("Controls")]
        public float pitchSpeed = 1000f;
        public float rollSpeed = 1000f;
        public float yawSpeed = 1000f;
        public float bankSpeed = 300f;
        public float throttleSpeed = 0.2f;
        [Tooltip("Airspeed at which the pitch and roll start to get sloppy (Squared for performance boost)")]
        public float minAirflow = 30f;

        [Header("Vehicle Composition")]
        public List<AirplaneEngine> engines = new List<AirplaneEngine>();
        public List<AirplaneWheel> wheels = new List<AirplaneWheel>();

        //Private
        float airFlowClamped = 0f;
        float stickThrottle = 0f;
        float pitchAngle = 0f;
        float rollAngle = 0f;
        float yawAngle = 0f;
        #region Properties

        #endregion;

        #region Unity Methods

        #endregion;

        #region Custom Methods
        protected override void InitVehicle()
        {
            base.InitVehicle();

            //Set up our wheels
            if (wheels != null) {
                if (wheels.Count > 0) {
                    foreach (AirplaneWheel wheel in wheels) {
                        wheel.InitWheel();
                    }
                }
            }
        }

        protected override void HandleInput()
        {
            base.HandleInput();
            

            if (controller.Input) {
                HandleAirflow();
                HandleThrottle();
                HandleEngines();
                HandleWheels();
                HandleRoll();
                HandlePitch();
                HandleYaw();
                HandleBanking();
            }

        }

        private void HandleAirflow()
        {
            airFlowClamped = Mathf.Clamp01(rb.velocity.sqrMagnitude / minAirflow);
        }

        private void HandleThrottle()
        {
            stickThrottle = Mathf.Clamp01(stickThrottle + (controller.Input.Height * throttleSpeed * Time.deltaTime));
            if (stickThrottle > 0) {
            }
        }

        private void HandleEngines()
        {
            if (engines != null) {
                if (engines.Count > 0) {
                    foreach (AirplaneEngine engine in engines) {
                        rb.AddForce(engine.CalculateForce(stickThrottle));
                    }
                }
            }
        }

        void HandleWheels()
        {
            if (wheels.Count > 0) {
                foreach (AirplaneWheel wheel in wheels) {
                    wheel.HandleWheel(controller.Input);
                }
            }
        }


        private void HandleRoll()
        {
            rollAngle = Vector3.SignedAngle(transform.right, -gravityRight, transform.forward);

            Vector3 rollTorque = -controller.Input.Width * rollSpeed * transform.forward * airFlowClamped;
            rb.AddTorque(rollTorque);
        }

        private void HandlePitch()
        {
            pitchAngle = Vector3.SignedAngle(transform.forward, gravityForward, transform.right);

            Vector3 pitchTorque = controller.Input.Depth * pitchSpeed * transform.right * airFlowClamped;
            rb.AddTorque(pitchTorque);
        }

        private void HandleYaw()
        {
            Vector3 yawTorque = controller.Input.RotateH * yawSpeed * transform.up;
            rb.AddTorque(yawTorque);
        }

        void HandleBanking()
        {
            float bankSide = Mathf.InverseLerp(-90f, 90f, rollAngle);
            float bankAmount = Mathf.Lerp(-1f, 1f, bankSide);

            //DEBUGING!!!!!!!!!!
            //Debug.Log(bankAmount + "/" + rollAngle);

            Vector3 bankTorque = bankAmount * bankSpeed * transform.up;
            rb.AddTorque(bankTorque);
        }
        #endregion
    }
}