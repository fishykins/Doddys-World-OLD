using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw {
	public class Flightbody : Physicsbody
    {
        #region Variables
        //Public


        [Header("Flight Settings")]
        public float maxMPH = 110f;

        [Header("Air Resistance")]
        public bool hasAirResistance = false;

        [Header("Lift")]
        public bool hasLift = true;
        public float maxLiftPower = 800f;
        public AnimationCurve liftCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Drag")]
        public bool hasDrag = true;
        public float dragFactor = 0.01f;
        public float flapDragFactor = 0.005f;

        [Header("Rigidbody Assist")]
        public bool hasRigidbodyAssist = true;
        [Range(0f, 1f)]
        public float rbAssist = 0.01f;
        public float rbaMinAirSpeed = 30f;
        public float rbaMaxAirSpeed = 50f;



        //Private
        private float airSpeed;
        private float mph;
        private float normalisedMPH;
        private float angleOfAttack;
        private float liftPower;
        private float rbAssistPower;
        private float dragPower;
        #endregion;



        #region Properties
        public float LiftPower { get { return liftPower; } }
        public float AngleOfAttack { get { return angleOfAttack; } }
        public float AirSpeed { get { return airSpeed; } }
        public float RbAssistPower { get { return rbAssistPower; } }
        public float DragPower { get { return dragPower; } }
        #endregion;

        #region Unity Methods

        #endregion;

        #region Custom Methods
        public override void HandlePhysics()
        {
            base.HandlePhysics();

            airSpeed = rb.velocity.magnitude;

            CalculateSpeed(); //Base class that just needs calling once
            CalculateForwardSpeed();
            if (hasDrag) {
                //GenerateOrganicDrag();
                GenerateRbDrag(); //Doesnt like gravity very much :(
            }
            if (hasLift) {
                GenerateLift();
            }
            if (hasRigidbodyAssist) {
                HandleRigidbodyAssist();
            }
        }

        //Just tweak the forward speed a little (found in base class)
        private void CalculateForwardSpeed()
        {
            forwardSpeed = Mathf.Max(0f, forwardSpeed);

            mph = forwardSpeed * mpsToMph;
            normalisedMPH = Mathf.InverseLerp(0f, maxMPH, mph);
        }

        private void GenerateRbDrag()
        {
            float speedDrag = forwardSpeed * dragFactor;
            float finalDrag = startDrag + speedDrag; // + flapsDrag;

            rb.drag = finalDrag;
            rb.angularDrag = startAngularDrag * forwardSpeed;
        }

        //Because RB drag doesn't allow for nice gravity, calculate it and apply our own
        private void GenerateOrganicDrag()
        {
            dragPower = forwardSpeed * forwardSpeed * dragFactor;
            rb.AddForce(-gravityForward.normalized * dragPower);

            rb.angularDrag = startAngularDrag * forwardSpeed;
        }

        private void GenerateLift()
        {
            angleOfAttack = Vector3.Dot(rb.velocity.normalized, transform.forward);
            angleOfAttack *= angleOfAttack;

            Vector3 liftDir = transform.up;
            liftPower = liftCurve.Evaluate(normalisedMPH) * maxLiftPower * angleOfAttack;

            Vector3 finalLiftForce = liftDir * liftPower;
            rb.AddForce(finalLiftForce);
        }

        void HandleRigidbodyAssist()
        {
            if (airSpeed > 1f) {

                rbAssistPower = rbAssist;// Mathf.InverseLerp(rbaMinAirSpeed, rbaMaxAirSpeed, airSpeed) * rbAssist;

                //Vector3 updatedVelocity = Vector3.Lerp(rb.velocity, transform.forward * forwardSpeed, forwardSpeed * angleOfAttack * Time.deltaTime * rbAssistPower);
                //rb.velocity = updatedVelocity;

                Quaternion updatedRotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(rb.velocity.normalized, transform.up), Time.deltaTime * rbAssistPower);
                rb.MoveRotation(updatedRotation);
            }
        }

        #endregion
    }
}