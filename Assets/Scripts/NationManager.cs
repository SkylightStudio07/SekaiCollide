using System.Collections.Generic;
using UnityEngine;

public class NationManager : MonoBehaviour
{
    public static NationManager Instance { get; private set; } // �̱��� ����

    [SerializeField] private List<Nation> nations = new List<Nation>(); // ��� ������ ����Ʈ

    void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ���� �߰�
    public void AddNation(Nation newNation)
    {
        nations.Add(newNation);
    }

    // ��� ������ ��ȯ
    public List<Nation> GetAllNations()
    {
        return nations;
    }

    // Ư�� ������ �̸����� ã��
    public Nation GetNationByName(string nationName)
    {
        return nations.Find(nation => nation.nationName == nationName);
    }

    // ������ ���¸� ������Ʈ
    public void UpdateNationStates()
    {
        foreach (Nation nation in nations)
        {
            // �� ������ �ڿ� ���� �� �Һ� ����
            // nation.ProduceFood(10f); // ��: �� ������Ʈ���� �ķ� 10��ŭ ����
            // nation.ConsumeFood(5f); // ��: �� ������Ʈ���� �ķ� 5��ŭ �Һ�
        }
    }

    void Update()
    {
        // ���� �������� ���� ���¸� ������Ʈ
        UpdateNationStates();
    }
}
