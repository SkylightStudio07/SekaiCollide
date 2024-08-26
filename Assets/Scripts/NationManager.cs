using System.Collections.Generic;
using UnityEngine;

public class NationManager : MonoBehaviour
{
    public static NationManager Instance { get; private set; } // 싱글톤 패턴

    [SerializeField] private List<Nation> nations = new List<Nation>(); // 모든 국가의 리스트

    void Awake()
    {
        // 싱글톤 패턴 설정
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

    // 국가 추가
    public void AddNation(Nation newNation)
    {
        nations.Add(newNation);
    }

    // 모든 국가를 반환
    public List<Nation> GetAllNations()
    {
        return nations;
    }

    // 특정 국가를 이름으로 찾기
    public Nation GetNationByName(string nationName)
    {
        return nations.Find(nation => nation.nationName == nationName);
    }

    // 국가의 상태를 업데이트
    public void UpdateNationStates()
    {
        foreach (Nation nation in nations)
        {
            // 각 국가의 자원 생산 및 소비 로직
            // nation.ProduceFood(10f); // 예: 매 업데이트마다 식량 10만큼 생산
            // nation.ConsumeFood(5f); // 예: 매 업데이트마다 식량 5만큼 소비
        }
    }

    void Update()
    {
        // 게임 루프에서 국가 상태를 업데이트
        UpdateNationStates();
    }
}
