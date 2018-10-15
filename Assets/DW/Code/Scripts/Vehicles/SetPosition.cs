using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    public class SetPosition : MonoBehaviour
    {
        #region Variables
        public GameObject planetObj;
        private GravitationalBody planet;
        public Vector3 Position;

        private VehicleBase vb;
        private float ObjectHeightOffset = 0f;
        #endregion

        #region Unity Methods
        private void Start()
        {
            vb = GetComponent<VehicleBase>();
            if (vb != null) {
                ObjectHeightOffset = vb.ObjectHeight;
            }
        }
        #endregion

        #region Custom Methods
        public void SetPos(Vector3 pos)
        {
            planet = planetObj.GetComponent<GravitationalBody>();
            if (planet) {
                GeoCoord.SetPosATL(transform, new LatLon(planet, pos.x, pos.y), pos.z + (ObjectHeightOffset / 2));
                Debug.Log(transform.name + " placed at (" + pos.x + "," + pos.y + ")");
            } else {
                Debug.LogWarning("No planet data provided!");
            }
        }
        #endregion
    }
}