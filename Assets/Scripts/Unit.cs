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

    private float actionInterval = 7f; // �ൿ �� ���� (��)
    private float actionTimer = 0f; // ���� �ൿ������ Ÿ�̸�

    private bool isAtTargetPosition = false; // ������ Ÿ�� ��ġ�� �����ߴ��� ����
    private bool isPerformingTask = false; // ������ �۾� ������ ����

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
                    FoundNation(); // ���� ����
                }
            }
        }
        else
        {
            // ������ �۾� ���� �ƴ� ���� �ൿ
            if (!isPerformingTask)
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
        }

        // ������ �۾� ���� �ƴ� ���� Ÿ�� ��ġ�� �̵�
        if (!isPerformingTask)
        {
            MoveToTarget();
        }
    }

    IEnumerator PerformPostNationTasks()
    {
        isPerformingTask = true; // �۾� ������ ǥ��


        int randomTask = Random.Range(0, 2);

        switch (randomTask)
        {
            case 0:
                Debug.Log("���� �޽���...");
                yield return StartCoroutine(Rest());
                break;
            case 1:
                Debug.Log("���� ��� ���� ���� ����...");
                yield return StartCoroutine(CultivateLand());
                break;
        }


        // ���� �۾��� ����
        // yield return StartCoroutine(CultivateLand());

        // ���� �۾��� �Ϸ�� �� ���ο� Ÿ�� ��ġ ����
        Debug.Log("�� Ÿ�� ��ġ ������...");
        SetNewRandomTarget();

        isPerformingTask = false; // �۾� �Ϸ�
    }

    IEnumerator Rest()
    {
        yield return new WaitForSeconds(3.0f);
    }

    IEnumerator CultivateLand()
    {
        if (farmingSystem == null)
        {
            Debug.LogError("FarmingSystem�� �������� �ʾҽ��ϴ�.");
            yield break;
        }

        // ������ ���� ��ġ�� Ÿ�ϸ� ��ǥ�� ��ȯ
        Vector3Int tilePosition = farmingSystem.groundTilemap.WorldToCell(transform.position);

        if (farmingSystem.CanCultivate(tilePosition))
        {
            // ���� �۾� ����
            Debug.Log("���� �۾� ����...");
            farmingSystem.CultivateLand(tilePosition);

            // ���� �۾��� �Ϸ�� ������ ���
            yield return new WaitUntil(() => farmingSystem.IsCultivationComplete(tilePosition));

            Debug.Log($"��� �۾� ���� ��!: {farmingSystem.groundTilemap.CellToWorld(tilePosition)}");
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
        // ������ ���� ���� �� �ִ� ������ ���� (��: Ư�� ��ġ, �ڿ� ���� ��)
        return true; // ������ ���Ƿ� true�� ����
    }

    void FoundNation()
    {
        NationData nationData = NationLoader.Instance.GetRandomNationData();

        if (nationData != null)
        {
            Nation newNation = new Nation(nationData.Name, nationData.GovernmentType, nationData.CultureGroup);
            NationManager.Instance.AddNation(newNation);

            // ���� �������� ������ ����
            if (nationOverlayPrefab != null)
            {
                Instantiate(nationOverlayPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogError("NationOverlayPrefab�� �������� �ʾҽ��ϴ�.");
            }

            Debug.Log($"���� ����: {newNation.nationName}");
            hasFoundedNation = true;
        }
        else
        {
            Debug.LogError("���� �����͸� �����ϴµ� �����߽��ϴ�.");
        }
    }
}
