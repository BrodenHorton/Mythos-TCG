using UnityEngine;

[CreateAssetMenu(fileName = "StaticKeywordIcon", menuName = "Scriptable Objects/Effect/StaticKeywordIcon")]
public class StaticKeywordIcon : ScriptableObject {
    [SerializeField] private string id;
    [SerializeField] private Sprite icon;

    public string Id { get { return id; } }

    public Sprite Icon { get { return icon; } }
}