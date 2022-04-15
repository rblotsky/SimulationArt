using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TinyParticlesSimManager : MonoBehaviour
{
    // References
    public GameObject planetPrefab;
    public BoxCollider2D spawnBox;
    public GameObject particleInfoPanel;
    public TextMeshProUGUI particleStartSpeedText;
    public TextMeshProUGUI particleStartVelocityText;
    public TextMeshProUGUI particleStartOrbitText;
    public TextMeshProUGUI particleCurrentVelocityText;

    // Cached data
    private Stack<TinyPlanet> addedPlanets;
    private TinyParticle currentViewedParticle;


    // FUNCTIONS //
    // Unity Defaults
    private void Awake()
    {
        addedPlanets = new Stack<TinyPlanet>();
    }

    private void LateUpdate()
    {
        // Updates the info display
        if(currentViewedParticle != null && currentViewedParticle.isActiveAndEnabled)
        {
            particleCurrentVelocityText.SetText("Current Velocity: " + currentViewedParticle.Velocity.ToString() + "(" + currentViewedParticle.Velocity.magnitude + " u/s)");
        }
    }


    // External Functions
    public void ResetParticles()
    {
        // Gets all tiny planets, removes all their particles.
        TinyPlanet[] allPlanets = FindObjectsOfType<TinyPlanet>();

        foreach(TinyPlanet planet in allPlanets)
        {
            planet.ResetParticles();
        }
    }

    public void AddNewPlanetAtRandomPos()
    {
        // Gets a random position within the spawn box and 
        Vector3 planetPos = MathFunctions.RandomPointInBounds(spawnBox.bounds);
        TinyPlanet addedPlanet = Instantiate(planetPrefab, planetPos, planetPrefab.transform.rotation).GetComponent<TinyPlanet>();
        addedPlanets.Push(addedPlanet);
    }

    public void RemoveLastAddedPlanet()
    {
        if (addedPlanets.Count != 0)
        {
            addedPlanets.Peek().DestroyPlanet();
            addedPlanets.Pop();
        }
    }

    public void ModifyTimescale(Slider valueSlider)
    {
        Time.timeScale = valueSlider.value;
    }

    public void CloseInfoDisplay()
    {
        particleStartOrbitText.SetText("");
        particleStartVelocityText.SetText("");
        particleStartSpeedText.SetText("");
        particleCurrentVelocityText.SetText("");
        particleInfoPanel.SetActive(false);
        currentViewedParticle = null;
    }

    public void OpenInfoDisplay(Vector2 startVelocity, float startOrbit, TinyParticle particle)
    {
        // Resets the trail of the current selected particle
        if(currentViewedParticle != null)
        {
            currentViewedParticle.SetTrailDefault();
        }

        // Enables the panel, caches new particle
        particleInfoPanel.SetActive(true);
        currentViewedParticle = particle;

        // Only displays data if the particle exists
        if (particle != null && particle.isActiveAndEnabled)
        {
            // Sets the particle to have a red trail
            currentViewedParticle.SetTrailSelected();

            // Updates text
            particleStartSpeedText.SetText("Start Speed: " + startVelocity.magnitude + " u/s");
            particleStartVelocityText.SetText("Start Velocity: " + startVelocity.ToString());
            particleStartOrbitText.SetText("Start Orbit: " + startOrbit + " units");
            particleCurrentVelocityText.SetText("Current Velocity: " + particle.Velocity.ToString());
        }
    }
}
