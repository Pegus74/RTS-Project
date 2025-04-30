using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ВыборЮнитов : MonoBehaviour
{
    public LayerMask Clickable;
    public LayerMask terrain;
    public LayerMask attackable;
    public bool attackCursorVisible;
    public GameObject groundMarker;
    public GameObject workerUI; // Ссылка на UI для рабочего

    private Camera cam;

    public static ВыборЮнитов Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public List<GameObject> allUnitsList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();

    private void Start()
    {
        cam = Camera.main;
        unitsSelected.Clear();
        if (workerUI != null) workerUI.SetActive(false); // Скрываем UI при старте
    }

    private void Update()
    {
        // Проверяем выделение и обновляем UI рабочего
        CheckWorkerSelection();

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Clickable))
            {
                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    SeveralSelect(hit.collider.gameObject);
                }
                else
                {
                    SelectClick(hit.collider.gameObject);
                }
            }
            else
            {
                if (!Input.GetKey(KeyCode.LeftAlt))
                {
                    DeselectAll();
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && unitsSelected.Count > 0)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrain))
            {
                groundMarker.transform.position = hit.point;
                groundMarker.SetActive(false);
                groundMarker.SetActive(true);
            }
        }

        if (unitsSelected.Count > 0 && AtLeastOneUnit(unitsSelected))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, attackable))
            {
                attackCursorVisible = true;

                if (Input.GetMouseButtonDown(1))
                {
                    Transform target = hit.transform;

                    foreach (GameObject unit in unitsSelected)
                    {
                        unit.GetComponent<AttackController>().targetToAttack = target;
                    }
                }
            }
            else
            {
                attackCursorVisible = false;
            }
        }
    }

    // Новый метод для проверки выделения рабочего
    private void CheckWorkerSelection()
    {
        if (workerUI == null) return;

        if (unitsSelected.Count == 1 && unitsSelected[0].GetComponent<Worker>() != null)
        {
            workerUI.SetActive(true); // Активируем UI если выбран один рабочий
        }
        else
        {
            workerUI.SetActive(false); // Скрываем UI в других случаях
        }
    }

    private bool AtLeastOneUnit(List<GameObject> unitsSelected)
    {
        foreach (GameObject unit in unitsSelected)
        {
            if (unit.GetComponent<AttackController>())
            {
                return true;
            }
        }
        return false;
    }

 

    private void SeveralSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);
            SelectUnit(unit, true);
        }
        else
        {
            SelectUnit(unit, false);
            unitsSelected.Remove(unit);
        }
    }

    private void SelectClick(GameObject unit)
    {
        DeselectAll();
        unitsSelected.Add(unit);
        SelectUnit(unit, true);
    }

    public void DeselectAll()
    {
        foreach (var unit in unitsSelected)
        {
            SelectUnit(unit, false);
        }
        groundMarker.SetActive(false);
        unitsSelected.Clear();

        // Скрываем UI при снятии выделения
       if (workerUI != null) workerUI.SetActive(false);
    }

    private void UnitMovement(GameObject unit, bool Move)
    {
        unit.GetComponent<ПеремещениеЮнит>().enabled = Move;
    }

    private void SelectIndicator(GameObject unit, bool isVisible)
    {
        unit.transform.Find("Indicator").gameObject.SetActive(isVisible);
    }

    internal void DragSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);
            SelectUnit(unit, true);
        }
    }

    private void SelectUnit(GameObject unit, bool isSelected)
    {
        SelectIndicator(unit, isSelected);
        UnitMovement(unit, isSelected);
    }
}