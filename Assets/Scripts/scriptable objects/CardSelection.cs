using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CardSelection : MonoBehaviourPun
{
    //public List<Card> cards;  // Lista de cartas disponibles

    //// Este m�todo se llama cuando un jugador elige una carta
    //public void ChooseCard(Card selectedCard)
    //{
    //    PlayerStats playerStats = PhotonNetwork.LocalPlayer.GetComponent<PlayerStats>();
    //    playerStats.ApplyCardEffect(selectedCard);

    //    // Enviar la carta elegida a trav�s de la red
    //    PhotonView photonView = PhotonNetwork.LocalPlayer.GetComponent<PhotonView>();
    //    photonView.RPC("ApplyCardToPlayer", RpcTarget.All, selectedCard);

    //    // Continuar con la selecci�n del siguiente jugador
    //    CardSelectionManager cardSelectionManager = FindObjectOfType<CardSelectionManager>();
    //    cardSelectionManager.NextSelector();
    //}
}
