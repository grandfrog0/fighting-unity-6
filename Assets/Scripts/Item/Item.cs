using UnityEngine;

[CreateAssetMenu(fileName = "item", menuName = "SO/Item")]
public class Item : ScriptableObject
{
    public string Name;
    public string Description;
    public bool IsTalisman;
}

