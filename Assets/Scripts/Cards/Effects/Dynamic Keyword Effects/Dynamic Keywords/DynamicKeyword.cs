using UnityEngine;

[CreateAssetMenu(fileName = "DynamicKeyword", menuName = "Scriptable Objects/Effect/Dynamic Keyword")]
public class DynamicKeyword : ScriptableObject {
    [SerializeField] private string id;
    [SerializeField] private string keywordName;
    [SerializeField, TextArea] private string description;

    public string Id { get { return id; } }

    public string KeywordName { get { return keywordName; } }

    public string Description { get { return description; } }
}
