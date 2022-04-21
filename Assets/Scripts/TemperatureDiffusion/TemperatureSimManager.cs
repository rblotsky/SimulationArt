using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TemperatureSimManager : MonoBehaviour
{
    // DATA //
    // Game References
    public Collider2D spawnBox;
    public GameObject particlePrefab;

    // Sim Management Data
    public float energyAddRadius = 10;
    public float spreadDistance = 10;
    public float heatCapacity = 1;
    public int requiredParticleCount;
    public float startEnergy;

    // Cached Data
    private List<TemperatureParticle> allParticles;
    private float addedEnergyAmount = 0;


    // FUNCTIONS //
    // Unity Defaults
    private void Awake()
    {
        allParticles = new List<TemperatureParticle>();   
    }

    private void Update()
    {
        // First, removes unnecessary particles or adds new ones
        int numParticlesSpawned = allParticles.Count;

        // Loop through all particles currently or potentially spawned. (Stops when both numbers are equal)
        while(numParticlesSpawned != requiredParticleCount)
        {
            // If the number of particles is too high, checks if this one has to be removed
            if (numParticlesSpawned > requiredParticleCount)
            {
                // If we're past the limit, removes the last particle
                Destroy(allParticles[numParticlesSpawned-1].gameObject);
                allParticles.RemoveAt(numParticlesSpawned-1);
                numParticlesSpawned--;
            }

            // If it's too low, adds new particles until we reach the required amount
            else if (numParticlesSpawned < requiredParticleCount)
            {
                SpawnNewParticle();
                numParticlesSpawned++;
            }
        }

        // Calculates new temperatures for all particles
        foreach(TemperatureParticle particle in allParticles)
        {
            particle.UpdateEnergy(Time.deltaTime);
            particle.UpdateParticleData(heatCapacity, spreadDistance);
        }

        // If the user clicks anywhere, adds energy to all particles within radius
        if(Input.GetMouseButton(0))
        {
            AddEnergyAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }


    // Simulation Management
    public void SpawnAllParticles()
    {
        for(int i = 0; i < requiredParticleCount; i++)
        {
            SpawnNewParticle();
        }
    }

    public void SpawnNewParticle()
    {
        // Finds a random position in the spawn box
        GameObject spawnedObj = Instantiate(particlePrefab, MathFunctions.RandomPointInBounds(spawnBox.bounds), particlePrefab.transform.rotation, spawnBox.transform);
        TemperatureParticle particleRef = spawnedObj.GetComponent<TemperatureParticle>();
        allParticles.Add(particleRef);
        particleRef.AddRawEnergy(startEnergy);
    }


    // UI Events
    public void ModifyEnergyAddAmount(Slider amountSlider)
    {
        addedEnergyAmount = amountSlider.value;
    }

    public void ModifyParticleAmount(Slider amountSlider)
    {
        requiredParticleCount = (int)amountSlider.value;
    }    

    public void AddEnergyAtPosition(Vector2 worldPos)
    {
        // Finds all colliders within radius
        Collider2D[] foundColliders = Physics2D.OverlapCircleAll(worldPos, energyAddRadius);

        // Adds energy to all of them
        foreach(Collider2D collider in foundColliders)
        {
            TemperatureParticle particle = collider.GetComponent<TemperatureParticle>();
            
            if(particle != null)
            {
                particle.AddRawEnergy(addedEnergyAmount);
            }
        }
    }
}
