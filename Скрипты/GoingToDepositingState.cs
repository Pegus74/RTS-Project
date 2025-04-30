using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoingToDepositingState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Worker worker = animator.GetComponent<Worker>();
        worker.MoveTo(worker.supplyCenter);
    }

   
}
