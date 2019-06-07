using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Utils
{
    public class TextUtils
    {
        public static Dictionary<string, List<string>> ReadLocaleText(string text, List<char> seps)
        {
            var segments = new List<string>();

            string temp = "";
            bool quote = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (seps.Contains(text[i]) && !quote)
                {
                    // Seperator. Let us separate
                    segments.Add(temp);
                    temp = "";
                    continue;
                }
                temp += text[i];
                if (text[i] == '"')
                {
                    quote = !quote;
                }
            }
            segments.Add(temp);
            Dictionary<string, List<string>> o = new Dictionary<string, List<string>>();
            for (int i = 0; i < segments.Count - seps.Count + 1; i += seps.Count)
            {
                List<string> segs = new List<string>();
                for (int j = 1; j < seps.Count; j++)
                {
                    segs.Add(segments[i + j]);
                }
                o.Add(segments[i], segs);
            }
            return o;
        }

        public static void ApplyWatermark(Dictionary<string, List<string>> localeValues)
        {
            string header = "\n<size=150%><color=#EC1C24FF>Quest Modders</color></size>";
            string testersHeader = "<color=#E543E5FF>Testers</color>";

            string sc2ad = "<color=#EDCE21FF>Sc2ad</color>";
            string trishume = "<color=#40E0D0FF>trishume</color>";
            string emulamer = "<color=#00FF00FF>emulamer</color>";
            string jakibaki = "<color=#4268F4FF>jakibaki</color>";
            string elliotttate = "<color=#67AAFBFF>elliotttate</color>";
            string leo60228 = "<color=#00FF00FF>leo60228</color>";
            string trueavid = "<color=#FF8897FF>Trueavid</color>";
            string kayTH = "<color=#40FE97FF>kayTH</color>";

            string message = '\n' + header + '\n' + sc2ad + '\n' + trishume + '\n' + emulamer + '\n' + jakibaki +
                '\n' + elliotttate + '\n' + leo60228 + '\n' + testersHeader + '\n' + trueavid + '\n' + kayTH;

            var value = localeValues["CREDITS_CONTENT"];
            string item = value[value.Count - 1];
            if (item.Contains(message)) return;
            localeValues["CREDITS_CONTENT"][value.Count - 1] = item.Remove(item.Length - 2) + message + '"';
        }

        public static string WriteLocaleText(Dictionary<string, List<string>> values, List<char> seps)
        {
            string temp = "";
            foreach (string s in values.Keys)
            {
                temp += s + seps[0];
                for (int i = 1; i < seps.Count; i++)
                {
                    temp += values[s][i - 1];
                    temp += seps[i];
                }
            }
            temp = temp.Remove(temp.Length - 1);
            return temp;
        }
    }
}
