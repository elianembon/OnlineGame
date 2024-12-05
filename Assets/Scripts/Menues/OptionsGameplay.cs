using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsGameplay : MonoBehaviour
{
    public GameObject menuPause;
    private bool isPaused = false;

    private void Update()
    {
        // Detectar si se presionó la tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        menuPause.SetActive(true); // Mostrar el menú de pausa
        Debug.Log("Juego Pausado");

    }

    public void Resume()
    {
        isPaused = false;
        menuPause.SetActive(false); // Ocultar el menú de pausa
        Debug.Log("Juego despausado");

    }

}
