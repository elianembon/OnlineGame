using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoundManager : MonoBehaviourPun
{
    private List<PlayerStats> losers = new List<PlayerStats>();
    public Dictionary<PlayerController, PlayerStats> playerStatsMap = new Dictionary<PlayerController, PlayerStats>();

    public void PlayerDefeated(PlayerController player)
    {
        if (playerStatsMap.TryGetValue(player, out PlayerStats playerStats))
        {
            if (!losers.Contains(playerStats))
            {
                losers.Add(playerStats);
                Debug.Log($"{playerStats.name} ha sido derrotado");
            }

            // Si todos menos uno están derrotados
            if (losers.Count == 2)
            {
                PlayerStats winner = null;
                foreach (var pair in playerStatsMap)
                {
                    if (!losers.Contains(pair.Value))
                    {
                        winner = pair.Value;
                        break;
                    }
                }
                EndRound(winner);
            }
        }
        else
        {
            Debug.LogError("No se encontró PlayerStats para el jugador derrotado.");
        }
    }

    private void EndRound(PlayerStats winner)
    {
        photonView.RPC("SendLosersToCardScene", RpcTarget.All);
    }

    [PunRPC]
    private void SendLosersToCardScene()
    {
        foreach (PlayerStats loser in losers)
        {
            if (loser.photonView.IsMine)
            {
                PhotonNetwork.LoadLevel("CardSelectionScene");
            }
        }
    }
}
