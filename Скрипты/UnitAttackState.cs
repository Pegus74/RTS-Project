using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAttackState : StateMachineBehaviour
{
    NavMeshAgent agent;
    AttackController attackController;

    public float stopAttackingDistance = 10f;
   
    public float attackRate = 2f;
    private float attackTimer;
 


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        attackController = animator.GetComponent<AttackController>();
        attackController.SetAttackMaterial();
        animator.SetBool("IsAttacking", true);
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackController.targetToAttack != null && animator.transform.GetComponent<ПеремещениеЮнит>().isCommandedToMove == false)
        {
            LookAtTarget();
            //перемещение при атаке(может понадобится)
            //agent.SetDestination(attackController.targetToAttack.position);

            if(attackTimer <= 0) 
            {
                Attack();
              
                attackTimer = 1f / attackRate;
             
            
            }
            else
            {
                attackTimer -= Time.deltaTime; 

            }



            float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);
            if (distanceFromTarget > stopAttackingDistance || attackController.targetToAttack == null)
            {

                animator.SetBool("IsAttacking", false);


            }
        }
        else
        {
            animator.SetBool("IsAttacking", false);
        }
    }

    private void Attack()
    {
        if (attackController.targetToAttack == null) return;

        Debug.Log($"Атака по цели: {attackController.targetToAttack.name}");

        var damageable = attackController.targetToAttack.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log($"Наносится урон: {attackController.unitDamage}");
            damageable.TakeDamage(attackController.unitDamage);
        }
    }


    private void LookAtTarget()
    {
        Vector3 direction = attackController.targetToAttack.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);
        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

  
}
