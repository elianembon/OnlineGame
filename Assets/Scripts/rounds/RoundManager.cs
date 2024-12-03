using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviourPun
{
    public List<PlayerStats> losers = new List<PlayerStats>();

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
                EndRound(playerStats);
            }
        }
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

    private PlayerStats FindPlayerStatsByPhotonPlayer(Photon.Realtime.Player photonPlayer)
    {
        PlayerStats[] allPlayers = FindObjectsOfType<PlayerStats>();
        foreach (PlayerStats playerStats in allPlayers)
        {
            if (playerStats.photonView.Owner == photonPlayer) // Compara propietarios
            {
                return playerStats;
            }
        }
        return null; // No se encontró un jugador con ese PhotonPlayer
    }

    // Método para encontrar PlayerStats a partir de ActorNumber
    private PlayerStats FindPlayerStatsByActorNumber(int actorNumber)
    {
        PlayerStats[] allPlayers = FindObjectsOfType<PlayerStats>();
        foreach (PlayerStats playerStats in allPlayers)
        {
            if (playerStats.photonView.Owner.ActorNumber == actorNumber) // Compara ActorNumber
            {
                return playerStats;
            }
        }
        return null; // No se encontró el PlayerStats con ese ActorNumber
    }

    private void EndRound(PlayerStats winner)
    {
        Debug.Log($"Ronda terminada. Ganador: {winner.name}");

        // Obtén las referencias a los jugadores que perdieron
        List<Photon.Realtime.Player> loserPlayers = new List<Photon.Realtime.Player>();
        foreach (var loser in losers)
        {
            loserPlayers.Add(loser.photonView.Owner);
        }

        // Enviar a los perdedores a la escena "PickMejora"
        photonView.RPC("SendToCardSelection", RpcTarget.All, loserPlayers[0].ActorNumber, loserPlayers[1].ActorNumber);
    }

    [PunRPC]
    private void SendToCardSelection(int firstLoserActor, int secondLoserActor)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == firstLoserActor || PhotonNetwork.LocalPlayer.ActorNumber == secondLoserActor)
        {
            PhotonNetwork.LoadLevel("PickMejora");
        }
    }
}
