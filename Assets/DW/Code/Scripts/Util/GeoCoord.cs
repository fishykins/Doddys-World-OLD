using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw
{
    public class GeoCoord
    {

        //Internal function to do the fundemental maths
        private static Vector3 GetPosRadius(LatLon geoCoord, float radius)
        {
            float lat = geoCoord.lat * Mathf.Deg2Rad;
            float lon = ((geoCoord.lon) * Mathf.Deg2Rad); //Why -90? No idea. Puts in in sync with GetLatLon

            float xPos = (radius) * Mathf.Cos(lat) * Mathf.Cos(lon);
            float zPos = (radius) * Mathf.Cos(lat) * Mathf.Sin(lon);
            float yPos = (radius) * Mathf.Sin(lat);

            Vector3 worldPos = new Vector3(xPos, yPos, zPos);
            return worldPos;
        }

        //Returns the world coordinate of given lat/long and height (above sea level).
        public static Vector3 GetPosASL(LatLon geoCoord, float height = 0)
        {
            float radius = geoCoord.radius + height;
            Vector3 worldPos = GetPosRadius(geoCoord, radius);
            return worldPos;
        }

        //Returns the world coordinate of given lat/long and height (above sea level). Pretty crap method, but works
        public static Vector3 GetPosATL(LatLon geoCoord, float height = 0)
        {
            float elevation = GetTerrainElevation(geoCoord);
            return GetPosASL(geoCoord, height + elevation);
        }

        public static float GetHeightATL(GravitationalBody planet, Vector3 position, float offset = 0f)
        {
            LatLon geoPos = GetLatLon(planet, position);
            float terrainElevation = GetTerrainElevation(geoPos);
            float height = Vector3.Distance(position, geoPos.centre) - terrainElevation - geoPos.radius - offset;
            return height;
        }

        public static void SetPosATL(Transform transform, LatLon geoCoord, float height = 0)
        {
            Vector3 pos = GetPosATL(geoCoord, height);
            Vector3 gravityUp = (pos - geoCoord.centre).normalized;
            Vector3 localUp = transform.up;
            transform.rotation = Quaternion.FromToRotation(localUp, gravityUp) * transform.rotation;
            transform.position = pos;
        }

        //Converts a world coordiante into a lat/long value. 
        public static LatLon GetLatLon(GravitationalBody planet, Vector3 position)
        {
            float lon = (float)Mathf.Atan2(position.z, position.x); //phi
            float xzLength = new Vector2(position.x, position.z).magnitude;
            float lat = (float)Mathf.Atan2(position.y, xzLength); //phi

            return new LatLon(planet, Mathf.Rad2Deg * lat, Mathf.Rad2Deg * lon);
        }

        public static float GetDistance(LatLon posA, LatLon posB)
        {
            if (posA.body != posB.body) {
                Debug.LogError("Planet " + posA.body + " and " + posB.body + "provided: Must be the same!");
                return -1f;
            }

            float latA = posA.lat * Mathf.Deg2Rad;
            float lonA = posA.lon * Mathf.Deg2Rad;
            float latB = posB.lat * Mathf.Deg2Rad;
            float lonB = posB.lon * Mathf.Deg2Rad;

            float dLat = latB - latA;
            float dLon = lonB - lonA;

            float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) + Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2) * Mathf.Cos(latA) * Mathf.Cos(latB);
            return posA.radius * 2 * Mathf.Asin(Mathf.Sqrt(a));
        }

        public static float GetTerrainElevation(LatLon geoCoord)
        {
            float elevation = 0f;

            Vector3 pointOnUnitSphere = GeoCoord.GetPosRadius(geoCoord, 1);
            //Vector3 realWorldPos = geoCoord.planet.GetShapeGenerator().CalculatePointOnPlanet(pointOnUnitSphere);
            //geoCoord.planet.marker.transform.position = realWorldPos;
            //elevation = Vector3.Distance(geoCoord.centre, realWorldPos) - geoCoord.radius;
            elevation = 0f; // geoCoord.planet.GetShapeGenerator().CalculateHeight(pointOnUnitSphere) - geoCoord.radius;

            return elevation;
        }

        public static GameObject NearestGravitationalBody(Vector3 pos)
        {
            GameObject[] planets = GameObject.FindGameObjectsWithTag("GravitationalBody");

            GameObject nearestPlanet = null;
            float closestDistanceSqr = Mathf.Infinity;

            foreach (GameObject planet in planets) {
                Vector3 directionToPlanet = planet.transform.position - pos;
                float dSqrToPlanet = directionToPlanet.sqrMagnitude;
                if (dSqrToPlanet < closestDistanceSqr) {
                    closestDistanceSqr = dSqrToPlanet;
                    nearestPlanet = planet;
                }
            }

            return nearestPlanet;
        }

    }

    public struct LatLon
    {
        public GravitationalBody body;
        public float radius;
        public Vector3 centre;
        public float lat;
        public float lon;

        public LatLon(GravitationalBody body, float latitude, float longitude)
        {
            this.body = body;
            this.lat = latitude;
            this.lon = longitude;
            this.radius = this.body.radius;
            this.centre = this.body.transform.position;
        }
    }
}