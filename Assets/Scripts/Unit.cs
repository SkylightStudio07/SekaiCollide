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

    private float actionInterval = 7f; // 행동 간 간격 (초)
    private float actionTimer = 0f; // 다음 행동까지의 타이머

    private bool isAtTargetPosition = false; // 유닛이 타겟 위치에 도착했는지 여부
    private bool isPerformingTask = false; // 유닛이 작업 중인지 여부

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
                    FoundNation(); // 국가 수립
                }
            }
        }
        else
        {
            // 유닛이 작업 중이 아닐 때만 행동
            if (!isPerformingTask)
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
        }

        // 유닛이 작업 중이 아닐 때만 타겟 위치로 이동
        if (!isPerformingTask)
        {
            MoveToTarget();
        }
    }

    IEnumerator PerformPostNationTasks()
    {
        isPerformingTask = true; // 작업 중으로 표시


        int randomTask = Random.Range(0, 2);

        switch (randomTask)
        {
            case 0:
                Debug.Log("유닛 휴식중...");
                yield return StartCoroutine(Rest());
                break;
            case 1:
                Debug.Log("유닛 농사 로직 수행 시작...");
                yield return StartCoroutine(CultivateLand());
                break;
        }


        // 경작 작업을 수행
        // yield return StartCoroutine(CultivateLand());

        // 경작 작업이 완료된 후 새로운 타겟 위치 설정
        Debug.Log("새 타겟 위치 설정중...");
        SetNewRandomTarget();

        isPerformingTask = false; // 작업 완료
    }

    IEnumerator Rest()
    {
        yield return new WaitForSeconds(3.0f);
    }

    IEnumerator CultivateLand()
    {
        if (farmingSystem == null)
        {
            Debug.LogError("FarmingSystem이 설정되지 않았습니다.");
            yield break;
        }

        // 유닛의 현재 위치를 타일맵 좌표로 변환
        Vector3Int tilePosition = farmingSystem.groundTilemap.WorldToCell(transform.position);

        if (farmingSystem.CanCultivate(tilePosition))
        {
            // 경작 작업 수행
            Debug.Log("경작 작업 시작...");
            farmingSystem.CultivateLand(tilePosition);

            // 경작 작업이 완료될 때까지 대기
            yield return new WaitUntil(() => farmingSystem.IsCultivationComplete(tilePosition));

            Debug.Log($"농사 작업 수행 중!: {farmingSystem.groundTilemap.CellToWorld(tilePosition)}");
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
        isAtTargetPosition = false; 
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

    bool CanFoundNation()
    {
        // 유닛이 나라를 세울 수 있는 조건을 정의 (예: 특정 위치, 자원 조건 등)
        return true; // 조건을 임의로 true로 설정
    }

    void FoundNation()
    {
        NationData nationData = NationLoader.Instance.GetRandomNationData();

        if (nationData != null)
        {
            Nation newNation = new Nation(nationData.Name, nationData.GovernmentType, nationData.CultureGroup);
            NationManager.Instance.AddNation(newNation);

            // 국가 오버레이 프리팹 생성
            if (nationOverlayPrefab != null)
            {
                Instantiate(nationOverlayPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogError("NationOverlayPrefab이 설정되지 않았습니다.");
            }

            Debug.Log($"국가 생성: {newNation.nationName}");
            hasFoundedNation = true;
        }
        else
        {
            Debug.LogError("국가 데이터를 생성하는데 실패했습니다.");
        }
    }
}
