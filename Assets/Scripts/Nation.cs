using UnityEngine;

[System.Serializable]
public class Nation
{
    public string nationName; // ���� �̸�
    public string governmentType; // ���� ü��
    public int population; // �α� ��
    public float food; // �ķ�
    public string cultureGroup; // ��ȭ��
    public float wood; // ����
    public float ore; // ����

    public Nation(string name, string governmentType, string cultureGroup)
    {
        this.nationName = name;
        this.governmentType = governmentType;
        this.cultureGroup = cultureGroup;

        // �ʱ� �ڿ� �� ����
        this.population = 100;
        this.food = 500f;
        this.wood = 300f;
        this.ore = 200f;
    }

    // �߰����� ��ɵ�...
}
