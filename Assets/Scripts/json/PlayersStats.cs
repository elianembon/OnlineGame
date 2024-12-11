using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayersStats:MonoBehaviour
{
    public float damage;
    public float cooldown;
    public int bullets;

    // Constructor predeterminado
    public PlayersStats()
    {
        damage = 10f;
        cooldown = 1.0f;
        bullets = 10;
    }
}
