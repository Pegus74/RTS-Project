using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private int startUnitsPerWave = 3;
    [SerializeField] private float startSpawnInterval = 2f;
    [SerializeField] private float waveCooldown = 10f;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Настройки второго типа юнитов")]
    [SerializeField] private GameObject secondEnemyTypePrefab;
    [SerializeField] private int secondTypeStartWave = 3;
    [SerializeField][Range(0f, 1f)] private float secondTypeSpawnChance = 0.3f;

    private Transform playerBase;
    private List<GameObject> enemyUnits = new List<GameObject>();
    public GameObject enemyUnitPrefab;
    private int currentWave = 0;
    private float nextWaveTime;
    private bool isSpawning = false;

    private void Start()
    {
        playerBase = GameObject.FindGameObjectWithTag("PlayerBase").transform;
        nextWaveTime = Time.time;
    }

    private void Update()
    {
        if (Time.time > nextWaveTime && !isSpawning)
        {
            StartCoroutine(SpawnWave());
            nextWaveTime = Time.time + waveCooldown;
        }
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;
        currentWave++;
        int unitsToSpawn = startUnitsPerWave + currentWave;

        Debug.Log($"Начало волны {currentWave}. Будет создано {unitsToSpawn} юнитов");

        for (int i = 0; i < unitsToSpawn; i++)
        {
            SpawnUnit();
            yield return new WaitForSeconds(startSpawnInterval);
        }

        isSpawning = false;
    }

    private void SpawnUnit()
    {
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        spawnPosition.y = 0f;

        GameObject unitPrefab = ShouldSpawnSecondType() ? secondEnemyTypePrefab : enemyUnitPrefab;
        GameObject newUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        enemyUnits.Add(newUnit);

        // Автоматическая атака при достаточном количестве
        if (enemyUnits.Count >= startUnitsPerWave + currentWave)
        {
            AttackPlayerBase();
        }
    }

    private bool ShouldSpawnSecondType()
    {
        return currentWave >= secondTypeStartWave &&
               Random.value < secondTypeSpawnChance;
    }

    private void AttackPlayerBase()
    {
        foreach (var unit in enemyUnits)
        {
            if (unit != null)
            {
                NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.SetDestination(playerBase.position);
                    agent.speed = moveSpeed;
                }
            }
        }
        enemyUnits.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}