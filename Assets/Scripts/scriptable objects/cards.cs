using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class cards :ScriptableObject
{
    public string name;
    public float damage;
    public float cooldown;
    public int bullets;

    public void Print()
    {
        Debug.Log(name + ": " + damage + cooldown + bullets);
    }
}
