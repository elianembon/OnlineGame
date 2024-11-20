using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public cards card;

    public Text nameText;
    public Text damageText;
    public Text bulletsText;
    public Text cooldownText;

    public Button selectButton;

    void Start()
    {
        nameText.text = card.name;
        damageText.text = "Damage: " + card.damage.ToString();
        bulletsText.text = "Bullets: " + card.bullets.ToString();
        cooldownText.text = "Cooldown: " + card.cooldown.ToString();

        // Agrega un listener al botón
        selectButton.onClick.AddListener(OnCardSelected);

    }

    void OnCardSelected()
    {
        Debug.Log($"{card.name} selected!");
        
    }
}
