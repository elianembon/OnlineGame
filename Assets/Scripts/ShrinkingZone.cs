using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkingZone : MonoBehaviour
{
    private PhotonView pv;
    private float shrinkSpeed = 0.1f; // Velocidad de reducci�n
    private float minScale = 0.1f; // Escala m�nima del objeto
    public Vector2 finalPosition = Vector2.zero; // Posici�n final

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Soy el Master Client. Controlando la zona segura.");
        }
        else
        {
            Debug.Log("Cliente regular. Solo observando la zona segura.");
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && GgGameManager.canShootAndPlaySone)
        {
            ShrinkZone();
        }
    }

    void ShrinkZone()
    {
        // Reducir la escala y mover el objeto solo si la escala no es menor al m�nimo
        if (transform.localScale.x > minScale)
        {
            // Calcular nueva escala y posici�n
            Vector3 newScale = Vector3.MoveTowards(transform.localScale, Vector3.one * minScale, shrinkSpeed * Time.deltaTime);
            Vector2 newPosition = Vector2.MoveTowards(transform.position, finalPosition, shrinkSpeed * Time.deltaTime);

            // Aplicar cambios localmente
            transform.localScale = newScale;
            transform.position = newPosition;

            // Sincronizar con otros jugadores
            pv.RPC("SyncZone", RpcTarget.Others, newScale, newPosition);
        }
        else
        {
            Debug.Log("La zona segura ha alcanzado el tama�o m�nimo.");
        }
    }

    [PunRPC]
    void SyncZone(Vector3 scale, Vector2 position)
    {
        // Aplicar cambios en clientes remotos
        transform.localScale = scale;
        transform.position = position;
    }
}