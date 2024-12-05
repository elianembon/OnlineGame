using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    private bool isSpawned = false; // Bandera para controlar el spawn del jugador

    private void Start()
    {
        // Espera a recibir la señal de la pantalla de carga
        Debug.Log("Esperando señal para instanciar al jugador...");
    }

    public void SpawnPlayer()
    {
        if (isSpawned) return; // Evitar instanciar más de una vez
        isSpawned = true;

        PhotonNetwork.Instantiate(playerPrefab.name,
            new Vector2(Random.Range(-4, 4), Random.Range(-4, 4)), Quaternion.identity);

        Debug.Log($"Jugador {PhotonNetwork.NickName} conectado. Total: {PhotonNetwork.PlayerList.Length}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Nuevo jugador conectado: {newPlayer.NickName}. Total jugadores: {PhotonNetwork.PlayerList.Length}");

        if (PhotonNetwork.PlayerList.Length == 3 && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Se alcanzó el número de jugadores requerido. Iniciando cuenta regresiva.");
            photonView.RPC("StartCountdown", RpcTarget.All);
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
        photonView.RPC("EnableMovement", RpcTarget.All); // Esto activa `GgGameManager.canMove` en todos los clientes

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