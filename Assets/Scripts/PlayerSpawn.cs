using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] playerPrefabs; // Lista de prefabs disponibles
    private List<int> assignedIndices = new List<int>(); // Lista de índices ya asignados
    private bool isSpawned = false; // Bandera para evitar spawnear varias veces
    private PhotonView pv;

    private int playersEliminated = 0; // Contador de jugadores eliminados
    private int Reinicio = 0; // Contador de jugadores eliminados

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        // El Master Client verifica si puede iniciar la cuenta regresiva
        if (PhotonNetwork.IsMasterClient)
        {
            CheckAllPlayersReady();
        }
    }

    private void Update()
    {
        OnWillRenderObject();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // El Master Client verifica si ya están todos los jugadores conectados
        if (PhotonNetwork.IsMasterClient)
        {
            CheckAllPlayersReady();
        }
    }

    public void SpawnPlayer()
    {
        if (isSpawned) return; // Evitar instanciar más de una vez
        isSpawned = true;

        // Asignar un prefab aleatorio sin repetir
        int prefabIndex = GetRandomPrefabIndex();
        GameObject prefabToSpawn = playerPrefabs[prefabIndex];

        // Instanciar el prefab asignado
        PhotonNetwork.Instantiate(prefabToSpawn.name,
            new Vector2(Random.Range(-4, 4), Random.Range(-4, 4)), Quaternion.identity);
    }

    private int GetRandomPrefabIndex()
    {
        if (assignedIndices.Count >= playerPrefabs.Length)
        {
            return 0; // Como fallback
        }

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, playerPrefabs.Length);
        } while (assignedIndices.Contains(randomIndex)); // Reintentar si ya fue asignado

        assignedIndices.Add(randomIndex); // Registrar el índice como asignado
        photonView.RPC("SyncAssignedIndices", RpcTarget.Others, randomIndex); // Sincronizar con otros jugadores

        return randomIndex;
    }

    [PunRPC]
    private void SyncAssignedIndices(int index)
    {
        if (!assignedIndices.Contains(index))
        {
            assignedIndices.Add(index);
        }
    }

    public void CheckAllPlayersReady()
    {
        // Condición para iniciar el juego (ajusta según el mínimo necesario)
        if (PhotonNetwork.PlayerList.Length >= 3) // Cambia 3 al número mínimo que desees
        {
            photonView.RPC("StartCountdown", RpcTarget.All); // Llama al RPC en todos los clientes
        }
        else
        {
            Debug.Log("Esperando más jugadores para iniciar...");
        }
    }

    [PunRPC]
    void StartCountdown()
    {
        GgGameManager.canMove = false; // Reinicia el estado de movimiento
        StartCoroutine(GameStartCoroutine());
    }

    private IEnumerator GameStartCoroutine()
    {
        UiManager uiManager = FindObjectOfType<UiManager>();
        if (uiManager == null)
        {
            yield break;
        }

        // Espera de 5 segundos antes de iniciar la primera cuenta regresiva
        yield return new WaitForSeconds(5);

        // Primera cuenta regresiva
        for (int i = 3; i > 0; i--)
        {
            uiManager.UpdateCountdown(i);
            yield return new WaitForSeconds(1);
        }

        uiManager.UpdateCountdown(0);

        // Habilitar movimiento para todos los jugadores
        photonView.RPC("EnableMovement", RpcTarget.All);

        // Esperar brevemente antes de iniciar la segunda cuenta regresiva
        yield return new WaitForSeconds(1);

        // Segunda cuenta regresiva (20 segundos)
        photonView.RPC("RestrictActions", RpcTarget.All, true); // Deshabilita disparos y zona
        for (int i = 5; i > 0; i--)
        {
            uiManager.UpdateCountdown(i);
            yield return new WaitForSeconds(1);
        }

        uiManager.UpdateCountdown(0);
        photonView.RPC("RestrictActions", RpcTarget.All, false); // Habilita disparos y zona
    }

    [PunRPC]
    void EnableMovement()
    {
        GgGameManager.canMove = true;

    }

    [PunRPC]
    void RestrictActions(bool restrict)
    {
        GgGameManager.canShootAndPlaySone = !restrict;
        Debug.Log(restrict
            ? "Disparos y reducción de zona deshabilitados."
            : "Disparos y reducción de zona habilitados.");
    }

    public void PlayerDied()
    {
        playersEliminated++;

        // Verificar si dos jugadores han sido eliminados
        if (playersEliminated >= 2)
        {
            PlayerRestart();
            CheckAllPlayersReady();
        }
    }

    public void PlayerRestart()
    {
        playersEliminated = 0; // Reiniciar el contador de eliminados
        Reinicio += 1;
        // Encontrar todos los objetos con la clase PlayerController
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        Debug.Log($"Número de jugadores encontrados: {players.Length}");

        foreach (var player in players)
        {
            // Llamar a la función para dar nueva posición
            player.ResetPosition();
            Debug.Log($"Posición reiniciada para el jugador: {player.name}");
        }


        Debug.Log("Juego reiniciado: posiciones y contadores reiniciados para todos los jugadores.");
    }

    private void OnWillRenderObject()
    {
        if (Reinicio == 1)
        {

            PlayerController[] players = FindObjectsOfType<PlayerController>();
            foreach (var player in players)
            {         
                // Llamar a la función para dar nueva posición
                player.PerdisteOGanaste();
            }
        }
    }
}
