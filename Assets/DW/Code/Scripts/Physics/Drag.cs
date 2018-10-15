using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
	public class Drag : PhysicsLimb
    {
        #region Classes
        [System.Serializable]
        public class DragInfo
        {
            [Tooltip("For drag equation")]
            public float dragCoefficient;
            [Tooltip("in meters square")]
            public float sectionArea;
            [Tooltip("Direction of face")]
            public Vector3 normal;
            [Tooltip("Disable for Debug purposes?")]
            public bool isActive;

            [HideInInspector]
            public Vector3 dragForce = Vector3.zero;

            public DragInfo(float dragCoefficient, float sectionArea, Vector3 normal)
            {
                this.dragCoefficient = dragCoefficient;
                this.sectionArea = sectionArea;
                this.normal = normal;
            }
        }
        #endregion

        #region Variables
        //Public
        [SerializeField]
        private DragInfo[] dragSections;
        [Tooltip("Protects us from any crazy results")]
        public float maxDrag = 1000000f;

        //Private
        float dragForce; //Resulting force
        #endregion;

        #region Properties
        public DragInfo[] DragSections { get { return dragSections; } }
        #endregion;

        #region Unity Methods
        #endregion;

        #region Custom Methods
        protected override void HandlePhysics()
        {
            base.HandlePhysics();

            //CalculateSqrVelocity(); //Base 
            CalculateAirDensity(); //Base
            CalculateSqrVelocity(); //Remove if not debuging!
            GenerateDrag();
        }

        private void GenerateDrag()
        {
            //Handle each drag surface provided
            foreach (DragInfo info in dragSections) {
                if (info.isActive) {
                    Vector3 dragForce = CalculateDrag(info);
                    rb.AddForce(dragForce);
                    Debug.DrawRay(transform.position, dragForce, Color.red);
                }
            }
        }

        private Vector3 CalculateDrag(DragInfo info)
        {
            float area = info.sectionArea;
            float dragCoefficient = info.dragCoefficient;
            Vector3 vectorDir = transform.TransformDirection(info.normal);
            float sqrVelocity = Mathf.Max(0f, Vector3.Dot(rb.velocity, vectorDir)); 
            
            sqrVelocity *= sqrVelocity;

            float dragStrength = -Mathf.Min(maxDrag, dragCoefficient * area * .5f * airDensity * sqrVelocity);

            //Return force, IN NEWTONS
            info.dragForce = vectorDir * dragStrength * lbsToN;
            return info.dragForce;
        }
        #endregion
    }
}