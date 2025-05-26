using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private LayerMask previewLayer;
   
    [SerializeField] private float previewYOffset = 0.06f;
    [SerializeField] private Material previewMaterialPrefab;

    private GameObject previewObject;
    private Material previewMaterialInstance;

    private void Awake() // �������� � Start �� Awake
    {
        previewLayer = LayerMask.NameToLayer("Preview");
        previewMaterialInstance = new Material(previewMaterialPrefab); // ������������� ���������
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        StopShowingPreview();
        previewObject = Instantiate(prefab);
        SetLayerRecursively(previewObject, previewLayer);
        PreparePreview(previewObject); // ���������� ������ ����� ����������
    }


    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    private void ApplyPreviewMaterials()
    {
        foreach (var renderer in previewObject.GetComponentsInChildren<Renderer>())
        {
            renderer.material = previewMaterialInstance;
        }
    }

    private void DisableComponents(GameObject obj)
    {
        // ��������� ���������� (���� ��� ��� �� �������� ��-�� ����)
        foreach (var collider in obj.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        // ��������� �������, ������� �� ������ �������� � ������
        var constructable = obj.GetComponent<Constructable>();
        if (constructable != null) constructable.enabled = false;

        var attackable = obj.GetComponent<IDamageable>();
        if (attackable != null) Destroy(attackable as MonoBehaviour);
    }

    public void ConfirmPlacement()
    {
        if (previewObject == null) return;
        previewObject.tag = "AllyBuilding"; // ��� ������ ������ ���
        SetLayerRecursively(previewObject, LayerMask.NameToLayer("Default"));

            // ���������� ���������� ���� (��������, "Default")
            SetLayerRecursively(previewObject, LayerMask.NameToLayer("Default"));

        // �������� ������� ���������� � ����������
        foreach (var collider in previewObject.GetComponentsInChildren<Collider>())
        {
            collider.enabled = true;
        }

        var constructable = previewObject.GetComponent<Constructable>();
        if (constructable != null) constructable.enabled = true;
    }

    public void ShowConstructionPreview(GameObject prefab)
    {
        StopShowingPreview();
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
    }

    internal void StartShowingRemovePreview()
    {
        ApplyFeedbackToCursor(false);
    }

    private void PreparePreview(GameObject previewObject)
    {
        previewObject.tag = "Preview";
        previewObject.layer = LayerMask.NameToLayer("Preview");
        foreach (Renderer renderer in previewObject.GetComponentsInChildren<Renderer>())
        {
            renderer.material = previewMaterialInstance;
        }
      
        // 2. ���������� �����������
        foreach (var collider in previewObject.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        var constructable = previewObject.GetComponent<Constructable>();
        if (constructable != null) constructable.enabled = false;
    }

    public void StopShowingPreview()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (previewObject == null || previewMaterialInstance == null) return;

        MovePreview(position);
        ApplyFeedbackToPreview(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        if (previewMaterialInstance == null) return;

        Color c = validity ? Color.green : Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        // ����� ����� ���� ������ ��������� �������
        // ���� ��������� ������, ��� ��� ���������� �� ��������
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(
            position.x,
            position.y + previewYOffset,
            position.z
        );
    }
}