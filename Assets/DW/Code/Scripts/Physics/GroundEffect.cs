using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(VehicleBase))]
    public class GroundEffect : MonoBehaviour
    {
        public float maxGroundDistance = 3f;
        public float liftForce = 100f;
        public float maxSpeed = 15f;
        private Rigidbody rb;
        private VehicleBase vb;

        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            vb = GetComponent<VehicleBase>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (rb && vb) {
                HandleGroundEffect();
            }
        }

        protected virtual void HandleGroundEffect()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -vb.GravityUp, out hit)) {
                if (hit.transform.tag == "Terrain" && hit.distance < maxGroundDistance) {

                    float currentSPeed = rb.velocity.magnitude;
                    float normSpeed = Mathf.Clamp01(currentSPeed / maxSpeed);

                    float distance = maxGroundDistance - hit.distance;
                    float finalForce = liftForce * distance * normSpeed;
                    rb.AddForce(vb.GravityUp * finalForce);
                }
            }
        }
    }
}