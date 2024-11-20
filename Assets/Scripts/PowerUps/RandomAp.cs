using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAp : MonoBehaviour
{
    public GameObject Object;
    public Vector2 spawnAreaMin; //segun el tamaño actual del mapa, seria x: -160 y y: -65
    public Vector2 spawnAreaMax; //segun el tamaño actual del mapa, seria x: 170 y y: 75

    void Start()
    {
        SpawnObjRm();
    }

    void SpawnObjRm()
    {
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);

        Vector2 randPos = new Vector2(randomX, randomY);

        Instantiate(Object, randPos, Quaternion.identity);

    }
    /*Para cuando obj aparezca en otro lado al ser consumido, funciona bien
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);

            Invoke("SpawnObjRm", 0.5f);

        }

    }*/

}
