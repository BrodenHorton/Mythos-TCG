public static class CardRichTextUtil {
    private static readonly string EFFECT_KEYWORD_COLOR = "#fff47d";

    public static string GetKeywordLinkTagText(string keywordId, string body) {
        return "<color=" + EFFECT_KEYWORD_COLOR + "><link=" + keywordId + ">" + body + "</link></color>";
    }
}