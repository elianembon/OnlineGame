using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAp : MonoBehaviour
{
    public List<GameObject> things; // Lista de objetos a instanciar
    public Vector2 spawnAreaMin; //segun el tamaño actual del mapa, seria x: -160 y y: -65
    public Vector2 spawnAreaMax; //segun el tamaño actual del mapa, seria x: 170 y y: 75
    public int spawnCount;
    /*public int spawnCount;
    public int spawnCountMax;*/

    void Start()
    {
        SpawnObjRm();
    }

    void SpawnObjRm()
    {
        for (int i = 0; i < spawnCount; i++) // Generar "spawnCount" objetos
        {
            // Seleccionar un objeto aleatorio de la lista
            GameObject randomThing = things[Random.Range(0, things.Count)];

            float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            Vector2 randPos = new Vector2(randomX, randomY);

            PhotonNetwork.Instantiate(randomThing.name, randPos, Quaternion.identity);
        }
  
    }
  

}
