using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OreDrill : MonoBehaviour
{
    private PrefabManager prefabManager;
    private StatsManager statsManager;
    private AudioSource audioSource;
    private Camera mainCamera;
    private ItemCardsDisplay itemCardsDisplay;
    private EnvironmentManager environmentManager;
    private OreSpawner oreSpawner;

    private float cost = 50f;
    private bool isPlaced = false;
    private bool isMining = false;
    private RawOre currentOre = null;
    private float drillSpeed;

    // fuel
    public GameObject noFuelImage;
    public float currentFuel = 0f;
    private float fuelEfficiency;
    private int oresMinedSinceLastFuel = 0; 

    // Radius for detecting conveyors
    private float conveyorDetectionRadius = 1.0f;



    private void Start()
    {
        statsManager = FindObjectOfType<StatsManager>();
        audioSource = GetComponent<AudioSource>();
        drillSpeed = statsManager.GetDrillSpeed();
        itemCardsDisplay = FindObjectOfType<ItemCardsDisplay>();
        environmentManager = FindObjectOfType<EnvironmentManager>();
        oreSpawner = FindObjectOfType<OreSpawner>();

        fuelEfficiency = statsManager.GetFuelEfficiency();

        noFuelImage.SetActive(false);
        mainCamera = Camera.main;

        SetPlaced(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            environmentManager.DecrementCurrentOresOnMap();
        }

        if (noFuelImage.activeSelf)
        {
            Vector3 direction = noFuelImage.transform.position - mainCamera.transform.position; // Flip direction
            direction.y = 0; // Lock rotation to Y-axis
            noFuelImage.transform.rotation = Quaternion.LookRotation(direction);
        }

        if (isPlaced && isMining == false)
        {
            if (currentFuel <= 0)
            {
                noFuelImage.SetActive(true);
                StopMining();
                return;
            }

            bool oreDetected = CheckForOreBelow();
            if (oreDetected)  // Start mining if ore is detected and not already mining
            {
                if (!audioSource.isPlaying) audioSource.Play();
                StartCoroutine(MineOreCycle());  // Start the mining process
            }
        }

        if (currentFuel > 0)
        {
            noFuelImage.SetActive(false);
        }
    }

    private bool CheckForOreBelow()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("ore"))
            {
                RawOre rawOre = collider.GetComponent<RawOre>();
                if (rawOre != null && rawOre.GetGemsRemaining() > 0)
                {
                    currentOre = rawOre; // Set the current ore reference
                    return true;
                }
            }
        }
        currentOre = null; // Clear the current ore if none found
        return false; // No ore detected
    }

    private IEnumerator MineOreCycle()
    {
        isMining = true;

        while (isMining)  // Continuously mine while ore is detected below
        {
            // Wait 7 seconds between mining attempts
            yield return new WaitForSeconds(drillSpeed);

            // Check if there is still ore below
            if (currentOre == null || currentOre.GetGemsRemaining() <= 0)
            {
                StopMining();
                yield break;
            }

            if (currentFuel <= 0)
            {
                noFuelImage.SetActive(true);
                StopMining();
                yield break;
            }

            ConveyorBelt selectedConveyor = FindRandomAvailableConveyor();
            if (selectedConveyor != null && selectedConveyor.itemsOnBelt.Count < selectedConveyor.maxItems)
            {
                
                GameObject roughOrePrefab = currentOre.GetRoughPrefab(); // Use the rough prefab from RawOre
                if (roughOrePrefab != null)
                {
                    selectedConveyor.AddItem(roughOrePrefab);

                    // decrement ore vein
                    currentOre.DecrementOreCount(statsManager.GetOreAmountPerCycle());

                    environmentManager.DecrementCurrentOresOnMap();

                    oresMinedSinceLastFuel++;

                    if (oresMinedSinceLastFuel >= statsManager.GetFuelEfficiency())
                    {
                        currentFuel--;
                        itemCardsDisplay.UpdateFuelText();
                        oresMinedSinceLastFuel = 0;
                        Debug.Log("removing fuel");
                    }

                    if (currentOre.GetGemsRemaining() <= 0)
                    {
                        if (audioSource.isPlaying) audioSource.Stop();
                        Destroy(currentOre.gameObject);
                        isMining = false;  // Stop mining if no ore is detected
                    }
                }
            }
        }
    }


    private void StopMining()
    {
        isMining = false;
        currentOre = null;
        if (audioSource.isPlaying) audioSource.Stop();
    }


    // Finds a random conveyor with available space within the detection radius
    private ConveyorBelt FindRandomAvailableConveyor()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, conveyorDetectionRadius);
        List<ConveyorBelt> availableConveyors = new List<ConveyorBelt>();

        foreach (Collider collider in hitColliders)
        {
            ConveyorBelt conveyor = collider.GetComponentInParent<ConveyorBelt>();

            // Check if the conveyor is valid and the "start" tag is within the detection radius
            if (conveyor != null && conveyor.IsPlaced() && conveyor.itemsOnBelt.Count < conveyor.maxItems)
            {
                Transform startTransform = conveyor.transform.Find("startDisplay");
                if (startTransform != null)
                {
                    // Check if the "start" object is part of the detected colliders
                    if (Array.Exists(hitColliders, hit => hit.transform == startTransform))
                    {
                        availableConveyors.Add(conveyor);
                    }
                }
            }
        }

        // Return a random conveyor from the available list if any are found
        if (availableConveyors.Count > 0)
        {
            return availableConveyors[UnityEngine.Random.Range(0, availableConveyors.Count)];
        }

        return null;
    }

    // Returns the appropriate ore prefab based on ore type
    private GameObject GetOrePrefabByType(string oreType)
    {
        switch (oreType)
        {
            case "T1": return prefabManager.GetDirtyGarnetPrefab();
            case "T2": return prefabManager.GetDirtyEmeraldPrefab() ;
            case "T3": return prefabManager.GetDirtyTanzanitePrefab();
            default: return null;
        }
    }

    public void SetPlaced(bool placed)
    {
        isPlaced = placed;
        noFuelImage.SetActive(true);
    }

    public float GetCost()
    {
        return cost;
    }

    public void AddFuel(float amount)
    {
        currentFuel += amount;
    }

    public float GetFuel()
    {
        return currentFuel;
    }



}
