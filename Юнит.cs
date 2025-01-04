using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Юнит : MonoBehaviour
{
    private float unitHealth;
    public float unitMaxHealth;
    public HealthTracker healthTracker;
    // Start is called before the first frame update
    void Start()
    {
        ВыборЮнитов.Instance.allUnitsList.Add(gameObject);
        unitHealth = unitMaxHealth;
        UpdateHealthUI();
    }

    private void OnDestroy()
    {
        ВыборЮнитов.Instance.allUnitsList.Remove(gameObject);
    }
    
    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);
        
        if (unitHealth<=0)
        {
            Destroy(gameObject);
        }
    
    }

    internal void TakeDamage(int damageToInflict)
    {
         unitHealth -= damageToInflict;
        UpdateHealthUI();
    }

}
