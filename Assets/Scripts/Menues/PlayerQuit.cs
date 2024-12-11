using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlayerQuit : MonoBehaviourPunCallbacks
{
     private const int MinPlayers = 1;

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        if (PhotonNetwork.CurrentRoom.PlayerCount == MinPlayers)
        {
            GoToMenu();
        }
    }
    private void GoToMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("GameFinished");
    }

}
