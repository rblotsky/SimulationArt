using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyParticle : MonoBehaviour
{
    // DATA //
    // External Data
    public Gradient massColourGradient;

    // Cached Data
    private Vector2 currentVelocity;
    private TinyPlanet manager;
    private List<TinyPlanet> attractingBodies;
    private bool canMove = false;


    // FUNCTIONS //
    // Unity Defaults
    private void Awake()
    {
        attractingBodies = new List<TinyPlanet>(FindObjectsOfType<TinyPlanet>());
    }

    // Movement Functions
    private void MoveParticle(Vector3 velocity, float deltaTime)
    {
        transform.position += (velocity * deltaTime);
    }

    public void MovementUpdate(float deltaTime)
    {
        if (canMove)
        {
            // Runs gravity for all attractors
            foreach (TinyPlanet attractor in attractingBodies)
            {
                // If the planet no longer exists, doesn't run anything for it
                if(attractor == null)
                {
                    continue;
                }

                // Gets squared magnitude of distance from attractor
                float attractorSqrDist = (attractor.transform.position - transform.position).sqrMagnitude;

                // Accelerates toward center according to gravity
                if (attractorSqrDist != 0)
                {
                    currentVelocity += (Vector2)(attractor.transform.position - transform.position).normalized * MathFunctions.CalculateGravityAcceleration(1, attractor.managerMass, attractorSqrDist);
                }

                // Removes if square distance is too high or too low
                if (attractorSqrDist > attractor.deletionDistance * attractor.deletionDistance || attractorSqrDist < attractor.crashDistance * attractor.crashDistance)
                {
                    manager.RemoveParticleFromManager(this);
                    Destroy(gameObject);
                    return;
                }
            }

            // Moves according to current velocity and fixeddeltatime
            MoveParticle(currentVelocity, deltaTime);
        }
    }

    // Management Functions
    public void SetupParticle(TinyPlanet newManager)
    {
        // Caches required data
        manager = newManager;

        // Starts moving!
        canMove = true;
        currentVelocity = Vector2.zero;
    }

    public void AddInstantAcceleration(Vector2 acceleration)
    {
        currentVelocity += acceleration;
    }

    public void AddAttractingBody(TinyPlanet newBody)
    {
        attractingBodies.Add(newBody);
    }

    public void RemoveAttractingBody(TinyPlanet body)
    {
        attractingBodies.Remove(body);
    }
}
