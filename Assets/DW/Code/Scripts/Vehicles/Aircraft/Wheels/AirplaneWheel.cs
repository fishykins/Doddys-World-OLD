using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace dw
{
    public class AirplaneWheel : MonoBehaviour
    {

        #region Variables
        public Transform wheelGraphic;
        public bool isBraking = false;
        public bool isSteering = false;
        public float steerAngle = 20f;
        public float brakePower = 5f;
        public float steerSmoothSpeed = 2f;

        private WheelCollider wheelCol;
        private Vector3 worldPos;
        private Quaternion worldRot;
        private float finalBrakeForce;
        private float finalSteerAngle;
        #endregion



        #region built in Methods
        // Use this for initialization
        void Start()
        {
            wheelCol = GetComponent<WheelCollider>();
        }

        #endregion

        #region Custom Methods
        public void InitWheel()
        {
            if (wheelCol) {
                wheelCol.motorTorque = 0.00000001f;
                wheelCol.brakeTorque = 0f;
            }
        }

        
        public void HandleWheel(InputBase input)
        {
            if (wheelCol) {
                //out stores output at this variable
                wheelCol.GetWorldPose(out worldPos, out worldRot);
                if (wheelGraphic) {
                    wheelGraphic.rotation = worldRot;
                    wheelGraphic.position = worldPos;
                }

                /*
                if (isBraking) {
                    if (input.Brake > 0.1f) {
                        finalBrakeForce = Mathf.Lerp(finalBrakeForce, input.Brake * brakePower, Time.deltaTime);
                        wheelCol.brakeTorque = finalBrakeForce;
                    }
                    else {
                        wheelCol.brakeTorque = 0f;
                    }
                }

                if (isSteering) {
                    finalSteerAngle = Mathf.Lerp(finalSteerAngle, -input.Yaw * steerAngle, Time.deltaTime * steerSmoothSpeed);
                    wheelCol.steerAngle = finalSteerAngle;
                }*/
            }
        }
        #endregion
    }
}