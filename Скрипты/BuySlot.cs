using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuySlot : MonoBehaviour
{
    private bool isAvailable;

    public BuySystem buySystem;

    public int databaseItemID;

    private void Start()
    {
        ResourceManager.Instance.OnResourceChanged += HandleResourcesChanged;
        HandleResourcesChanged();

        ResourceManager.Instance.OnBuildingsChanged += HandleBuildingsChanged;
        HandleBuildingsChanged();
    }

    public void ClickedOnSlot()
    {
        if (isAvailable)
        {
            buySystem.placementSystem.StartPlacement(databaseItemID);
        }


    }

    private void UpdateAvailabilityUI()
    {
        if (isAvailable)
        {
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Button>().interactable = false;
        }
    }
    
    private void HandleResourcesChanged()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.objectsData[databaseItemID];
        bool requirementMet = true;
        foreach (BuildRequirement req in objectData.Requirements)
        {
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
            {
                requirementMet = false;
                break;
            }

        }
        isAvailable = requirementMet;

        UpdateAvailabilityUI();
    }

    private void HandleBuildingsChanged()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.objectsData[databaseItemID];
        foreach (BuildingType dependency in objectData.buildDependency)
        {
            if(dependency == BuildingType.None)
            {
                gameObject.SetActive(true);
                return;
            }
            if(ResourceManager.Instance.allExistingBuildings.Contains(dependency)==false)
            {
                gameObject.SetActive(false);
                return;
            }

        }
        gameObject.SetActive(true); 
    }


}
