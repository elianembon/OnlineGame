using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //[SerializeField] private Vector3 offset = new Vector3(0, 5, -10); // Desplazamiento de la c�mara
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
    //        Debug.LogError("No se encontr� un jugador controlado localmente para asignar como objetivo de la c�mara.");
    //    }
    //}

    //private void LateUpdate()
    //{
    //    if (target != null)
    //    {
    //        // Calcula la posici�n deseada de la c�mara
    //        Vector3 desiredPosition = target.position + offset;

    //        // Suaviza la transici�n hacia la posici�n deseada
    //        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

    //        // Actualiza la posici�n de la c�mara
    //        transform.position = smoothedPosition;

    //        // Mantiene la rotaci�n fija
    //        transform.rotation = Quaternion.identity;
    //    }
    //}
}
