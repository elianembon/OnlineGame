using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class Bullet : MonoBehaviour
{

    [SerializeField] private float speed = 10f;
    [SerializeField] private float radius = 0.2f;
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
        CheckCollision();
    }

    private void CheckCollision()
    {
        PhotonView pv = GetComponent<PhotonView>();

        if (pv != null && !pv.IsMine) return; // Solo el propietario de la bala procesa la colisión.

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PhotonView playerView = hit.GetComponent<PhotonView>();
                if (playerView != null)
                {
                    ApplyDamage(playerView, 10);
                    DestroyBullet();
                    return;
                }
            }
        }

        // Detectar colisión con objetos destructibles.
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Destructible"))
        {
            PhotonView objectView = obj.GetComponent<PhotonView>();
            ObjectsHealth healthComponent = obj.GetComponent<ObjectsHealth>();
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
        if (targetView != null)
        {
            targetView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
        }
    }

    private void DestroyBullet()
    {
        PhotonView pv = GetComponent<PhotonView>();

        if (PhotonNetwork.IsConnected && pv != null && pv.IsMine)
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