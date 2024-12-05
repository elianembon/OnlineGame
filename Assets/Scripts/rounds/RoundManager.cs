using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class RoundManager : MonoBehaviourPun
{
    public List<PlayerStats> losers = new List<PlayerStats>();
    public int winnerScore = 0;
    public PlayerStats player1Stats;
    public PlayerStats player2Stats;
    public PlayerStats player3Stats;

    public int player1Points = 0;
    public int player2Points = 0;
    public int player3Points = 0;
    public void UpdateScore(PlayerStats winnerPlayerStats)
    {
        int winnerPlayerID = GetPlayerID(winnerPlayerStats);  // Obtener el ID del jugador ganador

        if (winnerPlayerID == 1)
        {
            player1Points++;
        }
        else if (winnerPlayerID == 2)
        {
            player2Points++;
        }
        else if (winnerPlayerID == 3)
        {
            player3Points++;
        }

        CheckForWinner();
    }

    private int GetPlayerID(PlayerStats playerStats)
    {
        if (playerStats == player1Stats)  // Suponiendo que player1Stats es un objeto de PlayerStats
        {
            return 1;  // ID del jugador 1
        }
        else if (playerStats == player2Stats)
        {
            return 2;  // ID del jugador 2
        }
        else if (playerStats == player3Stats)
        {
            return 3;  // ID del jugador 3
        }

        return -1;  // En caso de que no se encuentre el jugador
    }

    public void CheckForWinner()
    {
        if (player1Points >= 3)
        {
            EndGame(1); // Jugador 1 gana
        }
        else if (player2Points >= 3)
        {
            EndGame(2); // Jugador 2 gana
        }
        else if (player3Points >= 3)
        {
            EndGame(3); // Jugador 3 gana
        }
    }

    [PunRPC]
    public void PlayerDefeated(Photon.Realtime.Player photonPlayer)
    {
        PlayerStats playerStats = FindPlayerStatsByPhotonPlayer(photonPlayer);
        if (playerStats == null)
        {
            Debug.LogError($"No se encontró PlayerStats para el jugador {photonPlayer.NickName}.");
            return;
        }

        if (!losers.Contains(playerStats))
        {
            losers.Add(playerStats);
            Debug.Log($"Jugador {playerStats.name} agregado a la lista de perdedores.");

            photonView.RPC("UpdateLosersList", RpcTarget.All, playerStats.photonView.Owner.ActorNumber);

            if (losers.Count == 2) // Si dos jugadores perdieron
            {
                EndRound();
            }
        }
    }
    private void EndRound()
    {
        Debug.Log($"Ronda terminada.");

        // Mandamos a los jugadores a la selección de cartas
        CardSelectionManager cardSelectionManager = FindObjectOfType<CardSelectionManager>();
        if (cardSelectionManager != null)
        {
            cardSelectionManager.SetLosers(losers); // Pasa la lista de perdedores al CardSelectionManager
        }

        // Enviar a los perdedores a la escena "PickMejora"
        photonView.RPC("SendToCardSelection", RpcTarget.All, losers[0].photonView.Owner.ActorNumber, losers[1].photonView.Owner.ActorNumber);
    }

    private PlayerStats FindPlayerStatsByActorNumber(int actorNumber)
    {
        PlayerStats[] allPlayers = FindObjectsOfType<PlayerStats>();
        foreach (PlayerStats playerStats in allPlayers)
        {
            if (playerStats.photonView.Owner.ActorNumber == actorNumber)
            {
                return playerStats;
            }
        }
        return null; // Si no se encuentra, devuelve null
    }

    [PunRPC]
    private void UpdateLosersList(int actorNumber)
    {
        // Obtener el PlayerStats correspondiente al actorNumber
        PlayerStats playerStats = FindPlayerStatsByActorNumber(actorNumber);
        if (playerStats != null && !losers.Contains(playerStats))
        {
            losers.Add(playerStats);
        }
    }

    [PunRPC]
    private void SendToCardSelection(int firstLoserActor, int secondLoserActor)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == firstLoserActor || PhotonNetwork.LocalPlayer.ActorNumber == secondLoserActor)
        {
            PhotonNetwork.LoadLevel("PickMejora");
        }
    }

    public void IncrementScore(PlayerStats winner)
    {
        winnerScore++;


        if (winnerScore >= 3)
        {

            int winnerPlayerID = GetPlayerID(winner);


            EndGame(winnerPlayerID);
        }
    }

    public void ResetGame()
    {
        player1Points = 0;
        player2Points = 0;
        player3Points = 0;
        // Reiniciar otros elementos como la salud, las cartas, etc.
        PhotonNetwork.LoadLevel("LoadScene");
    }

    public void UpdatePlayerPoints(int playerID, int points)
    {
        if (playerID == 1)
        {
            player1Points = points;
        }
        else if (playerID == 2)
        {
            player2Points = points;
        }
        else if (playerID == 3)
        {
            player3Points = points;
        }
    }

    public void EndGame(int winnerPlayerID)
    {
        // Mostrar mensaje de victoria
        Debug.Log("¡El jugador " + winnerPlayerID + " ha ganado!");

        // Reiniciar la partida o cargar una nueva escena
        PhotonNetwork.LoadLevel("EndGameScene");
    }

    private PlayerStats FindPlayerStatsByPhotonPlayer(Photon.Realtime.Player photonPlayer)
    {
        PlayerStats[] allPlayers = Resources.FindObjectsOfTypeAll<PlayerStats>(); // Busca incluso en objetos desactivados
        foreach (PlayerStats playerStats in allPlayers)
        {
            if (playerStats.photonView != null && playerStats.photonView.Owner == photonPlayer)
            {
                return playerStats;
            }
        }
        return null; // Si no se encuentra, devuelve null
    }
}