using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingInterface : MonoBehaviour
{
    [Header("Настройки UI")]
    [SerializeField] private string canvasTag = "UnitUI";
    [SerializeField] private int destroyButtonIndex = 0;
    [SerializeField] private int spawnButtonIndex = 1;
    [SerializeField] private int cost = 50;
    [SerializeField] private bool disableSpawnButtonWhenActive = true;

    [Header("Настройки спавна")]
    public GameObject objectToSpawn;
    [SerializeField] private float spawnDelay = 3f; // Новая переменная для задержки спавна
    [SerializeField] private float spawnDistance = 1f; // Дистанция от здания

    private Transform uiPanel;
    private Button destroyButton;
    private Button spawnButton;
    private ResourceManager resourceManager;
    private bool isUIOpen;
    private bool isSpawning = false; // Флаг процесса спавна
    public static event System.Action<BuildingInterface> OnBuildingSelected;
    private static BuildingInterface _currentlySelected;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
        InitializeUI();
    }

    private void Update()
    {
        if (isUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
        }
        if (isUIOpen && Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out hit) || hit.transform != transform)
            {
                CloseUI();
            }
        }
    }

    private bool IsPointerOverUI()
    {
        // Проверяем, находится ли курсор над UI-элементом
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    private void InitializeUI()
    {
        Canvas targetCanvas = GameObject.FindGameObjectWithTag(canvasTag)?.GetComponent<Canvas>();

        if (targetCanvas == null)
        {
            Debug.LogError($"Canvas с тегом '{canvasTag}' не найден!");
            return;
        }

        if (targetCanvas.transform.childCount == 0)
        {
            Debug.LogError("У Canvas нет дочерних объектов!", targetCanvas);
            return;
        }

        // Активируем первый дочерний объект Canvas
        uiPanel = targetCanvas.transform.GetChild(1);
        uiPanel.gameObject.SetActive(true);

        // Находим и деактивируем кнопки
        FindButtons();
        SetButtonsActive(false);
    }

    private void FindButtons()
    {
        // Поиск кнопки уничтожения
        if (uiPanel.childCount > destroyButtonIndex)
        {
            destroyButton = uiPanel.GetChild(destroyButtonIndex).GetComponent<Button>();
            if (destroyButton != null)
            {
                destroyButton.onClick.AddListener(DestroyObject);
            }
            else
            {
                Debug.LogError($"Объект с индексом {destroyButtonIndex} не является кнопкой!");
            }
        }

        // Поиск кнопки спавна
        if (uiPanel.childCount > spawnButtonIndex)
        {
            spawnButton = uiPanel.GetChild(spawnButtonIndex).GetComponent<Button>();
            if (spawnButton != null)
            {
                spawnButton.onClick.AddListener(SpawnUnit);
            }
            else
            {
                Debug.LogError($"Объект с индексом {spawnButtonIndex} не является кнопкой!");
            }
        }
    }

    private void SetButtonsActive(bool active)
    {
        if (destroyButton != null) destroyButton.gameObject.SetActive(active);
        if (spawnButton != null)
        {
            spawnButton.gameObject.SetActive(active);
            if (active) UpdateSpawnButtonState();
        }
    }

    void OnMouseDown()
    {
        if (uiPanel != null)
        {
            // Если кликаем на уже выбранное здание - закрываем UI
            if (_currentlySelected == this)
            {
                CloseUI();
                _currentlySelected = null;
                return;
            }

            // Уведомляем о новом выбранном здании
            OnBuildingSelected?.Invoke(this);
            _currentlySelected = this;
            ToggleUI();
        }
    }

    private void ToggleUI()
    {
        isUIOpen = !isUIOpen;
        SetButtonsActive(isUIOpen);
    }
    public bool IsUIOpen() => isUIOpen;

    public void CloseUI()
    {
        if (_currentlySelected == this)
        {
            _currentlySelected = null;
        }
        isUIOpen = false;
        SetButtonsActive(false);
    }

    private void DestroyObject()
    {
        SetButtonsActive(false);
        resourceManager.IncreaseResource(ResourceManager.ResourcesType.Gold, cost);
        Destroy(gameObject);
    }

    private void SpawnUnit()
    {
        if (_currentlySelected != this || isSpawning) return;

        if (resourceManager.GetGold() >= cost)
        {
            resourceManager.DecreaseResource(ResourceManager.ResourcesType.Gold, cost);
            StartCoroutine(SpawnWithDelay());

            if (disableSpawnButtonWhenActive)
            {
                spawnButton.interactable = false;
            }
        }
        else
        {
            Debug.Log("Недостаточно золота для создания объекта.");
        }
    }

    private IEnumerator SpawnWithDelay()
    {
        isSpawning = true;

        
        if (spawnButton != null)
        {
            spawnButton.interactable = false;
        }

        yield return new WaitForSeconds(spawnDelay);

        Vector3 spawnPosition = transform.position;
        spawnPosition.y = 0f; // Убедимся, что юнит появляется на земле

        GameObject spawnedUnit = Instantiate(
            objectToSpawn,
            spawnPosition,
            Quaternion.identity
        );

        Worker workerComponent = spawnedUnit.GetComponent<Worker>();
        if (workerComponent != null)
        {
            workerComponent.supplyCenter = this.transform;
            Debug.Log($"Назначен supplyCenter для рабочего: {this.name}");
        }

        isSpawning = false;

        // Восстанавливаем кнопку, если нужно
        if (!disableSpawnButtonWhenActive && spawnButton != null)
        {
            spawnButton.interactable = true;
        }
    }

    private void UpdateSpawnButtonState()
    {
        if (spawnButton != null)
        {
            spawnButton.interactable = resourceManager.GetGold() >= cost && !isSpawning;
        }
    }
    private void OnDestroy()
    {
        if (destroyButton != null) destroyButton.onClick.RemoveListener(DestroyObject);
        if (spawnButton != null) spawnButton.onClick.RemoveListener(SpawnUnit);
    }
}