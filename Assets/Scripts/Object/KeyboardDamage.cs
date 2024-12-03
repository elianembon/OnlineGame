using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KeyboardDamage : MonoBehaviour
{
    public ObjectsHealth healthManager;  // El objeto que recibe el da�o
    public int damageAmount = 20;  // Cantidad de da�o que se aplica
    public KeyCode damageKey = KeyCode.Space;  // Tecla para causar da�o (por defecto: Espacio)

    void Update()
    {
        // Detectar si la tecla ha sido presionada
        if (Input.GetKeyDown(damageKey))
        {
            if (healthManager != null)
            {
                // Aplicar el da�o al objeto
                healthManager.TakeDamage(damageAmount);
            }
        }
    }
}
