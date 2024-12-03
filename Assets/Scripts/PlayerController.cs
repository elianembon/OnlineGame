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
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 5, -10);
    [SerializeField] private float cameraFollowSpeed = 5f;
    [SerializeField] private float rotationSmoothSpeed = 10f;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            // Configurar la cámara
            playerCamera = Camera.main;
            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("No se encontró una cámara principal.");
            }

            // Configurar el Slider si está asignado
            if (healthSlider != null)
            {
                healthSlider.maxValue = 60;
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
            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) direction += Vector3.up;
            if (Input.GetKey(KeyCode.S)) direction += Vector3.down;
            if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
            if (Input.GetKey(KeyCode.D)) direction += Vector3.right;

            if (direction != Vector3.zero)
            {
                transform.position += direction.normalized * 5 * Time.deltaTime;

                Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
            }

            UpdateCameraPosition();

            if (Input.GetKeyDown(KeyCode.K))
            {
                pv.RPC("TakeDamage", RpcTarget.AllBuffered, 15);
            }
        }
    }

    private void UpdateCameraPosition()
    {
        if (playerCamera != null)
        {
            Vector3 targetPosition = transform.position + cameraOffset;
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, targetPosition, Time.deltaTime * cameraFollowSpeed);
            playerCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    [PunRPC]
    void TakeDamage(int damage)
    {
        if (!pv.IsMine) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, 60); // Aseguramos que la vida no sea menor que 0

        // Actualizar el Slider
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        Debug.Log($"Nave {pv.Owner.NickName} recibió daño. Vida restante: {health}");

        if (health <= 0)
        {
            Debug.Log($"Nave {pv.Owner.NickName} destruida.");
            PhotonNetwork.Destroy(gameObject);
        }
    }
}