using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DamagePowerUp : MonoBehaviour
{
    [SerializeField] private int damageBoost = 10; // Aumento de daño

    private void Update()
    {
        CheckForPlayerPickup();
    }

    private void CheckForPlayerPickup()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView playerView = player.GetComponent<PhotonView>();
            if (playerView != null && CircleRectangleCollision(transform.position, 0.5f, player.transform.position, player.transform.localScale))
            {
                ApplyDamageBoost(playerView);
                Destroy(gameObject); // Destruir el objeto después de que lo haya recogido el jugador
                return;
            }
        }
    }

    private void ApplyDamageBoost(PhotonView playerView)
    {
        if (PhotonNetwork.IsMasterClient) // Solo el MasterClient envía el RPC.
        {
            playerView.RPC("IncreaseBulletDamage", RpcTarget.AllBuffered, damageBoost); // Aumenta el daño de las balas
        }
    }

    private bool CircleRectangleCollision(Vector3 circlePos, float circleRadius, Vector3 rectPos, Vector3 rectSize)
    {
        float dx = Mathf.Max(rectPos.x - rectSize.x / 2, Mathf.Min(circlePos.x, rectPos.x + rectSize.x / 2));
        float dy = Mathf.Max(rectPos.y - rectSize.y / 2, Mathf.Min(circlePos.y, rectPos.y + rectSize.y / 2));
        return (circlePos.x - dx) * (circlePos.x - dx) + (circlePos.y - dy) * (circlePos.y - dy) < circleRadius * circleRadius;
    }
}
