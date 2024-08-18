using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("Nations")]
public class Nations
{
    [XmlElement("Race")]
    public List<Race> Races = new List<Race>();
}

public class Race
{
    [XmlAttribute("name")]
    public string Name;

    [XmlArray("CulturalSphere")]
    [XmlArrayItem("Culture")]
    public List<CulturalGroup> CulturalSphere = new List<CulturalGroup>();
}

public class CulturalGroup
{
    [XmlElement("MajorCulture")]
    public string MajorCulture;

    [XmlElement("SubCulture")]
    public string SubCulture;

    [XmlElement("MicroCulture")]
    public string MicroCulture;
}
