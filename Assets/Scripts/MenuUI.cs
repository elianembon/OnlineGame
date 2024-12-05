using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMPro.TMP_InputField createInput;
    [SerializeField] private TMPro.TMP_InputField joinInput;

    private void Awake()
    {
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);
    }

    private void OnDestroy()
    {
        createButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Conectando a Photon...");
            PhotonNetwork.ConnectUsingSettings(); // Conecta al servidor de Photon.
        }
    }

    private void CreateRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            RoomOptions roomConfiguration = new RoomOptions();
            roomConfiguration.MaxPlayers = 3;

            if (!string.IsNullOrEmpty(createInput.text))
            {
                PhotonNetwork.CreateRoom(createInput.text, roomConfiguration);
                Debug.Log($"Intentando crear la sala: {createInput.text}");
            }
            else
            {
                Debug.LogWarning("El nombre de la sala no puede estar vacío.");
            }
        }
        else
        {
            Debug.LogWarning("No estás conectado a Photon. Espera a estar listo antes de crear una sala.");
        }
    }

    private void JoinRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (!string.IsNullOrEmpty(joinInput.text))
            {
                PhotonNetwork.JoinRoom(joinInput.text);
                Debug.Log($"Intentando unirse a la sala: {joinInput.text}");
            }
            else
            {
                Debug.LogWarning("El nombre de la sala no puede estar vacío.");
            }
        }
        else
        {
            Debug.LogWarning("No estás conectado a Photon. Espera a estar listo antes de unirte a una sala.");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al servidor maestro. Uniéndose al lobby...");
        PhotonNetwork.JoinLobby(); // Únete al lobby automáticamente después de conectar.
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Unido al lobby. Ahora puedes crear o unirte a salas.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Unido a la sala: {PhotonNetwork.CurrentRoom.Name}. Cargando nivel de juego...");
        PhotonNetwork.LoadLevel("Gameplay"); // Cambia a la escena de juego.
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Error al crear la sala: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Error al unirse a la sala: {message}");
    }
}