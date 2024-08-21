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

    private FarmingSystem farmingSystem; // ���� �ý��� ����
    private Vector2 nationCenter; // ������ �߽� ��ġ
    private float nationRadius = 10f; // ������ �ݰ�

    void Start()
    {
        farmingSystem = FindObjectOfType<FarmingSystem>(); // FarmingSystem�� ã�Ƽ� ����
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

            // ������ ������ ���� ����
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
            // ���� ���� �Ŀ��� ��� �۾� ����
            PerformPostNationTasks();
        }
    }

    void SetNewRandomTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = nationCenter + randomDirection; // ������ �߽��� �������� ���ο� ��ġ�� ����

        // ���� �������� �̵��ϵ��� ��ġ ����
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
                return false; // ��ȿ���� ���� ��ġ
            }
        }

        return true; // ��ȿ�� ��ġ
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
        Debug.Log("���� �������ϴ�!");
    }

    // ���� ���� �� �߰� �۾��� ����
    void PerformPostNationTasks()
    {
        int randomTask = Random.Range(0, 3); // 0: �ǹ� ����, 1: ��� ����, 2: �ܼ��� ���ƴٴϱ�

        switch (randomTask)
        {
            case 0:
                BuildStructure(); // �ǹ� ���� �۾�
                break;
            case 1:
                CultivateLand(); // ��� �۾�
                break;
            case 2:
                SetNewRandomTarget(); // �ܼ��� ���ƴٴϱ�
                break;
        }
    }

    void BuildStructure()
    {
        Vector3 buildPosition = transform.position + (Vector3)(Random.insideUnitCircle * 2f);
        // �ǹ� �������� Instantiate�Ͽ� �ش� ��ġ�� ���� (�ǹ� �������� ���� �ʿ�)
        Debug.Log("�ǹ� ���� �۾� ����!");
    }

    void CultivateLand()
    {
        Vector3Int tilePosition = farmingSystem.groundTilemap.WorldToCell(transform.position);
        farmingSystem.CultivateLand(tilePosition); // ���� �۾� ����
        Debug.Log("��� �۾� ����!");
        SetNewRandomTarget(); // ��� �۾� �� ���ο� ��ġ�� �̵�
    }
}
