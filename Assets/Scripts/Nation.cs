using UnityEngine;

[System.Serializable]
public class Nation
{
    public string nationName;
    public string governmentType;

    public string majorCulture;
    public string subCulture;
    public string microCulture;

    public float food;
    public float timber;
    public float POP;

    public GameObject nationOverlay;    

    public Nation(string name, string governmentType, string majorCulture, string subCulture, string microCulture)
    {
        this.nationName = name;
        this.governmentType = governmentType;
        this.majorCulture = majorCulture;
        this.subCulture = subCulture;
        this.microCulture = microCulture;

        this.food = 10f;  // 시작 식량 (예: 10 단위)
        this.timber = 5f; // 시작 목재 (예: 5 단위)
        this.POP = 1f;    // 유로파의 인력 내지는 스텔라리스의 팝을 생각해야 한다.
    }

    public void ManagePopulationAndFood()
    {
        // 인구가 소비하는 식량 계산
        float foodConsumed = POP * 2f;

        // 남은 식량 계산
        food -= foodConsumed;

        if (food < 0)
        {
            // 식량이 부족할 경우 인구 감소... 라는 로직을 구현해야 한다. 지금 건들 일은 아님.
        }
        else
        {
            // 식량이 충분할 경우 점진적으로 인구가 증가하는 로직이 필요.
        }
    }

    public void AddFood(float amount)
    {
        food += amount;
    }

}
