using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabseSO database;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabseSO database,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No object with ID {iD}");
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
      
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
        {
            return;
        }

        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));

        ResourceManager.Instance.DecreaseResourcesBasedOnRequirement(database.objectsData[selectedObjectIndex]);

        BuildingType buildingType = database.objectsData[selectedObjectIndex].thisBuildingType;
        ResourceManager.Instance.UpdateBuildingChanged(buildingType, true,new Vector3());

     
        GridData selectedData = floorData;

        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private List<int> GetAllFloorIDs()
    {
        return new List<int> { 11 }; 
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
       
        GridData selectedData = floorData;
        if (!selectedData.CanPlaceObjectAt(gridPosition,database.objectsData[selectedObjectIndex].Size))
        {
            return false;
        }
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Collider[] colliders = Physics.OverlapBox(worldPosition, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity);

        foreach (var collider in colliders) 
        {
            if (collider.CompareTag("Ally")|| collider.CompareTag("Enemy") || collider.CompareTag("Terrain"))//добавить другие тэги
            {
                return false;
            }
        
        
        }

        return true;



    }

    public void UpdateState(Vector3Int gridPosition)
    {
        
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}
