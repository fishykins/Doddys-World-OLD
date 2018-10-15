using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace dw
{
    [RequireComponent(typeof(ControllerBase))]
    public class PlayerUI : MonoBehaviour
    {

        //Display objects
        public GameObject gravityUIObj;
        public GameObject positionUIObj;
        public GameObject heightUIObj;
        public GameObject tempUIObj;
        public GameObject liftUIObj;

        //Variable caching
        private float gravity = 0f;
        private VehicleBase vehicle;

        //Component Caching
        private TextMeshProUGUI tmpGravity;
        private TextMeshProUGUI tmpPosition;
        private TextMeshProUGUI tmpHeight;
        private TextMeshProUGUI tmpTemp;
        private TextMeshProUGUI tmpLift;
        private ControllerBase controller;

        void Start()
        {
            controller = GetComponent<ControllerBase>();
            tmpGravity = gravityUIObj.GetComponent<TextMeshProUGUI>();
            tmpPosition = positionUIObj.GetComponent<TextMeshProUGUI>();
            tmpHeight = heightUIObj.GetComponent<TextMeshProUGUI>();
            tmpTemp = tempUIObj.GetComponent<TextMeshProUGUI>();
            tmpLift = liftUIObj.GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            vehicle = controller.ControlTarget;
            if (vehicle != null) {

                LatLon geoPos = vehicle.GetGeoPos();
                float TerrainHeight = GeoCoord.GetTerrainElevation(geoPos);
                tmpPosition.text = ("LatLonHeight: (" + Mathf.Round(geoPos.lat * 10f) / 10f + "," + Mathf.Round(geoPos.lon * 10f) / 10f + "," + Mathf.Round(TerrainHeight * 100f) / 100f) + ")";

                /*
                tmpGravity.text = ("Gravity: " + Mathf.Round(vehicle.GetGravitationalForce() * 100f) / 100f);

                tmpHeight.text = ("Height: " + Mathf.Round(vehicle.GetHeight() * 100f) / 100f);

                tmpTemp.text = ("Temp: " + Mathf.Round(vehicle.GetAirDensity() * 1000f) / 1000f);

                tmpLift.text = ("Lift: " + Mathf.Round(vehicle.GetLiftForce() * 100f) / 100f);
                */

            }

        }
    }
}


