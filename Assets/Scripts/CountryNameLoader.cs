using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CountryNameLoader : MonoBehaviour
{
    public string xmlFilePath = "Assets/Data/CountryNames.xml"; // XML ���� ���
    public CountryNameData countryNameData;

    void Start()
    {
        countryNameData = LoadCountryNames(xmlFilePath);

        if (countryNameData == null)
        {
            Debug.LogError("XML �����͸� �ε� ����.");
            return;
        }

        // �����Ͱ� ����� �ε�Ǿ����� Ȯ���ϴ� ����� �޽���
        foreach (var culture in countryNameData.Cultures)
        {
            Debug.Log($"��ȭ��: {culture.Name}");
            foreach (var subCulture in culture.SubCultures)
            {
                Debug.Log($"  �ҹ�ȭ��: {subCulture.Name}");
                foreach (var microCulture in subCulture.MicroCultures)
                {
                    Debug.Log($"    ���� ��ȭ��: {microCulture.Name}");
                }
            }
        }
    }

    public CountryNameData LoadCountryNames(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError($"XML ������ ã�� �� �����ϴ�: {path}");
            return null;
        }

        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CountryNameData));
            using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                return serializer.Deserialize(stream) as CountryNameData;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"XML ������ �Ľ� ����! {e.Message}");
            return null;
        }
    }

    // �� �޼��带 �߰��Ͽ� ���� �̸��� �����ɴϴ�.
    public string GetRandomCountryName(string cultureName)
    {
        if (countryNameData == null || countryNameData.Cultures == null)
        {
            Debug.LogError("CountryNameData / Cultures ����Ʈ�� �ʱ�ȭ���� �ʾ���.");
            return "Unknown";
        }

        // 3�� ������ ��ȭ�� Ž��
        foreach (var majorCulture in countryNameData.Cultures)
        {
            foreach (var subCulture in majorCulture.SubCultures)
            {
                var microCulture = subCulture.MicroCultures.Find(m => m.Name == cultureName);
                if (microCulture != null)
                {
                    if (microCulture.Names != null && microCulture.Names.Count > 0)
                    {
                        return microCulture.Names[Random.Range(0, microCulture.Names.Count)];
                    }
                    else
                    {
                        Debug.LogError($"'{microCulture.Name}' ��ȭ���� �̸� �ε����� �������.");
                        return "Unknown";
                    }
                }
            }
        }

        Debug.LogError($"'{cultureName}' ��ȭ���� ã�� �� �����ϴ�.");
        return "Unknown"; // ã�� ���� ��� �⺻ �� ��ȯ
    }

    /* ���� ���� ���� ���Ž� �ڵ�
    public NationData GetRandomNationData()
    {
        if (nationsData == null || nationsData.Races.Count == 0)
        {
            return null;
        }

        var randomRace = nationsData.Races[Random.Range(0, nationsData.Races.Count)];
        var randomCulture = randomRace.CulturalSphere[Random.Range(0, randomRace.CulturalSphere.Count)];


        NationData newNation = new NationData
        {
            Name = randomCulture.MicroCulture,
            GovernmentType = "Kingdom", 
            cultureGroup = randomCulture 
        };

        return newNation;
    }
    */
}
