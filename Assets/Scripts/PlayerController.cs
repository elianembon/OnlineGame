using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private PhotonView pv;
    private Camera playerCamera;

    // Player
    public int health = 60; // Vida inicial de la nave
    public int maxHealth = 60; // Vida m�xima inicial
    [SerializeField] private float rotationSmoothSpeed = 10f;
    private bool isOutsideSafeZone = false; // Indica si el jugador est� fuera de la zona segura

    // Camera
    [SerializeField] private float cameraFollowSpeed = 5f;

    // HUD
    [SerializeField] private Slider healthSlider; // Referencia al Slider
    [SerializeField] private TextMeshProUGUI countdownText; // Texto del canvas del jugador

    // Variables de sincronizaci�n de red
    private Vector3 networkedPosition; // Posici�n objetivo de red
    private Quaternion networkedRotation; // Rotaci�n objetivo de red
    [SerializeField] private float interpolationSpeed = 10f; // Velocidad de interpolaci�n

    private float damageInterval = 1f; // Intervalo para aplicar da�o (1 segundo)
    private float damageTimer = 0f; // Temporizador para el da�o

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerCamera = GetComponentInChildren<Camera>();
        countdownText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            // Activa la c�mara solo para el jugador propietario
            playerCamera.gameObject.SetActive(true);

            UiManager uiManager = FindObjectOfType<UiManager>();
            if (uiManager != null && countdownText != null)
            {
                uiManager.RegisterCountdownText(countdownText);
            }

            // Configurar el Slider de vida
            if (healthSlider != null)
            {
                healthSlider.maxValue = health;
                healthSlider.value = health;
            }
            else
            {
                Debug.LogError("No se asign� un Slider en el inspector.");
            }
        }
        else
        {
            // Desactiva la c�mara para los jugadores no propietarios
            playerCamera.gameObject.SetActive(false);
        }

        // Inicializa las variables de posici�n y rotaci�n para la interpolaci�n
        networkedPosition = transform.position;
        networkedRotation = transform.rotation;
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            HandleMovement();
            HandleDamageOutsideSafeZone();
        }
        else
        {
            // Interpolaci�n suave hacia la posici�n y rotaci�n objetivo
            transform.position = Vector3.Lerp(transform.position, networkedPosition, Time.deltaTime * interpolationSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkedRotation, Time.deltaTime * interpolationSpeed);
        }
    }

    private void HandleMovement()
    {
        if (!pv.IsMine || !GgGameManager.canMove) return; // Solo el propietario puede manejar su movimiento

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

            // Sincronizar posici�n y rotaci�n con otros jugadores
            pv.RPC("UpdateTransform", RpcTarget.Others, transform.position, transform.rotation);
        }
    }

    private void HandleDamageOutsideSafeZone()
    {
        if (!isOutsideSafeZone) return;

        damageTimer += Time.deltaTime;
        if (damageTimer >= damageInterval)
        {
            damageTimer = 0f;
            TakeDamage(2); // Aplicar da�o peri�dico
        }
    }

    [PunRPC]
    void UpdateTransform(Vector3 newPosition, Quaternion newRotation)
    {
        if (!pv.IsMine)
        {
            // Actualiza la posici�n y rotaci�n objetivo para interpolaci�n
            networkedPosition = newPosition;
            networkedRotation = newRotation;
        }
    }

    [PunRPC]
    void HealPlayerCurrentHealth(int amount)
    {
        if (!pv.IsMine) return;

        // Incrementar solo la vida actual, sin afectar la vida m�xima
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth); // Asegurarse de no exceder el l�mite m�ximo.

        // Actualizar el HUD
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        //Debug.Log($"Jugador {pv.Owner.NickName}: vida actual {health}, vida m�xima {maxHealth}");
    }

    [PunRPC]
    void UpdateHealth(int newHealth)
    {
        health = newHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    [PunRPC]
    void IncreaseMaxHealthOnly(int amount)
    {
        if (!pv.IsMine) return;

        // Incrementar solo la vida m�xima, sin afectar la vida actual
        maxHealth += amount;

        // Actualizar el HUD para reflejar la nueva vida m�xima
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
        }

       // Debug.Log($"Jugador {pv.Owner.NickName}: vida actual {health}, vida m�xima {maxHealth}");
    }
    [PunRPC]
    void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        // Sincronizar la nueva vida con los dem�s jugadores
        pv.RPC("UpdateHealth", RpcTarget.AllBuffered, health);

        // Actualizar el Slider local
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        if (health <= 0)
        {
            Debug.Log($"Nave {pv.Owner.NickName} destruida.");
            HandleDefeat();
        }
    }



    private void HandleDefeat()
    {
        if (PhotonNetwork.IsConnected && pv.IsMine)
        {
            // Notificar a los dem�s jugadores que este jugador ha sido eliminado
            pv.RPC("NotifyPlayerDefeated", RpcTarget.AllBuffered, pv.Owner.NickName);
        }

        gameObject.SetActive(false); // Desactivar el jugador para simular su muerte
        //NotifyRoundManager(); // Notificar que este jugador ha sido derrotado
    }

    [PunRPC]
    void NotifyPlayerDefeated(string playerName)
    {
        Debug.Log($"El jugador {playerName} ha sido eliminado.");
    }


    //private void NotifyRoundManager()
    //{
    //    RoundManager roundManager = FindObjectOfType<RoundManager>();
    //    if (roundManager != null)
    //    {
    //        roundManager.PlayerDefeated(pv.Owner); // Notifica al RoundManager
    //    }
    //    else
    //    {
    //        Debug.LogError("No se encontr� un RoundManager en la escena.");
    //    }
    //}

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isOutsideSafeZone = true;

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isOutsideSafeZone = false;

        }
    }
}


