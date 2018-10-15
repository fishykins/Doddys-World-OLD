using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw {
	public class Infantry : VehicleBase {
        #region Variables
        //Public
        [Header("Infantry Settings")]
        public float maxSpeed = 10f;
        public float jumpForce = 200f;
        [Range(0f, 90f)]
        public float PitchMax = 45;
        [Range(-90f, 0f)]
        public float PitchMin = -45;

        //Private
        private float pitchActual = 0f;
        private float slidingLimit = 30f;
        private float slideSpeed = 10f;
        #endregion;

        #region Properties

        #endregion;

        #region Custom Methods
        protected override void InitVehicle()
        {
            base.InitVehicle();

            //Stop us rotating freely!
            if (rb) {
                rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            }
        }

        protected override void HandleInput()
        {
            base.HandleInput();
            HandleStrafe();
            HandleRotate();
        }


        private void HandleStrafe()
        {
            //STRAFE
            Vector3 _movHorizontal = transform.right * controller.Input.Width;  //.right = (1,0,0)
            Vector3 _movVertical = transform.forward * controller.Input.Depth;  //.forward = (0,0,1)
            Vector3 _velocity = (_movHorizontal + _movVertical);

            //JUMP
            if (controller.Input.Height > 0f && grounded) {
                rb.AddForce(transform.up * jumpForce);
                Debug.Log("JUMPO");
            }


            //MOVE
            if (_velocity != Vector3.zero) {
                rb.MovePosition(rb.position + _velocity * Time.fixedDeltaTime * maxSpeed);
            }

            
        }

        private void HandleRotate()
        {
            //YAW
            rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0f, controller.Input.RotateH, 0f)));

            //PITCH 
            pitchActual = Mathf.Clamp(pitchActual - controller.Input.RotateV, PitchMin, PitchMax);
            if (cam != null) {
                cam.transform.localEulerAngles = new Vector3(pitchActual, 0f, 0f);
            }
        }

        #endregion
    }
}