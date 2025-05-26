using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int Id, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccuply = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccuply, Id, placedObjectIndex);
        foreach (var pos in positionToOccuply)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception();
            }
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal1 = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal1.Add(gridPosition + new Vector3Int(x,0,y));
            }
        }
        return returnVal1;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }
        }
        return true;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        // Проверяем, есть ли объект в словаре
        if (!placedObjects.ContainsKey(gridPosition))
        {
            Debug.LogWarning($"Позиция {gridPosition} не найдена в GridData");
            return;
        }

        // Получаем данные о размещении
        PlacementData data = placedObjects[gridPosition];

        // Удаляем все занятые позиции
        foreach (var pos in data.occupiedPositions)
        {
            if (placedObjects.ContainsKey(pos))
            {
                placedObjects.Remove(pos);
            }
            else
            {
                Debug.LogWarning($"Позиция {pos} не найдена при удалении объекта");
            }
        }
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    public bool ContainsPosition(Vector3Int gridPosition)
    {
        return placedObjects.ContainsKey(gridPosition);
    }
}


public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}