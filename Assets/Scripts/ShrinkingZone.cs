using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkingZone : MonoBehaviour
{
    public float shrinkSpeed = 1f; // Aumentar para que sea más notable
    public float minRadius = 0.5f;
    public Vector2 finalPosition = Vector2.zero;
    public CircleCollider2D safeZoneCollider;

    private void Start()
    {
        if (safeZoneCollider == null)
        {
            safeZoneCollider = GetComponent<CircleCollider2D>();
        }

        Debug.Log("Zona inicializada. Radio actual: " + safeZoneCollider.radius);
    }

    private void Update()
    {
        ShrinkZone();
    }

    void ShrinkZone()
    {
        if (safeZoneCollider.radius > minRadius)
        {
            // Mostrar en consola el radio antes de reducir
            Debug.Log("Reduciendo zona. Radio actual: " + safeZoneCollider.radius);

            safeZoneCollider.radius = Mathf.MoveTowards(safeZoneCollider.radius, minRadius, shrinkSpeed * Time.deltaTime);
            transform.position = Vector2.MoveTowards(transform.position, finalPosition, shrinkSpeed * Time.deltaTime);

            // Mostrar en consola el radio después de reducir
            Debug.Log("Radio reducido a: " + safeZoneCollider.radius);
        }
        else
        {
            Debug.Log("El radio ha alcanzado el tamaño mínimo.");
        }
    }
}