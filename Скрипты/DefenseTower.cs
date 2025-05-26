using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DefenseTower : MonoBehaviour
{
    [Header("��������� �����")]
    [SerializeField] private float attackRange = 10f; // ������ ����������� ������
    [SerializeField] private float attackRate = 1f;   // �������� ����� (��������� � �������)
    [SerializeField] private int damage = 10;        // ���� �� �������

    [Header("���������� �������")]
    [SerializeField] private GameObject projectilePrefab; // ������ �������
    [SerializeField] private Transform firePoint;         // ����� ��������

    private float nextAttackTime;
    private Transform currentTarget;

    private void Update()
    {
        // ����� ����, ���� ��� ������� ��� ���� ���� �� �������
        if (currentTarget == null || !currentTarget.CompareTag("Enemy") ||
            Vector3.Distance(transform.position, currentTarget.position) > attackRange)
        {
            FindTarget();
        }

        // �����, ���� ���� ���� � ������ � ���������� ��������
        if (currentTarget != null && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    private void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        currentTarget = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < attackRange && distance < closestDistance)
            {
                closestDistance = distance;
                currentTarget = enemy.transform;
            }
        }
    }

    private void Attack()
    {
       

        // �������� �������
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(currentTarget, damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}