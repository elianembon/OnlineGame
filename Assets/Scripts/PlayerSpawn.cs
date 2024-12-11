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

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        // El Master Client verifica si puede iniciar la cuenta regresiva
        if (PhotonNetwork.IsMasterClient)
        {
            CheckAllPlayersReady();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Nuevo jugador conectado: {newPlayer.NickName}. Total jugadores: {PhotonNetwork.PlayerList.Length}");

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

        Debug.Log($"Jugador {PhotonNetwork.NickName} spawneado con prefab: {prefabToSpawn.name}");
    }

    private int GetRandomPrefabIndex()
    {
        if (assignedIndices.Count >= playerPrefabs.Length)
        {
            Debug.LogError("Todos los prefabs ya han sido asignados.");
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
            Debug.Log("Todos los jugadores están conectados. Iniciando cuenta regresiva...");
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
        Debug.Log("Inicio de cuenta regresiva en todos los clientes.");
        StartCoroutine(GameStartCoroutine());
    }

    private IEnumerator GameStartCoroutine()
    {
        UiManager uiManager = FindObjectOfType<UiManager>();
        if (uiManager == null)
        {
            Debug.LogError("No se encontró un UiManager en la escena.");
            yield break;
        }

        // Espera de 5 segundos antes de iniciar la primera cuenta regresiva
        Debug.Log("Esperando 5 segundos antes de iniciar la primera cuenta regresiva.");
        yield return new WaitForSeconds(5);

        // Primera cuenta regresiva
        Debug.Log("Primera cuenta regresiva iniciada.");
        for (int i = 3; i > 0; i--)
        {
            Debug.Log($"Mostrando {i} en el contador.");
            uiManager.UpdateCountdown(i);
            yield return new WaitForSeconds(1);
        }

        Debug.Log("Primera cuenta regresiva finalizada.");
        uiManager.UpdateCountdown(0);

        // Habilitar movimiento para todos los jugadores
        Debug.Log("Habilitando el movimiento para todos los jugadores.");
        photonView.RPC("EnableMovement", RpcTarget.All);

        // Esperar brevemente antes de iniciar la segunda cuenta regresiva
        yield return new WaitForSeconds(1);

        // Segunda cuenta regresiva (20 segundos)
        Debug.Log("Iniciando la segunda cuenta regresiva (zona y disparos deshabilitados).");
        photonView.RPC("RestrictActions", RpcTarget.All, true); // Deshabilita disparos y zona
        for (int i = 20; i > 0; i--)
        {
            Debug.Log($"Mostrando {i} en la segunda cuenta regresiva.");
            uiManager.UpdateCountdown(i);
            yield return new WaitForSeconds(1);
        }

        Debug.Log("Segunda cuenta regresiva finalizada. Habilitando disparos y reducción de zona.");
        uiManager.UpdateCountdown(0);
        photonView.RPC("RestrictActions", RpcTarget.All, false); // Habilita disparos y zona
    }

    [PunRPC]
    void EnableMovement()
    {
        GgGameManager.canMove = true;
        Debug.Log("El movimiento está ahora habilitado para los jugadores.");
    }

    [PunRPC]
    void RestrictActions(bool restrict)
    {
        GgGameManager.canShootAndPlaySone = !restrict;
        Debug.Log(restrict
            ? "Disparos y reducción de zona deshabilitados."
            : "Disparos y reducción de zona habilitados.");
    }
}