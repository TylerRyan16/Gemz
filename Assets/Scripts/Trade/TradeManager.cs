using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeManager : MonoBehaviour
{
    public GameObject tradePage;
    public GameObject staticUI;
    public GameObject shopUI;

    // supply
    public List<GameObject> supplyList;

    // demand
    public List<GameObject> demandList;


    private bool isTradePageOpen = false;
    private bool firstTimeOpening = true;

    
    private int currentINdex = 0;


    private void Start()
    {
        
    }


    public void OpenOrCloseTradePage()
    {
        if (!isTradePageOpen)
        {
            if (firstTimeOpening)
            {

            }
            Debug.Log("opening trade page");
            tradePage.SetActive(true);
            staticUI.SetActive(false);
            shopUI.SetActive(false);
            isTradePageOpen = true;
        }
        else
        {
            Debug.Log("closing trade page");
            tradePage.SetActive(false);
            staticUI.SetActive(true);
            shopUI.SetActive(true);
            isTradePageOpen = false;
        }

    }
}
