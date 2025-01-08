using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ѕеремещениеёнит : MonoBehaviour
{
    public LayerMask terrain;
    Camera cam;
    UnityEngine.AI.NavMeshAgent agent;
 
    public bool isCommandedToMove;

    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        
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
                StartCoroutine(NoCommand());
                agent.SetDestination(hit.point);
               
            }
        }


       // if (agent.hasPath == false || agent.remainingDistance<=agent.stoppingDistance)
       // {
       //     isCommandedToMove=false;
           
      //  }
       
    }
    
    IEnumerator NoCommand()
    {
        yield return new WaitForSeconds(1);
        isCommandedToMove = false;
    }
     

}
