using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TinyParticle : MonoBehaviour, IPointerDownHandler
{
    // DATA //
    // Data
    public Gradient trailDefault;
    public Gradient trailSelected;

    // Properties
    public Vector2 Velocity { get { return currentVelocity; } }

    // Cached Data
    private Vector2 currentVelocity;
    private TinyPlanet manager;
    private List<TinyPlanet> attractingBodies;
    private bool canMove = false;
    private TinyParticlesSimManager ui;
    private Vector2 startVelocity;
    private float startOrbit;
    private TrailRenderer trail;


    // FUNCTIONS //
    // Unity Defaults
    private void Awake()
    {
        attractingBodies = new List<TinyPlanet>(FindObjectsOfType<TinyPlanet>());
        trail = GetComponent<TrailRenderer>();
        SetTrailDefault();
    }

    public void OnPointerDown(PointerEventData clickData)
    {
        ui.OpenInfoDisplay(startVelocity, startOrbit, this);
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
    public void SetupParticle(TinyPlanet newManager, TinyParticlesSimManager uiReference, float orbitAtStart)
    {
        // Caches required data
        manager = newManager;
        ui = uiReference;

        // Starts moving!
        canMove = true;
        currentVelocity = Vector2.zero;
        startOrbit = orbitAtStart;
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

    public void CacheStartVelocity()
    {
        startVelocity = currentVelocity;
    }

    public void SetTrailDefault()
    {
        trail.colorGradient = trailDefault;
    }

    public void SetTrailSelected()
    {
        trail.colorGradient = trailSelected;
    }
}
