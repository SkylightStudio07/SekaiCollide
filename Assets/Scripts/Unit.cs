using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region * ���� ������Ƽ

    public float moveSpeed = 2f; // �̵� �ӵ�
    public float wanderRadius = 5f; // ���ƴٴϴ� �ݰ�
    public float foundingProbability = 0.01f; // ���� ���� Ȯ��

    public GameObject nationOverlayPrefab; // ���� �������� ������
    [SerializeField] private bool hasFoundedNation = false; // ���� ����

    private Vector2 targetPosition; // ������ ��ǥ�� �ϴ� ��ġ

    private float actionInterval = 7f; // �ൿ �� ���� (��)
    private float actionTimer = 0f; // ���� �ൿ������ Ÿ�̸�

    private bool isAtTargetPosition = false; // ������ Ÿ�� ��ġ�� �����ߴ��� ����
    private bool isPerformingTask = false; // ������ �۾� ������ ����

    #endregion

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
    private Nation assignedNation; // ���� ����

    public LayerMask territoryLayer;


    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Unit");
        // FarmingSystem �ν��Ͻ� ã��(���ӸŴ���)
        farmingSystem = FindObjectOfType<FarmingSystem>();

        // CountryNameLoader �ν��Ͻ� ã��(���ӸŴ���)
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

        // ������ ��ȭ�� ���� (���÷� Western, Germanic, Bavarian ����)
        NationData randomNationData = NationLoader.Instance.GetRandomNationData();
        if (randomNationData != null)
        {
            // CulturalGroup�� ���� �Ӽ��� ����
            majorCulture = randomNationData.cultureGroup.MajorCulture;
            subCulture = randomNationData.cultureGroup.SubCulture;
            microCulture = randomNationData.cultureGroup.MicroCulture;
        }
        else
        {
            Debug.LogError("������ ��ȭ�� �����͸� �������µ� �����߽��ϴ�.");
        }

        SetNewRandomTarget(); // ���� Ÿ�� ã�� ����
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
                    StartCoroutine(PerformPostNationTasks()); 
                    actionTimer = actionInterval; // Ÿ�̸� �ʱ�ȭ
                }
            }
        }

        // �۾� ���� �ƴ� �� Ÿ�� ��ġ�� �̵�
        if (!isPerformingTask)
        {
            MoveToTarget();
        }
    }

    IEnumerator PerformPostNationTasks()
    {
        isPerformingTask = true; // ������ ���� �۾� ��

        // ���⼭ ����� �� ���� �ְ�... �ǹ��� ���� ���� �ְ�.
        // �׷��� �� ��쿡 ������ ħ���ؿ��� ��� �ؾ� �ϳ�? ���� ������ �� �� �����غ��� �Ѵ�.
        // ����ڽ��� �ٸ� �뼱�� Ż ���� �ִ� �Ű�, Ȥ�� ������ �ൿ�� ����ġ�� �ο��� ���� �ִ� �Ű�.
        // �̶����ʹ� ��ġ�������µ�..


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
            Debug.LogError("FarmingSystem Undefined");
            yield break;
        }

        // ������ ���� ��ġ�� Ÿ�ϸ� ��ǥ
        Vector3Int tilePosition = farmingSystem.groundTilemap.WorldToCell(transform.position);

        if (farmingSystem.CanCultivate(tilePosition))
        {
            // ���� �۾� ����
            Debug.Log("���� �۾� ����...");
            farmingSystem.CultivateLand(tilePosition, assignedNation); // ������ �ݿ�

            // ���� �۾��� �Ϸ�� ������ ���
            yield return new WaitForSeconds(3f);

            Debug.Log($"��� �۾� �Ϸ�!: {farmingSystem.groundTilemap.CellToWorld(tilePosition)}");
        }
        else
        {
            Debug.Log("�� ��ġ�� ������ �� �����ϴ�.");
        }
    }

    void MoveToTarget()
    {
        if (assignedNation != null && assignedNation.nationOverlay != null)
        {
            Vector2 overlayPosition = (Vector2)assignedNation.nationOverlay.transform.position;
            float overlayRadius = 8f; // ���������� �ݰ� ���� (�ʿ信 ���� ����). 10f�� �ϸ� ������ ������

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

        // ���� �������� �̵��ϵ���..
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
        // ������ ���� ���� �� �ִ� ������ �������� (��: Ư�� ��ġ, �ڿ� ���� ��).. ���� �ʱ���.
        return true; // ������ ���Ƿ� true�� ����
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

        Debug.Log($"���� ����: {newNation.nationName}");
        hasFoundedNation = true;

        // ���� �̵��� �ٽ� �簳
        ResumeUnitMovement();
    }

    void StopUnitMovement()
    {
        moveSpeed = 0f;
        isPerformingTask = true; // ������ �۾� ������ ��Ÿ��
    }

    void ResumeUnitMovement()
    {
        moveSpeed = 2f; // �⺻ �ӵ��� ����
        isPerformingTask = false; // ������ �۾� ���� �ƴ��� ��Ÿ��
    }

}
