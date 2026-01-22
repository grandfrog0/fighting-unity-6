using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelector : MonoBehaviour
{
    [SerializeField] Transform selection;
    [SerializeField] List<Button> players;
    public int SelectedPlayer;

    private void Start()
    {
        for(int i = 0; i < players.Count; i++)
        {
            players[i].onClick.AddListener(() => OnElementClicked(i));
            Debug.Log(i);
        }
    }

    private void OnElementClicked(int player)
    {
        Debug.Log(player);
        SelectedPlayer = player;
        selection.position = players[player].transform.position;
    }
}
