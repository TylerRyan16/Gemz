using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSetup : MonoBehaviour
{
    public Button conveyorBeltButton;
    public Button oreDrillButton;
    public Button crateButton;
    public Button cornerConveyorButton;

    private PrefabManager prefabManager;

    void Start()
    {
        prefabManager = FindObjectOfType<PrefabManager>();


        conveyorBeltButton.onClick.AddListener(() => prefabManager.SetCurrentItem(0));
        oreDrillButton.onClick.AddListener(() => prefabManager.SetCurrentItem(1));
        crateButton.onClick.AddListener(() => prefabManager.SetCurrentItem(2));


    }
}
