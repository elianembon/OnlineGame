using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CardSelectionManager : MonoBehaviourPun
{
    private int currentSelectorIndex = 0; // Índice del jugador que selecciona
    public List<cards> availableCards; // Lista de cartas disponibles

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetSelector", RpcTarget.AllBuffered, PhotonNetwork.PlayerList[currentSelectorIndex].ActorNumber);
        }
    }

    [PunRPC]
    private void SetSelector(int actorNumber)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            Debug.Log("Es tu turno de elegir una carta.");
            // Mostrar UI para elegir carta
        }
    }

    public void CardSelected(cards selectedCard)
    {
        // Enviar datos de la carta seleccionada al servidor
        photonView.RPC("ApplyCardToPlayer", RpcTarget.MasterClient, selectedCard.name);

        // Pasar al siguiente jugador
        photonView.RPC("NextSelector", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void NextSelector()
    {
        currentSelectorIndex++;
        if (currentSelectorIndex < PhotonNetwork.PlayerList.Length)
        {
            photonView.RPC("SetSelector", RpcTarget.AllBuffered, PhotonNetwork.PlayerList[currentSelectorIndex].ActorNumber);
        }
        else
        {
            // Volver al juego principal
            PhotonNetwork.LoadLevel("MainGameScene");
        }
    }

    [PunRPC]
    private void ApplyCardToPlayer(string cardName)
    {
        // Asignar los efectos de la carta al jugador correspondiente
        Debug.Log($"Carta {cardName} aplicada al jugador.");
    }
}
