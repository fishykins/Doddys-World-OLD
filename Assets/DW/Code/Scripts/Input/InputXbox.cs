using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    public class InputXbox : InputBase
    {
        #region Variables

        #endregion

        #region Unity Methods

        #endregion

        #region Custom Methods
        protected override void HandleInput()
        {
            depth = Input.GetAxis("JoystickDepth");
            width = Input.GetAxis("JoystickWidth");
            lean = Input.GetAxis("Lean");
            height = Input.GetAxis("JoystickRotateV");
            rotateH = Input.GetAxis("JoystickRotateH");
            rotateV = Input.GetAxis("JoystickRotateV");
        }
        #endregion
    }
}