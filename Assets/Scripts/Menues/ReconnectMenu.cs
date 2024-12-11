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
        reconnectPanel.SetActive(false);
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        StartCoroutine(WaitBeforeStartGame());
    }



    private IEnumerator WaitBeforeStartGame()
    {
        
        yield return new WaitForSeconds(5f);
        connectPanel.SetActive(false);
        NotifySpawnPlayer(); // Notifica que el jugador puede ser instanciado
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        ShowPanelFailed();
    }
    public void ShowPanelFailed()
    {
        reconnectPanel.SetActive(true);
        StartCoroutine(WaitBeforeGoToMenu());
    }

    private IEnumerator WaitBeforeGoToMenu()
    {
        Debug.Log("Esperando 5 segundos antes de entrar ir al menu...");
        yield return new WaitForSeconds(5f); // Espera 5 segundos para volver al menu
        PhotonNetwork.LoadLevel("Menu");
    }


    private void NotifySpawnPlayer()
    {
        PlayerSpawn playerSpawn = FindObjectOfType<PlayerSpawn>();
        if (playerSpawn != null)
        {

            playerSpawn.SpawnPlayer(); // Llama al método para instanciar al jugador
        }
        else
        {
            Debug.LogError("No se encontró el script PlayerSpawn en la escena.");
        }
    }
}
