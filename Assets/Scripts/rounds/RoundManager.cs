using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using Photon.Realtime;

public class RoundManager : MonoBehaviourPun
{
    public List<PlayerStats> losers = new List<PlayerStats>();
    public int winnerScore = 0;

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
            EndGame(winner);
        }
    }

    private void EndGame(PlayerStats winner)
    {
        // Fin del juego
        Debug.Log($"Juego terminado. Ganador: {winner.name}");
        // Aquí puedes hacer que el juego termine o lo que desees
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