using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureSimManager : MonoBehaviour
{
    // DATA //
    // Game References
    public Collider2D spawnBox;
    public GameObject particlePrefab;

    // Sim Management Data
    public float spreadDistance = 5;
    public float heatCapacity = 1;
    public int numParticles;
    public float startEnergy;

    // Cached Data
    private List<TemperatureParticle> allParticles;


    // FUNCTIONS //
    // Unity Defaults
    private void Awake()
    {
        allParticles = new List<TemperatureParticle>();   
    }

    private void Start()
    {
        SpawnAllParticles();
    }

    private void Update()
    {
        // Calculates new temperatures for all particles
        foreach(TemperatureParticle particle in allParticles)
        {
            particle.CacheNewEnergy(Time.deltaTime);
        }

        // Updates temperatures for all particles, updates their properties
        foreach(TemperatureParticle particle in allParticles)
        {
            particle.UpdateCurrentEnergy();
            particle.UpdateParticleData(heatCapacity, spreadDistance);
        }
    }


    // Simulation Management
    public void SpawnAllParticles()
    {
        for(int i = 0; i < numParticles; i++)
        {
            GameObject spawnedObj = Instantiate(particlePrefab, MathFunctions.RandomPointInBounds(spawnBox.bounds), particlePrefab.transform.rotation, spawnBox.transform);
            TemperatureParticle particleRef = spawnedObj.GetComponent<TemperatureParticle>();
            allParticles.Add(particleRef);
            particleRef.AddRawEnergy(startEnergy);
            particleRef.UpdateCurrentEnergy();
        }
    }


    // UI Events
    public void UpdateParticleTemp(TemperatureParticle particleToUpdate, float newTemperature)
    {
        //TODO
    }
}
