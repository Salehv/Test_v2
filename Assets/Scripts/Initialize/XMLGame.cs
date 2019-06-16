using System.Collections.Generic;
using System.Xml.Serialization;

namespace Initialize
{
    [XmlRoot(ElementName = "Game")]
    public class XMLGame
    {
        [XmlElement(ElementName = "Chapter")] public List<XMLChapter> chapters;
    }

    [XmlRoot(ElementName = "Chapter")]
    public class XMLChapter
    {
        [XmlAttribute(AttributeName = "name")] public string name;

        [XmlAttribute(AttributeName = "cost")] public string cost;

        [XmlAttribute(AttributeName = "id")] public string id;

        [XmlElement(ElementName = "Level")] public List<XMLLevel> levels;
    }

    [XmlRoot(ElementName = "Level")]
    public class XMLLevel
    {
        [XmlAttribute(AttributeName = "id")] public string id;

        [XmlAttribute(AttributeName = "type")] public string type;

        [XmlElement(ElementName = "Start")] public string start;

        [XmlElement(ElementName = "End")] public string end;

        [XmlElement(ElementName = "Way")] public XMLWay way;
    }

    [XmlRoot(ElementName = "Way")]
    public class XMLWay
    {
        [XmlElement(ElementName = "Word")] public List<string> words;
    }
}