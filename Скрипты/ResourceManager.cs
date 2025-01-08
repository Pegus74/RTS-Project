using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    public int gold = 300 ;
    public TextMeshProUGUI goldUI;
    public event Action OnResourceChanged;
    public event Action OnBuildingsChanged;
    public List<BuildingType> allExistingBuildings;

    public enum ResourcesType
    {
        Gold 
    }
    private void Start()
    {
        UpdateUI();
    }
    public int GetGold()
    {
        return gold;
    }
    public void UpdateBuildingChanged(BuildingType buildingType,bool isNew)
    {
        if (isNew)
        {
            allExistingBuildings.Add(buildingType);

        }
        else
        {
            allExistingBuildings.Remove(buildingType);
        }
        OnBuildingsChanged?.Invoke();
    }


    public void IncreaseResource(ResourcesType resource, int amountToIncrease)
    {
        switch (resource) 
        {
            case ResourcesType.Gold:
                gold += amountToIncrease;
                break;
            default:
                break;
        }
        OnResourceChanged?.Invoke();
    }
    public void DecreaseResource(ResourcesType resource, int amountToDecrease)
    {
        switch (resource)
        {
            case ResourcesType.Gold:
                gold -= amountToDecrease;
                break;
            default:
                break;
        }
        OnResourceChanged?.Invoke();
    }
  

    private void UpdateUI()
    {
        goldUI.text = $"{gold}";
    }
    internal int GetResourceAmount(ResourcesType resource)
    {

        switch (resource) 
        {
            case ResourcesType.Gold:
                return gold;
            default:
                break;
        }
        return 0;
    }
    internal void DecreaseResourcesBasedOnRequirement(ObjectData objectData)
    {
        foreach (BuildRequirement req in objectData.Requirements) 
        { 
            DecreaseResource(req.resource,req.amount );
        }
    }
    private void OnEnable()
    {
        OnResourceChanged += UpdateUI;
    }
    private void OnDisable()
    {
        OnResourceChanged -= UpdateUI;
    }
}
