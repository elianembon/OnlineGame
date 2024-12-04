using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{
    private List<TextMeshProUGUI> countdownTexts = new List<TextMeshProUGUI>(); // Lista de textos de cada jugador

    public void RegisterCountdownText(TextMeshProUGUI text)
    {
        if (!countdownTexts.Contains(text)) // Evitar registros duplicados
        {
            countdownTexts.Add(text);
        }
    }

    public void UpdateCountdown(int seconds)
    {
        string message = seconds > 0 ? seconds.ToString() : "";

        foreach (TextMeshProUGUI text in countdownTexts)
        {
            if (text != null) text.text = message;
        }
    }
}
