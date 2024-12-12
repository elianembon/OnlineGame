using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ReconnectMenu : MonoBehaviourPunCallbacks
{
    public GameObject PanelLostSession;
    void Start()
    {
        PanelLostSession.SetActive(false);
        
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        ShowPanelFailed();
    }
    public void ShowPanelFailed()
    {
        PanelLostSession.SetActive(true);

        StartCoroutine(WaitBeforeGoToMenu());
    }

    private IEnumerator WaitBeforeGoToMenu()
    {
        Debug.Log("Esperando 5 segundos antes de entrar ir al menu...");
        yield return new WaitForSeconds(5f); // Espera 5 segundos para volver al menu
        PhotonNetwork.LoadLevel("Menu");
    }
}
