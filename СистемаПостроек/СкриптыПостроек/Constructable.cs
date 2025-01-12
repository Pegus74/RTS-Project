using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Constructable : MonoBehaviour,IDamageable
{
    private float constHealth;
    public float constMaxHealth;
    public HealthTracker healthTracker;
    public bool isEnemy = false;
    NavMeshObstacle obstacle;
    public BuildingType buildingType;
    public Vector3 buildPosition;
    public BoxCollider boxCollider;


    private void Start()
    {
        constHealth = constMaxHealth;
        boxCollider = GetComponent<BoxCollider>();
        UpdateHealthUI();

    }
    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(constHealth, constMaxHealth);
        if (constHealth <= 0) 
        {
            Destroy(gameObject);
            if (isEnemy == false)
            {
                ResourceManager.Instance.UpdateBuildingChanged(buildingType, false, buildPosition);
            }
        }
    }
    public void TakeDamage(int damage)
    {
        constHealth-=damage;
        UpdateHealthUI();
    }
    public void ConstructableWasPlaced()
    {
        healthTracker.gameObject.SetActive(true);
         ActivateObstacle();

        if (boxCollider != null)
        {
            boxCollider.enabled = true;
        }

        if (isEnemy)
        {
            gameObject.tag = "Enemy";
        }

    }

    private void ActivateObstacle()
    {
        obstacle = GetComponentInChildren<NavMeshObstacle>();
        obstacle.enabled = true;
    }
}
