using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopMenuManager : MonoBehaviour
{
    // main item shop that holds the submenus
    public GameObject itemShop;
    public GameObject upArrow;
    public GameObject downArrow;

    // Category buttons for each of the nine categories
    public Button transportButton;
    public Button miningButton;
    public Button storageButton;
    public Button constructionButton;
    public Button energyButton;
    public Button refiningButton;
    public Button automationButton;
    public Button qualityControlButton;
    public Button researchButton;

    // Panels that contain item buttons for each category
    public GameObject transportPanel;
    public GameObject miningPanel;
    public GameObject storagePanel;
    public GameObject constructionPanel;
    public GameObject energyPanel;
    public GameObject refiningPanel;
    public GameObject automationPanel;
    public GameObject qualityControlPanel;
    public GameObject researchPanel;

    public TextMeshProUGUI moneyDisplay;
    public PrefabManager prefabManager; 
    private GameObject player;
    private PlayerController playerController;

    private float conveyerBeltCost = 1f;

    private bool isShopOpen;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();

        // Attach listeners to each category button
        transportButton.onClick.AddListener(() => ToggleSubmenu(transportPanel));
        miningButton.onClick.AddListener(() => ToggleSubmenu(miningPanel));
        storageButton.onClick.AddListener(() => ToggleSubmenu(storagePanel));
        constructionButton.onClick.AddListener(() => ToggleSubmenu(constructionPanel));
        energyButton.onClick.AddListener(() => ToggleSubmenu(energyPanel));
        refiningButton.onClick.AddListener(() => ToggleSubmenu(refiningPanel));
        automationButton.onClick.AddListener(() => ToggleSubmenu(automationPanel));
        qualityControlButton.onClick.AddListener(() => ToggleSubmenu(qualityControlPanel));
        researchButton.onClick.AddListener(() => ToggleSubmenu(researchPanel));

        isShopOpen = true;
        // update UI money display
        UpdateMoneyDisplay();
    }

    void ToggleSubmenu(GameObject categoryPanel)
    {
        // Toggle visibility for the selected category's items
        bool isActive = categoryPanel.activeSelf;
        categoryPanel.SetActive(!isActive);

        // Close other categories' menus
        DeactivateAllOtherCategories(categoryPanel);
    }


    void DeactivateAllOtherCategories(GameObject activePanel)
    {
        if (activePanel != transportPanel) transportPanel.SetActive(false);
        if (activePanel != miningPanel) miningPanel.SetActive(false);
        if (activePanel != storagePanel) storagePanel.SetActive(false);
        if (activePanel != constructionPanel) constructionPanel.SetActive(false);
        if (activePanel != energyPanel) energyPanel.SetActive(false);
        if (activePanel != refiningPanel) refiningPanel.SetActive(false);
        if (activePanel != automationPanel) automationPanel.SetActive(false);
        if (activePanel != qualityControlPanel) qualityControlPanel.SetActive(false);
        if (activePanel != researchPanel) researchPanel.SetActive(false);
    }


    public void BuySelectedItem()
    {
        Item currentItem = prefabManager.GetCurrentItem();

        if (playerController.GetCurrentMoney() >= currentItem.cost)
        {
            playerController.SubtractMoney(currentItem.cost);
            UpdateMoneyDisplay();
        }
        else
        {
            Debug.Log("not enough cash!");
        }
    }


    public void UpdateMoneyDisplay()
    {
        moneyDisplay.text = "$" + playerController.GetCurrentMoney().ToString("F2");
    }


    public void CloseAllSubmenus()
    {
        transportPanel.SetActive(false);
        miningPanel.SetActive(false);
        storagePanel.SetActive(false);
        constructionPanel.SetActive(false);
        energyPanel.SetActive(false);
        refiningPanel.SetActive(false);
        automationPanel.SetActive(false);
        qualityControlPanel.SetActive(false);
        researchPanel.SetActive(false);
    }

    public void CloseMainShop()
    {
        downArrow.SetActive(false);
        itemShop.SetActive(false);
        upArrow.SetActive(true);
    }

    public void OpenMainShop()
    {
        downArrow.SetActive(true);
        itemShop.SetActive(true);
        upArrow.SetActive(false);
        isShopOpen = true;
    }

    public void CloseMainShopMenuAndSubmenus()
    {
        CloseMainShop();
        CloseAllSubmenus();
        isShopOpen = false;
    }

    public bool IsShopOpen()
    {
        return isShopOpen;
    }
}



