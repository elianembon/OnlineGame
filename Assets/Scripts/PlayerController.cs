using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private PhotonView pv;
    private Camera playerCamera;
    private int health = 60; // Vida inicial de la nave

    [SerializeField] private Slider healthSlider; // Referencia al Slider
     //private Vector3 cameraOffset = new Vector3(0, 0, -10);
    [SerializeField] private float cameraFollowSpeed = 5f;
    [SerializeField] private float rotationSmoothSpeed = 10f;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        
            playerCamera.gameObject.SetActive(pv.IsMine);
        if (pv.IsMine)
        {
            // Configurar el Slider de vida
            if (healthSlider != null)
            {
                healthSlider.maxValue = health;
                healthSlider.value = health;
            }
            else
            {
                Debug.LogError("No se asignó un Slider en el inspector.");
            }
        }
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            HandleMovement();
           

            // Restar vida al presionar K
            if (Input.GetKeyDown(KeyCode.K))
            {
                pv.RPC("TakeDamage", RpcTarget.AllBuffered, 15);
            }
        }
    }

    private void HandleMovement()
    {

        if (!pv.IsMine) return; // Solo el propietario puede manejar su movimiento

        Vector2 direction = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) direction += Vector2.up;
        if (Input.GetKey(KeyCode.S)) direction += Vector2.down;
        if (Input.GetKey(KeyCode.A)) direction += Vector2.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector2.right;

        if (direction != Vector2.zero)
        {
            transform.position += (Vector3)(direction.normalized * 5 * Time.deltaTime);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);

            // Sincronizar posición y rotación con otros jugadores
            pv.RPC("UpdateTransform", RpcTarget.Others, transform.position, transform.rotation);
        }
    }

    [PunRPC]
    void UpdateHealth(int newHealth)
    {
        health = newHealth;
        healthSlider.value = health;
    }

    [PunRPC]
    void TakeDamage(int damage)
    {
        if (!pv.IsMine) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, 60);

        // Sincroniza la nueva vida con los demás jugadores
        pv.RPC("UpdateHealth", RpcTarget.AllBuffered, health);

        Debug.Log($"Nave {pv.Owner.NickName} recibió daño. Vida restante: {health}");

        if (health <= 0)
        {
            Debug.Log($"Nave {pv.Owner.NickName} destruida.");
            NotifyRoundManager(); // Notificar que este jugador ha sido derrotado
            HandleDefeat();
        }
    }

    private void HandleDefeat()
    {
        gameObject.SetActive(false); // Desactivar el jugador para simular su muerte
        NotifyRoundManager(); // Notificar que este jugador ha sido derrotado
    }

    private void NotifyRoundManager()
    {
        RoundManager roundManager = FindObjectOfType<RoundManager>();
        if (roundManager != null)
        {
            roundManager.PlayerDefeated(pv.Owner); // Notifica al RoundManager
        }
        else
        {
            Debug.LogError("No se encontró un RoundManager en la escena.");
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
    void UpdateRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
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

    [PunRPC]
    void UpdateTransform(Vector3 newPosition, Quaternion newRotation)
    {
        if (!pv.IsMine)
        {
            transform.position = newPosition;
            transform.rotation = newRotation;
        }
    }
}