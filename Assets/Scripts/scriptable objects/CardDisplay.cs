using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public cards card;

    public TMP_Text nameText;
    public TMP_Text damageText;
    public TMP_Text bulletsText;
    public TMP_Text cooldownText;

    public Button selectButton;

    void Start()
    {
        selectButton.onClick.AddListener(OnCardSelected);
    }

    

    public void SetCard(cards newCard)
    {
        card = newCard;  // Asignar la carta al slot

        if (card == null)
        {
            Debug.LogError("No card assigned in SetCard!");
        }
        else
        {
            // Actualizar UI
            nameText.text = card.name;
            damageText.text = card.damage.ToString();
            bulletsText.text = card.bullets.ToString();
            cooldownText.text = card.cooldown.ToString();
        }
    }

    void OnCardSelected()
    {
        if (card != null)
        {
            Debug.Log($"{card.name} selected!");
        }
        else
        {
            Debug.LogError("No card assigned in OnCardSelected!");
        }
    }
}
