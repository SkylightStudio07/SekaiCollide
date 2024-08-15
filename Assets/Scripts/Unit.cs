using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float wanderRadius = 5f;
    public float foundingProbability = 0.01f; // ���� ���� Ȯ��
    public LayerMask territoryLayer;

    private Vector2 targetPosition;
    [SerializeField] private bool hasFoundedNation = false;

    void Update()
    {
        if (!hasFoundedNation)
        {
            MoveToTarget();

            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                SetNewRandomTarget();
            }

            // ������ �����ϸ� ���� ����
            if (Random.Range(0f, 1f) < foundingProbability && IsValidTerritory())
            {
                FoundNation();
            }
        }
    }

    void SetNewRandomTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = (Vector2)transform.position + randomDirection;
    }

    void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    bool IsValidTerritory()
    {
        // ����: �ֺ��� �ٸ� ���� ������ Ȯ��
        Collider2D[] nearbyTerritories = Physics2D.OverlapCircleAll(transform.position, 5f, territoryLayer);
        return nearbyTerritories.Length == 0;
    }

    void FoundNation()
    {
        // ���� ���� ����
        hasFoundedNation = true;
        Debug.Log("���� ����!");
        // ���⿡ ���� ������Ʈ�� �����ϰ�, ���並 �����ϴ� ������ �߰��մϴ�.
    }
}
