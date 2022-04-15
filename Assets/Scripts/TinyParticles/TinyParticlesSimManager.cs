using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TinyParticlesSimManager : MonoBehaviour
{
    // References
    public GameObject planetPrefab;
    public BoxCollider2D spawnBox;

    // Cached data
    private Stack<TinyPlanet> addedPlanets;


    // FUNCTIONS //
    // Unity Defaults
    private void Awake()
    {
        addedPlanets = new Stack<TinyPlanet>();
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
}
