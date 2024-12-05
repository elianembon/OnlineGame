using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CardDisplay : MonoBehaviour
{
    public cards card;

    public TMP_Text nameText;
    public TMP_Text damageText;
    public TMP_Text bulletsText;
    public TMP_Text cooldownText;

    public Button selectButton;

    private CardSelectionManager cardSelectionManager;

    void Start()
    {
        cardSelectionManager = FindObjectOfType<CardSelectionManager>(); // Busca el gestor de selección de cartas
        selectButton.onClick.AddListener(OnCardSelected);
    }

    public void SetCard(cards newCard)
    {
        card = newCard;

        nameText.text = card.name;
        damageText.text = "" + card.damage.ToString();
        bulletsText.text = "" + card.bullets.ToString();
        cooldownText.text = "" + card.cooldown.ToString();
    }

    void OnCardSelected()
    {
        Debug.Log($"{card.name} selected!");
        if (cardSelectionManager != null && card != null)
        {
            cardSelectionManager.CardSelected(card); // Notificamos al CardSelectionManager
        }
        else
        {
            Debug.LogError("CardSelectionManager o card es null.");
        }
    }
}