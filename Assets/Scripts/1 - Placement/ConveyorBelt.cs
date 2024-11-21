using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using static UnityEditor.Progress;

public class ConveyorBelt : MonoBehaviour
{
    private PrefabManager prefabManager;
    private StatsManager stats;



    public List<GameObject> itemsOnBelt = new List<GameObject>();
    public int maxItems = 1;
    private GameObject miniatureItemPrefab;

    // line rendering
    private LineRenderer lineRenderer;
    private Transform displayStartPoint;
    private Transform displayMidPoint;
    private Transform displayEndPoint;

    public bool isCornerConveyor = false;
    private bool isPlaced = false;

    // check for transfers in stationary items
    private float transferCheckInterval = 0.2f; // Interval in seconds to re-check transfer
    private float transferCheckTimer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        // grab references
        prefabManager = FindObjectOfType<PrefabManager>();
        stats = FindObjectOfType<StatsManager>();

        InitializePositions();
    }

    private void InitializePositions()
    {
        displayStartPoint = transform.Find("startDisplay");


        if (isCornerConveyor)
        {

            displayMidPoint = transform.Find("MiddleDisplay");
        }

        displayEndPoint = transform.Find("endDisplay");
    }

    private void Update()
    {
        if (!isPlaced) return;


        if (itemsOnBelt.Count >= maxItems)
        {
            transferCheckTimer += Time.deltaTime;
            if (transferCheckTimer >= transferCheckInterval)
            {
                transferCheckTimer = 0f;
                CheckForAvailableTransfers();
            }
        }

    }

    public bool AddItem(GameObject item)
    {
        if (!isPlaced || itemsOnBelt.Count >= maxItems) return false;

        if (item == null)
        {
            Debug.LogError("Item is null! Check what is being passed into AddItem.");
            return false;
        }

        // Instantiate the miniature item
        GameObject miniatureItem = Instantiate(item, displayStartPoint.position, Quaternion.identity, transform);
        itemsOnBelt.Add(miniatureItem);
        StartCoroutine(MoveItemAlongConveyor(miniatureItem));

        return true;
    }


    // Coroutine to move an item from displayStartPoint to displayEndPoint
    private IEnumerator MoveItemAlongConveyor(GameObject item)
    {
        if (!isPlaced) yield break;

        // movement for corner conveyors
        if (isCornerConveyor)
        {
            if (displayMidPoint != null)
            {
                while (item != null && Vector3.Distance(item.transform.position, displayMidPoint.position) > 0.01f)
                {
                    item.transform.position = Vector3.MoveTowards(
                        item.transform.position,
                        displayMidPoint.position,
                        stats.GetConveyorBeltSpeed() * Time.deltaTime
                    );
                    yield return null;
                }
            }

        }


        // movement for straight conveyors
        while (item != null && Vector3.Distance(item.transform.position, displayEndPoint.position) > 0.01f)
        {
            item.transform.position = Vector3.MoveTowards(
                item.transform.position,
                displayEndPoint.position,
                stats.GetConveyorBeltSpeed() * Time.deltaTime
            );
            yield return null;
        }

        // check for next conveyor
        ConveyorBelt nextConveyor = GetNextConveyor();

        if (nextConveyor != null)
        {
            if (nextConveyor.itemsOnBelt.Count < nextConveyor.maxItems)
            {
                itemsOnBelt.Remove(item);
                nextConveyor.AddItem(item);
                Destroy(item);
            }
            else
            {
                // Stay at end point
                item.transform.position = displayEndPoint.position;
            }
        }
        else
        {
            // Stay at end point
            if (item != null)
            {
                item.transform.position = displayEndPoint.position;
            }
        }


    }

    private ConveyorBelt GetNextConveyor()
    {
        if (!isPlaced) return null;

        Collider[] hitColliders = Physics.OverlapSphere(displayEndPoint.position, 0.3f);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("start") && collider.transform.parent != transform)
            {
                ConveyorBelt nextConveyor = collider.GetComponentInParent<ConveyorBelt>();
                if (nextConveyor != null && nextConveyor.itemsOnBelt.Count < nextConveyor.maxItems)
                {
                    Debug.Log("Found next conveyor, issue is after GetNextConveyor()");
                    return nextConveyor;
                }
            }
        }
        Debug.Log("no next conveyor found");
        return null;
    }


    private void CheckForAvailableTransfers()
    {
        if (!isPlaced) return;

        ConveyorBelt nextConveyor = GetNextConveyor();

        if (nextConveyor == null || nextConveyor.itemsOnBelt.Count >= nextConveyor.maxItems) return;

        if (nextConveyor.isPlaced)
        {
            List<GameObject> itemsToTransfer = new List<GameObject>();

            foreach (GameObject item in itemsOnBelt)
            {
                // Check if the item is stationary and near the end of the conveyor
                if (Vector3.Distance(item.transform.position, displayEndPoint.position) < 0.1f)
                {
                    itemsToTransfer.Add(item); // Mark for transfer
                }
            }

            // Process the transfer after checking
            foreach (GameObject item in itemsToTransfer)
            {
                itemsOnBelt.Remove(item); // Remove from the current conveyor
                nextConveyor.AddItem(item); // Add it to the next conveyor
                Destroy(item); // Destroy the item instance here
            }
        }
    }

    public void RemoveItem(GameObject item)
    {
        if (!isPlaced) return;

        if (itemsOnBelt.Contains(item))
        {
            itemsOnBelt.Remove(item);
            Destroy(item);
        }
    }

    public void SetAsPlaced(bool isCorner)
    {
        isPlaced = true;
        isCornerConveyor = isCorner;
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }

}