using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawn : MonoBehaviour
{
    public GameObject objectToSpawn;
    public int cost = 50; 

    private void Start()
    {
        
    }

    void OnMouseDown()
    {
        
        if (ResourceManager.Instance.GetGold() >= cost)
        {
          
            ResourceManager.Instance.DecreaseResource(ResourceManager.ResourcesType.Gold, cost);

          
            Vector3 spawnPosition = transform.position + new Vector3(1, 0, 0);
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.Log("Недостаточно золота для создания объекта.");
        }
    }
}
