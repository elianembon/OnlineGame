using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using Photon.Realtime;

public class RoundManager : MonoBehaviourPun
{
    private Dictionary<string, int> playerRoundWins = new Dictionary<string, int>();  // Guardar el conteo de rondas ganadas por cada jugador
    private List<Photon.Realtime.Player> playersInRound = new List<Photon.Realtime.Player>();  // Jugadores activos en la ronda

    void Start()
    {
        // Registrar a los jugadores que participan en la ronda
        playersInRound.AddRange(PhotonNetwork.PlayerList);
    }

    // Método para registrar cuando un jugador ha sido derrotado
    [PunRPC]
    public void PlayerDefeated(Photon.Realtime.Player player)
    {
        playersInRound.Remove(player); // El jugador derrotado es eliminado de la lista

        if (playersInRound.Count == 1)  // Si solo queda un jugador, significa que ese jugador ganó la ronda
        {
            PlayerWins(playersInRound[0]);
        }
    }

    // Método para registrar cuando un jugador gana la ronda
    private void PlayerWins(Photon.Realtime.Player winner)
    {
        // Aumentar el conteo de rondas ganadas para el ganador
        if (playerRoundWins.ContainsKey(winner.NickName))
        {
            playerRoundWins[winner.NickName]++;
        }
        else
        {
            playerRoundWins.Add(winner.NickName, 1);
        }

        Debug.Log($"{winner.NickName} ha ganado la ronda. Rondas ganadas: {playerRoundWins[winner.NickName]}");

        // Verificar si algún jugador ha ganado 3 rondas
        if (playerRoundWins[winner.NickName] >= 3)
        {
            EndGame(winner);
        }
        else
        {
            // Reiniciar la ronda
            ResetRound();
        }
    }

    // Método para finalizar el juego cuando un jugador gana 3 rondas
    private void EndGame(Photon.Realtime.Player winner)
    {
        Debug.Log($"{winner.NickName} ha ganado el juego!");
        PhotonNetwork.LoadLevel("GameFinish"); // Cargar la escena de fin de juego
    }

    // Método para reiniciar la ronda
    private void ResetRound()
    {
        // Reiniciar la lista de jugadores para la siguiente ronda
        playersInRound.Clear();
        playersInRound.AddRange(PhotonNetwork.PlayerList);
    }

}