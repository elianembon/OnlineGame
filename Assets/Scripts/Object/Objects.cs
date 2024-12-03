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
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("dañado, salud de ahora" + currentHealth);

        if (currentHealth <= 0 )
        {
            Destroy(gameObject);
            PhotonNetwork.Destroy(photonView);
            Debug.Log("murioelobjeto");
        }
    }

}
