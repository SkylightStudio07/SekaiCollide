using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region * 유닛 프로퍼티

    public float moveSpeed = 2f; // 이동 속도
    public float wanderRadius = 5f; // 돌아다니는 반경
    public float foundingProbability = 0.01f; // 나라를 세울 확률

    public GameObject nationOverlayPrefab; // 국가 오버레이 프리팹
    [SerializeField] private bool hasFoundedNation = false; // 개국 여부

    private Vector2 targetPosition; // 유닛이 목표로 하는 위치

    private float actionInterval = 7f; // 행동 간 간격 (초)
    private float actionTimer = 0f; // 다음 행동까지의 타이머

    private bool isAtTargetPosition = false; // 유닛이 타겟 위치에 도착했는지 여부
    private bool isPerformingTask = false; // 유닛이 작업 중인지 여부

    #endregion

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
    private Nation assignedNation; // 유닛 국적

    public LayerMask territoryLayer;


    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Unit");
        // FarmingSystem 인스턴스 찾기(게임매니저)
        farmingSystem = FindObjectOfType<FarmingSystem>();

        // CountryNameLoader 인스턴스 찾기(게임매니저)
        countryNameLoader = FindObjectOfType<CountryNameLoader>();

        if (farmingSystem == null)
        {
            Debug.LogError("FarmingSystem Undefined");
            return;
        }

        if (countryNameLoader == null)
        {
            Debug.LogError("CountryNameLoader Undefined");
            return;
        }

        // 유닛의 문화권 설정 (예시로 Western, Germanic, Bavarian 설정)
        NationData randomNationData = NationLoader.Instance.GetRandomNationData();
        if (randomNationData != null)
        {
            // CulturalGroup의 개별 속성에 접근
            majorCulture = randomNationData.cultureGroup.MajorCulture;
            subCulture = randomNationData.cultureGroup.SubCulture;
            microCulture = randomNationData.cultureGroup.MicroCulture;
        }
        else
        {
            Debug.LogError("임의의 문화권 데이터를 가져오는데 실패했습니다.");
        }

        SetNewRandomTarget(); // 다음 타겟 찾기 시작
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
                    StartCoroutine(PerformPostNationTasks()); 
                    actionTimer = actionInterval; // 타이머 초기화
                }
            }
        }

        // 작업 중이 아닐 때 타겟 위치로 이동
        if (!isPerformingTask)
        {
            MoveToTarget();
        }
    }

    IEnumerator PerformPostNationTasks()
    {
        isPerformingTask = true; // 유닛은 현재 작업 중

        // 여기서 사냥을 할 수도 있고... 건물을 지을 수도 있고.
        // 그런데 이 경우에 외적이 침입해오면 어떻게 해야 하나? 게임 로직을 좀 더 생각해봐야 한다.
        // 월드박스와 다른 노선을 탈 수도 있는 거고, 혹은 유닛의 행동에 가중치를 부여할 수도 있는 거고.
        // 이때부터는 골치아파지는데..


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
            Debug.LogError("FarmingSystem Undefined");
            yield break;
        }

        // 유닛의 현재 위치를 타일맵 좌표
        Vector3Int tilePosition = farmingSystem.groundTilemap.WorldToCell(transform.position);

        if (farmingSystem.CanCultivate(tilePosition))
        {
            // 경작 작업 수행
            Debug.Log("경작 작업 시작...");
            farmingSystem.CultivateLand(tilePosition, assignedNation); // 국가에 반영

            // 경작 작업이 완료될 때까지 대기
            yield return new WaitForSeconds(3f);

            Debug.Log($"농사 작업 완료!: {farmingSystem.groundTilemap.CellToWorld(tilePosition)}");
        }
        else
        {
            Debug.Log("이 위치는 경작할 수 없습니다.");
        }
    }

    void MoveToTarget()
    {
        if (assignedNation != null && assignedNation.nationOverlay != null)
        {
            Vector2 overlayPosition = (Vector2)assignedNation.nationOverlay.transform.position;
            float overlayRadius = 8f; // 오버레이의 반경 설정 (필요에 따라 조정). 10f로 하면 밖으로 나간다

            if (Vector2.Distance(overlayPosition, targetPosition) > overlayRadius)
            {
                targetPosition = overlayPosition + (targetPosition - overlayPosition).normalized * overlayRadius;
            }
        }

        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            isAtTargetPosition = false;
        }
        else
        {
            isAtTargetPosition = true;
        }
    }

    void SetNewRandomTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = nationCenter + randomDirection;

        // 나라 내에서만 이동하도록..
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
        // 유닛이 나라를 세울 수 있는 조건을 정의하자 (예: 특정 위치, 자원 조건 등).. 쉽지 않구만.
        return true; // 조건을 임의로 true로 설정
    }

    void FoundNation()
    {
        StopUnitMovement();

        NationData nationData = new NationData
        {
            Name = microCulture,
            GovernmentType = "Kingdom",
            cultureGroup = new CulturalGroup
            {
                MajorCulture = majorCulture,
                SubCulture = subCulture,
                MicroCulture = microCulture
            }
        };

        Nation newNation = new Nation(
            nationData.Name,
            nationData.GovernmentType,
            nationData.cultureGroup.MajorCulture,
            nationData.cultureGroup.SubCulture,
            nationData.cultureGroup.MicroCulture
        );

        NationManager.Instance.AddNation(newNation);

        assignedNation = newNation;

        if (nationOverlayPrefab != null)
        {
            GameObject overlayInstance = Instantiate(nationOverlayPrefab, transform.position, Quaternion.identity);
            overlayInstance.layer = LayerMask.NameToLayer("territoryLayer");
            newNation.nationOverlay = overlayInstance;
        }
        else
        {
            Debug.LogError("NationOverlayPrefab UNDEFINED");
        }

        Debug.Log($"국가 생성: {newNation.nationName}");
        hasFoundedNation = true;

        // 유닛 이동을 다시 재개
        ResumeUnitMovement();
    }

    void StopUnitMovement()
    {
        moveSpeed = 0f;
        isPerformingTask = true; // 유닛이 작업 중임을 나타냄
    }

    void ResumeUnitMovement()
    {
        moveSpeed = 2f; // 기본 속도로 설정
        isPerformingTask = false; // 유닛이 작업 중이 아님을 나타냄
    }

}
