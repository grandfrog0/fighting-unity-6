using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class GamePreparer : NetworkBehaviour
{
    public UnityEvent OnGameReady = new();

    [SerializeField] PlayerSpawner playerSpawner;
    [SerializeField] PlayerSelector playerSelector;
    [SerializeField] ItemSelector talismanSelector, elixirSelector;
    [SerializeField] List<string> itemNames;
    [SerializeField] List<Item> items;

    public void PrepareGame()
    {
        PrepareGameServerRpc(
            playerSelector.SelectedPlayer, talismanSelector.SelectedItem.Item.Name, 
            elixirSelector.SelectedItem.Item.Name, NetworkManager.Singleton.LocalClientId
            );

        OnGameReady.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PrepareGameServerRpc(int selectedPlayerIndex, string talismanName, string elixirName, ulong clientId)
    {
        PlayerInfo playerInfo = new PlayerInfo()
        {
            SelectedPlayerIndex = selectedPlayerIndex,
            Talisman = items[itemNames.IndexOf(talismanName)],
            Elixir = items[itemNames.IndexOf(elixirName)]
        };

        playerSpawner.SetPlayerConfig(playerInfo, clientId);
    }

    public override void OnNetworkSpawn()
    {
        Debugger.Log("Prepared spawndef", IsServer);
        base.OnNetworkSpawn();
    }

    private void Start()
    {
        foreach(Item item in items)
        {
            itemNames.Add(item.Name);
        }
    }
}
