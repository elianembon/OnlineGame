using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class panelWin : MonoBehaviourPunCallbacks
{
    public GameObject PanelWin;

    public GameObject PanelLost;


    void Start()
    {
        PanelWin.SetActive(false);
        PanelLost.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
