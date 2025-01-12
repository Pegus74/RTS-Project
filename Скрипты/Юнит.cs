using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Юнит : MonoBehaviour,IDamageable
{
    private float unitHealth;
    public float unitMaxHealth;
    public HealthTracker healthTracker;
    Animator animator;
    UnityEngine.AI.NavMeshAgent navMeshAgent;
    // Start is called before the first frame update
    void Start()
    {
        ВыборЮнитов.Instance.allUnitsList.Add(gameObject);
        unitHealth = unitMaxHealth;
        UpdateHealthUI();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    private void OnDestroy()
    {
        ВыборЮнитов.Instance.unitsSelected.Remove(gameObject);
    }
    
    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);
        
        if (unitHealth<=0)
        {
            Destroy(gameObject);
            ВыборЮнитов.Instance.allUnitsList.Remove(gameObject);
            ВыборЮнитов.Instance.unitsSelected.Remove(gameObject);
        }
    
    }

    public void TakeDamage(int damageToInflict)
    {
         unitHealth -= damageToInflict;
        UpdateHealthUI();
    }

    private void Update()
    {
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

}
