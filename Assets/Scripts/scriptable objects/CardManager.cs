using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<cards> allCards; // Lista de todas las cartas disponibles

    public cards GetRandomCard()
    {
        // Seleccionar una carta al azar de la lista
        return allCards[Random.Range(0, allCards.Count)];
    }
}
