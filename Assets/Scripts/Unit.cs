using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float wanderRadius = 5f;
    public LayerMask territoryLayer;
    public GameObject nationOverlayPrefab;

    [SerializeField] private bool hasFoundedNation = false;
    private Vector2 targetPosition;
    private float searchTime = 0f;
    private float maxSearchTime = 7f;
    public float checkRadius = 10f;

    private FarmingSystem farmingSystem; // 경작 시스템 참조
    private Vector2 nationCenter; // 나라의 중심 위치
    private float nationRadius = 10f; // 나라의 반경

    void Start()
    {
        farmingSystem = FindObjectOfType<FarmingSystem>(); // FarmingSystem을 찾아서 참조
        SetNewRandomTarget();
    }

    void Update()
    {
        if (!hasFoundedNation)
        {
            MoveToTarget();

            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                SetNewRandomTarget();
            }

            searchTime += Time.deltaTime;

            // 조건이 맞으면 나라를 세움
            if (Random.Range(0f, 1f) < 0.01f && IsValidTerritory())
            {
                if (IsSpaceAvailable())
                {
                    FoundNation();
                }
            }
        }
        else
        {
            // 나라를 세운 후에도 계속 작업 수행
            PerformPostNationTasks();
        }
    }

    void SetNewRandomTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = nationCenter + randomDirection; // 나라의 중심을 기준으로 새로운 위치를 설정

        // 나라 내에서만 이동하도록 위치 제한
        if (Vector2.Distance(nationCenter, targetPosition) > nationRadius)
        {
            targetPosition = nationCenter + (targetPosition - nationCenter).normalized * nationRadius;
        }
    }

    void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    bool IsValidTerritory()
    {
        float checkRadius = 5f;

        for (int i = 0; i < 10; i++)
        {
            Vector2 checkPosition = (Vector2)transform.position + Random.insideUnitCircle * checkRadius;
            Collider2D[] nearbyTerritories = Physics2D.OverlapCircleAll(checkPosition, checkRadius, territoryLayer);
            if (nearbyTerritories.Length > 0)
            {
                return false; // 유효하지 않은 위치
            }
        }

        return true; // 유효한 위치
    }

    bool IsSpaceAvailable()
    {
        Collider2D[] nearbyNations = Physics2D.OverlapCircleAll(transform.position, checkRadius, territoryLayer);
        return nearbyNations.Length == 0;
    }

    void FoundNation()
    {
        hasFoundedNation = true;
        nationCenter = transform.position; // 나라의 중심을 유닛의 현재 위치로 설정
        Instantiate(nationOverlayPrefab, transform.position, Quaternion.identity);
        Debug.Log("나라를 세웠습니다!");
    }

    // 나라를 세운 후 추가 작업을 수행
    void PerformPostNationTasks()
    {
        int randomTask = Random.Range(0, 3); // 0: 건물 짓기, 1: 농사 짓기, 2: 단순히 돌아다니기

        switch (randomTask)
        {
            case 0:
                BuildStructure(); // 건물 짓기 작업
                break;
            case 1:
                CultivateLand(); // 농사 작업
                break;
            case 2:
                SetNewRandomTarget(); // 단순히 돌아다니기
                break;
        }
    }

    void BuildStructure()
    {
        Vector3 buildPosition = transform.position + (Vector3)(Random.insideUnitCircle * 2f);
        // 건물 프리팹을 Instantiate하여 해당 위치에 생성 (건물 프리팹은 설정 필요)
        Debug.Log("건물 짓기 작업 수행!");
    }

    void CultivateLand()
    {
        Vector3Int tilePosition = farmingSystem.groundTilemap.WorldToCell(transform.position);
        farmingSystem.CultivateLand(tilePosition); // 경작 작업 시작
        Debug.Log("농사 작업 수행!");
        SetNewRandomTarget(); // 농사 작업 후 새로운 위치로 이동
    }
}
