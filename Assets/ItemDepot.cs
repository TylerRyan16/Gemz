using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDepot : MonoBehaviour
{
    public List<GameObject> inputZones = new List<GameObject>(); // Assign small cubes as input zones
    private List<Vector3> zonePositions = new List<Vector3>();  // Store initial positions
    private float checkInterval = 1f; // Check for items every second

    private StatsManager statsManager;

    void Start()
    {
        InitializePositions();
        statsManager = StatsManager.Instance; // Access the StatsManager instance
        StartCoroutine(CheckForItems());
    }

    // Save initial positions of the input zones
    void InitializePositions()
    {
        foreach (GameObject zone in inputZones)
        {
            if (zone != null)
            {
                zonePositions.Add(zone.transform.position);
            }
        }
    }

    // Coroutine to periodically check for items colliding with input zones
    private IEnumerator CheckForItems()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            foreach (GameObject zone in inputZones)
            {
                if (zone != null)
                {
                    CheckZoneForItems(zone);
                }
            }
        }
    }

    // Check a specific input zone for items within its collider
    private void CheckZoneForItems(GameObject zone)
    {
        Collider[] colliders = Physics.OverlapSphere(zone.transform.position, 0.5f); // Adjust radius if needed
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("item")) // Items must have the "item" tag
            {
                ProcessItem(collider.gameObject, zone);
            }
        }
    }

    // Process an item, adding it to StatsManager's inventory and destroying it
    private void ProcessItem(GameObject item, GameObject zone)
    {
        string oreType = GetOreType(item); // Determine ore type from the item
        if (!string.IsNullOrEmpty(oreType))
        {
            statsManager.AddOre(oreType, 1); // Add the ore to the StatsManager inventory
            Debug.Log($"Collected {oreType} from {zone.name}");
        }

        Destroy(item); // Destroy the item after collection
    }

    // Determine the ore type based on the item's tag or another identifier
    private string GetOreType(GameObject item)
    {
        if (item.CompareTag("T1")) return "T1"; // Garnet
        if (item.CompareTag("T2")) return "T2"; // Emerald
        if (item.CompareTag("T3")) return "T3"; // Tanzanite
        return null;
    }

    // Visualize input zones in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (GameObject zone in inputZones)
        {
            if (zone != null)
            {
                Gizmos.DrawWireSphere(zone.transform.position, 0.5f); // Adjust radius as needed
            }
        }
    }
}
