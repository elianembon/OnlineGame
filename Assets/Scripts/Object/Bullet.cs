using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviour
{

    [SerializeField] private float speed = 10f; // Velocidad de la bala.
    [SerializeField] private float radius = 0.2f; // Radio de la bala para detecci�n de colisiones.
    [SerializeField] private float lifetime = 5f; // Tiempo de vida de la bala antes de destruirse.

    private void Start()
    {
        // Destruir la bala autom�ticamente despu�s del tiempo de vida.
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Movimiento constante hacia adelante.
        transform.position += transform.right * speed * Time.deltaTime;

        // Detectar colisi�n con jugadores.
        CheckCollision();
    }

    private void CheckCollision()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView playerView = player.GetComponent<PhotonView>();

            // Verifica que el jugador no sea nulo y que tenga un PhotonView.
            if (playerView != null)
            {
                Vector3 playerPosition = player.transform.position;
                Vector3 bulletPosition = transform.position;

                // Detecta colisi�n entre el c�rculo (bala) y el rect�ngulo (jugador).
                if (CircleRectangleCollision(bulletPosition, radius, playerPosition, player.transform.localScale))
                {
                    // Llama a TakeDamage en el jugador correspondiente.
                    playerView.RPC("TakeDamage", RpcTarget.AllBuffered, 10); // Aplica 10 de da�o.

                    // Destruye la bala.
                    DestroyBullet();
                    break;
                }
            }
        }
    }

    private void DestroyBullet()
    {
        if (PhotonNetwork.IsConnected)
        {
            // Usa PhotonNetwork.Destroy para sincronizar la destrucci�n con todos los clientes.
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            // En caso de no estar conectado a Photon, usa Destroy normal (modo offline).
            Destroy(gameObject);
        }
    }

    private bool CircleRectangleCollision(Vector3 circlePos, float circleRadius, Vector3 rectPos, Vector3 rectSize)
    {
        // Calcula la distancia m�s cercana entre el c�rculo y el rect�ngulo.
        float dx = Mathf.Max(rectPos.x - rectSize.x / 2, Mathf.Min(circlePos.x, rectPos.x + rectSize.x / 2));
        float dy = Mathf.Max(rectPos.y - rectSize.y / 2, Mathf.Min(circlePos.y, rectPos.y + rectSize.y / 2));

        // Comprueba si la distancia entre el c�rculo y el punto m�s cercano es menor al radio del c�rculo.
        return (circlePos.x - dx) * (circlePos.x - dx) + (circlePos.y - dy) * (circlePos.y - dy) < circleRadius * circleRadius;
    }
}