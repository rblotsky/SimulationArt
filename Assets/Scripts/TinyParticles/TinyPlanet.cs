using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TinyPlanet : MonoBehaviour, IPointerDownHandler
{
    // DATA //
    // Particle Setup
    [Min(0)] public int maxParticleCount;
    [Min(0)] public int particlesPerSecond;
    [Min(0)] public int managerMass = 100;
    [Min(0)] public int startSpeedMax = 10;
    [Min(0)] public int deletionDistance = 500;
    public int crashDistance = 0;
    public int startOrbit = 20;
    public GameObject tinyParticlePrefab;

    // Internal Cached Data
    private List<TinyParticle> particles;
    private float timeBetweenCreation;
    private float lastParticleCreated = 0f;
    private int numParticlesPerUpdate = 1;
    private int particlesSpawned = 0;
    private int totalSpawned = 0;
    private List<TinyPlanet> allPlanets;
    private List<TinyParticle> particlesToRemove;
    private TinyParticlesSimManager ui;
    private bool isDraggingPlanet = false;
    private Camera mainCamera;


    // FUNCTIONS //
    // Unity Default Functions
    private void Awake()
    {
        // Creates lists
        particles = new List<TinyParticle>();
        particlesToRemove = new List<TinyParticle>();
        allPlanets = new List<TinyPlanet>(FindObjectsOfType<TinyPlanet>());

        // Finds all existing particles and adds itself as an attracting body
        TinyParticle[] allParticles = FindObjectsOfType<TinyParticle>();
        foreach(TinyParticle particle in allParticles)
        {
            particle.AddAttractingBody(this);
        }
    }

    private void Start()
    {
        // Adds itself to all other planets
        foreach (TinyPlanet planet in allPlanets)
        {
            planet.AddNewPlanet(this);
        }

        // Finds some required objects
        ui = FindObjectOfType<TinyParticlesSimManager>();
        mainCamera = Camera.main;

    }

    private void FixedUpdate()
    {
        // Spawns particle if it is long enough since the last one
        if (Time.time - lastParticleCreated >= timeBetweenCreation)
        {
            // Stops if reached max
            if (totalSpawned < maxParticleCount)
            {
                // Creates particles
                for (int i = 0; i < numParticlesPerUpdate; i++)
                {
                    GameObject spawnedParticleObject = Instantiate(tinyParticlePrefab, transform.position+(Vector3)Random.insideUnitCircle.normalized*startOrbit, transform.rotation);
                    TinyParticle spawnedParticle = spawnedParticleObject.GetComponent<TinyParticle>();
                    spawnedParticle.SetupParticle(this, ui, (spawnedParticle.transform.position-transform.position).magnitude);
                    spawnedParticle.AddInstantAcceleration(Random.insideUnitCircle.normalized * Random.Range(0.0f, startSpeedMax));
                    spawnedParticle.CacheStartVelocity();

                    particles.Add(spawnedParticle);
                    particlesSpawned++;
                    totalSpawned++;
                }

                // Updates time of creation
                lastParticleCreated = Time.time;
            }
        }

        // Runs through all particles, runs their movementUpdate function
        int currentParticleIndex = 0;
        while(currentParticleIndex < particlesSpawned)
        {
            // If this particle is past the max particles, removes it
            if (currentParticleIndex >= maxParticleCount)
            {
                Destroy(particles[currentParticleIndex].gameObject);
                particles.RemoveAt(currentParticleIndex);
                particlesSpawned--;
            }

            else
            {
                TinyParticle particleCached = particles[currentParticleIndex];
                particleCached.MovementUpdate(Time.fixedDeltaTime);

                // If this particle needs to be removed, removes it and doesn't update index
                if (particlesToRemove.Contains(particleCached))
                {
                    particles.RemoveAt(currentParticleIndex);
                    particlesSpawned--;
                }

                else
                {
                    currentParticleIndex++;
                }
            }
        }

        // Updates timing of creation and min/max distances
        UpdateCreationTiming();
    }

    private void Update()
    {
        // If dragging, checks if needs to stop or move planet
        if (isDraggingPlanet)
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDraggingPlanet = false;
            }
            else
            {
                transform.position = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }

    public void OnPointerDown(PointerEventData clickData)
    {
        // Starts dragging
        Debug.Log("Clicked Planet!");
        isDraggingPlanet = true;
    }


    // External Functions
    public void RemoveParticleFromManager(TinyParticle particle)
    {
        particlesToRemove.Add(particle);
    }

    public void AddNewPlanet(TinyPlanet newPlanet)
    {
        if (newPlanet != null && this != newPlanet && !allPlanets.Contains(newPlanet))
        {
            allPlanets.Add(newPlanet);
        }
    }

    public void RemoveOtherPlanet(TinyPlanet otherPlanet)
    {
        if(allPlanets.Contains(otherPlanet))
        {
            allPlanets.Remove(otherPlanet);
        }
    }

    public void ResetParticles()
    {
        // Removes all particles
        foreach(TinyParticle particle in particles)
        {
            Destroy(particle.gameObject);
        }

        particles.Clear();

        // Resets counts of particles spawned
        totalSpawned = 0;
        particlesSpawned = 0;
    }

    public void DestroyPlanet()
    {
        // Resets its particles
        ResetParticles();

        // Removes itself from all particles and other planets
        foreach (TinyPlanet planet in allPlanets)
        {
            if (planet != this)
            {
                planet.RemoveOtherPlanet(this);
            }
        }

        foreach (TinyParticle particle in particles)
        {
            particle.RemoveAttractingBody(this);
        }

        Destroy(gameObject);
    }


    // Internal Functions
    private void UpdateCreationTiming()
    {
        // Decides time between creation of particles
        timeBetweenCreation = 1.0f / particlesPerSecond;

        // If time between creation is lower than updates per second, determines how many particles to create per update
        if(timeBetweenCreation <= Time.fixedDeltaTime)
        {
            numParticlesPerUpdate = Mathf.RoundToInt(particlesPerSecond / (1.0f / Time.fixedDeltaTime));
        }
        else
        {
            numParticlesPerUpdate = 1;
        }
    }

}
