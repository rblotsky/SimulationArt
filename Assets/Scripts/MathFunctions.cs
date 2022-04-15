using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathFunctions
{
    // Constants
    public static readonly float GRAVITY_CONSTANT = 1;


    // Math Functions
    public static float CalculateGravityAcceleration(float mass1, float mass2, float distanceSquared)
    {
        return GRAVITY_CONSTANT * (mass1 * mass2 / distanceSquared);
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return bounds.center + new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z));
    }
}
