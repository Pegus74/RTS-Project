using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex;
    private Grid grid;
    private PreviewSystem previewSystem;
    private ObjectsDatabseSO database;
    private GridData floorData;
    private ObjectPlacer objectPlacer;

    public PlacementState(int id, Grid grid, PreviewSystem previewSystem,
                        ObjectsDatabseSO database, GridData floorData,
                        ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == id);
        if (selectedObjectIndex == -1)
            throw new System.Exception($"Object with ID {id} not found in database");

        previewSystem.StartShowingPlacementPreview(
            database.objectsData[selectedObjectIndex].Prefab,
            database.objectsData[selectedObjectIndex].Size);
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        if (!CheckPlacementValidity(gridPosition))
            return;

        int index = objectPlacer.PlaceObject(
            database.objectsData[selectedObjectIndex].Prefab,
            grid.CellToWorld(gridPosition));

        floorData.AddObjectAt(
            gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index);

        ResourceManager.Instance.DecreaseResourcesBasedOnRequirement(
            database.objectsData[selectedObjectIndex]);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool isValid = CheckPlacementValidity(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), isValid);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        // Проверка занятости клетки
        if (!floorData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size))
            return false;

        // Проверка коллизий
        Vector3 worldPos = grid.CellToWorld(gridPosition);
        Collider[] colliders = Physics.OverlapBox(worldPos, Vector3.one * 0.5f, Quaternion.identity);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Ally") || collider.CompareTag("Enemy") || collider.CompareTag("Terrain"))
                return false;
        }

        return true;
    }
}