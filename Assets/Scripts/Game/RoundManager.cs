using System.Diagnostics.Contracts;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public int roundsCount = 0;

    public void EndRound()
    {
        roundsCount++;

        // heal players

        if (roundsCount >= 3)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        roundsCount = 0;
        Debug.Log("игра окончена.");
    }
}
