using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using System;

public class PlayerStatsManager : MonoBehaviour
{
    //private string playerStatsFolder;
    //public static PlayerStatsManager Instance;
    //private string statsDirectory;

    //private void Start()
    //{
    //    // Verifica que la instancia de PlayerStatsManager existe
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //    }

    //    // Carga las estadísticas directamente desde PlayerStats
    //    string playerID = PhotonNetwork.NickName;
    //    PlayerStats stats = FindPlayerStatsByPlayerID(playerID);

    //    if (stats != null)
    //    {
    //        Debug.Log($"Cargando estadísticas para: {playerID}");
    //        Debug.Log($"Damage: {stats.currentDamage}, Cooldown: {stats.currentCooldown}, Bullets: {stats.currentBullets}");
    //    }
    //    else
    //    {
    //        Debug.LogError($"No se encontraron estadísticas para el jugador {playerID}");
    //    }
    //}

    //public PlayerStats FindPlayerStatsByPlayerID(string playerID)
    //{
    //    PlayerStats[] allPlayers = FindObjectsOfType<PlayerStats>();
    //    foreach (PlayerStats playerStats in allPlayers)
    //    {
    //        if (playerStats.photonView.Owner.NickName == playerID)
    //        {
    //            return playerStats;
    //        }
    //    }
    //    return null; // Si no se encuentra, devuelve null
    //}

    //// Método para actualizar estadísticas
    //public void UpdateStat(string playerID, string statName, float value)
    //{
    //    PlayerStats stats = FindPlayerStatsByPlayerID(playerID);
    //    if (stats == null)
    //    {
    //        Debug.LogError($"No se encontraron estadísticas para el jugador {playerID}");
    //        return;
    //    }

    //    switch (statName)
    //    {
    //        case "damage":
    //            stats.currentDamage = value;
    //            break;
    //        case "cooldown":
    //            stats.currentCooldown = value;
    //            break;
    //        case "bullets":
    //            stats.currentBullets = (int)value;
    //            break;
    //    }

    //    Debug.Log($"Estadísticas de {playerID} actualizadas: Damage: {stats.currentDamage}, Cooldown: {stats.currentCooldown}, Bullets: {stats.currentBullets}");
    //}
}