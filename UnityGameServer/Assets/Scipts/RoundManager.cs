using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static List<Player> playersInRound;
    public static List<Player> playersFinishedRound;

    public static int AmountOfActivePlayersInRound;

    public int PlayerAmountNeeded;

    private static bool _IsNewRoundStarting;

    private void Awake()
    {
        playersInRound = new List<Player>();
        playersFinishedRound = new List<Player>();
        AmountOfActivePlayersInRound = 0;
        _IsNewRoundStarting = false;
    }

    private void FixedUpdate()
    {
        //Debug.Log($"FINSIHED PLAYERS: {playersFinishedRound.Count()}");
        //Debug.Log($"IN ROUND PLAYERS: {playersInRound.Count()}");
        //Debug.Log("----------------");
    }

    public static void ClearRound()
    {
        playersInRound.Clear();
        playersFinishedRound.Clear();
        AmountOfActivePlayersInRound = 0;
        _IsNewRoundStarting = true;
        // Reset scene?
    }

    public void JoinRound(Player player) // raise conditions?
    {
        AmountOfActivePlayersInRound++;
        playersInRound.Add(player);
        if (IsRoundFull())
        {
            _IsNewRoundStarting = false;
        }
    }

    public static void RemoveFromRound(Player player)
    {
        AmountOfActivePlayersInRound--;
        playersInRound.Remove(player);
    }

    public static void CheckForEndOfRound()
    {
        AmountOfActivePlayersInRound--;
        if (IsRoundFinished())
        {
            ServerSend.GameFinished(GetFinishedPlayerNamesInOrder(), GetDiedPlayerNamesInOrder());
            ClearRound();
        }
    }

    public static void AddToFinishedPlayers(Player player)
    {
        playersFinishedRound.Add(player);
    }

    public static bool IsRoundFinished()
    {
        //playersInRound.FindAll(player => player != null && player.IsAlive).Count 
        return AmountOfActivePlayersInRound <= 1;
    }

    public static string[] GetFinishedPlayerNamesInOrder()
    {
        return playersFinishedRound
            .Select(player => player != null ? player.username : null)
            .Where(name => name != null).ToArray();
    }

    public static string[] GetDiedPlayerNamesInOrder()
    {
        return playersInRound
            .Select(player => player != null && !player.IsAlive ? player.username : null)
            .Where(name => name != null).ToArray();
    }

    public static bool IsNewRoundStarting()
    {
        return _IsNewRoundStarting;
    }

    public bool IsRoundFull()
    {
        return AmountOfActivePlayersInRound >= PlayerAmountNeeded;
    }
}
