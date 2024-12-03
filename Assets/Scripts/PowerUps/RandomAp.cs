using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAp : MonoBehaviour
{
    public GameObject thing;
    public Vector2 spawnAreaMin; //segun el tamaño actual del mapa, seria x: -160 y y: -65
    public Vector2 spawnAreaMax; //segun el tamaño actual del mapa, seria x: 170 y y: 75

    /*public int spawnCount;
    public int spawnCountMax;*/

    void Start()
    {
        SpawnObjRm();
    }

    void SpawnObjRm()
    {
        /*if(spawnCount < spawnCountMax)
        {*/
            float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);

            Vector2 randPos = new Vector2(randomX, randomY);

        PhotonNetwork.Instantiate(thing.name, randPos, Quaternion.identity);

        /*  spawnCount++;
      }
      else
      {
          Debug.Log("No mas power ups");
      }

      */
    }
    /*Para cuando obj aparezca en otro lado al ser consumido
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            PhotonNetwork.Destroy (photonView) (para que funcione tiene que ser un MonobehaviorPun)

       if (spawnCount < spawnCountMax)
            {
                Invoke("SpawnObjRm", 0.5f);
            }

        }

    }*/

}
