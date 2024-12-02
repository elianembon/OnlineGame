using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerStats : MonoBehaviourPun
{
    public float baseDamage = 10f;
    public float baseCooldown = 1f;
    public int baseBullets = 1;

    public float currentDamage;
    public float currentCooldown;
    public int currentBullets;

    void Start()
    {
        // Inicializar las estadísticas actuales con las bases
        currentDamage = baseDamage;
        currentCooldown = baseCooldown;
        currentBullets = baseBullets;
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
