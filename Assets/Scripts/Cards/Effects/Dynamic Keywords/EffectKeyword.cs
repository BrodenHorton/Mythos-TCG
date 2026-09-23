using UnityEngine;

[CreateAssetMenu(fileName = "EffectKeyword", menuName = "Scriptable Objects/Effect/Effect Keyword")]
public class EffectKeyword : ScriptableObject {
    [SerializeField] private string id;
    [SerializeField] private string keywordName;
    [SerializeField, TextArea] private string description;

    public string Id { get { return id; } }

    public string KeywordName { get { return keywordName; } }

    public string Description { get { return description; } }
}
