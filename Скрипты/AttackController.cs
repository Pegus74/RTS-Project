 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController  : MonoBehaviour
{
    public Transform targetToAttack;

    public Material idleStateMaterial;
    public Material followStateMaterial;
    public Material attackStateMaterial;

    public bool isPlayer;
    public bool isEnemy;
    [SerializeField] private LayerMask ignoreLayers;
    public int unitDamage;

    private bool ShouldIgnoreTarget(Collider target)
    {
        // 1. Проверка слоя (игнорируем Preview)
        if (ignoreLayers == (ignoreLayers | (1 << target.gameObject.layer)))
            return true;

        // 2. Дополнительная проверка для недостроенных зданий
        Constructable constructable = target.GetComponent<Constructable>();
        if (constructable != null && !constructable.IsConstructed)
            return true;

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ShouldIgnoreTarget(other)) return;

        if (isPlayer && other.CompareTag("Enemy") && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }
        if (isEnemy && (other.CompareTag("Ally") || other.CompareTag("AllyBuilding")) && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
     if (ShouldIgnoreTarget(other)) return; // Добавлена проверка

     if (isPlayer && other.CompareTag("Enemy") && targetToAttack == null)
    {
        targetToAttack = other.transform;
    }
    if (isEnemy && (other.CompareTag("Ally") || other.CompareTag("AllyBuilding")) && targetToAttack == null)
    {
        targetToAttack = other.transform;
    }
    }

private void OnTriggerExit(Collider other)
{
    // Исправлено условие - убрана проверка targetToAttack == null
    if (other.CompareTag("Enemy") && targetToAttack == other.transform)
    {
        targetToAttack = null;
    }
    if (isEnemy && (other.CompareTag("Ally") || other.CompareTag("AllyBuilding")) && targetToAttack == other.transform)
    {
        targetToAttack = null;
    }
}
public void SetIdleMaterial()
    {
       // GetComponent<Renderer>().material = idleStateMaterial;
    }
    public void SetFollowMaterial()
    {
       // GetComponent<Renderer>().material = followStateMaterial;

    }
    public void SetAttackMaterial()
    {
       // GetComponent<Renderer>().material = attackStateMaterial;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,10f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,3f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position,4f);


    }
} 
