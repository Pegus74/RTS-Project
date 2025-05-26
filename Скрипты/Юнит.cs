using System;
using UnityEngine;

public class Юнит : MonoBehaviour, IDamageable
{
    private float unitHealth;
    public float unitMaxHealth;
    public HealthTracker healthTracker;
    Animator animator;
    UnityEngine.AI.NavMeshAgent navMeshAgent;

    // Событие, вызываемое при уничтожении юнита
    public event Action OnDestroyEvent;

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
        // Уведомляем подписчиков о смерти юнита
        OnDestroyEvent?.Invoke();

        // Удаляем юнит из списков
        ВыборЮнитов.Instance.unitsSelected.Remove(gameObject);
        ВыборЮнитов.Instance.allUnitsList.Remove(gameObject);
    }

    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);

        if (unitHealth <= 0)
        {
            Destroy(gameObject);
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