using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

[XmlRoot("GovernmentTypes")]
public class GovernmentTypeData
{
    [XmlElement("Government")]
    public List<GovernmentType> Governments = new List<GovernmentType>();
}
public class GovernmentType
{
    [XmlAttribute("name")]
    public string Name;

    [XmlArray("Prefixes")]
    [XmlArrayItem("Prefix")]
    public List<string> Prefixes = new List<string>();

    [XmlElement("Suffix")]
    public string Suffix;
}

public class GovernmentTypeLoader : MonoBehaviour
{
    public string xmlFilePath = "Assets/Data/GovernmentTypes.xml";
    public GovernmentTypeData governmentTypeData;

    void Start()
    {
        governmentTypeData = LoadGovernmentTypes(xmlFilePath);
    }

    public GovernmentTypeData LoadGovernmentTypes(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"XML 파일을 찾을 수 없습니다: {path}");
            return null;
        }
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GovernmentTypeData));
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return serializer.Deserialize(stream) as GovernmentTypeData;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"XML 데이터 파싱 오류 발생: {e.Message}");
            return null;
        }
    }

    public GovernmentType GetGovernmentType(string governmentName)
    {
        return governmentTypeData.Governments.Find(g => g.Name == governmentName);
    }

    public string GetRandomPrefix(GovernmentType governmentType)
    {
        if (governmentType.Prefixes.Count > 0)
        {
            return governmentType.Prefixes[Random.Range(0, governmentType.Prefixes.Count)];
        }
        return "";
    }
}
