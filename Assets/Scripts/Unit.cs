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
    private float maxSearchTime = 7f; // 7�� ���� �ڸ��� �� ã���� �շ� �õ�
    public float checkRadius = 10f; // ���� ��ġ���� Ȯ���� �ݰ�

    public string UnitName;
    [SerializeField] private CulturalGroup UnitCulture; // ������ ��ȭ�� ����, �����Ϳ��� Ȯ�� ����
    private NationLoader nationLoader;
    private CountryNameLoader countryNameLoader;
    private GovernmentTypeLoader governmentTypeLoader;

    [SerializeField] private string countryName; // ������ ���� ���� �̸� (�����Ϳ��� Ȯ�� ����)
    private string governmentType = "Republic"; // �⺻ ���� ü�� ����

    void Start()
    {
        nationLoader = FindObjectOfType<NationLoader>();
        countryNameLoader = FindObjectOfType<CountryNameLoader>();
        governmentTypeLoader = FindObjectOfType<GovernmentTypeLoader>();

        if (nationLoader == null || countryNameLoader == null || governmentTypeLoader == null)
        {
            Debug.LogError("�ʼ� �δ��� ã�� �� �����ϴ�. GameManager�� ��ũ��Ʈ�� �ִ��� Ȯ���ϼ���.");
            return;
        }

        // ������ �������� ����
        Race selectedRace = nationLoader.nationsData.Races[Random.Range(0, nationLoader.nationsData.Races.Count)];

        // ���õ� ������ ��ȭ�� �� �ϳ��� �������� ����
        UnitCulture = selectedRace.CulturalSphere[Random.Range(0, selectedRace.CulturalSphere.Count)];

        // XML���� ��ȭ�ǿ� ���� ���� �̸� ��������
        string baseCountryName = GetCountryNameFromCulture(UnitCulture.MicroCulture);

        // ���� ü���� �������� ����
        governmentType = GetRandomGovernmentType();

        // XML���� ���� Ÿ���� ������ ������ ���λ�� ���̻縦 ����
        GovernmentType selectedGovernment = governmentTypeLoader.GetGovernmentType(governmentType);
        if (selectedGovernment != null)
        {
            string prefix = governmentTypeLoader.GetRandomPrefix(selectedGovernment);
            countryName = $"{prefix} {baseCountryName} {selectedGovernment.Suffix}".Trim();
        }
        else
        {
            countryName = baseCountryName; // ���� Ÿ���� ã�� ���� ��� �⺻ �̸� ���
        }

        Debug.Log($"���� �̸�: {UnitName}, ����: {countryName}");

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

            // 7�ʰ� ������ ���� ������ ���ϸ� �շ� �õ�
            if (searchTime >= maxSearchTime && !hasFoundedNation)
            {
                DecideNextAction();
                return;
            }

            // ������ �����ϸ� ���� ����
            if (Random.Range(0f, 1f) < foundingProbability && IsValidTerritory())
            {
                if (IsSpaceAvailable()) // ������ ��� �ִ��� Ȯ��
                {
                    FoundNation();
                }
                else
                {
                    Debug.Log("�浹 ����: �ٸ� ���� �̹� �����մϴ�.");
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
                return false; // ��ȿ���� ���� ��ġ
            }
        }

        return true; // ��ȿ�� ��ġ
    }

    bool IsSpaceAvailable()
    {
        // �ش� ��ġ �ֺ��� �ٸ� ���� �ִ��� Ȯ��
        Collider2D[] nearbyNations = Physics2D.OverlapCircleAll(transform.position, checkRadius, territoryLayer);
        return nearbyNations.Length == 0;
    }

    void FoundNation()
    {
        hasFoundedNation = true;
        Instantiate(nationOverlayPrefab, transform.position, Quaternion.identity);
        Debug.Log($"���� '{countryName}'�� �����Ǿ����ϴ�!"); // ���� �̸��� �ֿܼ� ���
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius); // checkRadius�� �ð�ȭ
    }

    void DecideNextAction()
    {
        // 7�ʰ� ������ ���� ������ ���ϸ� 50% Ȯ���� �շ� �Ǵ� ���ο� ��ġ Ž��
        if (Random.value <= 0.5f)
        {
            // 50% Ȯ���� ����� ������ �շ� �õ�
            TryToJoinNearbyNation();
        }
        else
        {
            // 50% Ȯ���� ���ο� ��ġ���� ���� ����� �õ�
            SetNewRandomTarget();
            searchTime = 0f; // Ÿ�̸� �ʱ�ȭ
        }
    }

    string GetCountryNameFromCulture(string cultureName)
    {
        if (countryNameLoader == null || countryNameLoader.countryNameData == null)
        {
            Debug.LogError("CountryNameLoader�� �ʱ�ȭ���� �ʾҽ��ϴ�.");
            return "Unknown";
        }

        return countryNameLoader.GetRandomCountryName(cultureName);
    }

    void TryToJoinNearbyNation()
    {
        // �ֺ��� �ִ� ���� Ž��
        float searchRadius = 10f;
        Collider2D[] nearbyNations = Physics2D.OverlapCircleAll(transform.position, searchRadius, territoryLayer);

        if (nearbyNations.Length > 0)
        {
            // ���� ����� ���� ����
            Collider2D closestNation = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider2D nation in nearbyNations)
            {
                NationOverlay nationScript = nation.GetComponent<NationOverlay>();
                if (nationScript != null && nationScript.race == UnitCulture.MajorCulture) // ���� üũ
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
                // ������ ���� �շ�
                Debug.Log("����� ���� �շ��߽��ϴ�: " + closestNation.name);
                // �̰����� ������ �����ͳ� ���� ������ ������Ʈ���ָ� �˴ϴ�.
                // ���� ��Ȱ��ȭ �Ǵ� �ش� ���� ���ӽ�Ŵ
                Destroy(gameObject); // �Ǵ� �ٸ� ���� ������ ��Ȱ��ȭ�ϰų� �շ���ų �� ����
            }
            else
            {
                // ���� ���� ������ ������ ���ο� ��ġ�� �̵�
                SetNewRandomTarget();
                searchTime = 0f; // Ÿ�̸� �ʱ�ȭ
            }
        }
        else
        {
            // ���� �ֺ��� ���� ������ �׳� ���� ����
            FoundNation();
        }
    }

    string GetRandomGovernmentType()
    {
        // ����: ���� ü���� ����Ʈ�� �����ϰ� �������� ����
        List<string> governmentTypes = new List<string> { "Republic", "Kingdom", "Empire", "Principality" };
        return governmentTypes[Random.Range(0, governmentTypes.Count)];
    }
}
