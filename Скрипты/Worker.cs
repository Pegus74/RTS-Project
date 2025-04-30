using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    [Header("Assignments")]
    public Transform assignedNode;
    public Transform supplyCenter;
    public float harvestAmountPerSecond = 1f;
    public float maxCapacity = 10f;
    public float currentCapacity = 0f;

    [Header("Visual Settings")]
    public GameObject defaultModel;       
    public GameObject carryingModel;      
    public float modelSwitchDelay = 0.5f; 

    [Header("Construction Settings")]
    public float constructionSpeed = 1.5f;

    private UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;
    private bool isDepositing = false;
   public Constructable currentConstruction;
    private Coroutine modelSwitchCoroutine;

    void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
        InitializeModels();
    }

    void InitializeModels()
    {
        if (defaultModel != null) defaultModel.SetActive(true);
        if (carryingModel != null) carryingModel.SetActive(false);
    }

    void Update()
    {
        UpdateAnimatorParameters();
        CheckNodeDepletion();
        AutoSwitchModelWhenFull();
    }

    void UpdateAnimatorParameters()
    {
        animator.SetBool("hasAssignedNode", assignedNode != null);
        animator.SetBool("isFull", currentCapacity >= maxCapacity);
    }

    void CheckNodeDepletion()
    {
        if (assignedNode != null && assignedNode.GetComponent<ResourceNode>().IsDepleted)
        {
            assignedNode = null;
            animator.SetBool("hasAssignedNode", false);
        }
    }

    void AutoSwitchModelWhenFull()
    {
        if (currentCapacity >= maxCapacity && !isDepositing)
        {
            DepositResources();
        }
    }

    public void StartHelpingConstruction(Constructable construction)
    {
        if (construction == null) return;

        currentConstruction = construction;
        animator.SetBool("IsConstructing", true);

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    public void StopHelpingConstruction()
    {
        currentConstruction = null;
        animator.SetBool("IsConstructing", false);
        if (agent != null) agent.isStopped = false;
    }

    public void MoveTo(Transform target)
    {
        if (target == null || agent == null) return;
        agent.SetDestination(target.position);
        SwitchModel(false); 
    }

    public void Harvest()
    {
        if (assignedNode == null) return;

        ResourceNode node = assignedNode.GetComponent<ResourceNode>();
        if (node != null && !node.IsDepleted)
        {
            node.Harvest(harvestAmountPerSecond * Time.deltaTime);
            currentCapacity = Mathf.Min(currentCapacity + harvestAmountPerSecond * Time.deltaTime, maxCapacity);
        }
    }

    public void DepositResources()
    {
        if (isDepositing) return;

        isDepositing = true;
        SwitchModel(true); 
        StartCoroutine(DepositProcess());
    }

    IEnumerator DepositProcess()
    {
        float tempCapacity = currentCapacity;

        while (currentCapacity > 0f)
        {
            currentCapacity = Mathf.Max(0f, currentCapacity - 3 * Time.deltaTime);
            yield return null;
        }

        ResourceManager.Instance.IncreaseResource(ResourceManager.ResourcesType.Gold, (int)tempCapacity * 10);
        isDepositing = false;

        if (assignedNode != null)
        {
            animator.SetTrigger("doneDepositing");
        }

        SwitchModel(false); 
    }

    void SwitchModel(bool isCarrying)
    {
        if (modelSwitchCoroutine != null)
        {
            StopCoroutine(modelSwitchCoroutine);
        }
        modelSwitchCoroutine = StartCoroutine(SwitchModelWithDelay(isCarrying));
    }

    IEnumerator SwitchModelWithDelay(bool isCarrying)
    {
        yield return new WaitForSeconds(modelSwitchDelay);

        if (defaultModel != null) defaultModel.SetActive(!isCarrying);
        if (carryingModel != null) carryingModel.SetActive(isCarrying);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (assignedNode != null && other.transform == assignedNode)
        {
            animator.SetBool("atNode", true);
        }
        else if (supplyCenter != null && other.transform == supplyCenter)
        {
            animator.SetBool("atSupply", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (assignedNode != null && other.transform == assignedNode)
        {
            animator.SetBool("atNode", false);
        }
        else if (supplyCenter != null && other.transform == supplyCenter)
        {
            animator.SetBool("atSupply", false);
        }
    }

    public bool IsAvailableForConstruction()
    {
        return currentConstruction == null
            && assignedNode == null
            && !isDepositing
            && agent != null
            && agent.isActiveAndEnabled;
    }
}