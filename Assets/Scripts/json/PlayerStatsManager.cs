using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance;
    private string statsDirectory;
    private Dictionary<string, PlayersStats> playerStatsDictionary;

    private void Awake()
    {
        // Singleton para acceso global
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        statsDirectory = Application.persistentDataPath + "/PlayersStats/";
        playerStatsDictionary = new Dictionary<string, PlayersStats>();

        // Crear directorio si no existe
        if (!Directory.Exists(statsDirectory))
            Directory.CreateDirectory(statsDirectory);
    }

    public void LoadStats(string playerID)
    {
        string filePath = statsDirectory + playerID + ".json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            playerStatsDictionary[playerID] = JsonUtility.FromJson<PlayersStats>(json);
        }
        else
        {
            // Crear estadísticas por defecto si el archivo no existe
            PlayersStats defaultStats = new PlayersStats
            {
                damage = 10.0f,
                cooldown = 1.5f,
                bullets = 25
            };
            playerStatsDictionary[playerID] = defaultStats;
            SaveStats(playerID);
        }
    }

    public void SaveStats(string playerID)
    {
        if (playerStatsDictionary.TryGetValue(playerID, out PlayersStats stats))
        {
            string json = JsonUtility.ToJson(stats, true);
            File.WriteAllText(statsDirectory + playerID + ".json", json);
        }
    }

    public PlayersStats GetStats(string playerID)
    {
        if (playerStatsDictionary.TryGetValue(playerID, out PlayersStats stats))
            return stats;

        return null;
    }

    public void UpdateStat(string playerID, string statName, float value)
    {
        if (playerStatsDictionary.TryGetValue(playerID, out PlayersStats stats))
        {
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
            SaveStats(playerID);
        }
    }
}
