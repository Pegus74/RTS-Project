using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private ObjectsDatabseSO database;

    [SerializeField] private GridData floorData, furnitureData; 

    [SerializeField] private PreviewSystem previewSystem;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacer objectPlacer;

    int selectedID;

    IBuildingState buildingState;

    private void Start()
    {

        floorData = new();
        furnitureData = new();
    }

    public void StartPlacement(int ID)
    {
 
        selectedID = ID;
        StopPlacement();
        buildingState = new PlacementState(ID, grid, previewSystem, database, floorData, furnitureData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }
    public void RemovePlacementData(Vector3 position)
    {
        floorData.RemoveObjectAt(grid.WorldToCell(position));
    }

    public void StartRemoving()
    {
        StopPlacement();

        buildingState = new RemovingState(grid, previewSystem, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI()){
            
            return;
        }
   
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);


      
        ObjectData ob = database.GetObjectByID(selectedID);

       
        foreach (BuildBenefits bf in ob.benefits)
        {
            CalculateAndAddBenefit(bf);
        }

        StopPlacement();
    }

    private void CalculateAndAddBenefit(BuildBenefits bf)
    {
        switch (bf.benefitType)
        {
            case BuildBenefits.BenefitType.Housing:
             
                break;
        }
    }

    private void StopPlacement()
    {
        if (buildingState == null)
            return;
       
        buildingState.EndState();

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;

        buildingState = null;
    }

    private void Update()
    {
      
        if (buildingState == null)
            return;
      
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }

    }
}
