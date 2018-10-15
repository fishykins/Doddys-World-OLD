using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This component is a custom physics module, and allows vehicles to obey gravity and stuff. Requires a VehicleBase to function
/// If no vehicle base is on object, this will do NADA. 
/// </summary>

namespace dw {
    [RequireComponent(typeof(Rigidbody))]
    public class Physicsbody : MonoBehaviour {

        #region Variables
        //Public
        [Header("Universal Physics")]
        public bool hasGravity = true;
        [Tooltip("Mass in kg (will override Rigidbody settings")]
        public float mass = 1000f;

        //Private
        protected Rigidbody rb;
        protected VehicleBase vehicle;
        protected ControllerBase controller;

        protected float startDrag;
        protected float startAngularDrag;

        protected float forwardSpeed;
        protected float verticalSpeed;
        protected float horizontalSpeed;

        protected GameObject[] gravityFields;

        protected float gravityStrength = 0f;
        protected GameObject nearestGravityField;
        protected GravitationalBody gb;
        protected Vector3 gravityUp;
        protected Vector3 gravityRight;
        protected Vector3 gravityForward;
        #endregion;

        #region constants
        protected const float mpsToMph = 2.23694f;
        #endregion

        #region Properties
        public float GravityStrength { get { return gravityStrength; } }
        public float ForwardSpeed { get { return forwardSpeed; } }
        public float VerticalSpeed { get { return verticalSpeed; } }
        public float HorizontalSpeed { get { return horizontalSpeed; } }
        public Vector3 GravityUp { get { return gravityUp; } }
        public Vector3 GravityRight { get { return gravityRight; } }
        public Vector3 GravityForward { get { return gravityForward; } }
        #endregion

        #region Unity methods
        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody>();
            vehicle = GetComponent<VehicleBase>();
            if (rb) {
                startDrag = rb.drag;
                startAngularDrag = rb.angularDrag;
                rb.mass = mass;
            }
        }

        protected virtual void Update()
        {
            if (rb) {
                HandlePhysics();
            }
        }
        #endregion

        #region Custom Methods

        //Called by vehicleBase every frame
        public virtual void HandlePhysics()
        {
            if (rb) {
                CalculateGravity();
                CalculateUp();

                //This is non-essential so dont do it!
                //CalculateSpeed();
            }
        }

        public void CalculateSpeed()
        {
            Vector3 sideVector = Vector3.Cross(rb.velocity, gravityUp);
            Vector3 forwardVector = Vector3.Cross(gravityUp, sideVector);
            Vector3 upVector = Vector3.Cross(gravityForward, rb.velocity);
            Vector3 horizontalVector = Vector3.Cross(rb.velocity, gravityRight);

            //Debug.DrawRay(transform.position, horizontalVector * 10f, Color.red);

            verticalSpeed = upVector.magnitude;
            forwardSpeed = forwardVector.magnitude;
            horizontalSpeed = horizontalVector.magnitude;
        }

        //Standard gravitational method 
        protected virtual void CalculateGravity()
        {
            gravityFields = GameObject.FindGameObjectsWithTag("GravitationalBody");
            Vector3 finalForce = Vector3.zero;

            if (hasGravity) {
                foreach (GameObject obj in gravityFields) {
                    GravitationalBody gb = obj.GetComponent<GravitationalBody>();
                    if (gb) {
                        finalForce += gb.GetGravitationalForce(transform.position, mass);
                    }
                }

                //Used for debuging. 
                gravityStrength = finalForce.magnitude;

                rb.AddForce(finalForce);
            }
        }

        protected virtual void CalculateUp()
        {
            //Get the nearest gravitational field and use this as our primary sourse of data
            GameObject fieldTmp = GeoCoord.NearestGravitationalBody(transform.position);
            if (fieldTmp != nearestGravityField && fieldTmp !=null) {
                nearestGravityField = fieldTmp;
                gb = nearestGravityField.GetComponent<GravitationalBody>();
            }

            if (gb) {
                gravityUp = gb.GravityUp(transform);

                //Also collect right and forward while we are here
                gravityRight = Vector3.Cross(rb.velocity.normalized, gravityUp);
                gravityForward = -Vector3.Cross(gravityUp, gravityRight);
            }
            

            //TODO: MOVE TO METHOD SO WE DONT HAVE DUPLICATE CODE

            //DEEEEBUG because this is probably wrong
            //Debug.DrawRay(transform.position, gravityUp * 10f, Color.green);
            //Debug.DrawRay(transform.position, gravityRight * 10f, Color.red);
            //Debug.DrawRay(transform.position, gravityForward * 10f, Color.blue);
        }
        #endregion
    }
}