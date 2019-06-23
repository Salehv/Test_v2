using System.Collections.Generic;
using System.Xml.Serialization;

namespace Initialize
{
    [System.Serializable]
    [XmlRoot(ElementName = "Game")]
    public class XMLGame
    {
        public XMLGame()
        {
        }

        [XmlElement(ElementName = "Chapter")] public XMLChapter[] chapters { get; set; }
    }

    [System.Serializable]
    [XmlRoot(ElementName = "Chapter")]
    public class XMLChapter
    {
        public XMLChapter()
        {
        }

        [XmlAttribute(AttributeName = "name")] public string name { get; set; }

        [XmlAttribute(AttributeName = "cost")] public string cost { get; set; }

        [XmlAttribute(AttributeName = "id")] public string id { get; set; }

        [XmlElement(ElementName = "Level")] public XMLLevel[] levels { get; set; }
    }

    [System.Serializable]
    [XmlRoot(ElementName = "Level")]
    public class XMLLevel
    {
        public XMLLevel()
        {
        }

        [XmlAttribute(AttributeName = "id")] public string id { get; set; }

        [XmlAttribute(AttributeName = "type")] public string type { get; set; }

        [XmlElement(ElementName = "Start")] public string start { get; set; }

        [XmlElement(ElementName = "End")] public string end { get; set; }

        [XmlElement(ElementName = "Way")] public XMLWay way { get; set; }
    }

    [System.Serializable]
    [XmlRoot(ElementName = "Way")]
    public class XMLWay
    {
        public XMLWay()
        {
        }

        [XmlElement(ElementName = "Word")] public string[] words { get; set; }
    }
}