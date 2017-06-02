using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkShared;
using GTANetworkServer;

namespace Accent
{
    [Serializable()]
    public class Accent
    {
        public String accentName { get; set; }
        public Dictionary<String, String> wordsToChange { get; private set; }

        public Accent(String accentName)
        {
            this.accentName = accentName;
            this.wordsToChange = new Dictionary<string, string>();
        }

        private Accent()
        {
            
        }

        public override string ToString()
        {
            TextInfo text = new CultureInfo("en-GB", false).TextInfo;
            return "[" + text.ToTitleCase(accentName.ToLower()) + " Accent]";
        }

        public String replaceWords(String message)
        {
            String[] strSplit = message.Split();
            for (var index = 0; index < strSplit.Length; index++)
            {
                String testword;
                if (wordsToChange.TryGetValue(strSplit[index].ToLower(), out testword))
                {
                    strSplit[index] = testword;
                }
            }
            return String.Join(" ",strSplit);
        }
    }
}
