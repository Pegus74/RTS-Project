using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoingToHarvestState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Worker worker = animator.GetComponent<Worker>();
        if (worker.assignedNode!=null) 
        {
            worker.MoveTo(worker.assignedNode);

        }
    }

    
}
