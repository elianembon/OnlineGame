using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerStats : MonoBehaviourPun
{
    public int score;
    public float baseDamage = 10f;
    public float baseCooldown = 1f;
    public int baseBullets = 1;
    public float currentDamage;
    public float currentCooldown;
    public int currentBullets;
    private bool isDead = false;

    void Start()
    {
        // Inicializar las estadísticas actuales con las bases
        currentDamage = baseDamage;
        currentCooldown = baseCooldown;
        currentBullets = baseBullets;
    }

    public void DisableMovement()
    {
        if (!isDead)
        {
            // Desactivar la capacidad de moverse
            isDead = true;
            GetComponent<PlayerController>().enabled = false; // Desactiva el script de control
            Debug.Log($"{name} desactivado.");
        }
    }

    public void EnableMovement()
    {
        if (isDead)
        {
            // Reactivar la capacidad de moverse
            isDead = false;
            GetComponent<PlayerController>().enabled = true; // Reactiva el script de control
            Debug.Log($"{name} activado.");
        }
    }

    [PunRPC]
    public void ApplyCardEffect(float damageModifier, float cooldownModifier, int bulletModifier)
    {
        currentDamage += damageModifier;
        currentCooldown += cooldownModifier;
        currentBullets += bulletModifier;

        Debug.Log($"Nuevas estadísticas -> Daño: {currentDamage}, Cooldown: {currentCooldown}, Balas: {currentBullets}");
    }
}
