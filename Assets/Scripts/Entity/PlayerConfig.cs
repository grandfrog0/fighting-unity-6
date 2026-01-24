using UnityEngine;

[CreateAssetMenu(fileName = "player", menuName = "SO/Player")]
public class PlayerConfig : ScriptableObject
{
    public string Name;
    public GameObject Model;

    public float Health;
    public float Damage1;
    public float Damage2;
    public float Cooldown1;
    public float Cooldown2;
    public float StunningProbability;
    public Vector2 StunningTimeRange;
}
