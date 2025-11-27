using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 40;
    public Rigidbody2D rb;
    public GameObject impactEffect;

    void Start()
    {
        rb.linearVelocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Jeśli trafiło bossa — zadaj obrażenia
        BossHealth enemy = hitInfo.GetComponent<BossHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // Jeśli trafiło gracza — zadaj obrażenia
        if (hitInfo.CompareTag("Player"))
        {
            PlayerHealth player = hitInfo.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }

        // Jeśli obiekt ma tag "Enemy", to go niszczymy
        if (hitInfo.CompareTag("Enemy"))
        {
            Destroy(hitInfo.gameObject);
        }

        // Tworzymy efekt uderzenia
        Instantiate(impactEffect, transform.position, transform.rotation);

        // Usuwamy pocisk
        Destroy(gameObject);
    }
}
