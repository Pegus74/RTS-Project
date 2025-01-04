using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ѕеремещениеёнит : MonoBehaviour
{
    public LayerMask terrain;
    Camera cam;
    UnityEngine.AI.NavMeshAgent agent;
    Animator animator;
    public bool isCommandedToMove;

    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit,Mathf.Infinity, terrain)) 
            { 
                isCommandedToMove = true;
                agent.SetDestination(hit.point);
                animator.SetBool("IsMoving", true); 
            }
        }

        if (agent.hasPath == false || agent.remainingDistance<=agent.stoppingDistance)
        {
            isCommandedToMove=false;
            animator.SetBool("IsMoving", false) ;
        }
        else
        {
            animator.SetBool("IsMoving", true);
        }

    }
     

}
