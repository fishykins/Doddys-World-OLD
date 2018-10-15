using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    public class CameraData : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private bool isFirstPerson = true;
        [SerializeField]
        private bool chase = false;
        #endregion

        #region Unity Methods

        #endregion

        #region Custom Methods
        public bool IsFirstPersonCamera()
        {
            return isFirstPerson;
        }

        public bool IsChaseCamera()
        {
            return chase;
        }
        #endregion
    }
}