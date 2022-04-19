using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureParticle : MonoBehaviour
{
    // DATA //
    public Gradient colourGradient;

    // Cached data
    private float spreadDistance = 5;
    private float currentEnergy = 0;
    private float heatCapacity = 1;
    private float newEnergy;
    private SpriteRenderer spriteRenderer;

    // Properties
    private float Temperature
    {
        get
        {
            return currentEnergy / (heatCapacity*PARTICLE_MASS);
        }
    }

    // Constants
    public static readonly float PARTICLE_MASS = 1;
    public static readonly float MAX_TEMPERATURE = 1000;
    public static readonly float MIN_TEMPERATURE = 0;
    public static readonly float BOLTZMANN_CONSTANT = 1;



    // FUNCTIONS //
    // Unity Defaults
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // Particle Management
    public void AddRawEnergy(float addedEnergy)
    {
        newEnergy += addedEnergy;
    }

    public void UpdateParticleData(float newHeatCapacity, float newSpreadDistance)
    {
        heatCapacity = newHeatCapacity;
        spreadDistance = newSpreadDistance;
    }

    public void CacheNewEnergy(float deltaTime)
    {
        // This function caches the energy to set to on the next frame. The energy has to be cached and set later to ensure that
        // one particle doesn't update its energy before others have time to calculate their new energy.

        // Finds nearby particles
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, spreadDistance);

        // Goes through each one and sets its next update temperature
        foreach(Collider2D collider in nearbyColliders)
        {
            TemperatureParticle particle = collider.GetComponent<TemperatureParticle>();

            if(particle != null && particle != this)
            {
                // Calculates energy transferred
                float energyTransfer = BOLTZMANN_CONSTANT * (particle.Temperature-Temperature) * (1); // The 1 at the end should be replaced by area that heat is being radiated at? Or use conduction formula in order to include distance
                newEnergy += energyTransfer*deltaTime;
            }
        }
    }

    public void UpdateCurrentEnergy()
    {
        // First clamps energy to within allowed range
        newEnergy = Mathf.Clamp(newEnergy, MIN_TEMPERATURE, MAX_TEMPERATURE);

        // Sets the current energy to equal the new one (should have been cached just before this runs).
        currentEnergy = newEnergy;
        UpdateParticleColour();
    }

    
    // Internal Management
    private void UpdateParticleColour()
    {
        spriteRenderer.color = colourGradient.Evaluate(Temperature / MAX_TEMPERATURE);
    }
}
