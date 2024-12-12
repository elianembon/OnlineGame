using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class LoadingGame : MonoBehaviourPunCallbacks
{
    public GameObject LoadGameplayPanel;

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
        LoadGameplayPanel.SetActive(false);
        NotifySpawnPlayer(); // Notifica que el jugador puede ser instanciado
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void NotifySpawnPlayer()
    {
        PlayerSpawn playerSpawn = FindObjectOfType<PlayerSpawn>();
        if (playerSpawn != null)
        {

            playerSpawn.SpawnPlayer(); // Llama al m�todo para instanciar al jugador
        }
        else
        {
            Debug.LogError("No se encontr� el script PlayerSpawn en la escena.");
        }
    }


}
