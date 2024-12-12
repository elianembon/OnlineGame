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
    private Animator animator;
    private PlayerSpawn playerSpawn;

    private int perdiste = 0;
    // Player
    private bool OnPlay = true;
    private Vector3 initialPosition; // Posición inicial del jugador
    public int health = 60; // Vida inicial de la nave
    public int maxHealth = 60; // Vida máxima inicial
    [SerializeField] private float rotationSmoothSpeed = 10f;
    private bool isOutsideSafeZone = false; // Indica si el jugador está fuera de la zona segura


    [SerializeField] private float acceleration = 10f; // Aceleración del movimiento
    [SerializeField] private float maxSpeed = 5f; // Velocidad máxima
    [SerializeField] private float friction = 0.1f; // Fricción para desacelerar
    private Vector2 currentVelocity; // Velocidad actual del jugador

    // Camera
    [SerializeField] private float cameraFollowSpeed = 5f;

    // HUD
    [SerializeField] private Slider healthSlider; // Referencia al Slider
    [SerializeField] private TextMeshProUGUI countdownText; // Texto del canvas del jugador

    // Variables de sincronización de red
    private Vector3 networkedPosition; // Posición objetivo de red
    private Quaternion networkedRotation; // Rotación objetivo de red
    [SerializeField] private float interpolationSpeed = 10f; // Velocidad de interpolación

    private float damageInterval = 1f; // Intervalo para aplicar daño (1 segundo)
    private float damageTimer = 0f; // Temporizador para el daño


    public GameObject panelLostRound;
    public GameObject panelWinRound;

    public bool win = false;

    

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        playerCamera = GetComponentInChildren<Camera>();
        countdownText = GetComponentInChildren<TextMeshProUGUI>();
        animator = GetComponent<Animator>();
        playerSpawn = FindObjectOfType<PlayerSpawn>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            // Activa la cámara solo para el jugador propietario
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
                Debug.LogError("No se asignó un Slider en el inspector.");
            }
        }
        else
        {
            // Desactiva la cámara para los jugadores no propietarios
            playerCamera.gameObject.SetActive(false);
        }
        initialPosition = transform.position;
        // Inicializa las variables de posición y rotación para la interpolación
        networkedPosition = transform.position;
        networkedRotation = transform.rotation;
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            if (OnPlay)
            {
                HandleMovement();
                HandleDamageOutsideSafeZone();
            }
            
        }
        else
        {
            // Interpolación suave hacia la posición y rotación objetivo
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

        bool isMoving = direction != Vector2.zero;
        animator.SetBool("IsMoving", isMoving); // Setea el parámetro IsMoving

        if (isMoving)
        {
            // Calcular la dirección normalizada
            Vector2 desiredVelocity = direction.normalized * maxSpeed;

            // Acelerar hacia la velocidad deseada
            currentVelocity = Vector2.MoveTowards(currentVelocity, desiredVelocity, acceleration * Time.deltaTime);

            // Mover al jugador
            transform.position += (Vector3)(currentVelocity * Time.deltaTime);

            // Calcular el ángulo de rotación
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);

            // Sincronizar posición y rotación con otros jugadores
            pv.RPC("UpdateTransform", RpcTarget.Others, transform.position, transform.rotation);
        }
        else
        {
            // Aplicar fricción cuando no se está moviendo
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, friction);
            transform.position += (Vector3)(currentVelocity * Time.deltaTime);
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
            TakeDamage(2); // Aplicar daño periódico
        }
    }

    [PunRPC]
    void UpdateTransform(Vector3 newPosition, Quaternion newRotation)
    {
        if (!pv.IsMine)
        {
            // Actualiza la posición y rotación objetivo para interpolación
            networkedPosition = newPosition;
            networkedRotation = newRotation;
        }
    }

    [PunRPC]
    void HealPlayerCurrentHealth(int amount)
    {
        if (!pv.IsMine) return;

        // Incrementar solo la vida actual, sin afectar la vida máxima
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth); // Asegurarse de no exceder el límite máximo.

        // Actualizar el HUD
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        //Debug.Log($"Jugador {pv.Owner.NickName}: vida actual {health}, vida máxima {maxHealth}");
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

        // Incrementar solo la vida máxima, sin afectar la vida actual
        maxHealth += amount;

        // Actualizar el HUD para reflejar la nueva vida máxima
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
        }

       // Debug.Log($"Jugador {pv.Owner.NickName}: vida actual {health}, vida máxima {maxHealth}");
    }



    [PunRPC]
    public void ResetPosition()
    {

        if (pv.IsMine)
        {
            transform.position = initialPosition;

            ResetCamera();

            OnPlay = true;

            pv.RPC("UpdateTransform", RpcTarget.Others, transform.position, transform.rotation);

            // Restaurar valores adicionales si es necesario
            health = maxHealth;
            if (healthSlider != null)
            {
                healthSlider.value = health;
            }
            pv.RPC("UpdateHealth", RpcTarget.AllBuffered, health);
        }    
    }


    [PunRPC]
    void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        // Sincronizar la nueva vida con los demás jugadores
        pv.RPC("UpdateHealth", RpcTarget.AllBuffered, health);

        // Actualizar el Slider local
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        if (health <= 0)
        {
            // Actualizar la posición y sincronizarla con los demás jugadores
            Vector3 newPosition = new Vector3(transform.position.x + 400f, 0f, transform.position.z);
            transform.position = newPosition; // Aplicar el cambio localmente

            // Sincronizar la nueva posición y rotación con los demás jugadores
            pv.RPC("UpdateTransform", RpcTarget.Others, newPosition, transform.rotation);


            SwitchCameraToAnotherPlayer();
            playerSpawn.PlayerDied();
            

            OnPlay = false;
        }
    }

    private void HandleDefeat()
    {
        if (PhotonNetwork.IsConnected && pv.IsMine)
        {
            // Notificar a los demás jugadores que este jugador ha sido eliminado
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

        if (other.CompareTag("Player") || other.CompareTag("Destructible"))
        {         
                // Determinar por qué lado del objeto colisionamos
                Vector3 direction = other.transform.position - transform.position;

                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    // Colisión desde la izquierda o la derecha
                    if (direction.x > 0)
                    {
                        Debug.Log("Colisión desde la izquierda");
                        // Rebote hacia la izquierda
                        currentVelocity.x = -Mathf.Abs(currentVelocity.x); // Invertir la velocidad en x
                    }
                    else
                    {
                        Debug.Log("Colisión desde la derecha");
                        // Rebote hacia la derecha
                        currentVelocity.x = Mathf.Abs(currentVelocity.x); // Invertir la velocidad en x
                    }
                }
                else
                {
                    // Colisión desde arriba o abajo
                    if (direction.y > 0)
                    {
                        Debug.Log("Colisión desde abajo");
                        // Rebote hacia abajo
                        currentVelocity.y = -Mathf.Abs(currentVelocity.y); // Invertir la velocidad en y
                    }
                    else
                    {
                        Debug.Log("Colisión desde arriba");
                        // Rebote hacia arriba
                        currentVelocity.y = Mathf.Abs(currentVelocity.y); // Invertir la velocidad en y
                    }
                }

                // Aplicar una pequeña fuerza adicional para simular un rebote
                currentVelocity += new Vector2(direction.x, direction.y).normalized * 4f; // Ajusta el valor según sea necesario
                pv.RPC("UpdateTransform", RpcTarget.Others, transform.position, transform.rotation);
        }
    }

    public void PerdisteOGanaste()
    {


        Debug.Log(perdiste + "Perdiste");
        if (pv.IsMine)
        {
             if (perdiste <= 1)
            {
                panelWinRound.SetActive(true);
                panelLostRound.SetActive(false);
            }
            else
            {
                panelLostRound.SetActive(true);
                panelWinRound.SetActive(false);
            }

            StartCoroutine(EsperarYIrMenu());
        }
    }


    [PunRPC]
    public void SwitchCameraToAnotherPlayer()
    {
        if (pv.IsMine)
        {
            perdiste += 1;
        }
        

        if (!pv.IsMine) return; // Solo el jugador local cambiará su cámara.

        // Define la posición central del mapa.
        Vector3 mapCenter = new Vector3(0, 0, -10); // Ajusta Z según sea necesario para tu juego.

        // Mueve la cámara al centro del mapa.
        playerCamera.transform.SetParent(null); // Desvincula la cámara de cualquier padre.
        playerCamera.transform.position = mapCenter;

        // Opcionalmente ajusta el campo de visión (FOV) de la cámara.
        Camera cameraComponent = playerCamera.GetComponent<Camera>();
        if (cameraComponent != null)
        {
            cameraComponent.fieldOfView = 130; // Ajusta según el tamaño de tu mapa.
        }
    }

    [PunRPC]
    public void ResetCamera()
    {
        if (!pv.IsMine) return; // Solo el jugador local cambiará su cámara.

        Vector3 mapPlayer = new Vector3(initialPosition.x, initialPosition.y, -10); // Ajusta Z según sea necesario para tu juego.

        // Mueve la cámara a la posición inicial del jugador.
        playerCamera.transform.SetParent(null); // Desvincula la cámara de cualquier padre.
        playerCamera.transform.position = mapPlayer;

        // Opcionalmente ajusta el campo de visión (FOV) de la cámara.
        Camera cameraComponent = playerCamera.GetComponent<Camera>();
        if (cameraComponent != null)
        {
            cameraComponent.fieldOfView = 60; // Ajusta según el tamaño de tu mapa.
        }
    }

    private IEnumerator EsperarYIrMenu()
    {
        // Espera 5 segundos
        yield return new WaitForSeconds(5f);

        // Desconecta de Photon
        PhotonNetwork.Disconnect();

        // Cambiar a la escena Menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu"); // Cambia "Menu" al nombre correcto de tu escena
    }


    [PunRPC]
    public void DisconnectPlayer()
    {
        // Asegurarse de que solo el jugador que ha muerto realice la desconexión
        if (PhotonNetwork.IsConnected && pv.IsMine)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("LoadScene");
            Debug.Log("Jugador desconectado de la sala.");
        }
    }

}


