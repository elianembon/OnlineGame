using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CardSelectionManager : MonoBehaviourPun
{
    public CardManager cardManager;
    public CardDisplay[] cardSlots;
    private int currentSelectorIndex = 0;
    private List<PlayerStats> losers = new List<PlayerStats>();

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            // Solo el Master Client genera las cartas y las distribuye a todos los jugadores
            AssignRandomCardsToPlayers();
        }
    }

    // El Master Client genera cartas aleatorias y las envía a todos los jugadores
    void AssignRandomCardsToPlayers()
    {
        // Aseguramos que el Master Client también reciba las cartas, incluso si no está en la lista de perdedores
        foreach (var player in PhotonNetwork.PlayerList)
        {
            // Generamos dos cartas aleatorias para cada jugador
            for (int i = 0; i < 2; i++)
            {
                var randomCard = cardManager.GetRandomCard();

                // Usamos RPC para enviar la carta a cada jugador
                photonView.RPC("AssignCardToPlayer", player, randomCard.name, randomCard.damage, randomCard.cooldown, randomCard.bullets);
            }
        }
    }

    [PunRPC]
    void AssignCardToPlayer(string cardName, float damage, float cooldown, float bullets)
    {
        // Este RPC es llamado por cada jugador y les asigna una carta correspondiente
        var assignedCard = new cards { name = cardName, damage = damage, cooldown = cooldown, bullets = bullets };

        // Asignamos la carta a los slots de la UI de los jugadores
        foreach (CardDisplay slot in cardSlots)
        {
            if (slot.card == null)  // Solo asignamos la carta si el slot está vacío
            {
                slot.SetCard(assignedCard);
                break;  // Solo asignamos una carta por slot
            }
        }

        // Activamos la UI para que los jugadores puedan seleccionar sus cartas
        EnableCardSelectionUI(true);
    }

    // Activamos o desactivamos la UI de selección de cartas
    private void EnableCardSelectionUI(bool isEnabled)
    {
        foreach (CardDisplay slot in cardSlots)
        {
            slot.selectButton.interactable = isEnabled;
        }
    }


    public void CardSelected(cards selectedCard)
    {
        // Aquí pasamos los valores de la carta seleccionada como parámetros
        photonView.RPC("ApplyCardToPlayer", RpcTarget.AllBuffered, selectedCard.name, selectedCard.damage, selectedCard.cooldown, selectedCard.bullets);

        // Pasamos al siguiente jugador
        photonView.RPC("NextSelector", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void ApplyCardToPlayer(string cardName, float damage, float cooldown, float bullets)
    {
        cards selectedCard = cardManager.GetCardByName(cardName);
        if (selectedCard != null)
        {
            PlayerStats currentPlayer = FindObjectOfType<PlayerStats>();
            currentPlayer.ApplyCardEffect(damage, cooldown, (int)bullets);
            Debug.Log($"Carta {cardName} aplicada al jugador.");
        }
    }

    // Llamamos al siguiente jugador para que elija su carta
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

    // Asignamos la lista de jugadores perdedores
    public void SetLosers(List<PlayerStats> losersList)
    {
        losers = losersList;
        SetNextPlayerToSelect();
    }

    // Establecemos quién es el siguiente jugador que debe elegir su carta
    void SetNextPlayerToSelect()
    {
        if (PhotonNetwork.IsMasterClient && losers.Count > 0)
        {
            photonView.RPC("SetSelector", RpcTarget.AllBuffered, losers[currentSelectorIndex].photonView.Owner.ActorNumber);
        }
    }

    // Este RPC selecciona al jugador que debe elegir una carta
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
}
