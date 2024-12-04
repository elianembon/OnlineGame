using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shoot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab; // Prefab de la bala
    [SerializeField] private Transform bulletSpawnPoint; // Punto de origen de la bala
    [SerializeField] private float bulletSpeed = 10f; // Velocidad de la bala

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView no encontrado en el objeto del jugador.");
        }
    }

    private void Update()
    {
        // Verifica si este es el jugador local para evitar que otros controlen disparos.
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Space))
        {
            Shooting();
        }
    }

    private void Shooting()
    {
        if (!GgGameManager.canShootAndPlaySone)
        {
            Debug.Log("No se puede disparar en este momento.");
            return;
        }

        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            // Instancia la bala usando PhotonNetwork para sincronizarla en todos los jugadores.
            PhotonNetwork.Instantiate(bulletPrefab.name, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

            Debug.Log($"Jugador {PhotonNetwork.NickName} disparó una bala.");
        }
        else
        {
            Debug.LogError("No se asignaron el prefab de bala o el punto de spawn.");
        }
    }
}