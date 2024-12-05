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
        // Establecer la carpeta donde se guardarán los archivos de estadísticas.
        playerStatsFolder = Path.Combine(Application.persistentDataPath, "Stats");

        // Crear la carpeta si no existe
        if (!Directory.Exists(playerStatsFolder))
        {
            Directory.CreateDirectory(playerStatsFolder);
            Debug.Log("Carpeta de estadísticas creada correctamente.");
        }

        // Asegúrate de que cada jugador tiene su archivo de estadísticas.
        string playerID = PhotonNetwork.NickName;
        PlayersStats stats = LoadStats(playerID);

        Debug.Log($"Cargando estadísticas para: {playerID}");
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
            Debug.LogWarning($"No se encontró el archivo de estadísticas para el jugador {playerID}. Creando estadísticas predeterminadas.");
            return new PlayersStats();  // Si no existe el archivo, crea una nueva instancia con valores predeterminados
        }
    }

    public void SaveStats(string playerID, PlayersStats stats)
    {
        string filePath = Path.Combine(playerStatsFolder, $"{playerID}.json");
        string json = JsonUtility.ToJson(stats, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"Estadísticas guardadas para {playerID}.");
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