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

        this.food = 10f;  // ���� �ķ� (��: 10 ����)
        this.timber = 5f; // ���� ���� (��: 5 ����)
        this.POP = 1f;    // �������� �η� ������ ���ڶ󸮽��� ���� �����ؾ� �Ѵ�.
    }

    public void ManagePopulationAndFood()
    {
        // �α��� �Һ��ϴ� �ķ� ���
        float foodConsumed = POP * 2f;

        // ���� �ķ� ���
        food -= foodConsumed;

        if (food < 0)
        {
            // �ķ��� ������ ��� �α� ����... ��� ������ �����ؾ� �Ѵ�. ���� �ǵ� ���� �ƴ�.
        }
        else
        {
            // �ķ��� ����� ��� ���������� �α��� �����ϴ� ������ �ʿ�.
        }
    }

    public void AddFood(float amount)
    {
        food += amount;
    }

}
