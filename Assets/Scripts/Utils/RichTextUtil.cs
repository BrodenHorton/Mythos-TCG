using System.Collections.Generic;
using UnityEngine;

public static class RichTextUtil {
    private static char richTextIndicator = '&';
    private static Dictionary<char, RichTextValues> richTextByCharCode = new Dictionary<char, RichTextValues> {
        { '1', new RichTextValues("<color=#d90000>", "</color>") },
        { '2', new RichTextValues("<color=#0000FF>", "</color>") },
        { '3', new RichTextValues("<color=#019409>", "</color>") },
        { '4', new RichTextValues("<color=#FFFF00>", "</color>") },
        { '5', new RichTextValues("<color=#FF00FF>", "</color>") },
        { '6', new RichTextValues("<color=#00FFFF>", "</color>") },
        { '7', new RichTextValues("<color=#ffae00>", "</color>") },
        { '8', new RichTextValues("<color=#CCCCCC>", "</color>") },
        { '9', new RichTextValues("<color=#888888>", "</color>") },
        { '0', new RichTextValues("<color=#000000>", "</color>") },
        { 'a', new RichTextValues("<color=#fc5353>", "</color>") },
        { 'b', new RichTextValues("<color=#2b84ff>", "</color>") },
        { 'c', new RichTextValues("<color=#00FF00>", "</color>") },
        { 'd', new RichTextValues("<color=#960285>", "</color>") },
        { 'e', new RichTextValues("<color=#f8ff70>", "</color>") },
        { 'f', new RichTextValues("<color=#FFFFFF>", "</color>") }
    };

    public static string ProcessRichText(string msg) {
        if (msg == null || msg.Length == 0)
            return "";

        string result = "";
        List<string> sections = new List<string>();
        int splitIndex = 0;
        for (int i = 1; i < msg.Length; i++) {
            if (msg[i] == richTextIndicator && i < msg.Length - 1 && richTextByCharCode.ContainsKey(msg[i + 1])) {
                sections.Add(msg.Substring(splitIndex, i - splitIndex));
                splitIndex = i;
            }
        }
        sections.Add(msg.Substring(splitIndex, msg.Length - splitIndex));
        for (int i = 0; i < sections.Count; i++)
            Debug.Log(sections[i]);

        for (int i = 0; i < sections.Count; i++) {
            if (sections[i].Length >= 2 && sections[i][0] == richTextIndicator && richTextByCharCode.ContainsKey(sections[i][1])) {
                string richTextStr = "";
                RichTextValues richTextValue = richTextByCharCode[sections[i][1]];
                richTextStr += richTextValue.prefix;
                if (sections[i].Length > 2)
                    richTextStr += sections[i].Substring(2, sections[i].Length - 2);
                richTextStr += richTextValue.suffix;
                result += richTextStr;
            }
            else
                result += sections[i];
        }

        return result;
    }
}
