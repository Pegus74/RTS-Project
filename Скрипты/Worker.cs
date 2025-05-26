using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Worker : MonoBehaviour
{

    #region Serialized Fields
    [Header("Assignments")]
    [SerializeField] public Transform assignedNode;
    [SerializeField] public Transform supplyCenter;
    [SerializeField] private float harvestAmountPerSecond = 1f;
    [SerializeField] private float maxCapacity = 10f;
    [SerializeField] private float currentCapacity = 0f;

    [Header("Visual Settings")]
    [SerializeField] private GameObject defaultModel;
    [SerializeField] private GameObject carryingModel;
    [SerializeField] private float modelSwitchDelay = 0.5f;

    [Header("Construction Settings")]
    [SerializeField] private float constructionSpeed = 1.5f;


   
    #endregion

    #region Private Fields
    private NavMeshAgent agent;
    private Animator animator;
    private bool isDepositing = false;
    private Constructable currentConstruction;
    private Coroutine modelSwitchCoroutine;

    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeComponents();
        InitializeModels();
    }

    private void Update()
    {
        UpdateState();
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleTriggerExit(other);
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void InitializeModels()
    {
        SetModelActive(defaultModel, true);
        SetModelActive(carryingModel, false);
    }
    #endregion

    #region Core Gameplay Logic
    private void UpdateState()
    {
        if (!IsInManualMode()) // Автономный режим
        {
            UpdateAnimatorParameters();
            CheckNodeDepletion();
            HandleFullCapacity();
        }
    }

    private void HandleFullCapacity()
    {
        if (currentCapacity >= maxCapacity && !isDepositing)
        {
            animator.SetTrigger("goToDeposit");
            MoveTo(supplyCenter); // Добавляем явное перемещение к складу
        }
    }
    #endregion

    #region Resource Handling
    public void Harvest()
    {
        if (assignedNode == null) return;

        var node = assignedNode.GetComponent<ResourceNode>();
        if (node == null || node.IsDepleted) return;

        node.Harvest(harvestAmountPerSecond * Time.deltaTime);
        float newCapacity = currentCapacity + harvestAmountPerSecond * Time.deltaTime;

        // Включаем модель с ресурсами при достижении максимальной вместимости
        if (newCapacity >= maxCapacity && currentCapacity < maxCapacity)
        {
            SwitchModel(true);
        }

        currentCapacity = Mathf.Min(newCapacity, maxCapacity);
    }

    public void DepositResources()
    {
        if (isDepositing || !animator.GetBool("atSupply")) return;

        isDepositing = true;
        // Модель уже должна быть carryingModel (переключилась при заполнении)
        StartCoroutine(DepositProcess());
    }

    private IEnumerator DepositProcess()
    {
        float tempCapacity = currentCapacity;

        while (currentCapacity > 0f)
        {
            currentCapacity = Mathf.Max(0f, currentCapacity - 3 * Time.deltaTime);
            yield return null;
        }

        CompleteDeposit(tempCapacity);
    }

    private void CompleteDeposit(float depositedAmount)
    {
        ResourceManager.Instance.IncreaseResource(ResourceManager.ResourcesType.Gold, (int)depositedAmount * 10);
        isDepositing = false;

        // Возвращаем обычную модель после полной сдачи
        SwitchModel(false);

        if (assignedNode != null)
        {
            animator.SetTrigger("doneDepositing");
        }
    }
    #endregion

    #region Movement & Construction
    public void MoveTo(Transform target)
    {
        if (target == null || agent == null) return;

        // Если рабочий выделен и цель не склад/шахта - разрешаем ручное управление
        if (IsInManualMode() && target != supplyCenter && target != assignedNode)
        {
            agent.SetDestination(target.position);
        }
        else if (!IsInManualMode()) // Автономное перемещение
        {
            agent.SetDestination(target.position);
        }
    }

    public void StartHelpingConstruction(Constructable construction)
    {
        if (construction == null) return;

        // Прерываем текущие действия, но сохраняем ресурсы
        if (isDepositing)
        {
            StopCoroutine(DepositProcess());
            isDepositing = false;
        }

        // Останавливаем движение, но не сбрасываем ресурсы
        currentConstruction = construction;
        animator.SetBool("IsConstructing", true);
        StopMovement();
    }

    public void StopHelpingConstruction()
    {
        if (currentConstruction == null) return;

        currentConstruction = null;
        animator.SetBool("IsConstructing", false);
        ResumeMovement();

        // Возвращаемся к логике в зависимости от текущего состояния
        if (currentCapacity >= maxCapacity)
        {
            // Если полный - идем сдавать
            animator.SetTrigger("goToDeposit");
        }
        else if (currentCapacity > 0 && assignedNode == null)
        {
            // Если есть ресурсы, но нет узла - идем сдавать
            animator.SetTrigger("goToDeposit");
        }
        else if (assignedNode != null)
        {
            // Если есть назначенный узел - продолжаем сбор
            animator.SetTrigger("continueHarvesting");
        }
    }

    private void StopMovement()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    private void ResumeMovement()
    {
        if (agent != null)
        {
            agent.isStopped = false;
        }
    }
    #endregion

    #region Model Management
    private void SwitchModel(bool isCarrying)
    {
        if (modelSwitchCoroutine != null)
        {
            StopCoroutine(modelSwitchCoroutine);
        }
        modelSwitchCoroutine = StartCoroutine(SwitchModelWithDelay(isCarrying));
    }

    private IEnumerator SwitchModelWithDelay(bool isCarrying)
    {
        yield return new WaitForSeconds(modelSwitchDelay);
        SetModelActive(defaultModel, !isCarrying);
        SetModelActive(carryingModel, isCarrying);
    }

    private void SetModelActive(GameObject model, bool active)
    {
        if (model != null)
        {
            model.SetActive(active);
        }
    }
    #endregion

    #region Trigger Handling
    private void HandleTriggerEnter(Collider other)
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

    private void HandleTriggerExit(Collider other)
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
    #endregion

    #region State Checks
    private void UpdateAnimatorParameters()
    {
        animator.SetBool("hasAssignedNode", assignedNode != null);
        animator.SetBool("isFull", currentCapacity >= maxCapacity);
    }

    private void CheckNodeDepletion()
    {
        if (assignedNode == null) return;

        var node = assignedNode.GetComponent<ResourceNode>();
        if (node != null && node.IsDepleted)
        {
            assignedNode = null;
            animator.SetBool("hasAssignedNode", false);
        }
    }

    public bool IsAvailableForConstruction()
    {
        return agent != null && agent.isActiveAndEnabled;
    }
    #endregion
    public bool IsInManualMode()
    {
        // Если рабочий выделен и получил команду (например, на строительство)
        return ВыборЮнитов.Instance.unitsSelected.Contains(gameObject) &&
          (currentConstruction != null || agent.hasPath && !isDepositing);
    }
}