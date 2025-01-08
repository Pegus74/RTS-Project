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

    public int unitDamage;


    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer && other.CompareTag("Enemy") && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }
        if (isEnemy && other.CompareTag("Ally") && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (isPlayer && other.CompareTag("Enemy") && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }

        if (isEnemy && other.CompareTag("Ally") && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }
    }
  
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Enemy") && targetToAttack != null)
        {
            targetToAttack = null;

        }

        if (other.CompareTag("Ally") && targetToAttack != null)
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
