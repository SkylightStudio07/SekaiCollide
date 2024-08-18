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
    [SerializeField] private bool hasFoundedNation = false;
    private Vector2 targetPosition;
    private float searchTime = 0f;
    private float maxSearchTime = 7f; // 7�� ���� �ڸ��� �� ã���� �շ� �õ�
    public float checkRadius = 10f; // ���� ��ġ���� Ȯ���� �ݰ�

    public string UnitName;
    public CulturalGroup UnitCulture; // ������ ��ȭ�� ����
    private NationLoader nationLoader; // NationLoader ��ũ��Ʈ�� ����

    private bool isAttemptingToFoundNation = false;

    void Start()
    {
        // ���� ������ NationLoader�� �ڵ����� ã��
        nationLoader = FindObjectOfType<NationLoader>();

        if (nationLoader == null)
        {
            Debug.LogError("NationLoader�� ã�� �� �����ϴ�. GameManager�� NationLoader ��ũ��Ʈ�� �ִ��� Ȯ���ϼ���.");
            return;
        }

        // �ӽ÷� Ư�� ������ ù ��° ��ȭ���� ������ (�׽�Ʈ ����)
        Race selectedRace = nationLoader.nationsData.Races[0]; // ù ��° ���� (��: Human)
        UnitCulture = selectedRace.CulturalSphere[0]; // ù ��° ��ȭ�� (��: Western - Germanic - Bavarian)

        // ���� ���� ���
        Debug.Log($"���� �̸�: {UnitName}");
        Debug.Log($"��ȭ��: {UnitCulture.MajorCulture} - {UnitCulture.SubCulture} - {UnitCulture.MicroCulture}");

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
        Debug.Log("���� ����!");
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
}
