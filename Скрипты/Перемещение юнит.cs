using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ПеремещениеЮнит : MonoBehaviour
{
    public LayerMask terrain;
    private Camera cam;
    public NavMeshAgent agent { get; private set; } // Делаем свойство с защищенным set
    public Vector3 targetPosition;
    public bool isCommandedToMove;

    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

    }
    private void Update()
    {
        // Проверяем, выделен ли юнит
        if (!ВыборЮнитов.Instance.unitsSelected.Contains(gameObject))
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrain))
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

    public void SetDestination(Vector3 destination)
    {
        if (agent != null && agent.enabled)
        {
            agent.SetDestination(destination);
            isCommandedToMove = true;
            StartCoroutine(ResetCommandAfterDelay(1f));
        }
    }
    public void StopMovement()
    {
        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
            isCommandedToMove = false;
        }
    }
    private IEnumerator ResetCommandAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isCommandedToMove = false;
    }


}