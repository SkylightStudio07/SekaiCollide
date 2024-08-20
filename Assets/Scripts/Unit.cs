using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float wanderRadius = 5f;
    public float foundingProbability = 0.01f;
    public LayerMask territoryLayer;

    public GameObject nationOverlayPrefab;
    private bool hasFoundedNation = false;
    private Vector2 targetPosition;
    private float searchTime = 0f;
    private float maxSearchTime = 7f; // 7초 동안 자리를 못 찾으면 합류 시도
    public float checkRadius = 10f; // 나라가 겹치는지 확인할 반경

    public string UnitName;
    [SerializeField] private CulturalGroup UnitCulture; // 유닛의 문화권 정보, 에디터에서 확인 가능
    private NationLoader nationLoader;
    private CountryNameLoader countryNameLoader;
    private GovernmentTypeLoader governmentTypeLoader;

    [SerializeField] private string countryName; // 유닛이 속한 국가 이름 (에디터에서 확인 가능)
    private string governmentType = "Republic"; // 기본 정부 체제 예시

    void Start()
    {
        nationLoader = FindObjectOfType<NationLoader>();
        countryNameLoader = FindObjectOfType<CountryNameLoader>();
        governmentTypeLoader = FindObjectOfType<GovernmentTypeLoader>();

        if (nationLoader == null || countryNameLoader == null || governmentTypeLoader == null)
        {
            Debug.LogError("필수 로더를 찾을 수 없습니다. GameManager에 스크립트가 있는지 확인하세요.");
            return;
        }

        // 종족을 무작위로 선택
        Race selectedRace = nationLoader.nationsData.Races[Random.Range(0, nationLoader.nationsData.Races.Count)];

        // 선택된 종족의 문화권 중 하나를 무작위로 선택
        UnitCulture = selectedRace.CulturalSphere[Random.Range(0, selectedRace.CulturalSphere.Count)];

        // XML에서 문화권에 따른 나라 이름 가져오기
        string baseCountryName = GetCountryNameFromCulture(UnitCulture.MicroCulture);

        // 정부 체제를 무작위로 선택
        governmentType = GetRandomGovernmentType();

        // XML에서 정부 타입을 가져와 무작위 접두사와 접미사를 적용
        GovernmentType selectedGovernment = governmentTypeLoader.GetGovernmentType(governmentType);
        if (selectedGovernment != null)
        {
            string prefix = governmentTypeLoader.GetRandomPrefix(selectedGovernment);
            countryName = $"{prefix} {baseCountryName} {selectedGovernment.Suffix}".Trim();
        }
        else
        {
            countryName = baseCountryName; // 정부 타입을 찾지 못한 경우 기본 이름 사용
        }

        Debug.Log($"유닛 이름: {UnitName}, 국적: {countryName}");

        SetNewRandomTarget();
    }


    void Update()
    {
        if (!hasFoundedNation)
        {
            MoveToTarget();
            searchTime += Time.deltaTime;

            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                SetNewRandomTarget();
            }

            // 7초가 지나도 나라를 세우지 못하면 합류 시도
            if (searchTime >= maxSearchTime && !hasFoundedNation)
            {
                DecideNextAction();
                return;
            }

            // 조건을 만족하면 나라를 세움
            if (Random.Range(0f, 1f) < foundingProbability && IsValidTerritory())
            {
                if (IsSpaceAvailable()) // 공간이 비어 있는지 확인
                {
                    FoundNation();
                }
                else
                {
                    Debug.Log("충돌 감지: 다른 나라가 이미 존재합니다.");
                }
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
        // 해당 위치 주변에 다른 나라가 있는지 확인
        Collider2D[] nearbyNations = Physics2D.OverlapCircleAll(transform.position, checkRadius, territoryLayer);
        return nearbyNations.Length == 0;
    }

    void FoundNation()
    {
        hasFoundedNation = true;
        Instantiate(nationOverlayPrefab, transform.position, Quaternion.identity);
        Debug.Log($"나라 '{countryName}'가 생성되었습니다!"); // 나라 이름을 콘솔에 출력
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius); // checkRadius를 시각화
    }

    void DecideNextAction()
    {
        // 7초가 지나도 나라를 세우지 못하면 50% 확률로 합류 또는 새로운 위치 탐색
        if (Random.value <= 0.5f)
        {
            // 50% 확률로 가까운 국가에 합류 시도
            TryToJoinNearbyNation();
        }
        else
        {
            // 50% 확률로 새로운 위치에서 나라를 세우기 시도
            SetNewRandomTarget();
            searchTime = 0f; // 타이머 초기화
        }
    }

    string GetCountryNameFromCulture(string cultureName)
    {
        if (countryNameLoader == null || countryNameLoader.countryNameData == null)
        {
            Debug.LogError("CountryNameLoader가 초기화되지 않았습니다.");
            return "Unknown";
        }

        return countryNameLoader.GetRandomCountryName(cultureName);
    }

    void TryToJoinNearbyNation()
    {
        // 주변에 있는 나라를 탐색
        float searchRadius = 10f;
        Collider2D[] nearbyNations = Physics2D.OverlapCircleAll(transform.position, searchRadius, territoryLayer);

        if (nearbyNations.Length > 0)
        {
            // 가장 가까운 나라 선택
            Collider2D closestNation = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider2D nation in nearbyNations)
            {
                NationOverlay nationScript = nation.GetComponent<NationOverlay>();
                if (nationScript != null && nationScript.race == UnitCulture.MajorCulture) // 종족 체크
                {
                    float distance = Vector2.Distance(transform.position, nation.transform.position);
                    if (distance < closestDistance)
                    {
                        closestNation = nation;
                        closestDistance = distance;
                    }
                }
            }

            if (closestNation != null)
            {
                // 선택한 나라에 합류
                Debug.Log("가까운 나라에 합류했습니다: " + closestNation.name);
                // 이곳에서 나라의 데이터나 성장 로직을 업데이트해주면 됩니다.
                // 유닛 비활성화 또는 해당 나라에 종속시킴
                Destroy(gameObject); // 또는 다른 논리로 유닛을 비활성화하거나 합류시킬 수 있음
            }
            else
            {
                // 같은 종족 국가가 없으면 새로운 위치로 이동
                SetNewRandomTarget();
                searchTime = 0f; // 타이머 초기화
            }
        }
        else
        {
            // 만약 주변에 나라가 없으면 그냥 나라를 세움
            FoundNation();
        }
    }

    string GetRandomGovernmentType()
    {
        // 예시: 정부 체제를 리스트로 정의하고 무작위로 선택
        List<string> governmentTypes = new List<string> { "Republic", "Kingdom", "Empire", "Principality" };
        return governmentTypes[Random.Range(0, governmentTypes.Count)];
    }
}
