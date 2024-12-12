using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class TechTree : MonoBehaviour
{

    public Texture2D defaultCursor;
    public Texture2D clickCursor;
    public Vector2 cursorHotspot = Vector2.zero;

    private StatsManager stats;
    public GameObject itemShopCanvas;
    public GameObject statsCanvas;
    public GameObject techTreeCanvas;
    public ItemCardsDisplay itemCardDisplay;
    public XPManager xpManager;
    public Color lineFilledColor;

    // skill points text
    public TextMeshProUGUI skillPointsText;
    public int skillPoints;


    // BUTTON SPRITES
    // Conveyor sprites
    public Sprite conveyorNotPressedSprite;
    public Sprite conveyorPressedSprite;

    // Drill sprites
    public Sprite drillNotPressedSprite;
    public Sprite drillPressedSprite;

    private bool techTreeOpen = false;

    // text
    public TextMeshProUGUI conveyorStatsText;
    public TextMeshProUGUI drillStatsText;


    // BUTTON REFERENCES
    // conveyor
    public Button conveyorSpeed1Button;
    public Button conveyorSpeed2Button;
    public Button conveyorSpeed3Button;

    // sliders
    public Slider conveyor1To2;
    public Slider conveyor2To3;
    public Slider conveyor1ToDrill1;

    // ore drill
    public Button drillSpeed1Button;
    public Button drillSpeed2Button;

    // sliders
    public Slider drill1To2;



    // BUTTON DISABLE BOOLS
    // conveyor
    private bool conveyorSpeedUpgrade1 = false;
    private bool conveyorSpeedUpgrade2 = false;
    private bool conveyorSpeedUpgrade3 = false;

    // ore drill
    private bool drillSpeedUpgrade1 = false;
    private bool drillSpeedUpgrade2 = false;

    // Info Cards (hover display)
    public GameObject conveyorSpeed1Card;
    public GameObject conveyorSpeed2Card;
    public GameObject conveyorSpeed3Card;
    public GameObject drillSpeed1Card;
    public GameObject drillSpeed2Card;

    // Stat Change Text Fields
    public TextMeshProUGUI conveyorSpeed1StatChange;
    public TextMeshProUGUI conveyorSpeed2StatChange;
    public TextMeshProUGUI conveyorSpeed3StatChange;
    public TextMeshProUGUI drillSpeed1StatChange;
    public TextMeshProUGUI drillSpeed2StatChange;

    void Start()
    {
        stats = StatsManager.Instance;
        DisableLockedButtons();

        conveyorStatsText.text = ":" + stats.GetConveyorBeltSpeed().ToString("F2"); ;
        drillStatsText.text = ":" + stats.GetDrillSpeed().ToString("F2"); ;

        SetupListeners();

        // Initial hover setup
        UpdateHoverEvents();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!techTreeOpen)
            {
                OpenTechTree();
            } else
            {
                CloseTechTree();
            }
        }

        skillPoints = xpManager.GetSkillPoints();
        skillPointsText.text = "Skill Points: " + skillPoints;
    }

    public void SetupListeners()
    {
        // Set button listeners for upgrades
        conveyorSpeed1Button.onClick.AddListener(UpgradeConveyorSpeed1);
        conveyorSpeed2Button.onClick.AddListener(UpgradeConveyorSpeed2);
        conveyorSpeed3Button.onClick.AddListener(UpgradeConveyorSpeed3);

        drillSpeed1Button.onClick.AddListener(UpgradeDrillSpeed1);
        drillSpeed2Button.onClick.AddListener(UpgradeDrillSpeed2);

    }

    void UpdateHoverEvents()
    {
        // Clear and re-add hover events for each button based on current upgrade status

        ResetHoverEvents(conveyorSpeed1Button);
        AddHoverEvents(conveyorSpeed1Button, conveyorSpeed1Card, conveyorSpeed1StatChange, 1.08f, true);

        ResetHoverEvents(conveyorSpeed2Button);
        AddHoverEvents(conveyorSpeed2Button, conveyorSpeed2Card, conveyorSpeed2StatChange, 1.15f, conveyorSpeedUpgrade1);

        ResetHoverEvents(conveyorSpeed3Button);
        AddHoverEvents(conveyorSpeed3Button, conveyorSpeed3Card, conveyorSpeed3StatChange, 1.23f, conveyorSpeedUpgrade2);

        ResetHoverEvents(drillSpeed1Button);
        AddHoverEvents(drillSpeed1Button, drillSpeed1Card, drillSpeed1StatChange, 0.9f, conveyorSpeedUpgrade1);

        ResetHoverEvents(drillSpeed2Button);
        AddHoverEvents(drillSpeed2Button, drillSpeed2Card, drillSpeed2StatChange, 0.9f, drillSpeedUpgrade1);
    }

    void ResetHoverEvents(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger != null)
        {
            trigger.triggers.Clear(); // Clear existing triggers to prevent duplicates
        }
        else
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }
    }


    void DisableLockedButtons()
    {
        SetButtonState(conveyorSpeed1Button, conveyorNotPressedSprite, true);
        SetButtonState(conveyorSpeed2Button, conveyorNotPressedSprite, false);
        SetButtonState(conveyorSpeed3Button, conveyorNotPressedSprite, false);
        SetButtonState(drillSpeed1Button, drillNotPressedSprite, false);
        SetButtonState(drillSpeed2Button, drillNotPressedSprite, false);
    }

    private void SetButtonState(Button button, Sprite sprite, bool interactable)
    {
        if (button != null)
        {
            button.GetComponent<Image>().sprite = sprite;
            button.interactable = interactable;
        }
    }


    public void OpenTechTree()
    {
        // set tech tree canvas to active
        techTreeCanvas.SetActive(true);
        // disable any other canvases
        itemShopCanvas.SetActive(false);
        statsCanvas.SetActive(false);
        techTreeOpen = true;

        Cursor.SetCursor(clickCursor, cursorHotspot, CursorMode.Auto);
    }

    public void CloseTechTree()
    {
        // set tech tree canvas to not active
        techTreeCanvas.SetActive(false);

        // enable any other canvases
        itemShopCanvas.SetActive(true);
        statsCanvas.SetActive(true);
        techTreeOpen = false;

        Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
    }

    void AddHoverEvents(Button button, GameObject infoCard, TextMeshProUGUI statChangeText, float multiplier, bool isUnlocked)
    {

        if (!isUnlocked) return;

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => ShowInfoCard(infoCard, statChangeText, multiplier));

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => HideInfoCard(infoCard));

        trigger.triggers.Add(entryEnter);
        trigger.triggers.Add(entryExit);
    }

    void ShowInfoCard(GameObject infoCard, TextMeshProUGUI statChangeText, float multiplier)
    {
        infoCard.SetActive(true);

        // Calculate the stat change based on the multiplier and update the text
        if (statChangeText != null)
        {
            float currentStat = 0;
            float upgradedStat = 0;

            if (statChangeText == conveyorSpeed1StatChange || statChangeText == conveyorSpeed2StatChange || statChangeText == conveyorSpeed3StatChange)
            {
                currentStat = stats.GetConveyorBeltSpeed();
                upgradedStat = currentStat * multiplier;
            }
            else if (statChangeText == drillSpeed1StatChange || statChangeText == drillSpeed2StatChange)
            {
                currentStat = stats.GetDrillSpeed();
                upgradedStat = currentStat * multiplier;
            }

            statChangeText.text = $"{currentStat:F2} -> {upgradedStat:F2}";
        }
    }

    void HideInfoCard(GameObject infoCard)
    {
        infoCard.SetActive(false);
    }

    public void UpgradeConveyorSpeed1()
    {
        if (!conveyorSpeedUpgrade1 && skillPoints > 0)
        {
            ApplyUpgrade(conveyorSpeed1Button, conveyorPressedSprite, stats.GetConveyorBeltSpeed() * 1.08f, false);
            conveyorSpeedUpgrade1 = true;
            conveyorSpeed2Button.interactable = true;
            drillSpeed1Button.interactable = true; // Unlock drill upgrade 1 if needed
            xpManager.UseSkillPoint();

            conveyorStatsText.text = ":" + stats.GetConveyorBeltSpeed().ToString("F2");
            ChangeSliderBackgroundColor(conveyor1To2, lineFilledColor);
            UpdateHoverEvents(); // Refresh hover events after upgrade
            itemCardDisplay.UpdateMachineStatsText();
        }


    }

    public void UpgradeConveyorSpeed2()
    {
        if (conveyorSpeedUpgrade1 && !conveyorSpeedUpgrade2 && skillPoints > 0)
        {
            ApplyUpgrade(conveyorSpeed2Button, conveyorPressedSprite, stats.GetConveyorBeltSpeed() * 1.15f, false);
            conveyorSpeedUpgrade2 = true;
            conveyorSpeed3Button.interactable = true; // Unlock next upgrade
            xpManager.UseSkillPoint();

            conveyorStatsText.text = ":" + stats.GetConveyorBeltSpeed().ToString("F2");
            ChangeSliderBackgroundColor(conveyor2To3, lineFilledColor);
            UpdateHoverEvents(); // Refresh hover events after upgrade
            itemCardDisplay.UpdateMachineStatsText();
        }
    }

    public void UpgradeConveyorSpeed3()
    {
        if (conveyorSpeedUpgrade2 && !conveyorSpeedUpgrade3 && skillPoints > 0)
        {
            ApplyUpgrade(conveyorSpeed3Button, conveyorPressedSprite, stats.GetConveyorBeltSpeed() * 1.23f, false);
            conveyorSpeedUpgrade3 = true;
            xpManager.UseSkillPoint();

            conveyorStatsText.text = ":" + stats.GetConveyorBeltSpeed().ToString("F2");
            UpdateHoverEvents(); // Refresh hover events after upgrade
            itemCardDisplay.UpdateMachineStatsText();
        }
    }

    public void UpgradeDrillSpeed1()
    {
        if (!drillSpeedUpgrade1 && skillPoints > 0)
        {
            ApplyUpgrade(drillSpeed1Button, drillPressedSprite, stats.GetDrillSpeed() * 0.9f, true);
            ChangeSliderBackgroundColor(conveyor1ToDrill1, lineFilledColor);
            drillSpeedUpgrade1 = true;
            xpManager.UseSkillPoint();


            drillSpeed2Button.interactable = true;

            drillStatsText.text = ":" + stats.GetDrillSpeed().ToString("F2");
            UpdateHoverEvents(); // Refresh hover events after upgrade
            itemCardDisplay.UpdateMachineStatsText();
        }
    }

    public void UpgradeDrillSpeed2()
    {
        if (drillSpeedUpgrade1 && !drillSpeedUpgrade2 && skillPoints > 0)
        {
            ApplyUpgrade(drillSpeed2Button, drillPressedSprite, stats.GetDrillSpeed() * 0.9f, true);
            ChangeSliderBackgroundColor(drill1To2, lineFilledColor);
            drillSpeedUpgrade2 = true;
            xpManager.UseSkillPoint();

            drillStatsText.text = ":" + stats.GetDrillSpeed().ToString("F2");
            UpdateHoverEvents(); // Refresh hover events after upgrade
            itemCardDisplay.UpdateMachineStatsText();
        }
    }

    private void ApplyUpgrade(Button button, Sprite pressedSprite, float newSpeed, bool isDrillUpgrade)
    {
        if (isDrillUpgrade)
        {
            stats.UpgradeDrillSpeed(newSpeed);
            Debug.Log("Drill speed upgraded to: " + newSpeed);
        }
        else
        {
            stats.UpgradeConveyorSpeed(newSpeed);
            Debug.Log("Conveyor speed upgraded to: " + newSpeed);
        }

        if (button != null)
        {
            button.GetComponent<Image>().sprite = pressedSprite;
        }


    }


    void ChangeSliderBackgroundColor(Slider slider, Color color)
    {
        // Find the background object within the slider
        Transform background = slider.transform.Find("Background");

        if (background != null)
        {
            // Get the Image component and change its color
            Image backgroundImage = background.GetComponent<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.color = color;
            }
            else
            {
                Debug.LogWarning("Background does not have an Image component.");
            }
        }
        else
        {
            Debug.LogWarning("Background object not found in slider.");
        }
    }
}
