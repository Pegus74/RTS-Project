using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class UnitFollowState : StateMachineBehaviour
{
    private AttackController attackController;
    private NavMeshAgent agent;
    private ПеремещениеЮнит movementController;

    public float attackingDistance = 30f; // Исправлено с 3000 на разумное значение
    private bool isInitialized = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        InitializeComponents(animator);

        if (attackController != null)
        {
            attackController.SetFollowMaterial();
        }
    }

    private void InitializeComponents(Animator animator)
    {
        if (isInitialized) return;

        attackController = animator.GetComponent<AttackController>();
        agent = animator.GetComponent<NavMeshAgent>();
        movementController = animator.GetComponent<ПеремещениеЮнит>();
        isInitialized = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!AreComponentsValid())
        {
            animator.SetBool("IsFollowing", false);
            return;
        }

        if (attackController.targetToAttack == null)
        {
            animator.SetBool("IsFollowing", false);
            return;
        }

        // Проверка на смерть юнита
        if (!animator.gameObject.activeSelf || !agent.isActiveAndEnabled)
        {
            animator.SetBool("IsFollowing", false);
            return;
        }

        if (!movementController.isCommandedToMove)
        {
            HandleFollowing(animator);
        }
    }

    private bool AreComponentsValid()
    {
        return attackController != null &&
               agent != null &&
               movementController != null;
    }

    private void HandleFollowing(Animator animator)
    {
        // Дополнительная проверка цели
        if (attackController.targetToAttack == null ||
            !attackController.targetToAttack.gameObject.activeInHierarchy)
        {
            animator.SetBool("IsFollowing", false);
            return;
        }

        // Безопасная установка пути
        if (agent.isOnNavMesh && agent.isActiveAndEnabled)
        {
            agent.SetDestination(attackController.targetToAttack.position);
            animator.transform.LookAt(new Vector3(
                attackController.targetToAttack.position.x,
                animator.transform.position.y,
                attackController.targetToAttack.position.z
            ));

            float distanceFromTarget = Vector3.Distance(
                attackController.targetToAttack.position,
                animator.transform.position
            );

            if (distanceFromTarget < attackingDistance)
            {
                agent.SetDestination(animator.transform.position);
                animator.SetBool("IsAttacking", true);
            }
        }
        else
        {
            animator.SetBool("IsFollowing", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Остановка агента при выходе из состояния
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
    }



}
