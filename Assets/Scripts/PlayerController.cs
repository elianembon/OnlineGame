using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private PhotonView pv;
    private Camera camera;
    private int health = 60; // Vida inicial de la nave

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        camera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        camera.gameObject.SetActive(pv.IsMine);
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            // Movimiento
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += Vector3.up * 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position += -Vector3.up * 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += -Vector3.right * 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.right * 5 * Time.deltaTime;
            }

            // Restar vida al presionar K
            if (Input.GetKeyDown(KeyCode.K))
            {
                pv.RPC("TakeDamage", RpcTarget.AllBuffered, 15);
            }
        }
    }

    [PunRPC]
    void TakeDamage(int damage)
    {
        if (!pv.IsMine) return; // Asegura que solo afecta a esta nave

        health -= damage;
        Debug.Log($"Nave {pv.Owner.NickName} recibió daño. Vida restante: {health}");

        if (health <= 0)
        {
            Debug.Log($"Nave {pv.Owner.NickName} destruida.");
            // Aquí puedes agregar lógica para destruir la nave o realizar otra acción
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pv.IsMine && collision.transform.CompareTag("Coin"))
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("CollectCoin", RpcTarget.AllBuffered, collision.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void CollectCoin(int coinViewID)
    {
        PhotonView coinPhotonView = PhotonView.Find(coinViewID);
        if (coinPhotonView != null)
        {
            PhotonNetwork.Destroy(coinPhotonView.gameObject);
            GameManager.instance.AddCoinToPool();
        }
    }
}