using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleBase : MonoBehaviour
    {
        #region Variables
        //Public
        public Transform centreOfGravity;
        public LayerMask groundedMask;

        //Private
        protected Rigidbody rb;
        protected Physicsbody pb;
        protected ControllerBase controller;

        protected bool grounded = false;
        protected float objectHeight;

        //Input (Declared here, needs to be set in child class
        protected float throttle;
        protected float depth;
        protected float width;
        protected float lean;
        protected float height;
        protected float rotateH;
        protected float rotateV;


        protected GameObject nearestGravityField;
        protected GravitationalBody gb;
        protected Vector3 gravityUp;
        protected Vector3 gravityRight;
        protected Vector3 gravityForward;

        //Camera View
        protected int currentCamera = 0;
        protected int cameraCount = 0;
        protected Transform cameraGroup;
        protected Transform[] cameras;
        protected Transform cam;
        #endregion

        #region Properties
        public Transform Cam { get { return cam; } }
        public Vector3 GravityUp { get { return gravityUp; } }
        public float ObjectHeight { get { return objectHeight; } }
        public float Throttle { get { return throttle; } }
        public float Depth { get { return depth; } }
        public float Width { get { return width; } }
        public float Lean { get { return lean; } }
        public float Height { get { return height; } }
        public float RotateV { get { return rotateV; } }
        public float RotateH { get { return rotateH; } }

        #endregion

        #region Unity methods
        protected virtual void Start()
        {
            InitVehicle();
        }

        protected virtual void Update()
        {
            if (controller) {
                HandleInput();
            }
            if (rb) {
                HandlePhysics();
            }
        }
        #endregion

        #region Custom methods

        //Init field of objects
        protected virtual void InitVehicle()
        {
            rb = GetComponent<Rigidbody>();
            pb = GetComponent<Physicsbody>();
            objectHeight = 2f;// GetComponent<MeshFilter>().mesh.bounds.extents.z;

            //Configure rb so it doesnt do stupid shit like gravity
            rb.useGravity = false;

            //Allocate Centre of Gravity
            if (rb && centreOfGravity) {
                rb.centerOfMass = centreOfGravity.localPosition;
            }

            //Set up some cameras!
            cameraGroup = transform.Find("Cameras");

            if (cameraGroup) {

                cameras = new Transform[cameraGroup.transform.childCount];

                if (cameras.Length > 0) {
                    foreach (Transform _child in cameraGroup) {
                        if (_child == null)
                            continue;

                        cameras[cameraCount] = _child;
                        cameraCount++;
                    }
                    cam = cameras[0];

                } else {
                    Debug.LogWarning(name + " found camera group but no cameras- don't forget to add some!");
                }
            }
            else {
                Debug.LogWarning(name + " Did not find a camera group- is this object properly initializsed?");
            }
        }


        //Called if we have a controller assigned on every update. Movement stuff basically
        protected virtual void HandleInput()
        {
            
        }
        
        //Called if we have a Rigidbody assigned on every update. Should leave most of this to a Physicsbody if we can help it...
        protected virtual void HandlePhysics()
        {
            HandleGravity();
            HandleGround();
        }

        protected virtual void HandleGravity()
        {
            //If we have a Physicsbody, borrow their calculation since we dont want to do it twice...
            if (pb) {
                gravityUp = pb.GravityUp;
                gravityRight = pb.GravityRight;
                gravityForward = pb.GravityForward;
            }
            else {
                //Get the nearest gravitational field and use this as our primary sourse of data
                GameObject fieldTmp = GeoCoord.NearestGravitationalBody(transform.position);
                if (fieldTmp != nearestGravityField) {
                    nearestGravityField = fieldTmp;
                    gb = nearestGravityField.GetComponent<GravitationalBody>();
                }

                gravityUp = gb.GravityUp(transform);

                //Also collect right and forward while we are here
                gravityRight = Vector3.Cross(rb.velocity.normalized, gravityUp);
                gravityForward = Vector3.Cross(gravityUp, gravityRight);

                //TODO: MOVE TO METHOD SO WE DONT HAVE DUPLICATE CODE
            }
        }

        protected virtual void HandleGround()
        {
            // Grounded check
            Ray ray = new Ray(transform.position, -gravityUp);
            RaycastHit hit;

            grounded = (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask));
        }
        //Cycle through the cameras
        public virtual void CycleCamera()
        {
            currentCamera++;
            if (currentCamera >= cameraCount) {
                currentCamera = 0;
            }
            cam = cameras[currentCamera];
        }

        //Set the controller that will move this object
        public virtual void SetController(ControllerBase controllerVar)
        {
            if (controller != null) {
                //TELL OLD CONTROLLER WE SHOULD STAY FRIENDS
            }

            controller = controllerVar;
        }

        //Remove old controller. Will only work if we know the old controller!
        public virtual void RemoveController(ControllerBase controllerVar)
        {
            if (controller == controllerVar) {
                controller = null;
            } else {
                Debug.LogWarning(name + " cannot remove controller: " + controllerVar + " does not match " + controller);
            }
        }

        //A quick script to get pos above ground
        public virtual LatLon GetGeoPos()
        {
            return GeoCoord.GetLatLon(gb, transform.position);
        }
        #endregion
    }
}