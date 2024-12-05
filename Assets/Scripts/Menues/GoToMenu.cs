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
        PhotonNetwork.LoadLevel("Menu");
    }

}
