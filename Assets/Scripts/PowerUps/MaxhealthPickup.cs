using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MaxHealthPickup : MonoBehaviour
{
    [SerializeField] private float radius = 0.5f; // Radio del �rea de colisi�n.
    [SerializeField] private int maxHealthIncrease = 30; // Incremento de vida m�xima.
    [SerializeField] private float lifetime = 10f; // Tiempo de vida del objeto.

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destruir el objeto autom�ticamente despu�s del tiempo de vida.
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient) // Solo el MasterClient procesa colisiones.
        {
            CheckCollision();
        }
    }

    private void CheckCollision()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView playerView = player.GetComponent<PhotonView>();
            if (playerView != null && CircleRectangleCollision(transform.position, radius, player.transform.position, player.transform.localScale))
            {
                NotifyPlayerMaxHealth(playerView.ViewID);
                DestroyPickup();
                return;
            }
        }
    }

    private void NotifyPlayerMaxHealth(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            targetView.RPC("IncreaseMaxHealthOnly", RpcTarget.AllBuffered, maxHealthIncrease);
        }
    }

    private void DestroyPickup()
    {
        PhotonNetwork.Destroy(gameObject); // El MasterClient destruye el objeto.
    }

    private bool CircleRectangleCollision(Vector3 circlePos, float circleRadius, Vector3 rectPos, Vector3 rectSize)
    {
        float dx = Mathf.Max(rectPos.x - rectSize.x / 2, Mathf.Min(circlePos.x, rectPos.x + rectSize.x / 2));
        float dy = Mathf.Max(rectPos.y - rectSize.y / 2, Mathf.Min(circlePos.y, rectPos.y + rectSize.y / 2));
        return (circlePos.x - dx) * (circlePos.x - dx) + (circlePos.y - dy) * (circlePos.y - dy) < circleRadius * circleRadius;
    }
}