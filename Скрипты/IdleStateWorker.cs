using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateWorker : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator,AnimatorStateInfo stateInfo, int layerIndex)
    {
        Worker worker = animator.GetComponent<Worker>();
        worker.assignedNode = null;

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

}
