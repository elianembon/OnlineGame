using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CardSelectionManager : MonoBehaviourPun
{
    public CardManager cardManager; // Referencia a la lista de cartas
    public CardDisplay[] cardSlots; // Slots donde se mostrarán las cartas
    private int currentSelectorIndex = 0; // Índice del jugador que selecciona

    void Start()
    {
        // Solo el MasterClient puede iniciar la asignación de cartas
        if (PhotonNetwork.IsMasterClient)
        {
            AssignRandomCards();
            photonView.RPC("SetSelector", RpcTarget.AllBuffered, PhotonNetwork.PlayerList[currentSelectorIndex].ActorNumber);
        }
    }

    // Método para asignar cartas aleatorias a los slots
    void AssignRandomCards()
    {
        foreach (CardDisplay slot in cardSlots)
        {
            cards randomCard = cardManager.GetRandomCard(); // Obtener una carta aleatoria
            slot.SetCard(randomCard); // Asignar la carta al slot
        }
    }

    [PunRPC]
    private void SetSelector(int actorNumber)
    {
        // Comprobar si este jugador es el que le toca elegir
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            Debug.Log("Es tu turno de elegir una carta.");
            // Mostrar UI para que este jugador pueda elegir una carta
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
            // Volver al juego principal cuando todos hayan seleccionado
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
