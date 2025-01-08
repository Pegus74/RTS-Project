using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuySystem : MonoBehaviour
{
    public GameObject BuildingsPanel;

    public Button buildingButton;

    public PlacementSystem placementSystem;

    void Start()
    {
        buildingButton.onClick.AddListener(BuildingCategorySelected);
    }
    private void BuildingCategorySelected()
    {
        BuildingsPanel.SetActive(true);
    }


}
 