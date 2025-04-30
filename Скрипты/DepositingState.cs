using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositingState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Worker worker = animator.GetComponent<Worker>();
        if (worker != null) 
        {
            worker.DepositResources();

        }
    }

   
}
