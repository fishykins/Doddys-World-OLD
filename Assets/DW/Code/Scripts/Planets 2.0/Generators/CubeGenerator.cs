using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dirty constructor to manually create the six faces of a cube
/// </summary>

namespace dw.planet {
	public class CubeGenerator  {
        #region Variables
        //Private
        private int index;
        private string name;
        private Vector3 axisA;
        private Vector3 axisB;
        private int north;
        private int south;
        private int west;
        private int east;
        private int correctionNorth;
        private int correctionSouth;
        private int correctionWest;
        private int correctionEast;

        private Face[] faces = new Face[6];
        #endregion;

        #region Properties

        #endregion;

        #region Custom Methods
        public Cube GenerateCube(Planet planet)
        {
            index = 0;

            //Top (0)
            name = "Top";
            axisA = Vector3.right;
            axisB = Vector3.forward;
            north = 4;
            east = 3;
            south = 5;
            west = 2;
            correctionNorth = 0;
            correctionSouth = 0;
            correctionWest = 0;
            correctionEast = 0;
            faces[index] = new Face(name, index, axisA, axisB, north, east, south, west, correctionNorth, correctionEast, correctionSouth, correctionWest, planet);

            index++;

            //Down (1)
            name = "Down";
            axisA = Vector3.left;
            axisB = Vector3.forward;
            north = 4;
            east = 0;
            south = 5;
            west = 1;
            correctionNorth = 2;
            correctionSouth = 2;
            correctionWest = 0;
            correctionEast = 0;
            faces[index] = new Face(name, index, axisA, axisB, north, east, south, west, correctionNorth, correctionEast, correctionSouth, correctionWest, planet);

            index++;

            //Left (2)
            name = "Left";
            axisA = Vector3.up;
            axisB = Vector3.forward;
            north = 4;
            east = 0;
            south = 5;
            west = 1;
            correctionNorth = 1;
            correctionSouth = 3;
            correctionWest = 0;
            correctionEast = 0;
            faces[index] = new Face(name, index, axisA, axisB, north, east, south, west, correctionNorth, correctionEast, correctionSouth, correctionWest, planet);

            index++;

            //Right (3)
            name = "Right";
            axisA = Vector3.down;
            axisB = Vector3.forward;
            north = 4;
            east = 1;
            south = 5;
            west = 0;
            correctionNorth = 3;
            correctionSouth = 1;
            correctionWest = 0;
            correctionEast = 0;
            faces[index] = new Face(name, index, axisA, axisB, north, east, south, west, correctionNorth, correctionEast, correctionSouth, correctionWest, planet);

            index++;

            //Front (4)
            name = "Front";
            axisA = Vector3.right;
            axisB = Vector3.down;
            north = 1;
            east = 3;
            south = 0;
            west = 2;
            correctionNorth = 2;
            correctionSouth = 0;
            correctionWest = 3;
            correctionEast = 1;
            faces[index] = new Face(name, index, axisA, axisB, north, east, south, west, correctionNorth, correctionEast, correctionSouth, correctionWest, planet);

            index++;

            //Back (5)
            name = "Back";
            axisA = Vector3.right;
            axisB = Vector3.up;
            north = 0;
            east = 3;
            south = 1;
            west = 2;
            correctionNorth = 0;
            correctionSouth = 2;
            correctionWest = 1;
            correctionEast = 3;
            faces[index] = new Face(name, index, axisA, axisB, north, east, south, west, correctionNorth, correctionEast, correctionSouth, correctionWest, planet);

            return new Cube(faces);
        }
        #endregion;
    }
}