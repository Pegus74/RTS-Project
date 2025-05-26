using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance { get; private set; }

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabseSO database;
    [SerializeField] private GridData floorData, furnitureData;
    [SerializeField] private PreviewSystem previewSystem;
    [SerializeField] private ObjectPlacer objectPlacer;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    private int selectedID;
    private IBuildingState buildingState;
    public bool InSellMode { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        floorData = new GridData();
        furnitureData = new GridData();
    }

    public void StartPlacement(int ID)
    {
        selectedID = ID;
        StopPlacement();
        buildingState = new PlacementState(ID, grid, previewSystem, database, floorData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void RemovePlacementData(Vector3 position)
    {
        Vector3Int gridPosition = grid.WorldToCell(position);

        // Проверяем наличие объекта перед удалением
        if (floorData.ContainsPosition(gridPosition))
        {
            floorData.RemoveObjectAt(gridPosition);
        }
        else
        {
            Debug.LogWarning($"No object found at grid position {gridPosition} to remove");
        }
    }

    public void StartRemoving()
    {
        StopPlacement();
        buildingState = new RemovingState(grid, previewSystem, floorData, furnitureData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
        InSellMode = true;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI()) return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);

        // Обновляем ресурсы
        ObjectData ob = database.GetObjectByID(selectedID);
        foreach (BuildBenefits bf in ob.benefits)
        {
            CalculateAndAddBenefit(bf);
        }
    }

    private void StopPlacement()
    {
        if (buildingState == null) return;

        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
        InSellMode = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartRemoving();
        }

        if (buildingState == null) return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }

    private void CalculateAndAddBenefit(BuildBenefits bf)
    {
        if (ResourceManager.Instance == null) return;

        switch (bf.benefitType)
        {
            case BuildBenefits.BenefitType.Housing:
                ResourceManager.Instance.AddHousing(bf.benefitAmount);
                break;
            case BuildBenefits.BenefitType.Production:
                ResourceManager.Instance.AddProduction(bf.benefitAmount);
                break;
            default:
                Debug.LogWarning($"Unknown benefit type: {bf.benefitType}");
                break;
        }
    }


    // Добавляем в GridData.cs
    public bool ContainsPosition(Vector3Int gridPosition)
    {
        return floorData.ContainsPosition(gridPosition) || furnitureData.ContainsPosition(gridPosition);
    }
}