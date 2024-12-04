using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class SessionLostInGameplay : MonoBehaviourPunCallbacks
{
    public GameObject reconnectPanel;
    void Start()
    {
        reconnectPanel.SetActive(false);
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        ShowPanelFailed();
    }
    public void ShowPanelFailed()
    {
        reconnectPanel.SetActive(true);
    }

    public void GoToMenu()
    {
        PhotonNetwork.LoadLevel("Menu");
    }

}
