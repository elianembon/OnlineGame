using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private GameObject player;
    private PhotonView pv;

    private void Awake()
    {
       pv = GetComponent<PhotonView>();
    }

    private void Start()
    {   
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector2(Random.Range(-4, 4), Random.Range(-4, 4)), Quaternion.identity);

        int playerIndex = PhotonNetwork.PlayerList.Length;
    }
}
