using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<cards> allCards;  // Lista de todas las cartas disponibles

    // M�todo que devuelve una carta por su nombre
    public cards GetCardByName(string cardName)
    {
        foreach (cards card in allCards)
        {
            if (card.name == cardName)
            {
                return card;
            }
        }
        Debug.LogError($"No se encontr� la carta con el nombre {cardName}.");
        return null;  // Retorna null si no encuentra la carta
    }

    // M�todo para obtener una carta aleatoria (puedes ajustarlo como necesites)
    public cards GetRandomCard()
    {
        if (allCards.Count == 0)
        {
            Debug.LogError("No hay cartas disponibles.");
            return null;
        }

        int randomIndex = Random.Range(0, allCards.Count);
        return allCards[randomIndex];
    }
}

