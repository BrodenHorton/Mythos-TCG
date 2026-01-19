using UnityEngine;

[CreateAssetMenu(fileName = "SpellCardBase", menuName = "Scriptable Objects/Card/SpellCardBase")]
public class SpellCardBase : CardBase {
    [SerializeField] SpellType spellType;
}
