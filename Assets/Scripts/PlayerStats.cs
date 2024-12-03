using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerStats : MonoBehaviourPun
{
    public int health;
    public int score;

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

    private PlayerStats FindPlayerStatsByPhotonPlayer(Photon.Realtime.Player photonPlayer)
    {
        PlayerStats[] allPlayers = FindObjectsOfType<PlayerStats>();
        foreach (PlayerStats playerStats in allPlayers)
        {
            if (playerStats.photonView.Owner == photonPlayer) // Compara propietarios
            {
                return playerStats;
            }
        }
        return null; // No se encontró un jugador con ese PhotonPlayer
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
