using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public CardManager cardManager; 
    public CardDisplay[] cardSlots; 

    void Start()
    {
       
        foreach (CardDisplay slot in cardSlots)
        {
            cards randomCard = cardManager.GetRandomCard();
            slot.SetCard(randomCard);
        }
    }
}
