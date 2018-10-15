using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    public class InputBase : MonoBehaviour
    {

        #region Variables
        protected float depth = 0f;
        protected float width = 0f;
        protected float lean = 0f;
        protected float height = 0f;

        protected float rotateH = 0f;
        protected float rotateV = 0f;

        protected float throttle = 0f;
        #endregion

        #region Properties
        public float Depth { get { return depth; } }
        public float Width { get { return width; } }
        public float Lean { get { return lean; } }
        public float Height { get { return height; } }
        public float RotateH { get { return rotateH; } }
        public float RotateV { get { return rotateV; } }
        #endregion

        #region Unity Methods
        void Update()
        {
            HandleInput();
        }
        #endregion

        #region Custom Methods
        protected virtual void HandleInput ()
        {
            depth = Input.GetAxis("Depth");
            width = Input.GetAxis("Width");
            lean = Input.GetAxis("Lean");
            height = Input.GetAxis("Height");
            rotateH = Input.GetAxis("Mouse X");
            rotateV = Input.GetAxis("Mouse Y");
        }
        #endregion
    }
}