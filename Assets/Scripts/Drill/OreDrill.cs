using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreDrill : MonoBehaviour
{
    private bool isPlaced = false;
    private bool isMining = false;
    private string currentOreType = "";
    private PrefabManager prefabManager;
    private StatsManager statsManager;
    private float drillSpeed;

    // Radius for detecting conveyors
    public float conveyorDetectionRadius = 1.0f;


    private void Start()
    {
        prefabManager = FindObjectOfType<PrefabManager>();
        statsManager = FindObjectOfType<StatsManager>();    
        drillSpeed = statsManager.GetDrillSpeed();

        SetPlaced(true);
    }

    private void Update()
    {
        if (isPlaced && isMining == false)
        {
            bool oreDetected = CheckForOreBelow();
            if (oreDetected)  // Start mining if ore is detected and not already mining
            {
                StartCoroutine(MineOreCycle());  // Start the mining process
            }
        }
    }

    private bool CheckForOreBelow()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("ore"))
            {
                Renderer oreRenderer = collider.GetComponent<Renderer>();
                if (oreRenderer != null)
                {
                    Material oreMaterial = oreRenderer.material;

                    // Set ore type and begin mining if applicable
                    if (oreMaterial.name.Contains("M_T1Gem"))
                    {
                        currentOreType = "T1";
                        return true;
                    }
                    else if (oreMaterial.name.Contains("M_T2Gem"))
                    {
                        currentOreType = "T2";
                        return true;
                    }
                    else if (oreMaterial.name.Contains("M_T3Gem"))
                    {
                        currentOreType = "T3";
                        return true;
                    }
                }
            }
        }
        return false;  // No ore detected
    }

    private IEnumerator MineOreCycle()
    {
        isMining = true;

        while (isMining)  // Continuously mine while ore is detected below
        {
            // Wait 7 seconds between mining attempts
            yield return new WaitForSeconds(drillSpeed);

            // Check if there is still ore below
            if (!CheckForOreBelow())
            {
                Debug.Log("Ore depleted or drill moved. Stopping mining.");
                isMining = false;  // Stop mining if no ore is detected
                yield break;
            }

            // Find nearby conveyors and try to add ore
            ConveyorBelt selectedConveyor = FindRandomAvailableConveyor();
            if (selectedConveyor != null && selectedConveyor.itemsOnBelt.Count < selectedConveyor.maxItems)
            {
                GameObject orePrefab = GetOrePrefabByType(currentOreType);
                if (orePrefab != null)
                {
                    selectedConveyor.AddItem(orePrefab);
                    isMining = false;  // Stop mining if no ore is detected
                }
            }
            else
            {
                //Debug.Log("No available conveyor nearby.");
            }
        }
    }


    // Finds a random conveyor with available space within the detection radius
    private ConveyorBelt FindRandomAvailableConveyor()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, conveyorDetectionRadius);
        List<ConveyorBelt> availableConveyors = new List<ConveyorBelt>();

        foreach (Collider collider in hitColliders)
        {
            ConveyorBelt conveyor = collider.GetComponentInParent<ConveyorBelt>();
            if (conveyor != null && conveyor.IsPlaced() && conveyor.itemsOnBelt.Count < conveyor.maxItems)
            {
                availableConveyors.Add(conveyor);
            }
        }

        if (availableConveyors.Count > 0)
        {
            return availableConveyors[Random.Range(0, availableConveyors.Count)];
        }

        return null;
    }

    // Returns the appropriate ore prefab based on ore type
    private GameObject GetOrePrefabByType(string oreType)
    {
        switch (oreType)
        {
            case "T1": return prefabManager.GetGarnetPrefab();
            case "T2": return prefabManager.GetEmeraldPrefab() ;
            case "T3": return prefabManager.GetTanzanitePrefab();
            default: return null;
        }
    }

    public void SetPlaced(bool placed)
    {
        isPlaced = placed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, conveyorDetectionRadius);
    }

}
