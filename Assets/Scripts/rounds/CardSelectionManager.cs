using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CardSelectionManager : MonoBehaviourPun
{
    public CardManager cardManager;
    public CardDisplay[] cardSlots;
    private int currentSelectorIndex = 0;
    private List<PlayerStats> losers = new List<PlayerStats>();

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
            cards randomCard = cardManager.GetRandomCard();
            slot.SetCard(randomCard);
        }
    }

    void SetNextPlayerToSelect()
    {
        if (PhotonNetwork.IsMasterClient && losers.Count > 0)
        {
            photonView.RPC("SetSelector", RpcTarget.AllBuffered, losers[currentSelectorIndex].photonView.Owner.ActorNumber);
        }
    }

    [PunRPC]
    private void SetSelector(int actorNumber)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            Debug.Log("Es tu turno de elegir una carta.");
            EnableCardSelectionUI(true);
        }
        else
        {
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
        photonView.RPC("ApplyCardToPlayer", RpcTarget.MasterClient, selectedCard.name);
        photonView.RPC("NextSelector", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void ApplyCardToPlayer(string cardName)
    {
        // Obtén la carta seleccionada por nombre
        cards selectedCard = cardManager.GetCardByName(cardName);
        if (selectedCard != null)
        {
            PlayerStats currentPlayer = FindObjectOfType<PlayerStats>(); // Asume que tienes una forma de identificar al jugador actual
            currentPlayer.ApplyCardEffect(selectedCard.damage, selectedCard.cooldown, (int)selectedCard.bullets);
            Debug.Log($"Carta {cardName} aplicada al jugador.");
        }
        else
        {
            Debug.LogError("No se pudo aplicar la carta, ya que no se encontró.");
        }
    }

    [PunRPC]
    private void NextSelector()
    {
        currentSelectorIndex++;

        if (currentSelectorIndex < losers.Count)
        {
            photonView.RPC("SetSelector", RpcTarget.AllBuffered, losers[currentSelectorIndex].photonView.Owner.ActorNumber);
        }
        else
        {
            PhotonNetwork.LoadLevel("MainGameScene");
        }
    }

    public void SetLosers(List<PlayerStats> losersList)
    {
        losers = losersList;
        SetNextPlayerToSelect();
    }
}
