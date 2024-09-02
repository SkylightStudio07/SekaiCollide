using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System.Collections.Generic;

public class NationLoader : MonoBehaviour
{
    public static NationLoader Instance { get; private set; } // 싱글톤 패턴

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
            Debug.LogError("XML 데이터가 로드되지 않았거나, 데이터가 없습니다.");
        }
    }

    public Nations LoadNations(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"XML 파일 찾기 실패: {path}");
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
            Debug.LogError($"입출력 오류 발생: {e.Message}");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"XML 파싱 도중 오류 발생: {e.Message}");
            return null;
        }
    }

    // 유닛이 건국할 때 사용되는 메서드: 국가 이름과 데이터를 반환
    public NationData GetRandomNationData()
    {
        if (nationsData == null || nationsData.Races.Count == 0)
        {
            return null;
        }

        // 무작위로 종족과 문화권을 선택
        var randomRace = nationsData.Races[Random.Range(0, nationsData.Races.Count)];
        var randomCulture = randomRace.CulturalSphere[Random.Range(0, randomRace.CulturalSphere.Count)];

        // NationData를 생성하고 CulturalGroup을 포함시킴
        NationData newNation = new NationData
        {
            Name = randomCulture.MicroCulture,
            GovernmentType = "Kingdom", // 정부 형태를 설정
            cultureGroup = randomCulture // CulturalGroup 전체를 NationData에 포함
        };

        return newNation;
    }

}
