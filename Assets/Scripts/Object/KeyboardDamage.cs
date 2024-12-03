using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KeyboardDamage : MonoBehaviour
{
    public ObjectsHealth healthManager;  // El objeto que recibe el daño
    public int damageAmount = 20;  // Cantidad de daño que se aplica
    public KeyCode damageKey = KeyCode.Space;  // Tecla para causar daño (por defecto: Espacio)

    void Update()
    {
        // Detectar si la tecla ha sido presionada
        if (Input.GetKeyDown(damageKey))
        {
            if (healthManager != null)
            {
                // Aplicar el daño al objeto
                healthManager.TakeDamage(damageAmount);
            }
        }
    }
}
