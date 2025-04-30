using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestingState : StateMachineBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Worker worker = animator.GetComponent<Worker>();
        worker.Harvest();

    }

    
}
