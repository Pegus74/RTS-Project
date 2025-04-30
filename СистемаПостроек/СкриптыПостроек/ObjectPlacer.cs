using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> placedGameObjects = new List<GameObject>();

    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
        Constructable constructable = newObject.GetComponent<Constructable>();

        if (constructable != null)
        {
            constructable.StartConstruction();
            constructable.OnConstructionCompleted += () => OnConstructionComplete(newObject);
        }

        placedGameObjects.Add(newObject);
        return placedGameObjects.Count - 1;
    }

    private void OnConstructionComplete(GameObject constructedObject)
    {
        // Можно добавить дополнительные действия после постройки
    }

    public void RemoveObjectAt(int gameObjectIndex)
    {
        if (gameObjectIndex < 0 || gameObjectIndex >= placedGameObjects.Count)
            return;

        if (placedGameObjects[gameObjectIndex] != null)
            Destroy(placedGameObjects[gameObjectIndex]);

        placedGameObjects[gameObjectIndex] = null;
    }

    public bool IsObjectConstructed(int gameObjectIndex)
    {
        if (gameObjectIndex < 0 || gameObjectIndex >= placedGameObjects.Count)
            return false;

        Constructable constructable = placedGameObjects[gameObjectIndex]?.GetComponent<Constructable>();
        return constructable != null && constructable.IsConstructed;
    }
}
    