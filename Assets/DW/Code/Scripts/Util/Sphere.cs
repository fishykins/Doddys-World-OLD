using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Shpere
{
    Vector3 centre;
    float radius;

    public Shpere(Vector3 centre, float radius)
    {
        this.centre = centre;
        this.radius = radius;
    }
}

public static class SphereMaths  {

    /// <summary>
    /// transforms unitCube position into unitSphere,
    /// implementation license: public domain,
    /// uses math from http://mathproofs.blogspot.cz/2005/07/mapping-cube-to-sphere.html
    /// </summary>
    /// <param name="unitCube">unitCube.xyz is in inclusive range [-1, 1]</param>
    /// <returns></returns>
    public static Vector3 UnitCubeToUnitSphere(Vector3 unitCube)
    {
        var unitCubePow2 = new Vector3(unitCube.x * unitCube.x, unitCube.y * unitCube.y, unitCube.z * unitCube.z);
        var unitCubePow2Div2 = unitCubePow2 / 2;
        var unitCubePow2Div3 = unitCubePow2 / 3;
        var unitSphere = new Vector3(
            unitCube.x * Mathf.Sqrt(1 - unitCubePow2Div2.y - unitCubePow2Div2.z + unitCubePow2.y * unitCubePow2Div3.z),
            unitCube.y * Mathf.Sqrt(1 - unitCubePow2Div2.z - unitCubePow2Div2.x + unitCubePow2.z * unitCubePow2Div3.x),
            unitCube.z * Mathf.Sqrt(1 - unitCubePow2Div2.x - unitCubePow2Div2.y + unitCubePow2.x * unitCubePow2Div3.y)
        );
        return unitSphere;
    }
}
