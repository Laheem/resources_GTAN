using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace Accent
{
    public class Accent
    {
        public Accent(string accentName)
        {
            this.accentName = accentName;
            wordsToChange = new Dictionary<string, string>();
        }

        [JsonProperty(Order = 1)]
        public string accentName { get; set; }

        [JsonProperty(Order = 2)]
        public Dictionary<string, string> wordsToChange { get; private set; }


        public void saveAccent()
        {
            Directory.CreateDirectory(AccentServer.filePath);
            File.WriteAllText(AccentServer.filePath + "\\" + accentName + ".json",
                JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public override string ToString()
        {
            var text = new CultureInfo("en-GB", false).TextInfo;
            return " [" + text.ToTitleCase(accentName.ToLower()) + " Accent]: ";
        }

        public string replaceWords(string message)
        {
            var strSplit = message.Split();
            for (var index = 0; index < strSplit.Length; index++)
            {
                string testword;
                if (wordsToChange.TryGetValue(strSplit[index].ToLower(), out testword))
                    strSplit[index] = testword;
            }
            return string.Join(" ", strSplit);
        }
    }
}