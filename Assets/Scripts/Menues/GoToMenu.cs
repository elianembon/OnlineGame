using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToMenu : MonoBehaviour
{
    public GameObject CanvasPause;

    private void Start()
    {
        CanvasPause.SetActive(false);
    }

    public void goToMenu()
    {
        // Aseg�rate de que este c�digo se ejecute solo para el jugador local
        if (PhotonNetwork.IsConnected && PhotonNetwork.LocalPlayer != null)
        {
            // Encuentra el GameObject del jugador local, que debe tener el componente PlayerController
            GameObject localPlayerObject = PhotonNetwork.LocalPlayer.TagObject as GameObject;

            if (localPlayerObject != null)
            {
                PlayerController playerController = localPlayerObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.DisconnectPlayer(); // Llamamos a la funci�n DisconnectPlayer
                }
            }

            // Cargar la escena del men� solo para el jugador que presion� el bot�n
            PhotonNetwork.LoadLevel("Menu");
        }
    }
}