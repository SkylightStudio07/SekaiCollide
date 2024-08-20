// Assets/Scripts/Data/CountryNameData.cs
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("CountryNames")]
public class CountryNameData
{
    [XmlElement("Culture")]
    public List<MajorCulture> Cultures = new List<MajorCulture>();
}

public class MajorCulture
{
    [XmlAttribute("name")]
    public string Name;

    [XmlElement("SubCulture")]
    public List<SubCulture> SubCultures = new List<SubCulture>();
}

public class SubCulture
{
    [XmlAttribute("name")]
    public string Name;

    [XmlElement("MicroCulture")]
    public List<MicroCulture> MicroCultures = new List<MicroCulture>();
}

public class MicroCulture
{
    [XmlAttribute("name")]
    public string Name;

    [XmlArray("Names")]
    [XmlArrayItem("Name")]
    public List<string> Names = new List<string>();
}
