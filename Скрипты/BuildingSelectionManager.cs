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
        // ���������������� ����� ������
        var allBuildings = FindObjectsOfType<BuildingInterface>();
        foreach (var building in allBuildings)
        {
            // ��������� UI ������ � ������ � �������� �����������
            if (building != selectedBuilding && building.IsUIOpen())
            {
                building.CloseUI();
            }
        }
    }
}
