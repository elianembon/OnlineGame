using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private float radius = 0.5f; // Radio del área de colisión.
    [SerializeField] private int healthAmount = 20; // Cantidad de vida a restaurar.
    [SerializeField] private float lifetime = 10f; // Tiempo de vida del objeto.

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destruir el objeto automáticamente después del tiempo de vida.
    }

    private void Update()
    {
        CheckCollision();
    }

    private void CheckCollision()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView playerView = player.GetComponent<PhotonView>();
            if (playerView != null && CircleRectangleCollision(transform.position, radius, player.transform.position, player.transform.localScale))
            {
                HealCurrentHealth(playerView);
                DestroyPickup();
                return;
            }
        }
    }

    private void HealCurrentHealth(PhotonView targetView)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            targetView.RPC("HealPlayerCurrentHealth", RpcTarget.AllBuffered, healthAmount);
        }
    }

    private void DestroyPickup()
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