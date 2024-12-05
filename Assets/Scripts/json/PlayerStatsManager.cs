using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using System;

public class PlayerStatsManager : MonoBehaviour
{
    private string playerStatsFolder;
    public static PlayerStatsManager Instance;
    private string statsDirectory;

    private void Start()
    {
        // Establecer la carpeta donde se guardar�n los archivos de estad�sticas.
        playerStatsFolder = Path.Combine(Application.persistentDataPath, "Stats");

        // Crear la carpeta si no existe
        if (!Directory.Exists(playerStatsFolder))
        {
            Directory.CreateDirectory(playerStatsFolder);
            Debug.Log("Carpeta de estad�sticas creada correctamente.");
        }

        // Aseg�rate de que cada jugador tiene su archivo de estad�sticas.
        string playerID = PhotonNetwork.NickName;
        PlayersStats stats = LoadStats(playerID);

        Debug.Log($"Cargando estad�sticas para: {playerID}");
        Debug.Log($"Damage: {stats.damage}, Cooldown: {stats.cooldown}, Bullets: {stats.bullets}");
    }

    public PlayersStats LoadStats(string playerID)
    {
        string filePath = Path.Combine(playerStatsFolder, $"{playerID}.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayersStats stats = JsonUtility.FromJson<PlayersStats>(json);
            return stats;
        }
        else
        {
            Debug.LogWarning($"No se encontr� el archivo de estad�sticas para el jugador {playerID}. Creando estad�sticas predeterminadas.");
            return new PlayersStats();  // Si no existe el archivo, crea una nueva instancia con valores predeterminados
        }
    }

    public void SaveStats(string playerID, PlayersStats stats)
    {
        string filePath = Path.Combine(playerStatsFolder, $"{playerID}.json");
        string json = JsonUtility.ToJson(stats, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"Estad�sticas guardadas para {playerID}.");
    }

    public void UpdateStat(string playerID, string statName, float value)
    {
        PlayersStats stats = LoadStats(playerID);

        switch (statName)
        {
            case "damage":
                stats.damage = value;
                break;
            case "cooldown":
                stats.cooldown = value;
                break;
            case "bullets":
                stats.bullets = (int)value;
                break;
        }

        SaveStats(playerID, stats);
    }
}