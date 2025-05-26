using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelectionManager : MonoBehaviour
{
    private void OnEnable()
    {
        BuildingInterface.OnBuildingSelected += HandleBuildingSelected;
    }

    private void OnDisable()
    {
        BuildingInterface.OnBuildingSelected -= HandleBuildingSelected;
    }

    private void HandleBuildingSelected(BuildingInterface selectedBuilding)
    {
        // Оптимизированный поиск зданий
        var allBuildings = FindObjectsOfType<BuildingInterface>();
        foreach (var building in allBuildings)
        {
            // Закрываем UI только у зданий с открытым интерфейсом
            if (building != selectedBuilding && building.IsUIOpen())
            {
                building.CloseUI();
            }
        }
    }
}
