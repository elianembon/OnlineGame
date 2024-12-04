using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private int countdownTime = 5;
     void Start()
    {
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        while (countdownTime >= 0)
        {
            timerText.text = countdownTime.ToString(); // Actualiza el texto
            yield return new WaitForSeconds(1f); // Espera 1 segundo
            countdownTime--; // Resta 1 al tiempo
        }
    }


}
