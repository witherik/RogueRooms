using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions
{
    public static bool CheckLineOfSight(Vector2 a, Vector2 b, LayerMask layerMask)
    {
        var dir = b - a;
        Debug.DrawRay(a, dir, Color.blue);
        return !Physics2D.Raycast(a, dir.normalized, dir.magnitude, layerMask);
    }
    public static Vector2 rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

}
