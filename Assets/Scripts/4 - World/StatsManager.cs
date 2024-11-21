using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{

    public static StatsManager Instance { get; private set; }

    // conveyor belt speed
    public float conveyorSpeed = 1f;

    // drill
    public float drillSpeed = 10f;
    public int oreAmountPerCycle = 1;  // Amount of ore mined per cycle

    // Ore counts
    public int garnetCount = 0;
    public int emeraldCount = 0;
    public int tanzaniteCount = 0;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public float GetConveyorBeltSpeed()
    {
        return conveyorSpeed;
    }

    public void UpgradeConveyorSpeed(float newSpeed)
    {
        conveyorSpeed = newSpeed;
    }

    public float GetDrillSpeed()
    {
        return drillSpeed;
    }

    public void UpgradeDrillSpeed(float newSpeed)
    {
        drillSpeed = newSpeed;
    }

    // Ore Management
    public void AddOre(string oreType, int amount)
    {
        switch (oreType)
        {
            case "T1":
                garnetCount += amount;
                Debug.Log("Garnet count: " + garnetCount);
                break;
            case "T2":
                emeraldCount += amount;
                Debug.Log("Emerald count: " + emeraldCount);
                break;
            case "T3":
                tanzaniteCount += amount;
                Debug.Log("Tanzanite count: " + tanzaniteCount);
                break;
        }
    }
}
