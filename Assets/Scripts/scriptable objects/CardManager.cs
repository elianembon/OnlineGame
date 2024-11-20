using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<cards> allCards; // Lista con todas las cartas disponibles
    public List<cards> currentSelection; // Lista de cartas mostradas al jugador
    public GameObject cardPrefab; // Prefab del UI de la carta
    public Transform cardParent; // Contenedor para las cartas

    void Start()
    {
        GenerateCardSelection();
    }

    void GenerateCardSelection()
    {
        currentSelection.Clear();
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }

        // Escoge 2 cartas al azar para mostrar
        for (int i = 0; i < 2; i++)
        {
            int randomIndex = Random.Range(0, allCards.Count);
            cards selectedCard = allCards[randomIndex];
            currentSelection.Add(selectedCard);

            // Instancia y configura la carta en la UI
            GameObject cardObj = Instantiate(cardPrefab, cardParent);
            CardDisplay display = cardObj.GetComponent<CardDisplay>();
            display.card = selectedCard;
        }
    }
}
