using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CountryNameLoader : MonoBehaviour
{
    public string xmlFilePath = "Assets/Data/CountryNames.xml"; // XML 파일 경로
    public CountryNameData countryNameData;

    void Start()
    {
        countryNameData = LoadCountryNames(xmlFilePath);

        if (countryNameData == null)
        {
            Debug.LogError("XML 데이터를 로드 실패.");
            return;
        }

        // 데이터가 제대로 로드되었는지 확인하는 디버깅 메시지
        foreach (var culture in countryNameData.Cultures)
        {
            Debug.Log($"문화권: {culture.Name}");
            foreach (var subCulture in culture.SubCultures)
            {
                Debug.Log($"  소문화권: {subCulture.Name}");
                foreach (var microCulture in subCulture.MicroCultures)
                {
                    Debug.Log($"    세부 문화권: {microCulture.Name}");
                }
            }
        }
    }

    public CountryNameData LoadCountryNames(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError($"XML 파일을 찾을 수 없습니다: {path}");
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
            Debug.LogError($"XML 데이터 파싱 오류! {e.Message}");
            return null;
        }
    }

    // 이 메서드를 추가하여 나라 이름을 가져옵니다.
    public string GetRandomCountryName(string cultureName)
    {
        if (countryNameData == null || countryNameData.Cultures == null)
        {
            Debug.LogError("CountryNameData / Cultures 리스트가 초기화되지 않았음.");
            return "Unknown";
        }

        // 3중 구조로 문화권 탐색
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
                        Debug.LogError($"'{microCulture.Name}' 문화권의 이름 인덱스가 비어있음.");
                        return "Unknown";
                    }
                }
            }
        }

        Debug.LogError($"'{cultureName}' 문화권을 찾을 수 없습니다.");
        return "Unknown"; // 찾지 못한 경우 기본 값 반환
    }

    /* 랜덤 국가 생성 레거시 코드
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
