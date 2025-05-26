using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAIController : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyUnitPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 10f;
    public int maxUnits = 5;
    public int minUnitsForAttack = 3;

    [Header("Attack Settings")]
    public Transform playerBaseRallyPoint; // ����� ����� ����� ���� ������
    public float regroupRadius = 5f;
    public float attackTriggerRadius = 7f; // ������ ��� �������������� �����

    private int currentUnits = 0;
    private bool isAttackInProgress = false;

    void Start()
    {
        StartCoroutine(EnemyAILoop());
    }

    IEnumerator EnemyAILoop()
    {
        while (true)
        {
            // ���� ���������� ������
            while (currentUnits < minUnitsForAttack)
            {
                if (currentUnits < maxUnits && !isAttackInProgress)
                {
                    SpawnEnemyUnit();
                }
                yield return new WaitForSeconds(spawnInterval);
            }

            // ���� �����
            if (!isAttackInProgress)
            {
                yield return StartCoroutine(LaunchAttackWave());
            }

            yield return null;
        }
    }

    IEnumerator LaunchAttackWave()
    {
        isAttackInProgress = true;
        int attackingUnits = currentUnits;

        // 1. ���� � ����� ����� ���� ������
        Vector3 rallyPoint = GetRandomRallyPoint();
        MoveUnitsTo(rallyPoint, attackingUnits);
        yield return new WaitForSeconds(3f); // ����� �� ����������

        // 2. ������� � ����� �����
        EnableAttackModeForUnits(attackingUnits);

        // ���� ���� ��������� ����� �� ����� ����������
        while (currentUnits > maxUnits - minUnitsForAttack)
        {
            yield return new WaitForSeconds(1f);
        }

        isAttackInProgress = false;
    }

    void SpawnEnemyUnit()
    {
        if (spawnPoints.Length == 0 || enemyUnitPrefab == null) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject newEnemy = Instantiate(enemyUnitPrefab, spawnPoint.position, spawnPoint.rotation);
        currentUnits++;

        var unit = newEnemy.GetComponent<����>();
        if (unit != null)
        {
            unit.OnDestroyEvent += () => currentUnits--;
        }

        // ��������� ��������� ������ �����
        if (isAttackInProgress)
        {
            // ���� ���� �����, ����� ���� �������� �� �����
            var mover = newEnemy.GetComponent<���������������>();
            if (mover != null)
            {
                mover.isCommandedToMove = false;
                mover.StopMovement(); // ������������� �� �����, ������� ��������
            }
        }
    }

    Vector3 GetRandomRallyPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * regroupRadius;
        return playerBaseRallyPoint.position + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    void MoveUnitsTo(Vector3 position, int count)
    {
        int movedUnits = 0;
        foreach (var unit in FindObjectsOfType<����>())
        {
            if (unit.CompareTag("Enemy") && movedUnits < count)
            {
                var mover = unit.GetComponent<���������������>();
                if (mover != null)
                {
                    mover.SetDestination(position);
                    mover.isCommandedToMove = true;
                    movedUnits++;
                }
            }
        }
    }

    void EnableAttackModeForUnits(int count)
    {
        int processedUnits = 0;
        foreach (var unit in FindObjectsOfType<����>())
        {
            if (unit.CompareTag("Enemy") && processedUnits < count)
            {
                var attackController = unit.GetComponent<AttackController>();
                if (attackController != null)
                {
                    // �������� �������������� ����� ����� �������
                    var collider = unit.GetComponent<SphereCollider>();
                    if (collider != null)
                    {
                        collider.radius = attackTriggerRadius;
                    }
                }
                processedUnits++;
            }
        }
    }
}    