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
        for (int i = 0; i < players.Count; i++)
        {
            int a = i;
            players[i].onClick.AddListener(() => OnElementClicked(a));
        }
    }
    private void OnElementClicked(int i)
    {
        Debug.Log(i);
        selection.position = players[i].transform.position;
        SelectedPlayer = i;
    }
}
