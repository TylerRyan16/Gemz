using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StatsManager : MonoBehaviour
{

    public static StatsManager Instance { get; private set; }
    public XPManager xpManager;

    public Vector3 originalScale;
    public Color originalColor;

    // coal usage
    private int fuelEfficiency = 3;

    // conveyor belt speed
    public float conveyorSpeed = 1f;

    // drill
    private float drillSpeed = 4f;
    private int oreAmountPerCycle = 1;  // Amount of ore mined per cycle

    // washer
    private float washerSpeed = 0.3f;

    private float money = 500f;
    private int carryCapacity = 100;

    #region Ore Amounts
    // coal
    public int coalCount = 0;

    // garnet
    public int dirtyGarnetCount = 0;
    public int cleanedGarnetCount = 0;
    public int purifiedGarnetCount = 0;
    public int cutGarnetCount = 0;
    public int polishedGarnetCount = 0;

    // emerald
    public int dirtyEmeraldCount = 0;
    public int cleanedEmeraldCount = 0;
    public int purifiedEmeraldCount = 0;
    public int cutEmeraldCount = 0;
    public int polishedEmeraldCount = 0;

    // tanzanite
    public int dirtyTanzaniteCount = 0;
    public int cleanedTanzaniteCount = 0;
    public int purifiedTanzaniteCount = 0;
    public int cutTanzaniteCount = 0;
    public int polishedTanzaniteCount = 0;
    #endregion

    #region XP Increase Value Variables
    // xp counts

    // coal
    private float coalXPIncrease = 4f;

    // Garnet
    private float dirtyGarnetXPIncrease = 5f;
    private float cleanedGarnetXPIncrease = 10f;
    private float cutGarnetXPIncrease = 25f;
    private float polishedGarnetXPIncrease = 50f;

    // Emerald
    private float dirtyEmeraldXPIncrease = 125f;
    private float cleanedEmeraldXPIncrease = 220f;
    private float cutEmeraldXPIncrease = 350f;
    private float polishedEmeraldXPIncrease = 700f;

    // Tanzanite
    private float dirtyTanzaniteXPIncrease = 225f;
    private float cleanedTanzaniteXPIncrease = 450f;
    private float cutTanzaniteXPIncrease = 1250f;
    private float polishedTanzaniteXPIncrease = 2500f;

    #endregion


    #region Quick Stats UI
    // quick stats display
    public TextMeshProUGUI moneyTextDisplay;
    public TextMeshProUGUI coalText;
    public TextMeshProUGUI garnetText;
    public TextMeshProUGUI emeraldText;
    public TextMeshProUGUI tanzaniteText;
    #endregion


    #region Stats Page Counts Texts
    // garnet
    public TextMeshProUGUI dirtyGarnetText;
    public TextMeshProUGUI cleanedGarnetText;
    public TextMeshProUGUI purifiedGarnetText;
    public TextMeshProUGUI cutGarnetText;
    public TextMeshProUGUI polishedGarnetText;

    // emerald
    public TextMeshProUGUI dirtyEmeraldText;
    public TextMeshProUGUI cleanedEmeraldText;
    public TextMeshProUGUI purifiedEmeraldText;
    public TextMeshProUGUI cutEmeraldText;
    public TextMeshProUGUI polishedEmeraldText;

    // tanzanite
    public TextMeshProUGUI dirtyTanzaniteText;
    public TextMeshProUGUI cleanedTanzaniteText;
    public TextMeshProUGUI purifiedTanzaniteText;
    public TextMeshProUGUI cutTanzaniteText;
    public TextMeshProUGUI polishedTanzaniteText;
    #endregion 

    #region Stats Page UI
    // STATS PAGE SHIT
    public GameObject statsPage;
    private bool statsPageOpen = false;

    // specific pages
    public GameObject resourcesPage;
    public GameObject machinesPage;
    public GameObject worldPage;
    #endregion

    #region Setup
    private void Start()
    {
        originalScale = coalText.transform.localScale;
        originalColor = coalText.color;
        UpdateMoneyText();

        UpdateStatsInfo();
    }

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

    private void Update()
    {

        if (!statsPageOpen && Input.GetKeyDown(KeyCode.G))
        {
            OpenStatsPage();
        }

        if (statsPageOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.G)))
        {
            CloseStatsPage();
        }
    }


    public void OpenOrCloseStatsPage()
    {
        if (statsPageOpen)
        {
            statsPage.SetActive(false);
            statsPageOpen = false;
        }
        else
        {
            statsPage.SetActive(true);
            statsPageOpen = true;
       
        
        }
    }

    public void OpenStatsPage()
    {
        statsPage.SetActive(true);
        StartCoroutine(SetStatsPageOpenAfterDelay(0.05f)); // Delay setting statsPageOpen to true
    }

    private IEnumerator SetStatsPageOpenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        statsPageOpen = true;
    }

    public void CloseStatsPage()
    {
        statsPage.SetActive(false);
        statsPageOpen = false;
        UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void UpdateStatsInfo()
    {
        coalText.text = coalCount.ToString();

        // Garnet Text Updates
        dirtyGarnetText.text = dirtyGarnetCount.ToString() + " Dirty";
        cleanedGarnetText.text = cleanedGarnetCount.ToString() + " Cleaned";
        purifiedGarnetText.text = purifiedGarnetCount.ToString() + " Purified";
        cutGarnetText.text = cutGarnetCount.ToString() + " Cut";
        polishedGarnetText.text = polishedGarnetCount.ToString() + " Polished";

        // Emerald Text Updates
        dirtyEmeraldText.text = dirtyEmeraldCount.ToString() + " Dirty";
        cleanedEmeraldText.text = cleanedEmeraldCount.ToString() + " Cleaned";
        purifiedEmeraldText.text = purifiedEmeraldCount.ToString() + " Purified";
        cutEmeraldText.text = cutEmeraldCount.ToString() + " Cut";
        polishedEmeraldText.text = polishedEmeraldCount.ToString() + " Polished";

        // Tanzanite Text Updates
        dirtyTanzaniteText.text = dirtyTanzaniteCount.ToString() + " Dirty";
        cleanedTanzaniteText.text = cleanedTanzaniteCount.ToString() + " Cleaned";
        purifiedTanzaniteText.text = purifiedTanzaniteCount.ToString() + " Purified";
        cutTanzaniteText.text = cutTanzaniteCount.ToString() + " Cut";
        polishedTanzaniteText.text = polishedTanzaniteCount.ToString() + " Polished";
    }


    private IEnumerator UpdateTextWithEffect(TextMeshProUGUI textField, string newValue)
    {
        textField.text = newValue;

        // Target scale and duration
        Vector3 targetScale = originalScale * 1.5f;
        Color targetColor = Color.green;
        float duration = 0.15f; 
        float elapsedTime = 0f;

        // FADE LARGER
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            textField.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            textField.color = Color.Lerp(originalColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        // Ensure final scale is exact
        textField.transform.localScale = targetScale;
        textField.color = targetColor;

        // Wait at the larger size
        yield return new WaitForSeconds(0.05f);
        
        // FADE SMALLER
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            textField.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / duration);
            textField.color = Color.Lerp(targetColor, originalColor, t);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        // reset properties
        textField.transform.localScale = originalScale;
        textField.color = originalColor;
    }
    #endregion




    #region Adding Ores
    public void AddOreToDepot(string oreType, int amount)
    {
        Debug.Log("adding : " + oreType + " to depot");
        switch (oreType)
        {
            // Coal
            case "Coal":
                coalCount += amount;
                xpManager.AddXP(coalXPIncrease);
                StartCoroutine(UpdateTextWithEffect(coalText, coalCount.ToString()));
                break;

            // Garnet
            case "Dirty Garnet":
                dirtyGarnetCount += amount;
                xpManager.AddXP(dirtyGarnetXPIncrease);
                StartCoroutine(UpdateTextWithEffect(garnetText, (dirtyGarnetCount + cleanedGarnetCount + cutGarnetCount + polishedGarnetCount).ToString()));
                break;
            case "Cleaned Garnet":
                cleanedGarnetCount += amount;
                xpManager.AddXP(cleanedGarnetXPIncrease);
                StartCoroutine(UpdateTextWithEffect(garnetText, (dirtyGarnetCount + cleanedGarnetCount + cutGarnetCount + polishedGarnetCount).ToString()));
                break;
            case "Cut Garnet":
                cutGarnetCount += amount;
                xpManager.AddXP(cutGarnetXPIncrease);
                StartCoroutine(UpdateTextWithEffect(garnetText, (dirtyGarnetCount + cleanedGarnetCount + cutGarnetCount + polishedGarnetCount).ToString()));
                break;
            case "Polished Garnet":
                polishedGarnetCount += amount;
                xpManager.AddXP(polishedGarnetXPIncrease);
                StartCoroutine(UpdateTextWithEffect(garnetText, (dirtyGarnetCount + cleanedGarnetCount + cutGarnetCount + polishedGarnetCount).ToString()));
                break;

            // Emerald
            case "Dirty Emerald":
                dirtyEmeraldCount += amount;
                xpManager.AddXP(dirtyEmeraldXPIncrease);
                StartCoroutine(UpdateTextWithEffect(emeraldText, (dirtyEmeraldCount + cleanedEmeraldCount + cutEmeraldCount + polishedEmeraldCount).ToString()));
                break;
            case "Cleaned Emerald":
                cleanedEmeraldCount += amount;
                xpManager.AddXP(cleanedEmeraldXPIncrease);
                StartCoroutine(UpdateTextWithEffect(emeraldText, (dirtyEmeraldCount + cleanedEmeraldCount + cutEmeraldCount + polishedEmeraldCount).ToString()));
                break;
            case "Cut Emerald":
                cutEmeraldCount += amount;
                xpManager.AddXP(cutEmeraldXPIncrease);
                StartCoroutine(UpdateTextWithEffect(emeraldText, (dirtyEmeraldCount + cleanedEmeraldCount + cutEmeraldCount + polishedEmeraldCount).ToString()));
                break;
            case "Polished Emerald":
                polishedEmeraldCount += amount;
                xpManager.AddXP(polishedEmeraldXPIncrease);
                StartCoroutine(UpdateTextWithEffect(emeraldText, (dirtyEmeraldCount + cleanedEmeraldCount + cutEmeraldCount + polishedEmeraldCount).ToString()));
                break;

            // Tanzanite
            case "Dirty Tanzanite":
                dirtyTanzaniteCount += amount;
                xpManager.AddXP(dirtyTanzaniteXPIncrease);
                StartCoroutine(UpdateTextWithEffect(tanzaniteText, (dirtyTanzaniteCount + cleanedTanzaniteCount + cutTanzaniteCount + polishedTanzaniteCount).ToString()));
                break;
            case "Cleaned Tanzanite":
                cleanedTanzaniteCount += amount;
                xpManager.AddXP(cleanedTanzaniteXPIncrease);
                StartCoroutine(UpdateTextWithEffect(tanzaniteText, (dirtyTanzaniteCount + cleanedTanzaniteCount + cutTanzaniteCount + polishedTanzaniteCount).ToString()));
                break;
            case "Cut Tanzanite":
                cutTanzaniteCount += amount;
                xpManager.AddXP(cutTanzaniteXPIncrease);
                StartCoroutine(UpdateTextWithEffect(tanzaniteText, (dirtyTanzaniteCount + cleanedTanzaniteCount + cutTanzaniteCount + polishedTanzaniteCount).ToString()));
                break;
            case "Polished Tanzanite":
                polishedTanzaniteCount += amount;
                xpManager.AddXP(polishedTanzaniteXPIncrease);
                StartCoroutine(UpdateTextWithEffect(tanzaniteText, (dirtyTanzaniteCount + cleanedTanzaniteCount + cutTanzaniteCount + polishedTanzaniteCount).ToString()));
                break;
        }
    }
    public void AddCoalToInventory(int amount)
    {
        coalCount += amount;
        xpManager.AddXP(coalXPIncrease);
        StartCoroutine(UpdateTextWithEffect(coalText, coalCount.ToString()));
    }
    #endregion

    public void UpgradeConveyorSpeed(float newSpeed)
    {
        conveyorSpeed = newSpeed;
    }

    public void UpgradeDrillSpeed(float newSpeed)
    {
        drillSpeed = newSpeed;
    }

    public void OpenPage(GameObject page)
    {
        page.SetActive(true);
        CloseOtherPages(page);
    }

    public void CloseOtherPages(GameObject page)
    {
        if (page == resourcesPage)
        {
            machinesPage.SetActive(false);
            worldPage.SetActive(false);
        }
        if (page == machinesPage)
        {
            resourcesPage.SetActive(false);
            worldPage.SetActive(false);
        }
        if (page == worldPage)
        {
            resourcesPage.SetActive(false);
            machinesPage.SetActive(false);
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

    #region Getters
    public float GetFuelEfficiency()
    {
        return fuelEfficiency;
    }

    public float GetConveyorBeltSpeed()
    {
        return conveyorSpeed;
    }

    public float GetDrillSpeed()
    {
        return drillSpeed;
    }

    public int GetOreAmountPerCycle()
    {
        return oreAmountPerCycle;
    }

    public float GetCurrentMoney()
    {
        return money;
    }

    public float GetWasherSpeed()
    {
        return washerSpeed;
    }

    // Coal
    public int GetCoalCount()
    {
        return coalCount;
    }

    // Garnet Getters
    public int GetDirtyGarnetCount()
    {
        return dirtyGarnetCount;
    }

    public int GetCleanedGarnetCount()
    {
        return cleanedGarnetCount;
    }

    public int GetCutGarnetCount()
    {
        return cutGarnetCount;
    }

    public int GetPolishedGarnetCount()
    {
        return polishedGarnetCount;
    }

    // Emerald Getters
    public int GetDirtyEmeraldCount()
    {
        return dirtyEmeraldCount;
    }

    public int GetCleanedEmeraldCount()
    {
        return cleanedEmeraldCount;
    }

    public int GetCutEmeraldCount()
    {
        return cutEmeraldCount;
    }

    public int GetPolishedEmeraldCount()
    {
        return polishedEmeraldCount;
    }

    // Tanzanite Getters
    public int GetDirtyTanzaniteCount()
    {
        return dirtyTanzaniteCount;
    }

    public int GetCleanedTanzaniteCount()
    {
        return cleanedTanzaniteCount;
    }

    public int GetCutTanzaniteCount()
    {
        return cutTanzaniteCount;
    }

    public int GetPolishedTanzaniteCount()
    {
        return polishedTanzaniteCount;
    }
    #endregion 

    public void RemoveCoal(int amount)
    {
        coalCount -= amount;
    }



    public void UpdateMoneyText()
    {
        moneyTextDisplay.text = money.ToString("F2");
    }
}
