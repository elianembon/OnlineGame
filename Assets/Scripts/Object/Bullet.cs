using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class Bullet : MonoBehaviour
{

    [SerializeField] private float speed = 10f; // Velocidad de la bala.
    [SerializeField] private float radius = 0.2f; // Radio de la bala para detección de colisiones.
    [SerializeField] private float lifetime = 5f; // Tiempo de vida de la bala antes de destruirse.

    private void Start()
    {
        // Destruir la bala automáticamente después del tiempo de vida.
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Movimiento constante hacia adelante.
        transform.position += transform.right * speed * Time.deltaTime;

        // Detectar colisiones.
        CheckCollision();
    }

    private void CheckCollision()
    {
        // Detectar colisión con jugadores.
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView playerView = player.GetComponent<PhotonView>();
            if (playerView != null && CircleRectangleCollision(transform.position, radius, player.transform.position, player.transform.localScale))
            {
                ApplyDamage(playerView, 10); // Aplica 10 de daño.
                DestroyBullet();
                return;
            }
        }

        // Detectar colisión con objetos destructibles.
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Destructible"))
        {
            PhotonView objectView = obj.GetComponent<PhotonView>();
            if (objectView != null && CircleRectangleCollision(transform.position, radius, obj.transform.position, obj.transform.localScale))
            {
                ApplyDamage(objectView, 10); // Aplica 10 de daño.
                DestroyBullet();
                return;
            }
        }
    }

    private void ApplyDamage(PhotonView targetView, int damage)
    {
        if (PhotonNetwork.IsMasterClient) // Solo el MasterClient envía el RPC.
        {
            targetView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
        }
    }

    private void DestroyBullet()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool CircleRectangleCollision(Vector3 circlePos, float circleRadius, Vector3 rectPos, Vector3 rectSize)
    {
        float dx = Mathf.Max(rectPos.x - rectSize.x / 2, Mathf.Min(circlePos.x, rectPos.x + rectSize.x / 2));
        float dy = Mathf.Max(rectPos.y - rectSize.y / 2, Mathf.Min(circlePos.y, rectPos.y + rectSize.y / 2));
        return (circlePos.x - dx) * (circlePos.x - dx) + (circlePos.y - dy) * (circlePos.y - dy) < circleRadius * circleRadius;
    }
}