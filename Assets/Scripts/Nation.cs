using UnityEngine;

[System.Serializable]
public class Nation
{
    public string nationName; // 국가 이름
    public string governmentType; // 국가 체제
    public int population; // 인구 수
    public float food; // 식량
    public string cultureGroup; // 문화권
    public float wood; // 목재
    public float ore; // 광석

    public Nation(string name, string governmentType, string cultureGroup)
    {
        this.nationName = name;
        this.governmentType = governmentType;
        this.cultureGroup = cultureGroup;

        // 초기 자원 값 설정
        this.population = 100;
        this.food = 500f;
        this.wood = 300f;
        this.ore = 200f;
    }

    // 추가적인 기능들...
}
