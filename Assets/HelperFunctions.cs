using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions
{
    public static bool CheckLineOfSight(Vector2 a, Vector2 b, LayerMask layerMask)
    {
        var dist = (a - b).magnitude;
        return Physics2D.Raycast(a, b, dist, layerMask);
    }
}
