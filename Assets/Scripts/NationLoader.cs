using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class NationLoader : MonoBehaviour
{
    public string xmlFilePath = "Assets/Data/nations.xml"; // XML 파일 경로
    public Nations nationsData;

    void Start()
    {
        nationsData = LoadNations(xmlFilePath);

        if (nationsData == null || nationsData.Races.Count == 0)
        {
            Debug.LogError("XML 데이터가 로드되지 않았거나, 데이터가 없습니다.");
            return;
        }

        // XML 데이터를 출력하여 확인
        foreach (var race in nationsData.Races)
        {
            Debug.Log("종족: " + race.Name);
            foreach (var culture in race.CulturalSphere)
            {
                Debug.Log($" - 대문화권: {culture.MajorCulture}");
                Debug.Log($"   소문화권: {culture.SubCulture}");
                Debug.Log($"   마이크로 문화권: {culture.MicroCulture}");
            }
        }
    }

    public Nations LoadNations(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"XML 파일을 찾을 수 없습니다: {path}");
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
            Debug.LogError($"파일을 여는 도중 오류가 발생했습니다: {e.Message}");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"XML 데이터를 파싱하는 도중 오류가 발생했습니다: {e.Message}");
            return null;
        }
    }
}