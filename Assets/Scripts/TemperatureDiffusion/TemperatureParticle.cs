using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureParticle : MonoBehaviour
{
    // DATA //
    public Gradient colourGradient;

    // Cached data
    private float spreadDistance = 5;
    public float currentEnergy = 0;
    private float heatCapacity = 1;
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
    public static readonly float MAX_ENERGY = 1000;
    public static readonly float MIN_ENERGY = 0;
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
        currentEnergy = Mathf.Clamp(currentEnergy+addedEnergy, MIN_ENERGY, MAX_ENERGY);
    }

    public void UpdateParticleData(float newHeatCapacity, float newSpreadDistance)
    {
        heatCapacity = newHeatCapacity;
        spreadDistance = newSpreadDistance;
    }

    public void UpdateEnergy(float deltaTime)
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
                if(particle.Temperature-Temperature > 999 || particle.Temperature - Temperature < -999)
                {
                    Debug.Log("AAAAAAAAAAAAAAAA! Other = " + particle.Temperature + ", This = " + Temperature);
                }
                float energyTransfer = BOLTZMANN_CONSTANT * (particle.Temperature - Temperature);
                currentEnergy += energyTransfer*deltaTime;
            }
        }

        UpdateParticleColour();
    }

    
    // Internal Management
    private void UpdateParticleColour()
    {
        spriteRenderer.color = colourGradient.Evaluate(Temperature / MAX_ENERGY);
    }
}
