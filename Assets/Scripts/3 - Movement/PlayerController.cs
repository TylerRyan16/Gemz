using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // prefab manager
    public PrefabManager prefabManager;
    public ShopMenuManager shopMenuManager;
    public float money = 0f;
    public int carryCapacity = 100;
    public bool isShopOpen;

    private void Start()
    {
        shopMenuManager = FindObjectOfType<ShopMenuManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            prefabManager.SetCurrentItem(0); // switch to conveyor
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            prefabManager.SetCurrentItem(1); // Switch to ore miner
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            prefabManager.SetCurrentItem(2); // Switch to ore washer
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            prefabManager.SetCurrentItem(3); // Switch to chest
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (shopMenuManager.IsShopOpen())
            {
                shopMenuManager.CloseMainShopMenuAndSubmenus();
            } else 
            {
                shopMenuManager.OpenMainShop();
            }
        }
    }

    public void AddMoney(float amount)
    {
        money += amount;
    }

    public void SubtractMoney(float amount)
    {
        money -= amount;
    }

    public void UpgradeCarryCapacity(int amount)
    {
        carryCapacity += amount;
    }

    public float GetCurrentMoney()
    {
        return money;
    }
}





