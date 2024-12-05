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



    private IEnumerator WaitBeforeStartGame()
    {
        
        yield return new WaitForSeconds(5f);
        connectPanel.SetActive(false);
        NotifySpawnPlayer(); // Notifica que el jugador puede ser instanciado
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
