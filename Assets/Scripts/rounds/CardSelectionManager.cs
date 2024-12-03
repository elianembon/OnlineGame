using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CardSelectionManager : MonoBehaviourPun
{
    public CardManager cardManager; 
    public CardDisplay[] cardSlots; 
    private int currentSelectorIndex = 0; 

    private List<Photon.Realtime.Player> losers = new List<Photon.Realtime.Player>(); 

    void Start()
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            AssignRandomCards();
            SetNextPlayerToSelect();
        }
    }

    void AssignRandomCards()
    {
        foreach (CardDisplay slot in cardSlots)
        {
            cards randomCard = cardManager.GetRandomCard(); // Obtener una carta aleatoria
            slot.SetCard(randomCard); // Asignar la carta al slot
        }
    }

    void SetNextPlayerToSelect()
    {
        if (PhotonNetwork.IsMasterClient && losers.Count > 0)
        {
            photonView.RPC("SetSelector", RpcTarget.AllBuffered, losers[currentSelectorIndex].ActorNumber);
        }
    }

    [PunRPC]
    private void SetSelector(int actorNumber)
    {
        // Comprobar si este jugador es el que le toca elegir
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            Debug.Log("Es tu turno de elegir una carta.");
            // Habilitar UI para que este jugador pueda elegir una carta
            EnableCardSelectionUI(true);
        }
        else
        {
            // Deshabilitar UI para los jugadores que no están eligiendo
            EnableCardSelectionUI(false);
        }
    }

    private void EnableCardSelectionUI(bool isEnabled)
    {
        foreach (CardDisplay slot in cardSlots)
        {
            slot.selectButton.interactable = isEnabled;
        }
    }

    public void CardSelected(cards selectedCard)
    {
        // Enviar la carta seleccionada al MasterClient
        photonView.RPC("ApplyCardToPlayer", RpcTarget.MasterClient, selectedCard.name);

        // Pasar al siguiente jugador
        photonView.RPC("NextSelector", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void ApplyCardToPlayer(string cardName)
    {
        // Aquí puedes aplicar los efectos de la carta seleccionada
        Debug.Log($"Carta {cardName} aplicada al jugador.");
    }

    [PunRPC]
    private void NextSelector()
    {
        currentSelectorIndex++;

        if (currentSelectorIndex < losers.Count)
        {
            photonView.RPC("SetSelector", RpcTarget.AllBuffered, losers[currentSelectorIndex].ActorNumber);
        }
        else
        {
            // Cuando todos hayan seleccionado, volver a la escena principal
            PhotonNetwork.LoadLevel("MainGameScene");
        }
    }

    public void SetLosers(List<Photon.Realtime.Player> losersList)
    {
        losers = losersList;
        SetNextPlayerToSelect();
    }
}
