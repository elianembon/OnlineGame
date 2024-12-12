using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KeyboardDamage : MonoBehaviourPunCallbacks
{
    /*public ObjectsHealth healthManager;
    public int damageAmount = 20;
    public KeyCode damageKey = KeyCode.Space;  // Tecla para causar daño*/
    public KeyCode Disconnect = KeyCode.Tab;  // Tecla para desconectar wifi 
    public KeyCode Reconnect = KeyCode.O;  // Tecla para conectar wifi 

    void Update()
    {
       /* if (Input.GetKeyDown(damageKey))
        {
            if (healthManager != null)
            {
                healthManager.TakeDamage(damageAmount);
            }
        }*/

        if (Input.GetKeyDown(Disconnect))
        {
            PhotonNetwork.Disconnect();
            Debug.Log("FuisteDesconectado");
        }
        if (Input.GetKeyDown(Reconnect))
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Fuisteconectado");
        }
    }
}