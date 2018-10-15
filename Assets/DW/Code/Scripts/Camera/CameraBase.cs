using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
	public class CameraBase : MonoBehaviour {
        #region Variables
        //Public
        public Camera cam;

        //Private
        protected ControllerBase controller;
        protected Transform cameraTarget;
        protected CameraData cameraData;
        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        protected virtual void Start()
        {
            controller = GetComponent<ControllerBase>();
        }

        protected virtual void Update()
        {
            if (controller) {
                if (cameraTarget != null && cameraData != null) {
                    HandleCamera();
                } else {
                    UpdateCamera();
                }
            }
        }
        #endregion;

        #region Custom Methods
        //Called every frame
        protected virtual void HandleCamera() { }

        //Gather info about the camera
        protected virtual void UpdateCamera()
        {
            if (controller.ControlTarget != null) {
                cameraTarget = controller.ControlTarget.Cam;
                if (cameraTarget) {
                    cameraData = cameraTarget.GetComponent<CameraData>();
                }
            }
        }
        #endregion
    }
}