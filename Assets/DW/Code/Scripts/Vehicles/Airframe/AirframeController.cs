using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A component to deal with airframe input
/// </summary>

namespace dw {
	public class AirframeController : VehicleBase {
        #region Variables
        //Public
        [Header("Controls")]
        public float pitchSpeed = 1000f;
        public float rollSpeed = 1000f;
        public float yawSpeed = 1000f;
        public float bankSpeed = 300f;
        public float throttleSpeed = 0.2f;

        //Private
        float pitchAngle = 0f;
        float rollAngle = 0f;
        float yawAngle = 0f;
        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        protected override void HandleInput()
        {
            base.HandleInput();
            HandleThrottle();
            HandlePitch();
            HandleRoll();
        }

        private void HandlePitch()
        {
            pitchAngle = Vector3.SignedAngle(transform.forward, gravityForward, transform.right);

            depth = controller.Input.Depth * pitchSpeed;
        }

        private void HandleRoll()
        {
            rollAngle = Vector3.SignedAngle(transform.right, -gravityRight, transform.forward);

            //Set up a public varible that the physicsbody can access
            width = -controller.Input.Width * rollSpeed;
        }

        private void HandleYaw()
        {
            Vector3 yawTorque = controller.Input.RotateH * yawSpeed * transform.up;
            rb.AddTorque(yawTorque);
        }

        private void HandleThrottle()
        {
            throttle = Mathf.Clamp01(throttle + (controller.Input.Height * throttleSpeed * Time.deltaTime));
        }
        #endregion;
    }
}