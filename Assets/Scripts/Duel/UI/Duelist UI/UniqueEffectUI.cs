using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UniqueEffectUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI effectDescription;
    [Header("Prefabs")]
    [SerializeField] private DynamicKeywordIndicator dynamicKeywordIndicatorPrefab;
    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Init(CreatureCardEffectPayload effect) {
        effectDescription.text = effect.RawDescription.ToString();
        effectDescription.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        Canvas.ForceUpdateCanvases();
        ParseDynamicKeywords();
    }

    private void ParseDynamicKeywords() {
        TMP_TextInfo textInfo = effectDescription.textInfo;

        for (int i = 0; i < textInfo.linkCount; i++) {
            TMP_LinkInfo linkInfo = textInfo.linkInfo[i];
            if (!linkInfo.GetLinkID().Equals("keyword_id", System.StringComparison.OrdinalIgnoreCase))
                continue;
            DynamicKeywordRegistry dynamicKeywordRegistry = ServiceLocator.Get<DynamicKeywordRegistry>();
            string keywordId = linkInfo.GetLinkText();
            if (!dynamicKeywordRegistry.Contains(keywordId)) {
                TcgLogger.Warn("Unable to find keyword with id:" + keywordId);
                continue;
            }

            int firstKeywordCharacterIndex = textInfo.characterInfo[linkInfo.linkTextfirstCharacterIndex].index;
            effectDescription.text = effectDescription.text.Remove(firstKeywordCharacterIndex, linkInfo.linkTextLength);
            effectDescription.text = effectDescription.text.Insert(firstKeywordCharacterIndex, dynamicKeywordRegistry.Get(keywordId).KeywordName);
            effectDescription.ForceMeshUpdate();
            string keywordDescription = dynamicKeywordRegistry.Get(keywordId).Description;
            AddDynamicKeywordIndicator(keywordDescription, linkInfo.linkTextfirstCharacterIndex, linkInfo.linkTextLength);
        }
    }

    private void AddDynamicKeywordIndicator(string keywordDescription, int startingCharIndex, int length) {
        DynamicKeywordIndicator indicator = Instantiate(dynamicKeywordIndicatorPrefab, effectDescription.transform);
        indicator.Init(keywordDescription);
        TMP_CharacterInfo startingCharInfo = effectDescription.textInfo.characterInfo[startingCharIndex];
        TMP_CharacterInfo endingCharInfo = effectDescription.textInfo.characterInfo[startingCharIndex + length - 1];
        Vector3 localTopLeftPos = startingCharInfo.topLeft;
        Vector3 localBottomRightPos = endingCharInfo.bottomRight;
        indicator.transform.localPosition = localTopLeftPos;
        indicator.RectTransform.sizeDelta = new Vector2(localBottomRightPos.x - localTopLeftPos.x,
                                                        localTopLeftPos.y - localBottomRightPos.y);
    }

    public RectTransform RectTransform { get { return rectTransform; } }
}
