using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SemesterWork
{
    public class LanguageEngine
    {
        private static Dictionary<string, XmlDocument> _languages = new Dictionary<string, XmlDocument>();
        public List<string> Languages => _languages.Keys.ToList();
        public static string Current { get; set; } = "Русский";

        public LanguageEngine()
        {
            foreach (var file in Directory.GetFiles("Languages"))
            {
                XmlDocument document = new XmlDocument();
                document.Load(file);
                XmlElement root = document.DocumentElement;
                string langName = root["language"].InnerText;
                _languages[langName] = document;
            }
        }

        public string this[string index]
        {
            get
            {
                return _languages[Current]
                    .DocumentElement["strings"]
                    .ChildNodes
                    .Cast<XmlNode>()
                    .FirstOrDefault(x => x.Attributes["name"].InnerText.ToString() == index)
                    .InnerText;
            }
        }
    }
}
