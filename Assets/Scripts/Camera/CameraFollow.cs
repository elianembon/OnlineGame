using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 fixedRotation = new Vector3(0, 0, 0); // Rotaci�n fija deseada

    private void LateUpdate()
    {
        // Mantener siempre la rotaci�n fija
        transform.rotation = Quaternion.Euler(fixedRotation);
    }
}
