using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class OreWasher : MonoBehaviour
{
    private StatsManager statsManager;
    private PrefabManager prefabManager;
    private Camera mainCamera;

    public GameObject noFuelImage;
    private float currentFuel = 0f;
    private float fuelEfficiency;
    private int oresWashedSinceLastFuel = 0;

    private float washerSpeed;
    private float cost = 25f;

    public List<GameObject> itemsOnWasher = new List<GameObject>();
    public int maxItems = 1;

    private Transform startPoint;
    private Transform endPoint;


    private bool isPlaced = false;
    private bool doneWashingItem = false;

    private float transferCheckInterval = 0.2f; // Interval in seconds to re-check transfer
    private float transferCheckTimer = 0f;



    // Start is called before the first frame update
    void Start()
    {
        InitializeReferences();
        fuelEfficiency = statsManager.GetFuelEfficiency();
        washerSpeed = statsManager.GetWasherSpeed();
        noFuelImage.SetActive(true);

    }


    void InitializeReferences()
    {
        statsManager = FindObjectOfType<StatsManager>();
        prefabManager = FindObjectOfType<PrefabManager>();

        startPoint = transform.Find("startPoint");
        endPoint = transform.Find("endPoint");
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (noFuelImage.activeSelf)
        {
            Vector3 direction = noFuelImage.transform.position - mainCamera.transform.position; // Flip direction
            direction.y = 0; // Lock rotation to Y-axis
            noFuelImage.transform.rotation = Quaternion.LookRotation(direction);
        }


        if (!isPlaced) return;


        if (doneWashingItem && itemsOnWasher.Count >= maxItems)
        {
            transferCheckTimer += Time.deltaTime;
            if (transferCheckTimer >= transferCheckInterval )
            {
                transferCheckTimer = 0f;
                CheckForAvailableTransfers();
            }
        }

        // Update noFuelImage visibility
        noFuelImage.SetActive(currentFuel <= 0);
    }

    private void CheckForAvailableTransfers()
    {
        if (!isPlaced) return;

        ConveyorBelt nextConveyor = GetNextConveyor();

        if (nextConveyor == null) return;

        List<GameObject> itemsToTransfer = new List<GameObject>();


        // get items to transfer
        foreach (GameObject item in itemsOnWasher)
        {
            // Check if the item is stationary and near the end of the conveyor
            if (Vector3.Distance(item.transform.position, endPoint.position) < 0.1f)
            {
                itemsToTransfer.Add(item); // Mark for transfer
            }
        }

        // Process the transfer after grabbing item
        foreach (GameObject item in itemsToTransfer)
        {

            if (nextConveyor.itemsOnBelt.Count < nextConveyor.maxItems && nextConveyor.IsPlaced())
            {
                itemsOnWasher.Remove(item); // Remove from the current conveyor
                nextConveyor.AddItem(item); // Add it to the next conveyor
                Destroy(item); // Destroy the item instance here
            }
            else
            {
                // Stay at end point if the next object is full
                item.transform.position = endPoint.position;
            }
        }
        
       
    }

    public bool AddItemToWasher(GameObject rawItem)
    {
        if (!isPlaced || itemsOnWasher.Count >= maxItems) return false;

        if (rawItem == null)
        {
            Debug.LogError("Attempted to add a null object to the washer!");
            return false;
        }

        if (currentFuel <= 0)
        {
            return false;
        }

        doneWashingItem = false;

        // Add item to washer and start processing
        GameObject itemToWash = Instantiate(rawItem, startPoint.position, Quaternion.identity, transform);
        itemsOnWasher.Add(itemToWash);
        StartCoroutine(WashItem(itemToWash));

        return true;
    }

    private IEnumerator WashItem(GameObject itemToWash)
    {
        if (!isPlaced) yield break;

        if (currentFuel <= 0)
        {
            yield break;
        }

        // Move the rough gem to the endpoint
        while (itemToWash != null && Vector3.Distance(itemToWash.transform.position, endPoint.position) > 0.01f)
        {
            itemToWash.transform.position = Vector3.MoveTowards(
                itemToWash.transform.position,
                endPoint.position,
                washerSpeed * Time.deltaTime
            );

            yield return null;
        }

        GameObject cleanedItemPrefab = GetCleanedItemPrefab(itemToWash);

        if (cleanedItemPrefab != null)
        {
            oresWashedSinceLastFuel++;

            if (oresWashedSinceLastFuel >= fuelEfficiency)
            {
                currentFuel--;
                oresWashedSinceLastFuel = 0;
            }

            // Check for nearby conveyors and add cleaned gem
            ConveyorBelt nextConveyor = GetNextConveyor();
            if (nextConveyor != null && nextConveyor.itemsOnBelt.Count < nextConveyor.maxItems)
            {
                itemsOnWasher.Remove(itemToWash);
                nextConveyor.AddItem(cleanedItemPrefab);
                Destroy(itemToWash);
                doneWashingItem = true;

            }
            else
            {
                // create cleaned gem at end point
                GameObject cleanedItem = Instantiate(cleanedItemPrefab, endPoint.position, Quaternion.identity, transform);
                itemsOnWasher.Remove(itemToWash);
                itemsOnWasher.Add(cleanedItem);
                Destroy(itemToWash);
                doneWashingItem = true;

            }

        }
    }

    private GameObject GetCleanedItemPrefab(GameObject itemToWash)
    {
        string itemName = itemToWash.name;

        // Map item names to their cleaned versions
        if (itemName.Contains("dirtyGarnet"))
        {
            return prefabManager.GetCleanedGarnetPilePrefab();
        }
        else if (itemName.Contains("dirtyEmerald"))
        {
            return prefabManager.GetCleanedEmeraldPilePrefab();
        }
        else if (itemName.Contains("dirtyTanzanite"))
        {
            return prefabManager.GetCleanedTanzanitePilePrefab();
        }

        Debug.LogError($"Unrecognized item type: {itemName}");
        return null;
    }

    private ConveyorBelt GetNextConveyor()
    {
        Collider[] hitColliders = Physics.OverlapSphere(endPoint.position, 0.2f);
        foreach (Collider collider in hitColliders)
        {
            ConveyorBelt conveyor = collider.GetComponentInParent<ConveyorBelt>();
            if (conveyor != null && conveyor.itemsOnBelt.Count < conveyor.maxItems && collider.transform.parent != transform)
            {
                return conveyor;
            }
        }
        return null;
    }

    public void SetPlaced(bool placed)
    {
        isPlaced = placed;
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }

    public void AddFuel(float amount)
    {
        currentFuel += amount;
        Debug.Log($"Drill refueled. Current fuel: {currentFuel}");
    }

    public float GetFuel()
    {
        return currentFuel;
    }

    public float GetCost()
    {
        return cost;
    }

}
