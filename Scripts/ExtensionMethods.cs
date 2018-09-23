using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{
    //oh this is just a static utility it's not an extension method whoopsies
    public static float SignedAngle(Vector2 from, Vector2 to)
    {
        float unsigned_angle = Vector2.Angle(from, to);
        float sign = Mathf.Sign(from.x * to.y - from.y * to.x);
        return unsigned_angle * sign;
    }
}