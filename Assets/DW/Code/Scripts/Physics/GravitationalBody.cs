using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    public class GravitationalBody : MonoBehaviour
    {
        #region Variablies
        public enum GravitySimulation { Advanced, Hybrid, Simple };

        [Header("Universal Gravity Settings")]
        public GravitySimulation gravitySimulation;
        [Tooltip("Body radius in meters (ie, planet radius)")]
        public float radius = 6371000f;

        [Header("Advanced Gravity Factors")]
        [Tooltip("Body density in kg/m3")]
        public float density = 5515.3f;
        [Tooltip("Override default value, if you so desire (* 10^-11)")]
        public float gravitationalConstantBase = 6.674f;

        [Header("Simple Gravity Factors")]
        [Tooltip("Range of gravity in meters")]
        public float gravitationalRange = 2000000f;
        [Tooltip("Strength in meters per second")]
        public float gravitationalStength = 9.8f;
        [Tooltip("Allows for non-linear gravity dropoff")]
        public AnimationCurve gravityCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        protected Rigidbody rb;
        protected float mass;
        protected float volume;
        protected float gravitationalConstant;

        protected float gravityMinRadius;
        protected float gravityMaxRadius;

        protected bool debugBody = false;
        #endregion

        #region Properties
        public Vector3 Centre { get { return transform.position; } }
        #endregion

        #region Unity Methods
        protected virtual void Start()
        {
            InitializeBody(debugBody);
        }

        #endregion

        #region Custom Methods 
        public virtual void InitializeBody(bool debugGrav = false)
        {
            rb = GetComponent<Rigidbody>();

            gravitationalConstant = gravitationalConstantBase * 1e-11f;
            gravityMinRadius = radius;
            gravityMaxRadius = radius + gravitationalRange;
            volume = 1.3f * Mathf.PI * Mathf.Pow(radius, 3);

            //If we have a rigidbody use the mass of that, otherwise use density to calculate
            mass = (rb) ? rb.mass : volume * density;

            if (debugGrav) {
                RunGravitySimulation();
            }
        }

        //Returns the up vector relative to passed transform
        public virtual Vector3 GravityUp(Transform passedTransform)
        {
            return (passedTransform.position - transform.position).normalized;
        }


        public virtual Vector3 GetGravitationalForce(Vector3 point, float objMass)
        {
            Vector3 gravitationalForce = Vector3.zero;
            float gravitationalStrength = GetGravitationalStrength(point, objMass);

            //We are just assuming that the passed object is not as heavy as we are. Maybe add a catch for this at some point...
            gravitationalForce = -gravitationalStrength * (point - transform.position).normalized;

            return gravitationalForce;
        }

        public virtual float GetGravitationalStrength (Vector3 point, float objMass)
        {
            float gravitationalStrength = 0f;

            switch (gravitySimulation) {
                case GravitySimulation.Advanced: 
                    gravitationalStrength = GetGravityAdvanced(point, objMass);
                    break;
                case GravitySimulation.Hybrid:
                    gravitationalStrength = GetGravityHybrid(point, objMass);
                    break;
                case GravitySimulation.Simple:
                    gravitationalStrength = GetGravitySimple(point, objMass);
                    break;
                default:
                    break;
            }

            //Make sure that the force applied is possitive!
            gravitationalStrength = Mathf.Abs(gravitationalStrength);
            return gravitationalStrength;
        }

        //Most realistc gravity
        protected virtual float GetGravityAdvanced(Vector3 point, float objMass)
        {
            return Gravity.GetForce(mass, objMass, transform.position, point, gravitationalConstant);

        }

        //A mixture of advanced and simple gravity calculation. Allows for a more controllable end result, but is also the most costly to perform.
        protected virtual float GetGravityHybrid(Vector3 point, float objMass)
        {
            float realForce = Gravity.GetForce(mass, objMass, transform.position, point, gravitationalConstant);
            float distance = Vector3.Distance(transform.position, point);
            //Return a strength from 0-1
            float strength = Mathf.InverseLerp(gravityMaxRadius, gravityMinRadius, distance);
            float gravity = realForce * gravityCurve.Evaluate(strength);
            return gravity * objMass;
        }

        //Arcade like results, easy to control using set variables.
        protected virtual float GetGravitySimple(Vector3 point, float objMass)
        {
            float distance = Vector3.Distance(transform.position, point);
            //Return a strength from 0-1
            float strength = Mathf.InverseLerp(gravityMaxRadius, gravityMinRadius, distance);
            float gravity = gravitationalStength * gravityCurve.Evaluate(strength);
            return gravity * objMass;
        }

        protected virtual void RunGravitySimulation()
        {
            Debug.Log("Mass of Gravity object: " + mass + "kg");

            float gravity = GetGravitationalStrength(new Vector3(0f, radius * 1, 0f), 100f);
            Debug.Log("Gravity at surface on a fishy = " + gravity + "N");

            gravity = GetGravitationalStrength(new Vector3(0f, radius * 1.5f, 0f), 100f);
            Debug.Log("Gravity at 1.5R on a fishy = " + gravity + "N");

            gravity = GetGravitationalStrength(new Vector3(0f, radius + (gravitationalRange / 2), 0f), 100f);
            Debug.Log("Gravity at orbit range /2 on a fishy = " + gravity + "N");
        }

        #endregion
    }
}