using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //[SerializeField] private Vector3 offset = new Vector3(0, 5, -10); // Desplazamiento de la cámara
    //[SerializeField] private float smoothSpeed = 0.125f; // Velocidad de suavizado

    //private Transform target; // Transform del jugador

    //private void Start()
    //{
    //    // Encuentra al jugador controlado localmente
    //    foreach (var player in FindObjectsOfType<PlayerController>())
    //    {
    //        PhotonView pv = player.GetComponent<PhotonView>();
    //        if (pv != null && pv.IsMine)
    //        {
    //            target = player.transform;
    //            break;
    //        }
    //    }

    //    if (target == null)
    //    {
    //        Debug.LogError("No se encontró un jugador controlado localmente para asignar como objetivo de la cámara.");
    //    }
    //}

    //private void LateUpdate()
    //{
    //    if (target != null)
    //    {
    //        // Calcula la posición deseada de la cámara
    //        Vector3 desiredPosition = target.position + offset;

    //        // Suaviza la transición hacia la posición deseada
    //        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

    //        // Actualiza la posición de la cámara
    //        transform.position = smoothedPosition;

    //        // Mantiene la rotación fija
    //        transform.rotation = Quaternion.identity;
    //    }
    //}
}
