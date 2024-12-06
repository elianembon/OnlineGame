using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAp : MonoBehaviour
{
    public List<GameObject> things; // Lista de objetos a instanciar
    public Vector2 spawnAreaMin = new Vector2(-160, -65); // Coordenadas mínimas del área de spawn
    public Vector2 spawnAreaMax = new Vector2(170, 75);  // Coordenadas máximas del área de spawn
    public int spawnCount = 10; // Cantidad de objetos a instanciar

    void Start()
    {
        // Solo el MasterClient puede instanciar objetos
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnObjects();
        }
    }

    void SpawnObjects()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // Seleccionar un objeto aleatorio de la lista
            GameObject randomThing = things[Random.Range(0, things.Count)];

            if (randomThing == null)
            {
                Debug.LogError("El prefab seleccionado es nulo. Revisa la lista de prefabs.");
                continue;
            }

            // Generar una posición aleatoria dentro del área
            float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            Vector2 randomPosition = new Vector2(randomX, randomY);

            // Instanciar el objeto en la red
            GameObject spawnedObject = PhotonNetwork.Instantiate(randomThing.name, randomPosition, Quaternion.identity);

            // Depuración para verificar el objeto instanciado
            Debug.Log($"Instanciado: {spawnedObject.name} en posición {randomPosition}");
        }
    }
}