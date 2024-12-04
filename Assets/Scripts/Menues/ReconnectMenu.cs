using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ReconnectMenu : MonoBehaviourPunCallbacks
{
    public GameObject reconnectPanel;
    public GameObject connectPanel;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        StartCoroutine(WaitBeforeStartGame());

    }

    public void ShowPanelFailed()
    {
        reconnectPanel.SetActive(true);
        connectPanel.SetActive(false);
        StartCoroutine(WaitBeforeGoToMenu());

    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        ShowPanelFailed();
    }

    /*public override void OnConnectedToMaster()
    {
        StartCoroutine(WaitBeforeStartGame());
    }*/

    private IEnumerator WaitBeforeStartGame()
    {
        Debug.Log("Esperando 5 segundos antes de entrar a la partida...");
        yield return new WaitForSeconds(5f); // Espera 5 segundos
        StartGame();
    }

    private IEnumerator WaitBeforeGoToMenu()
    {
        Debug.Log("Esperando 5 segundos antes de entrar ir al menu...");
        yield return new WaitForSeconds(5f); // Espera 5 segundos para volver al menu
        PhotonNetwork.LoadLevel("Menu");
    }


    public void StartGame()
    {
        Debug.Log("Intentando iniciar el juego...");

        if (PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Photon está conectado. Cargando la escena Gameplay...");
            PhotonNetwork.LoadLevel("Gameplay");
        }
        else
        {
            Debug.LogError("Photon no está conectado o listo. No se puede cargar la escena Gameplay.");
        }
    }

}
