using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectsHealth : MonoBehaviourPun
{
    public int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!PhotonNetwork.IsMasterClient) return; // Solo el MasterClient procesa el da�o.

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} recibi� da�o. Salud restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject); // Destruir el objeto sincronizadamente.
            Debug.Log($"{gameObject.name} ha sido destruido.");
        }
    }
}