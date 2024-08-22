using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float wanderRadius = 5f;
    public float foundingProbability = 0.01f; // 나라를 세울 확률
    public LayerMask territoryLayer;

    public GameObject nationOverlayPrefab;
    [SerializeField] private bool hasFoundedNation = false;
    private Vector2 targetPosition;
    private Vector2 nationCenter; // 나라의 중심
    private float nationRadius = 10f; // 나라의 반경
    public float checkRadius = 5f; // 나라의 겹침 여부를 확인하는 반경

    private FarmingSystem farmingSystem;
    private CountryNameLoader countryNameLoader; // 나라 이름 로더

    [Header("Cultural Information")]
    [SerializeField] private string majorCulture; // 대문화권
    [SerializeField] private string subCulture; // 소문화권
    [SerializeField] private string microCulture; // 마이크로 문화권

    [Header("Nation Info")]
    [SerializeField] private string nationName; // 유닛이 세운 나라의 이름

    private float actionInterval = 3f; // 행동 간 간격 (초)
    private float actionTimer = 0f; // 다음 행동까지의 타이머

    private bool isAtTargetPosition = false; // 유닛이 타겟 위치에 도착했는지 여부

    void Start()
    {
        // FarmingSystem 인스턴스 찾기
        farmingSystem = FindObjectOfType<FarmingSystem>();

        // CountryNameLoader 인스턴스 찾기
        countryNameLoader = FindObjectOfType<CountryNameLoader>();

        if (farmingSystem == null)
        {
            Debug.LogError("FarmingSystem을 찾을 수 없습니다! 게임 매니저에 FarmingSystem이 추가되어 있는지 확인하세요.");
            return;
        }

        if (countryNameLoader == null)
        {
            Debug.LogError("CountryNameLoader를 찾을 수 없습니다! 게임 매니저에 CountryNameLoader가 추가되어 있는지 확인하세요.");
            return;
        }

        // 유닛의 문화권 설정 (예시로 Western, Germanic, Bavarian 설정)
        majorCulture = "Western";
        subCulture = "Germanic";
        microCulture = "Bavarian";

        SetNewRandomTarget();
    }

    void Update()
    {
        if (!hasFoundedNation)
        {
            // 나라를 세울 조건을 지속적으로 확인
            if (Random.Range(0f, 1f) < foundingProbability && IsValidTerritory())
            {
                if (IsSpaceAvailable())
                {
                    FoundNation();
                }
            }
        }
        else
        {
            // 행동 타이머 업데이트
            actionTimer -= Time.deltaTime;

            // 유닛이 타겟 위치에 도착하면 경작 작업을 수행
            if (isAtTargetPosition && actionTimer <= 0f)
            {
                StartCoroutine(PerformPostNationTasks()); // Coroutine을 통해 경작 작업 수행
                actionTimer = actionInterval; // 다음 행동을 위한 타이머 초기화
            }
        }

        // 타겟 위치로 이동
        MoveToTarget();
    }

    IEnumerator PerformPostNationTasks()
    {
        // 도착한 후 잠시 대기
        yield return new WaitForSeconds(0.5f); // 0.5초 대기

        // 경작 작업을 수행
        CultivateLand();

        // 경작 작업이 끝난 후 새로운 타겟 위치 설정
        SetNewRandomTarget();
    }

    void CultivateLand()
    {
        if (farmingSystem == null)
        {
            Debug.LogError("FarmingSystem이 설정되지 않았습니다.");
            return;
        }

        // targetPosition을 타일맵 좌표로 변환
        Vector3Int tilePosition = farmingSystem.groundTilemap.WorldToCell(targetPosition);

        if (farmingSystem.CanCultivate(tilePosition))
        {
            // 경작 작업 수행
            farmingSystem.CultivateLand(tilePosition);
            Debug.Log($"농사 작업 수행 중... 위치: {farmingSystem.groundTilemap.CellToWorld(tilePosition)}");
        }
        else
        {
            Debug.Log("이 위치는 경작할 수 없습니다.");
        }
    }

    void MoveToTarget()
    {
        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            isAtTargetPosition = false; // 아직 도착하지 않았음을 표시
        }
        else
        {
            isAtTargetPosition = true; // 타겟 위치에 도착했음을 표시
        }
    }

    void SetNewRandomTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = nationCenter + randomDirection;

        // 나라 내에서만 이동하도록 위치 제한
        if (Vector2.Distance(nationCenter, targetPosition) > nationRadius)
        {
            targetPosition = nationCenter + (targetPosition - nationCenter).normalized * nationRadius;
        }

        Debug.Log($"새로운 타겟 위치 설정: {targetPosition}");
        isAtTargetPosition = false; // 새로운 타겟을 설정한 후, 아직 도착하지 않았음을 표시
    }

    bool IsValidTerritory()
    {
        Collider2D[] nearbyTerritories = Physics2D.OverlapCircleAll(transform.position, checkRadius, territoryLayer);
        return nearbyTerritories.Length == 0;
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

        // 나라 이름 설정
        if (countryNameLoader != null)
        {
            nationName = countryNameLoader.GetRandomCountryName(microCulture); // 유닛의 마이크로 문화권 기반으로 나라 이름 설정
            Debug.Log($"나라 이름: {nationName}");
        }
        else
        {
            nationName = "Unknown";
            Debug.LogError("CountryNameLoader를 찾지 못해 기본 이름을 사용합니다.");
        }

        // 나라를 세운 후 새로운 타겟 설정
        SetNewRandomTarget();
    }
}
