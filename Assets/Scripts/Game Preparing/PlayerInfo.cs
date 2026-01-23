using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;

public class PlayerInfo
{
    public int SelectedPlayerIndex;
    public Item Talisman;
    public Item Elixir;

    public override string ToString()
        => $"Player {SelectedPlayerIndex}: {Talisman.Name}, {Elixir.Name}.";
}
