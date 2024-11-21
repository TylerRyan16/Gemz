using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class TilePlacement : MonoBehaviour
{

    public ConveyorPlacement conveyor;

    // references
    public TileHover tileHover;
    public PrefabManager prefabManager;
    private GameObject player;
    private PlayerController playerController;
    private ShopMenuManager shopMenuManager;


    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        shopMenuManager = FindObjectOfType<ShopMenuManager>();
    }

    void Update()
    {

        if (IsMouseOverUI())
        {
            return;
        }

        // DESELECT ITEMS
        if (Input.GetMouseButtonDown(2))
        {
            prefabManager.DeselectItems();
            tileHover.DestroyPreview();
            return;
        }

        // ONLY ALLOW PLACEMENT IF ITEM IS SELECTED
        if (prefabManager.GetCurrentItem() != null)
        {
            // START LEFT CLICK = START CONVEYER PLACEMENT
            if (Input.GetMouseButtonDown(0))
            {
                conveyor.StartPlacement();
                shopMenuManager.CloseAllSubmenus();
            }

            // HOLD LEFT CLICK = UPDATE CONVEYOR PLACEMENT
            else if (Input.GetMouseButton(0))
            {
                conveyor.UpdatePlacement();
            }

            // LET GO OF LEFT CLICK = PLACE ALL
            else if (Input.GetMouseButtonUp(0))
            {
                conveyor.FinalizePlacement();                
            }
        }



        // keep below funcitons in update
        // ROTATE CLOCKWISE
        if (Input.GetKeyDown(KeyCode.E))
        {
            tileHover.RotatePreviewClockwise();
            conveyor.UpdateAllPreviews();
        }

        // ROTATE COUNTERCLOCKWISE
        if (Input.GetKeyDown(KeyCode.Q))
        {
            tileHover.RotatePreviewCounterClockwise();
            conveyor.UpdateAllPreviews();
        }
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}





