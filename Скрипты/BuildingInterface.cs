using UnityEngine;
using UnityEngine.UI;

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

    private Transform uiPanel;
    private Button destroyButton;
    private Button spawnButton;
    private ResourceManager resourceManager;
    private bool isUIOpen;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
        InitializeUI();
    }

    private void Update()
    {
        // Закрытие интерфейса при нажатии Esc
        if (isUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
        }
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
            ToggleUI();
        }
    }

    private void ToggleUI()
    {
        isUIOpen = !isUIOpen;
        SetButtonsActive(isUIOpen);
    }

    private void CloseUI()
    {
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
        if (resourceManager.GetGold() >= cost)
        {
            resourceManager.DecreaseResource(ResourceManager.ResourcesType.Gold, cost);
            Instantiate(objectToSpawn, transform.position + new Vector3(1, 0, 0), Quaternion.identity);

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

    private void UpdateSpawnButtonState()
    {
        if (spawnButton != null)
        {
            spawnButton.interactable = resourceManager.GetGold() >= cost;
        }
    }

    private void OnDestroy()
    {
        if (destroyButton != null) destroyButton.onClick.RemoveListener(DestroyObject);
        if (spawnButton != null) spawnButton.onClick.RemoveListener(SpawnUnit);
    }
}