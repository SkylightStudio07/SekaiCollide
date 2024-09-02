using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System.Collections.Generic;

public class NationLoader : MonoBehaviour
{
    public static NationLoader Instance { get; private set; } // �̱��� ����

    public Nations nationsData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadNationsData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadNationsData()
    {
        string xmlFilePath = "Assets/Data/nations.xml";
        nationsData = LoadNations(xmlFilePath);

        if (nationsData == null || nationsData.Races.Count == 0)
        {
            Debug.LogError("XML �����Ͱ� �ε���� �ʾҰų�, �����Ͱ� �����ϴ�.");
        }
    }

    public Nations LoadNations(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"XML ���� ã�� ����: {path}");
            return null;
        }

        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Nations));
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return serializer.Deserialize(stream) as Nations;
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"����� ���� �߻�: {e.Message}");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"XML �Ľ� ���� ���� �߻�: {e.Message}");
            return null;
        }
    }

    // ������ �Ǳ��� �� ���Ǵ� �޼���: ���� �̸��� �����͸� ��ȯ
    public NationData GetRandomNationData()
    {
        if (nationsData == null || nationsData.Races.Count == 0)
        {
            return null;
        }

        // �������� ������ ��ȭ���� ����
        var randomRace = nationsData.Races[Random.Range(0, nationsData.Races.Count)];
        var randomCulture = randomRace.CulturalSphere[Random.Range(0, randomRace.CulturalSphere.Count)];

        // NationData�� �����ϰ� CulturalGroup�� ���Խ�Ŵ
        NationData newNation = new NationData
        {
            Name = randomCulture.MicroCulture,
            GovernmentType = "Kingdom", // ���� ���¸� ����
            cultureGroup = randomCulture // CulturalGroup ��ü�� NationData�� ����
        };

        return newNation;
    }

}
