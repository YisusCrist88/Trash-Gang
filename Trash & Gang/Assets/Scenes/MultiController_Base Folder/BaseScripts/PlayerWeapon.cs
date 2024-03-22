using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public float damageTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Ejemplo de acceso directo + usando valores propiso
           GameObject hitObject = collision.gameObject;
            StunPlayer stunPlayer = hitObject.GetComponent<StunPlayer>();
            // StunPlayer stunScript = collision.gameObject.GetComponent<StunPlayer>(); válido para juegos pequeños pero menos seguro
            stunPlayer.StunEffect(damageTime);
        }
    }
}
