using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class КоробкаВыбора : MonoBehaviour
{
    Camera myCam;

    [SerializeField]
    RectTransform boxVisual;

    Rect selectionBox;

    Vector2 startPosition;
    Vector2 endPosition;

    private void Start()
    {
        myCam = Camera.main;
        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        DrawVisual();
    }

    private void Update()
    {
        // При нажатии ЛКМ
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            selectionBox = new Rect();
        }

        // При удержании ЛКМ (перетаскивание)
        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;
            DrawVisual();
            DrawSelection();

            // Если коробка выделения имеет размер, снимаем старое выделение и выбираем юнитов
            if (boxVisual.rect.width > 0 || boxVisual.rect.height > 0)
            {
                ВыборЮнитов.Instance.DeselectAll();
                SelectUnits();
            }
        }

        // При отпускании ЛКМ
        if (Input.GetMouseButtonUp(0))
        {
            SelectUnits();
            startPosition = Vector2.zero;
            endPosition = Vector2.zero;
            DrawVisual();
        }
    }

    void DrawVisual()
    {
        Vector2 boxStart = startPosition;
        Vector2 boxEnd = endPosition;
        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        boxVisual.position = boxCenter;
        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));
        boxVisual.sizeDelta = boxSize;
    }

    void DrawSelection()
    {
        if (Input.mousePosition.x < startPosition.x)
        {
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = startPosition.x;
        }
        else
        {
            selectionBox.xMin = startPosition.x;
            selectionBox.xMax = Input.mousePosition.x;
        }

        if (Input.mousePosition.y < startPosition.y)
        {
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = startPosition.y;
        }
        else
        {
            selectionBox.yMin = startPosition.y;
            selectionBox.yMax = Input.mousePosition.y;
        }
    }

    void SelectUnits()
    {
        foreach (var unit in ВыборЮнитов.Instance.allUnitsList)
        {
            // Проверяем, что юнит принадлежит игроку (не враг)
            if (IsPlayerUnit(unit) && selectionBox.Contains(myCam.WorldToScreenPoint(unit.transform.position)))
            {
                ВыборЮнитов.Instance.DragSelect(unit);
            }
        }
    }

    // Проверяет, является ли юнит "своим" (игроком)
    private bool IsPlayerUnit(GameObject unit)
    {
        // Проверяем, является ли юнит рабочим
        if (unit.GetComponent<Worker>() != null)
        {
            return true; // Рабочие всегда принадлежат игроку
        }

        // Стандартная проверка для других юнитов
        AttackController attackController = unit.GetComponent<AttackController>();
        if (attackController != null)
        {
            return attackController.isPlayer;
        }
        return false;
    }
}