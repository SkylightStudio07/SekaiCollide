using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float wanderRadius = 5f;
    public float foundingProbability = 0.01f; // ���� ���� Ȯ��
    public LayerMask territoryLayer;

    public GameObject nationOverlayPrefab;
    [SerializeField] private bool hasFoundedNation = false;
    private Vector2 targetPosition;
    private Vector2 nationCenter; // ������ �߽�
    private float nationRadius = 10f; // ������ �ݰ�
    public float checkRadius = 5f; // ������ ��ħ ���θ� Ȯ���ϴ� �ݰ�

    private FarmingSystem farmingSystem;
    private CountryNameLoader countryNameLoader; // ���� �̸� �δ�

    [Header("Cultural Information")]
    [SerializeField] private string majorCulture; // �빮ȭ��
    [SerializeField] private string subCulture; // �ҹ�ȭ��
    [SerializeField] private string microCulture; // ����ũ�� ��ȭ��

    [Header("Nation Info")]
    [SerializeField] private string nationName; // ������ ���� ������ �̸�

    private float actionInterval = 3f; // �ൿ �� ���� (��)
    private float actionTimer = 0f; // ���� �ൿ������ Ÿ�̸�

    private bool isAtTargetPosition = false; // ������ Ÿ�� ��ġ�� �����ߴ��� ����

    void Start()
    {
        // FarmingSystem �ν��Ͻ� ã��
        farmingSystem = FindObjectOfType<FarmingSystem>();

        // CountryNameLoader �ν��Ͻ� ã��
        countryNameLoader = FindObjectOfType<CountryNameLoader>();

        if (farmingSystem == null)
        {
            Debug.LogError("FarmingSystem�� ã�� �� �����ϴ�! ���� �Ŵ����� FarmingSystem�� �߰��Ǿ� �ִ��� Ȯ���ϼ���.");
            return;
        }

        if (countryNameLoader == null)
        {
            Debug.LogError("CountryNameLoader�� ã�� �� �����ϴ�! ���� �Ŵ����� CountryNameLoader�� �߰��Ǿ� �ִ��� Ȯ���ϼ���.");
            return;
        }

        // ������ ��ȭ�� ���� (���÷� Western, Germanic, Bavarian ����)
        majorCulture = "Western";
        subCulture = "Germanic";
        microCulture = "Bavarian";

        SetNewRandomTarget();
    }

    void Update()
    {
        if (!hasFoundedNation)
        {
            // ���� ���� ������ ���������� Ȯ��
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
            // �ൿ Ÿ�̸� ������Ʈ
            actionTimer -= Time.deltaTime;

            // ������ Ÿ�� ��ġ�� �����ϸ� ���� �۾��� ����
            if (isAtTargetPosition && actionTimer <= 0f)
            {
                StartCoroutine(PerformPostNationTasks()); // Coroutine�� ���� ���� �۾� ����
                actionTimer = actionInterval; // ���� �ൿ�� ���� Ÿ�̸� �ʱ�ȭ
            }
        }

        // Ÿ�� ��ġ�� �̵�
        MoveToTarget();
    }

    IEnumerator PerformPostNationTasks()
    {
        // ������ �� ��� ���
        yield return new WaitForSeconds(0.5f); // 0.5�� ���

        // ���� �۾��� ����
        CultivateLand();

        // ���� �۾��� ���� �� ���ο� Ÿ�� ��ġ ����
        SetNewRandomTarget();
    }

    void CultivateLand()
    {
        if (farmingSystem == null)
        {
            Debug.LogError("FarmingSystem�� �������� �ʾҽ��ϴ�.");
            return;
        }

        // targetPosition�� Ÿ�ϸ� ��ǥ�� ��ȯ
        Vector3Int tilePosition = farmingSystem.groundTilemap.WorldToCell(targetPosition);

        if (farmingSystem.CanCultivate(tilePosition))
        {
            // ���� �۾� ����
            farmingSystem.CultivateLand(tilePosition);
            Debug.Log($"��� �۾� ���� ��... ��ġ: {farmingSystem.groundTilemap.CellToWorld(tilePosition)}");
        }
        else
        {
            Debug.Log("�� ��ġ�� ������ �� �����ϴ�.");
        }
    }

    void MoveToTarget()
    {
        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            isAtTargetPosition = false; // ���� �������� �ʾ����� ǥ��
        }
        else
        {
            isAtTargetPosition = true; // Ÿ�� ��ġ�� ���������� ǥ��
        }
    }

    void SetNewRandomTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = nationCenter + randomDirection;

        // ���� �������� �̵��ϵ��� ��ġ ����
        if (Vector2.Distance(nationCenter, targetPosition) > nationRadius)
        {
            targetPosition = nationCenter + (targetPosition - nationCenter).normalized * nationRadius;
        }

        Debug.Log($"���ο� Ÿ�� ��ġ ����: {targetPosition}");
        isAtTargetPosition = false; // ���ο� Ÿ���� ������ ��, ���� �������� �ʾ����� ǥ��
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
        nationCenter = transform.position; // ������ �߽��� ������ ���� ��ġ�� ����
        Instantiate(nationOverlayPrefab, transform.position, Quaternion.identity);

        // ���� �̸� ����
        if (countryNameLoader != null)
        {
            nationName = countryNameLoader.GetRandomCountryName(microCulture); // ������ ����ũ�� ��ȭ�� ������� ���� �̸� ����
            Debug.Log($"���� �̸�: {nationName}");
        }
        else
        {
            nationName = "Unknown";
            Debug.LogError("CountryNameLoader�� ã�� ���� �⺻ �̸��� ����մϴ�.");
        }

        // ���� ���� �� ���ο� Ÿ�� ����
        SetNewRandomTarget();
    }
}
