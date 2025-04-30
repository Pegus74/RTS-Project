using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider))]
public class Constructable : MonoBehaviour, IDamageable
{
    [Header("Base Settings")]
    [SerializeField] private float constMaxHealth = 100f;
    [SerializeField] private HealthTracker healthTracker;
    [SerializeField] private bool isEnemy = false;
    [SerializeField] private BuildingType buildingType;
    [SerializeField] private Vector3 buildPosition;
    private BoxCollider boxCollider;

    [Header("Construction Settings")]
    [SerializeField] private float constructionTime = 3f;
    [SerializeField] private GameObject constructionModel;
    [SerializeField] private GameObject finishedModel;
    [SerializeField] private float constructionRadius = 5f;
    [SerializeField] private int workersRequired = 1;

    [Header("Collider Settings")]
    [SerializeField] private SphereCollider constructionZone;

    private NavMeshObstacle obstacle;
    private float constHealth;
    private List<Worker> nearbyWorkers = new List<Worker>();
    private bool constructionInProgress = false;
    public float constructionProgress = 0f;

    public bool IsConstructed { get; private set; }
    public float ConstructionRadius => constructionRadius;
    public event System.Action OnConstructionCompleted;

    private void Awake()
    {
        obstacle = GetComponentInChildren<NavMeshObstacle>();
        boxCollider = GetComponent<BoxCollider>();

        // Инициализация коллайдера
        constructionZone = GetComponent<SphereCollider>();
        if (constructionZone == null)
        {
            constructionZone = gameObject.AddComponent<SphereCollider>();
        }
        constructionZone.radius = constructionRadius;
        constructionZone.isTrigger = true;
        constructionZone.enabled = true;

        InitializeConstruction();
    }
    private void InitializeConstruction()
    {
        if (constructionModel != null) constructionModel.SetActive(true);
        if (finishedModel != null) finishedModel.SetActive(false);

        if (obstacle != null) obstacle.enabled = false;
        if (boxCollider != null) boxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Worker entered: {other.name}");
        if (!other.name.Contains("Worker")) return;

        Worker worker = other.GetComponent<Worker>();
        if (worker != null && worker.IsAvailableForConstruction())
        {
            worker.StartHelpingConstruction(this);
            nearbyWorkers.Add(worker);

            if (nearbyWorkers.Count >= workersRequired && !constructionInProgress)
            {
                StartConstruction();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Worker")) return;

        Worker worker = other.GetComponent<Worker>();
        if (worker != null && nearbyWorkers.Contains(worker))
        {
            worker.StopHelpingConstruction();
            nearbyWorkers.Remove(worker);

            if (nearbyWorkers.Count < workersRequired && constructionInProgress)
            {
                PauseConstruction();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, constructionRadius);
    }

    public void StartConstruction()
    {
        if (constructionInProgress) return;

        constructionProgress = 0f;
        constructionInProgress = true;
        IsConstructed = false;

        if (constructionModel != null) constructionModel.SetActive(true);
        if (finishedModel != null) finishedModel.SetActive(false);

        StartCoroutine(ConstructionProcess());
    }

    private void PauseConstruction()
    {
        constructionInProgress = false;
        StopCoroutine(nameof(ConstructionProcess));
    }

    public IEnumerator ConstructionProcess()
    {
        while (constructionProgress < constructionTime)
        {
            if (nearbyWorkers.Count >= workersRequired)
            {
                constructionProgress += Time.deltaTime;
                UpdateConstructionProgress(constructionProgress / constructionTime);
            }
            yield return null;
        }
        CompleteConstruction();

    }


    private void UpdateConstructionProgress(float progress)
    {
        // Можно добавить визуализацию прогресса
    }

    private void CompleteConstruction()
    {
        IsConstructed = true;
        constructionInProgress = false;

        if (constructionModel != null) constructionModel.SetActive(false);
        if (finishedModel != null) finishedModel.SetActive(true);

        if (boxCollider != null) boxCollider.enabled = true;
        if (obstacle != null) obstacle.enabled = true;

        if (isEnemy) gameObject.tag = "Enemy";

        HandleBuildingFunctionality();

        // Освобождаем рабочих
        foreach (var worker in nearbyWorkers)
        {
            worker.StopHelpingConstruction();
        }
        nearbyWorkers.Clear();

        OnConstructionCompleted?.Invoke();
    }

    private void HandleBuildingFunctionality()
    {
       // switch (buildingType)
        //{
       //     case BuildingType.Castle:
      //          SpawnWorker();
      //          break;
      //  }
    }

    private void SpawnWorker()
    {
        GameObject workerPrefab = Resources.Load<GameObject>("Worker");
        if (workerPrefab == null) return;

        Transform supplyDrop = transform.Find("ResourcesDrop");
        Vector3 spawnPosition = supplyDrop != null ? supplyDrop.position : transform.position;

        GameObject worker = Instantiate(workerPrefab, spawnPosition, Quaternion.identity);
        Worker workerComponent = worker.GetComponent<Worker>();
        if (workerComponent != null && supplyDrop != null)
        {
            workerComponent.supplyCenter = supplyDrop;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IsConstructed) return;

        constHealth -= damage;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthTracker != null)
            healthTracker.UpdateSliderValue(constHealth, constMaxHealth);

        if (constHealth <= 0)
            DestroyBuilding();
    }

    private void DestroyBuilding()
    {
        if (!isEnemy)
            ResourceManager.Instance.UpdateBuildingChanged(buildingType, false, buildPosition);

        Destroy(gameObject);
    }
}