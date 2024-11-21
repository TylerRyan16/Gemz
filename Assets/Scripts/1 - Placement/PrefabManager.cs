using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public List<Item> availableItems;  
    public Item currentItem = null;
    public GameObject displayPrefab;

    public GameObject straightConveyorPrefab; 
    public GameObject cornerConveyorPrefab;
    public GameObject mirroredCornerConveyorPrefab;

    public GameObject garnetPrefab;
    public GameObject emeraldPrefab;
    public GameObject tanzanitePrefab;

    public Item GetCurrentItem()
    {
        return currentItem;
    }

    public void SetCurrentItem(int index)
    {
        if (index >= 0 && index < availableItems.Count)
        {
            currentItem = availableItems[index];
        } else
        {
            Debug.LogError("invalid prefab index");
        }
    }

    public void DeselectItems()
    {
        currentItem = null;
    }

    public GameObject GetDisplayPrefab()
    {
        return displayPrefab;
    }

    public GameObject GetGarnetPrefab()
    {
        return garnetPrefab;
    }

    public GameObject GetEmeraldPrefab()
    {
        return emeraldPrefab; 
    }

    public GameObject GetTanzanitePrefab()
    {
        return tanzanitePrefab;
    }

    public GameObject GetCornerPrefab()
    {
        return cornerConveyorPrefab;
    }

    public GameObject GetMirroredCornerPrefab()
    {
        return mirroredCornerConveyorPrefab;
    }
}



