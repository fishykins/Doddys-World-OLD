using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    [RequireComponent(typeof(ControllerBase))]
    public class PlayerCamera : CameraBase
    {

        #region Variables
        //Public
        [SerializeField]
        private float smoothSpeed = 0.96f;

        //Private
        private Vector3 smoothVelocity;
        #endregion

        #region Unity Methods

        #endregion

        #region Custom Methods
        //Called every frame
        protected override void HandleCamera()
        {
            if (cameraData.IsFirstPersonCamera()) {
                //First Person
                cam.transform.position = cameraTarget.position;
                cam.transform.rotation = cameraTarget.transform.rotation;
            }
            else if (cameraData.IsChaseCamera()) {
                //Third Person Chase
                Vector3 gravityUp = controller.ControlTarget.GravityUp;
                Vector3 positionTarget = cameraTarget.position;
                Vector3 relativePos = controller.ControlTarget.transform.position - cam.transform.position;

                //Move
                transform.position = Vector3.SmoothDamp(cam.transform.position, positionTarget, ref smoothVelocity, smoothSpeed);
                cam.transform.rotation = Quaternion.LookRotation(relativePos, gravityUp);
            }
            else {
                //Third Person fixed
                cam.transform.position = cameraTarget.position;
                cam.transform.rotation = cameraTarget.transform.rotation;
            }

        }
        #endregion
    }
}