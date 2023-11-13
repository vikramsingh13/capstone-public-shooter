using UnityEngine;

[CreateAssetMenu(fileName = "NewCombatEntity", menuName = "Combat/Entity", order = 1)]
public class CombatEntity : ScriptableObject
{
    public string entityName;
    public float health;
    public float damage;
    public float defense;
    public string prefabAddress;
    //TODO: types, damage types, etc. 

    public void Init(string name, float hp, float attack, float def, string prefabKey)
    {
        entityName = name;
        health = hp;
        damage = attack;
        defense = def;
        prefabAddress = prefabKey;
    }
}