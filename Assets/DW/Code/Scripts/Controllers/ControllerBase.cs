using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw {
    public class ControllerBase : MonoBehaviour {
        #region Variables
        //Public 
        [Header("Controller Settup")]
        [SerializeField, Tooltip("The input component to listen to")]
        protected InputBase input;
        [SerializeField, Tooltip("The vehicle we are controlling")]
        protected VehicleBase controlTarget;

        //Private
        protected VehicleBase previousControlTarget;

        #endregion

        #region Properties
        public VehicleBase ControlTarget { get { return controlTarget; } }
        public InputBase Input { get { return input; } }
        #endregion

        #region Unity Methdods
        protected virtual void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        protected virtual void Update()
        {
            HandleInput();
            HandleControlTarget();
        }
        #endregion

        #region Custom Methods
        protected virtual void HandleInput()
        {

        }

        protected virtual void HandleControlTarget()
        {
            if (controlTarget != previousControlTarget) {
                //new controlTarget has been set- update everyone
                if (previousControlTarget) {
                    previousControlTarget.RemoveController(this);
                }
                controlTarget.SetController(this);
                previousControlTarget = controlTarget;
            }
        }
        #endregion
    }
}