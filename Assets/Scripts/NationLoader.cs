using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class NationLoader : MonoBehaviour
{
    public string xmlFilePath = "Assets/Data/nations.xml"; // XML ���� ���
    public Nations nationsData;

    void Start()
    {
        nationsData = LoadNations(xmlFilePath);

        if (nationsData == null || nationsData.Races.Count == 0)
        {
            Debug.LogError("XML �����Ͱ� �ε���� �ʾҰų�, �����Ͱ� �����ϴ�.");
            return;
        }

        // XML �����͸� ����Ͽ� Ȯ��
        foreach (var race in nationsData.Races)
        {
            Debug.Log("����: " + race.Name);
            foreach (var culture in race.CulturalSphere)
            {
                Debug.Log($" - �빮ȭ��: {culture.MajorCulture}");
                Debug.Log($"   �ҹ�ȭ��: {culture.SubCulture}");
                Debug.Log($"   ����ũ�� ��ȭ��: {culture.MicroCulture}");
            }
        }
    }

    public Nations LoadNations(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"XML ���� ã�� ���� : {path}");
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
            Debug.LogError($"XML �Ľ� ���� ������ �߻�: {e.Message}");
            return null;
        }
    }
}