using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float wanderRadius = 5f;
    public float foundingProbability = 0.01f; // 나라를 세울 확률
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

            // 조건을 만족하면 나라를 세움
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
        // 예시: 주변에 다른 나라가 없는지 확인
        Collider2D[] nearbyTerritories = Physics2D.OverlapCircleAll(transform.position, 5f, territoryLayer);
        return nearbyTerritories.Length == 0;
    }

    void FoundNation()
    {
        // 나라 생성 로직
        hasFoundedNation = true;
        Debug.Log("나라 생성!");
        // 여기에 나라 오브젝트를 생성하고, 영토를 설정하는 로직을 추가합니다.
    }
}
