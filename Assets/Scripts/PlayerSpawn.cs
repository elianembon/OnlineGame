using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    private PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
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
            pv.RPC("StartCountdown", RpcTarget.All);
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

        Debug.Log("Cuenta regresiva iniciada.");
        for (int i = 3; i > 0; i--)
        {
            Debug.Log($"Mostrando {i} en el contador.");
            uiManager.UpdateCountdown(i); // Actualizar todos los textos
            yield return new WaitForSeconds(1);
        }

        Debug.Log("Cuenta regresiva finalizada.");
        uiManager.UpdateCountdown(0); // Limpiar el texto del contador

        Debug.Log("Habilitando el movimiento para todos los jugadores.");
        pv.RPC("EnableMovement", RpcTarget.All);
    }

    [PunRPC]
    void EnableMovement()
    {
        Debug.Log("El movimiento está ahora habilitado para los jugadores.");
        GgGameManager.canMove = true;
    }
}